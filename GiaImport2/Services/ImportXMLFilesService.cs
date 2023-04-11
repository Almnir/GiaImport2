using DevExpress.LookAndFeel;
using DevExpress.Pdf.Native.BouncyCastle.Crypto;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Fields.Expression;
using GiaImport2.DataModels;
using GiaImport2.Models;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using NLog.Fluent;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Container = SimpleInjector.Container;

namespace GiaImport2.Services
{
    public class ImportXMLFilesService : IImportXMLFilesService
    {
        private readonly Container Container;
        private readonly ICommonRepository CommonRepository;
        // Словарь ошибок некорреляции
        ConcurrentDictionary<string, string> DependencyErrorDict;
        ConcurrentDictionary<string, FileInfo> LoadedFiles = new ConcurrentDictionary<string, FileInfo>();
        // статистика: таблица - xml записей
        Dictionary<string, long> ImportStatistics = new Dictionary<string, long>();
        // Таблица статистики
        DataTable DataStatTable;
        public class DependantTable
        {
            public string ParentColumn { get; set; }
            public string DependantTableName { get; set; }
            public string DependantTableColumn { get; set; }
            public bool IsNullable { get; set; }
        }

        public class ParentTable
        {
            public string ParentTableName { get; set; }
            public List<DependantTable> DependantTableInfo { get; set; }
        }

        public ImportXMLFilesService(Container container, ICommonRepository commonRepository)
        {
            this.Container = container;
            this.CommonRepository = commonRepository;
        }

        public bool TryCheckFilesNames(string zipFileName)
        {
            bool result = true;
            try
            {
                using (var zip = ZipFile.OpenRead(zipFileName))
                {
                    foreach (var ze in zip.Entries)
                    {
                        if (!Globals.TABLES_NAMES.Contains(Path.GetFileNameWithoutExtension(ze.Name)))
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                FormsHelper.ShowStyledMessageBox("Ошибка!", ex.ToString());
                CommonRepository.GetLogger().Error(ex.ToString());
            }
            return result;
        }
        public void ClearFiles()
        {
            string[] files = Directory.GetFiles(Globals.frmSettings.TempDirectoryText == null ?
                Path.Combine(Path.GetTempPath(), "Tempdir") : Path.Combine(Globals.frmSettings.TempDirectoryText, "Tempdir"));
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
        public bool UnpackFiles(string zipfilename, RibbonControl ribbonControl, Action<ImportXMLFilesDto> addFileToView)
        {
            bool error = false;
            try
            {
                this.LoadedFiles.Clear();
                CancellationTokenSource source = new CancellationTokenSource();
                ProgressBarSingleBar pbw = new ProgressBarSingleBar();
                pbw.SetCancellationToken(source);
                pbw.SetTitle("Распаковка файлов выполняется...");
                ProgressBarControl pbarTotal = pbw.GetProgressBarTotal();
                pbw.Refresh();
                LabelControl plabel = pbw.GetLabel();
                FileInfo fi = new FileInfo(zipfilename);
                plabel.Text = fi.Name;
                int entriesCount = 0;
                using (var zip = ZipFile.OpenRead(zipfilename))
                {
                    entriesCount = zip.Entries.Count;
                }
                pbarTotal.Properties.Maximum = entriesCount - 1;
                IProgress<int> progress = new Progress<int>(value =>
                {
                    ProgressBarControl pb = pbw.GetProgressBarTotal();
                    if (!pb.IsDisposed)
                    {
                        pb.Invoke((MethodInvoker)(() =>
                        {
                            pb.EditValue = value;
                        }));
                    }
                });
                pbw.FormClosed += (a, e) => { source.Cancel(); };
                // отключаем кнопки на риббоне
                FormsHelper.ToggleRibbonButtonsAll(ribbonControl, false);
                pbw.Show();
                Task task = Task.Run(() => RunUnpacker(addFileToView, zipfilename, progress,
                    () =>
                    {
                        if (pbw != null && pbw.IsDisposed != true)
                        {
                            pbw.Invoke((MethodInvoker)(() =>
                            {
                                pbw.Close();
                            }));
                        }
                        // включаем кнопки на риббоне
                        ribbonControl.Invoke((MethodInvoker)(() =>
                        {
                            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
                        }));
                    },
                    source.Token), source.Token);
                task.ContinueWith(taskc =>
                {
                    if (taskc.Exception == null && !source.IsCancellationRequested)
                    {
                        EndUnpacker(ribbonControl,
                            () =>
                            {
                                if (pbw != null && pbw.IsDisposed != true)
                                {
                                    pbw.Invoke((MethodInvoker)(() =>
                                    {
                                        pbw.Close();
                                    }));
                                }
                                // включаем кнопки на риббоне
                                ribbonControl.Invoke((MethodInvoker)(() =>
                                {
                                    FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
                                }));
                            }
                        );
                    }
                });
            }
            catch (TaskCanceledException)
            {
                error = true;
                FormsHelper.ShowStyledMessageBox("Внимание!", "Операция прервана!");
            }
            catch (Exception ex)
            {
                error = true;
                FormsHelper.ShowStyledMessageBox("Ошибка!", ex.ToString());
                CommonRepository.GetLogger().Error(ex.ToString());
            }
            return error;
        }

        private void EndUnpacker(RibbonControl ribbonControl, Action closeProgressBar)
        {
            closeProgressBar();
        }

        private void RunUnpacker(Action<ImportXMLFilesDto> addFileToView, string zipfilename, IProgress<int> progress, Action closeProgressBar, CancellationToken ct)
        {
            try
            {
                int counter = 0;
                using (ZipArchive zip = ZipFile.OpenRead(zipfilename))
                {
                    foreach (var e in zip.Entries)
                    {
                        ct.ThrowIfCancellationRequested();
                        string filenewpath = Path.Combine(Globals.frmSettings.TempDirectoryText == null ?
                            Path.Combine(Path.GetTempPath(), "Tempdir") : Path.Combine(Globals.frmSettings.TempDirectoryText, "Tempdir"), e.FullName);
                        e.ExtractToFile(filenewpath, true);
                        FileInfo fi = new FileInfo(filenewpath);
                        var ixf = new ImportXMLFilesDto();
                        ixf.Name = fi.Name;
                        ixf.CreationTime = fi.CreationTime;
                        ixf.Length = fi.Length;
                        // добавляем во вью
                        addFileToView(ixf);
                        // наполняем кэш открытых файлов
                        LoadedFiles.TryAdd(filenewpath, fi);
                        progress.Report(counter);
                        counter++;
                        //Thread.Sleep(500);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                closeProgressBar();
            }
            catch (Exception ex)
            {
                CommonRepository.GetLogger().Error(ex.ToString());
                closeProgressBar();
            }
        }
        public void ValidateFiles(ImportGridPanel importGridPanel, RibbonControl ribbonControl, List<ImportXMLFilesDto> checkedFiles, Action<string> showValidationErrors, Action<(List<TableInfo> tableInfos, ConcurrentDictionary<string, string> dependencyErrors)> showResultWindow)
        {
            ConcurrentDictionary<string, FileInfo> actualCheckedFiles = new ConcurrentDictionary<string, FileInfo>();
            ILookup<string, FileInfo> fileInfos = LoadedFiles.ToLookup(k => Path.GetFileName(k.Key), k => k.Value);
            foreach (var file in checkedFiles)
            {
                string nnn = fileInfos[file.Name].Select(x => x.FullName).First();
                FileInfo info = fileInfos[file.Name].Select(x => x).First();
                actualCheckedFiles.TryAdd(nnn, info);
            }
            // инициализируем словарь ошибок некорреляций
            DependencyErrorDict = new ConcurrentDictionary<string, string>();
            var source = new CancellationTokenSource();
            var pbw = new ProgressBarWindow();
            pbw.SetCancellationToken(source);
            pbw.SetTitle("Проверка схемы выполняется...");
            ProgressBarControl pbarTotal = pbw.GetProgressBarTotal();
            pbarTotal.Properties.Maximum = 2;
            ProgressBarControl pbarLine = pbw.GetProgressBarLine();
            pbarLine.Properties.Maximum = actualCheckedFiles.Count - 1;
            LabelControl plabel = pbw.GetLabel();
            Action changeProgressLine = () =>
            {
                if (!pbw.IsDisposed)
                {
                    pbw.Invoke((MethodInvoker)(() =>
                    {
                        Control panel = pbw.Controls.Find("panelControl1", true).FirstOrDefault();
                        Control pline = panel.Controls.Find("progressBarLine", true).FirstOrDefault();
                        panel.Controls.Remove(pline);
                        MarqueeProgressBarControl marqueeProgressBarControl = new MarqueeProgressBarControl();
                        panel.Controls.Add(marqueeProgressBarControl);
                        marqueeProgressBarControl.Dock = DockStyle.Bottom;
                        marqueeProgressBarControl.Width = 599;
                        marqueeProgressBarControl.Height = 46;
                        marqueeProgressBarControl.Show();
                    }));
                }
            };
            IProgress<int> progressLine = new Progress<int>(value =>
            {
                ProgressBarControl pb = pbw.GetProgressBarLine();
                if (!pb.IsDisposed)
                {
                    pb.Invoke((MethodInvoker)(() =>
                    {
                        pb.EditValue = value;
                    }));
                }
            });
            IProgress<int> progressTotal = new Progress<int>(value =>
            {
                ProgressBarControl pb = pbw.GetProgressBarTotal();
                if (!pbw.IsDisposed)
                {
                    pbw.Invoke((MethodInvoker)(() =>
                    {
                        pbw.SetTitle("Проверка консистентности выполняется...");
                    }));
                }
                if (!pb.IsDisposed)
                {
                    pb.Invoke((MethodInvoker)(() =>
                    {
                        pb.EditValue = value;
                    }));
                }
            });
            pbw.FormClosed += (a, e) => { source.Cancel(); };
            // отключаем кнопки на риббоне
            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, false);
            pbw.Show();
            pbw.Focus();
            try
            {
                Task<(ConcurrentDictionary<string, string>, ConcurrentDictionary<string, string>)> task = Task.Run(() =>
                RunVerifier(
                    actualCheckedFiles,
                    (fileName) =>
                    {
                        if (pbw != null && pbw.IsDisposed != true)
                        {
                            pbw.Invoke((MethodInvoker)(() =>
                            {
                                pbw.GetLabel().Text = fileName;
                                pbw.GetLabel().Update();
                            }));
                        }
                    },
                    () =>
                    {
                        if (pbw != null && pbw.IsDisposed != true)
                        {
                            pbw.Invoke((MethodInvoker)(() =>
                            {
                                pbw.Close();
                            }));
                        }
                        // включаем кнопки на риббоне
                        ribbonControl.Invoke((MethodInvoker)(() =>
                        {
                            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
                        }));
                    },
                    progressLine,
                    progressTotal,
                    changeProgressLine,
                    showValidationErrors,
                    source.Token
                    ), source.Token);
                task.ContinueWith(taskc =>
                {
                    if (taskc.Exception == null && taskc.IsFaulted == false && !source.IsCancellationRequested)
                    {
                        EndVerifier(taskc,
                                    () =>
                                    {
                                        if (pbw != null && pbw.IsDisposed != true)
                                        {
                                            pbw.Invoke((MethodInvoker)(() =>
                                            {
                                                pbw.Close();
                                            }));
                                        }
                                        // включаем кнопки на риббоне
                                        ribbonControl.Invoke((MethodInvoker)(() =>
                                        {
                                            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
                                        }));
                                    }
                                    , actualCheckedFiles, ribbonControl, showResultWindow);
                    }
                });
            }
            catch (TaskCanceledException)
            {
                FormsHelper.ShowStyledMessageBox("Ошибка!", "Операция отменена!");
            }
        }
        private (ConcurrentDictionary<string, string> verificationErrors, ConcurrentDictionary<string, string> dependencyErrors) RunVerifier(
            ConcurrentDictionary<string, FileInfo> actualCheckedFiles,
            Action<string> addFileToView,
            Action closeProgressBar,
            IProgress<int> progressLine,
            IProgress<int> progressTotal,
            Action changeProgressLine,
            Action<string> showValidationErrors,
            CancellationToken ct)
        {
            Verifier verifier = new Verifier();
            ConcurrentDictionary<string, string> dependencyErrorDict = new ConcurrentDictionary<string, string>();
            try
            {
                List<string> filesActual = new List<string>();
                string directoryActual = "";
                for (int i = 0; i <= actualCheckedFiles.Count - 1; i++)
                {
                    progressLine.Report(i);
                    //ct.ThrowIfCancellationRequested();
                    string fileName = actualCheckedFiles[actualCheckedFiles.Keys.ElementAt(i)].Name;
                    string xmlFilePath = actualCheckedFiles[actualCheckedFiles.Keys.ElementAt(i)].FullName;
                    string nm = fileName.Substring(0, fileName.Count() - 4);
                    // добавляем имя файла
                    filesActual.Add(nm);
                    // директорию
                    directoryActual = actualCheckedFiles[actualCheckedFiles.Keys.ElementAt(i)].DirectoryName;
                    string name = nm + ".xsd";
                    string xsdFilePath = Directory.GetCurrentDirectory() + @"\XSD\" + name;
                    // добавляем в отображение прогрессбара
                    addFileToView(fileName);
                    // верифицируем файл
                    verifier.VerifySingleFile(xsdFilePath, xmlFilePath, ct);
                }
                changeProgressLine();
                progressTotal.Report(1);
                // построим список зависимостей
                List<ParentTable> parentTables = GetDependencyMap();
                // ищем некорреляции
                dependencyErrorDict = SearchDependencies(addFileToView, directoryActual, filesActual, parentTables, ct);
                progressTotal.Report(2);
            }
            catch (VerifyException ve)
            {
                closeProgressBar();
                ConcurrentDictionary<string, string> errord = ve.errorDict;
                StringBuilder sb = new StringBuilder();
                foreach (var item in ve.errorDict)
                {
                    sb.Append(item.Value);
                }
                CommonRepository.GetLogger().Error(sb.ToString());
                showValidationErrors(sb.ToString());
            }
            catch (OperationCanceledException)
            {
                closeProgressBar();
            }
            catch (Exception ex)
            {
                CommonRepository.GetLogger().Error(ex.ToString());
                closeProgressBar();
                showValidationErrors(ex.ToString());
            }
            return (verifier.errorDict, dependencyErrorDict);
        }
        [STAThreadAttribute]
        private void EndVerifier(
            Task<(ConcurrentDictionary<string, string> verificationErrors, ConcurrentDictionary<string, string> dependencyErrors)> task,
            Action closeProgressBar,
            ConcurrentDictionary<string, FileInfo> actualCheckedFiles,
            RibbonControl ribbonControl,
            Action<(List<TableInfo> tableInfos,
                ConcurrentDictionary<string, string> dependencyErrors)> showResultWindow)
        {
            closeProgressBar();
            ConcurrentDictionary<string, string> result = task.Result.verificationErrors;
            List<TableInfo> idata = MakeInfoData(result, "Проверено, ошибок нет.", actualCheckedFiles);
            showResultWindow((idata, task.Result.dependencyErrors));
            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
        }
        private List<TableInfo> MakeInfoData(ConcurrentDictionary<string, string> result, string successStatus, ConcurrentDictionary<string, FileInfo> actualCheckedFiles)
        {
            var resultData = new List<TableInfo>();
            foreach (var check in actualCheckedFiles)
            {
                string tableName = Path.GetFileNameWithoutExtension(check.Key);
                TableInfo ti = new TableInfo();
                ti.Name = tableName;
                ti.Description = Globals.TABLES_INFO[tableName];
                if (result.ContainsKey(tableName))
                {
                    ti.Status = result[tableName];
                }
                else
                {
                    ti.Status = successStatus;
                }
                resultData.Add(ti);
            }
            return resultData;
        }
        /// <summary>
        /// Построение списка зависимостей по БД для таблиц из списка Globals.TABLES_NAMES
        /// </summary>
        /// <returns></returns>
        public List<ParentTable> GetDependencyMap()
        {
            List<ParentTable> parentTables = new List<ParentTable>();
            DataConnection dc = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    foreach (var parentTable in Globals.TABLES_NAMES)
                    {
                        List<DependantTable> query = db.Query<DependantTable>(string.Format(Globals.DependancyQuery, parentTable)).ToList();
                        if (query != null && query.Any())
                        {
                            ParentTable ptable = new ParentTable();
                            ptable.ParentTableName = parentTable;
                            ptable.DependantTableInfo = query;
                            parentTables.Add(ptable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
            return parentTables;
        }

        /// <summary>
        /// Поиск некореллирующих связок внешний ключ - первичный ключ
        /// Возвращает словарь таблица - ошибки некорреляций в текстовом виде
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="selectedTables"></param>
        /// <param name="dependentTables"></param>
        /// <returns></returns>
        public ConcurrentDictionary<string, string> SearchDependencies(Action<string> addFileToView, string directoryPath, List<string> selectedTables, List<ParentTable> dependentTables, CancellationToken ct)
        {
            ConcurrentDictionary<string, string> errorClasses = new ConcurrentDictionary<string, string>();
            // проходим по списку выбранных файлов
            foreach (string table in selectedTables)
            {
                addFileToView(table);
                ct.ThrowIfCancellationRequested();
                string filePath = Path.Combine(directoryPath, table + ".xml");
                // находим зависимости для выбраного файла по карте зависимостей
                ParentTable tableDependencies = dependentTables.Where(x => x.ParentTableName == table).FirstOrDefault();
                // пропускаем таблицы без внешних ключей
                if (tableDependencies == null) continue;
                // выбрать "свои"(родительские) ключи
                var parentForeignKeysNames = tableDependencies.DependantTableInfo.Select(x => "ns1:" + x.ParentColumn).ToList();
                // загрузить родительские ключи в словарь
                var parentForeignKeys = GetGuidsFromXML(filePath, table, parentForeignKeysNames);
                // проходим по зависимостям
                foreach (DependantTable tableDependency in tableDependencies.DependantTableInfo)
                {
                    ct.ThrowIfCancellationRequested();
                    string dependantTableName = tableDependency.DependantTableName;
                    var childFilePath = Path.Combine(directoryPath, dependantTableName + ".xml");
                    // если такого файла не существует, пропускаем
                    if (!File.Exists(childFilePath)) continue;
                    var childKeys = GetGuidsFromXML(childFilePath, dependantTableName, new List<string>() { "ns1:" + tableDependency.DependantTableColumn });
                    // ищем отсутствие ключей родительской таблицы для соответствующих внешних
                    if (!parentForeignKeys.ContainsKey("ns1:" + tableDependency.ParentColumn)) continue;
                    foreach (var pfk in parentForeignKeys["ns1:" + tableDependency.ParentColumn])
                    {
                        ct.ThrowIfCancellationRequested();
                        if (!childKeys["ns1:" + tableDependency.DependantTableColumn].Contains(pfk))
                        {
                            StringBuilder sb = new StringBuilder();
                            IEnumerable<string> dups = FindDuplicateKeys(childKeys);
                            if (dups != null && dups.Count() != 0)
                            {
                                sb.AppendLine(String.Format("таблица {0} содержит повторяющиеся значения первичного ключа {1}: {2}", table, tableDependency.ParentColumn, pfk, dependantTableName));
                            }
                            sb.AppendLine(String.Format("Для таблицы {0} и внешнего ключа {1} = {2} нет соответствующей записи в таблице {3}", table, tableDependency.ParentColumn, pfk, dependantTableName));
                            if (errorClasses.ContainsKey(table))
                            {
                                errorClasses[table] += sb.ToString();
                            }
                            else
                            {
                                errorClasses.TryAdd(table, sb.ToString());
                            }
                            CommonRepository.GetLogger().Info(sb.ToString());
                        }
                    }
                }
            }
            return errorClasses;
        }

        /// <summary>
        /// Поиск дублей
        /// </summary>
        /// <param name="keyValueDict"></param>
        /// <returns></returns>
        public IEnumerable<string> FindDuplicateKeys(Dictionary<string, List<string>> keyValueDict)
        {
            IEnumerable<string> duplicates = new List<string>();
            duplicates = keyValueDict?.FirstOrDefault().Value.GroupBy(x => x).Where(y => y.Count() > 1).Select(x => x.Key);
            return duplicates;
        }

        /// <summary>
        /// Извлечение заданных элементов из XML. Возвращает словарь: Внешний ключ - список значений
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tablename"></param>
        /// <param name="fkeys"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetGuidsFromXML(string filename, string tablename, List<string> fkeys)
        {
            Dictionary<string, List<string>> fkeysNamesValues = new Dictionary<string, List<string>>();
            using (XmlReader reader = XmlReader.Create(filename))
            {
                reader.ReadToDescendant("ns1:" + tablename);
                do
                {
                    reader.Read();
                    string elementName = reader.Name.ToString();
                    if (reader.NodeType == XmlNodeType.Element && fkeys.Contains(elementName))
                    {
                        // если такой список гуидов уже есть и там что-то есть
                        if (fkeysNamesValues.Keys.Contains(elementName) &&
                            fkeysNamesValues[elementName] != null &&
                            fkeysNamesValues[elementName].Count != 0)
                        {
                            fkeysNamesValues[elementName].Add(reader.ReadString());
                        }
                        // если нет такого ещё
                        else
                        {
                            List<string> guids = new List<string>();
                            guids.Add(reader.ReadString());
                            fkeysNamesValues[elementName] = guids;
                        }
                    }

                } while (!reader.EOF);
            }
            return fkeysNamesValues;
        }
        public void ImportFiles(
            ImportGridPanel importGridPanel,
            RibbonControl ribbonControl,
            BulkManager bulkManager,
            List<ImportXMLFilesDto> checkedFiles,
            Action showDeleteImport,
            Action<DeserializeException> deserializeErrorCallback,
            Action<BulkException> bulkErrorCallback,
            Action<Exception> exceptionCallback,
            Action<Dictionary<string, long>> showResultWindow,
            Action cancelCallback
            )
        {
            ConcurrentDictionary<string, FileInfo> actualCheckedFiles = new ConcurrentDictionary<string, FileInfo>();
            ILookup<string, FileInfo> fileInfos = LoadedFiles.ToLookup(k => Path.GetFileName(k.Key), k => k.Value);
            foreach (var file in checkedFiles)
            {
                string nnn = fileInfos[file.Name].Select(x => x.FullName).First();
                FileInfo info = fileInfos[file.Name].Select(x => x).First();
                actualCheckedFiles.TryAdd(nnn, info);
            }
            CancellationTokenSource source = new CancellationTokenSource();
            ProgressBarWindow pbw = new ProgressBarWindow();
            pbw.SetCancellationToken(source);
            pbw.SetTitle("Импорт выполняется...");
            ProgressBarControl pbarTotal = pbw.GetProgressBarTotal();
            ProgressBarControl pbarLine = pbw.GetProgressBarLine();
            pbarTotal.Properties.Maximum = checkedFiles.Count;
            LabelControl plabel = pbw.GetLabel();
            Action changeProgressLine = () =>
            {
                if (!pbw.IsDisposed)
                {
                    pbw.Invoke((MethodInvoker)(() =>
                    {
                        Control panel = pbw.Controls.Find("panelControl1", true).FirstOrDefault();
                        Control pline = panel.Controls.Find("progressBarLine", true).FirstOrDefault();
                        panel.Controls.Remove(pline);
                        MarqueeProgressBarControl marqueeProgressBarControl = new MarqueeProgressBarControl();
                        panel.Controls.Add(marqueeProgressBarControl);
                        marqueeProgressBarControl.Dock = DockStyle.Bottom;
                        marqueeProgressBarControl.Width = 599;
                        marqueeProgressBarControl.Height = 46;
                        marqueeProgressBarControl.Show();
                    }));
                }
            };
            IProgress<int> progressLine = new Progress<int>(value =>
            {
                ProgressBarControl pb = pbw.GetProgressBarLine();
                if (!pb.IsDisposed)
                {
                    pb.Invoke((MethodInvoker)(() =>
                    {
                        pb.EditValue = value;
                    }));
                }
            });
            IProgress<int> progressTotal = new Progress<int>(value =>
            {
                ProgressBarControl pb = pbw.GetProgressBarTotal();
                if (!pbw.IsDisposed)
                {
                    pbw.Invoke((MethodInvoker)(() =>
                    {
                        pbw.SetTitle("Проверка консистентности выполняется...");
                    }));
                }
                if (!pb.IsDisposed)
                {
                    pb.Invoke((MethodInvoker)(() =>
                    {
                        pb.EditValue = value;
                    }));
                }
            });
            pbw.FormClosed += (a, e) => { source.Cancel(); };
            // отключаем кнопки на риббоне
            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, false);
            pbw.Show();
            pbw.Focus();
            try
            {
                Task<ConcurrentDictionary<string, Tuple<string, long, TimeSpan>>> task = Task.Run(() =>
                RunImport(
                    actualCheckedFiles,
                    bulkManager,
                    (fileName) =>
                    {
                        if (pbw != null && pbw.IsDisposed != true)
                        {
                            pbw.Invoke((MethodInvoker)(() =>
                            {
                                pbw.GetLabel().Text = fileName;
                                pbw.GetLabel().Update();
                            }));
                        }
                    },
                    () =>
                    {
                        if (pbw != null && pbw.IsDisposed != true)
                        {
                            pbw.Invoke((MethodInvoker)(() =>
                            {
                                pbw.Close();
                            }));
                        }
                        // включаем кнопки на риббоне
                        ribbonControl.Invoke((MethodInvoker)(() =>
                        {
                            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
                        }));
                    },
                    progressLine,
                    progressTotal,
                    changeProgressLine,
                    showDeleteImport,
                    deserializeErrorCallback,
                    bulkErrorCallback,
                    cancelCallback,
                    exceptionCallback,
                    source.Token
                    ), source.Token);
                task.ContinueWith(taskc =>
                {
                    if (taskc.Exception == null && taskc.IsFaulted == false && !source.IsCancellationRequested)
                    {
                        EndImport(
                            () =>
                            {
                                if (pbw != null && pbw.IsDisposed != true)
                                {
                                    pbw.Invoke((MethodInvoker)(() =>
                                    {
                                        pbw.Close();
                                    }));
                                }
                                // включаем кнопки на риббоне
                                ribbonControl.Invoke((MethodInvoker)(() =>
                                {
                                    FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
                                }));
                            },
                            showResultWindow);
                    }
                });
            }
            catch (TaskCanceledException)
            {
                FormsHelper.ShowStyledMessageBox("Ошибка!", "Операция отменена!");
            }
        }
        public ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> RunImport(
            ConcurrentDictionary<string, FileInfo> actualCheckedFiles,
            BulkManager bm,
            Action<string> addFileToView,
            Action closeProgressBar,
            IProgress<int> progressLine,
            IProgress<int> progressTotal,
            Action changeProgressLine,
            Action showDeleteImport,
            Action<DeserializeException> deserializeErrorCallback,
            Action<BulkException> bulkErrorCallback,
            Action cancelCallback,
            Action<Exception> exceptionCallback,
            CancellationToken token)
        {
            this.ImportStatistics.Clear();
            ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> importStatus = new ConcurrentDictionary<string, Tuple<string, long, TimeSpan>>();
            try
            {
                int i = 0;
                int progressTotalCount = 0;
                for (; i <= actualCheckedFiles.Count - 1; i++)
                {
                    progressLine.Report(i);
                    token.ThrowIfCancellationRequested();
                    string tableName = Globals.TABLES_NAMES.Where(x => x.Contains(actualCheckedFiles.ElementAt(i).Value.Name)).First();
                    string xmlFilePath = actualCheckedFiles.ElementAt(i).Key;
                    changeProgressLine();
                    // если нет в статистике, то считаем элементы и добавляем (это чтобы не считалось заново для кусков)
                    if (!ImportStatistics.ContainsKey(tableName))
                    {
                        long countElements = GetElementsCount(xmlFilePath, "ns1:" + tableName);
                        ImportStatistics.Add(tableName, countElements);
                    }
                    bm.BulkStartNew(tableName, xmlFilePath, (e, v) =>
                    {
                        SqlRowsCopiedEventArgs rowsevent = v as SqlRowsCopiedEventArgs;
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            addFileToView(tableName);
                        }
                        catch (OperationCanceledException)
                        {
                            closeProgressBar();
                            showDeleteImport();
                        }
                    }
                    , importStatus);
                }
                // меняем тип прогрессбара
                //if (!plabel.IsDisposed)
                //{
                //    plabel.Invoke((MethodInvoker)(() => plabel.Text = "Табличное преобразование..."));
                //}
                changeProgressLine();
                // передвигаем общий прогресс
                progressTotal.Report(progressTotalCount);
                // TODO: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                bm.RunStoredSynchronize();
                // TODO: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                DataStatTable = bm.PrepareStatistics(ImportStatistics);
                // удаляем временные таблицы
                if (Globals.frmSettings.PuraSurEraro)
                {
                    CommonRepository.DeleteLoaderTables();
                }
            }
            catch (DeserializeException de)
            {
                closeProgressBar();
                deserializeErrorCallback(de);
            }
            catch (BulkException be)
            {
                closeProgressBar();
                bulkErrorCallback(be);
            }
            catch (OperationCanceledException)
            {
                closeProgressBar();
                cancelCallback();
            }
            catch (Exception ex)
            {
                closeProgressBar();
                exceptionCallback(ex);
            }
            return bm.errorDict;
        }

        /// <summary>
        /// Подсчёт количества элементов
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="element"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public long GetElementsCount(string filename, string element)
        {
            long countElements = 0;
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.ReadToFollowing(element))
                    {
                        countElements += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string errorStr = string.Format("При чтении xml файла {0} произошла ошибка {1}.", filename, ex.ToString());
                CommonRepository.GetLogger().Error(errorStr);
                throw new Exception(errorStr);
            }
            return countElements;
        }

        private void EndImport(Action closeProgressBar, Action<Dictionary<string, long>> showResultWindow)
        {
            closeProgressBar();
            showResultWindow(ImportStatistics);
        }
    }
}
