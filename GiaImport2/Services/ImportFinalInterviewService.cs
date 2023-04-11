using DevExpress.Utils.Extensions;
using GiaImport2.DataModels;
using GiaImport2.Enumerations;
using GiaImport2.Models;
using GiaImport2.Services;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using MFtcFinalInterview;
using MFtcPck.FtcXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Container = SimpleInjector.Container;

namespace GiaImport2
{
    public partial class ImportFinalInterviewService
    {
        private const string Message = "Файл не является валидным B2P файлом итогового собеседования";
        private string ImpFolder;
        private string fileName;
        private int participantsLoaded;
        private FinalInterview finalInterview;
        private BarcodeGenerator barcodeGenerator;

        public FilesInfo filesInfo;

        public FilesInfoStats filesInfoStats;
        Guid SessionId;
        private int goodCounter;

        private Action<FilesInfo.FilesInfoContainer> AddFileToView;
        private Action CloseProgressBar;
        private Action<int> ProgressBarLineStep;
        private Action<int> ProgressBarTotalStep;
        private Action<string> ProgressBarSetTitle;
        private Action EnableDisableButtons;
        private Action<string> ProgressBarLabel;

        private CancellationTokenSource source;

        private CacheData CacheData;

        BackgroundWorker bw;

        private readonly ICommonRepository CommonRepository;
        private readonly Container DIContainer;


        public void Init(string importFolder,
            Action<FilesInfo.FilesInfoContainer> addFileToView,
            Action<int> progressBarLineStep,
            Action<int> progressBarTotalStep,
            Action<string> progressBarLabel,
            Action closeProgressBar,
            Action enableDisableButtons,
            Action<string> progressBarSetTitle,
            CancellationTokenSource source
        )
        {
            this.ImpFolder = importFolder;
            this.AddFileToView = addFileToView;
            this.ProgressBarLineStep = progressBarLineStep;
            this.ProgressBarTotalStep = progressBarTotalStep;
            this.EnableDisableButtons = enableDisableButtons;
            this.ProgressBarLabel = progressBarLabel;
            this.source = source;
            this.CloseProgressBar = closeProgressBar;
            this.ProgressBarSetTitle = progressBarSetTitle;

            this.CacheData = new CacheData();

            filesInfo = new FilesInfo();

            filesInfoStats = new FilesInfoStats();
            barcodeGenerator = DIContainer.GetInstance<BarcodeGenerator>();
            this.participantsLoaded = 0;
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
        }

        public ImportFinalInterviewService(Container container, ICommonRepository commonRepository)
        {
            this.DIContainer = container;
            this.CommonRepository = commonRepository;
        }

        protected void DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> allFiles = (List<string>)e.Argument;
            bw.ReportProgress(0);
            goodCounter = 0;
            string status = string.Empty;
            MainProcess(allFiles, e);
            bw.ReportProgress(100);
        }

        protected void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int percentage = e.ProgressPercentage;
            // показываем процент выполнения
            ProgressBarLineStep(percentage);
        }

        protected async void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // закрываем сессию
            if (CommonRepository.TryCloseFinalInterviewSession(SessionId) == false)
            {
                CommonRepository.GetLogger().Error("Невозможно закрыть сессию!");
            }
            if (e.Cancelled)
            {
                MessageBox.Show("Операция загрузки была прервана!");
                // закрываем прогрессбар
                CloseProgressBar();
            }
            else if (e.Error != null)
            {
                FormsHelper.ShowStyledMessageBox("Ошибка!", "Загрузка произошла с ошибками!");
                // закрываем прогрессбар
                CloseProgressBar();
            }
            else
            {
                // подсчёт статистики по количеству файлов
                this.filesInfoStats.FilesCount = goodCounter;

                ProgressBarSetTitle("Импорт данных выполняется...");

                ProgressBarTotalStep(1);
                string gpm = "";
                BulkManager bm = DIContainer.GetInstance<BulkManager>();
                string packMask = bm.GetPackageMask(out gpm);
                if (!string.IsNullOrEmpty(gpm))
                {
                    FormsHelper.ShowStyledMessageBox("Внимание!", "Невозможно получить код региона! Проверьте настройки соединения с БД.");
                    // закрываем прогрессбар
                    CloseProgressBar();
                    return;
                }
                try
                {
                    ProgressBarLabel("Процедура слияния...");
                    ProgressBarTotalStep(80);
                    await Task.Run(() => bm.RunStoredSynchronize());
                    ProgressBarTotalStep(100);
                }
                catch (Exception ex)
                {
                    // закрываем прогрессбар
                    CloseProgressBar();
                    FormsHelper.ShowStyledMessageBox("Внимание!", $"Процедура слияния завершилась некорректно!\n{ex}");
                }
                // закрываем прогрессбар
                CloseProgressBar();

                DataTable dt = BulkManager.PrepareImportXMLStatistics(filesInfo, filesInfoStats);
                ResultForm resultForm = new ResultForm();
                resultForm.SetTitle("Итого");
                ResultStatisticsControl resultStatistics = new ResultStatisticsControl(dt, bm.outLog.ToString());
                resultForm.GetPanelControl().AddControl(resultStatistics);
                resultForm.GetPanelControl().Dock = DockStyle.Fill;
                resultForm.ShowDialog();
                resultForm.Focus();
                //CommonRepository.DeleteLoaderTables(Globals.GetConnectionString());
                // включаем кнопки, фокус
                EnableDisableButtons();
                this.CacheData = null;
            }
        }

        private void MainProcess(List<string> allFiles, DoWorkEventArgs e)
        {
            int i = 1;
            int total = allFiles.Count;
            foreach (var file in allFiles)
            {
                this.fileName = Path.GetFileNameWithoutExtension(file);
                this.finalInterview = new FinalInterview();
                string currentFileInfo = "";

                // отображаем текущий файл
                ProgressBarLabel("this.fileName + \".b2p\"");

                if (bw.CancellationPending || source.IsCancellationRequested) { e.Cancel = true; return; }

                if (ProcessFile(file, out currentFileInfo) == false)
                {
                    filesInfo.Add(this.fileName, FileStatus.ConditionReexport, currentFileInfo);
                    continue;
                }
                var fileContainer = filesInfo.Add(this.fileName, FileStatus.ConditionSavedNoErrors, currentFileInfo);
                AddFileToView(fileContainer);
                // обновляем статус в файле на успешный 20
                this.finalInterview.UpdateFileConditionToSuccess();
                Interlocked.Increment(ref goodCounter);
                // суммируем участников загруженных
                Interlocked.Add(ref this.filesInfoStats.ParticipantsLoaded, this.participantsLoaded);
                int percent = (i * 100) / total;
                bw.ReportProgress(percent);
                i += 1;
            }
        }
        public void ImportAllData()
        {
            StringBuilder errorssb = new StringBuilder();
            StringBuilder successb = new StringBuilder();
            if (!CommonRepository.CheckAccessToFolder(this.ImpFolder))
            {
                FormsHelper.ShowStyledMessageBox("Ошибка доступа!", "Нет доступа к указанному каталогу!");
                // закрываем прогрессбар
                CloseProgressBar();
                return;
            }
            List<string> allFiles = Directory.GetFiles(this.ImpFolder, "*.b2p", SearchOption.AllDirectories).ToList();
            if (allFiles.Count == 0)
            {
                errorssb.Append("Ни одного B2P файла по заданному пути не найдено! ").Append(this.ImpFolder).Append(Environment.NewLine);
                // закрываем прогрессбар
                CloseProgressBar();
                return;
            }
            //this.statusLabel.Text = "Проверка...";
            // предварительная проверка всех файлов на адекватность
            List<string> properFiles = PreCheckAllFiles(allFiles);
            // если есть файлы с другими статусами, то показать результат и диалог
            if (!this.filesInfo.CheckForOK())
            {
                // покажем статистику по предварительной проверке
                ResultForm resultForm = new ResultForm();
                resultForm.SetTitle("Предварительная проверка файлов");
                PreCheckControl preCheckControl = new PreCheckControl();
                resultForm.GetPanelControl().AddControl(preCheckControl);
                resultForm.GetPanelControl().Dock = DockStyle.Fill;
                preCheckControl.GetGridControl().DataSource = MakeInfoData();
                resultForm.ShowDialog();
                resultForm.Focus();
                DialogResult dlg = FormsHelper.ShowStyledYesNoMessageBox("Продолжить загрузку?", "Подтверждение загрузки");
                if (dlg == DialogResult.No)
                {
                    CloseProgressBar();
                    return;
                }
            }
            // создаём сессию
            if (CommonRepository.TryCreateFinalInterviewSession(out Guid sessionid) == false)
            {
                CommonRepository.GetLogger().Error("Невозможно создать сессию!");
                FormsHelper.ShowStyledMessageBox("Ошибка!", "Невозможно создать сессию!");
                // закрываем прогрессбар
                CloseProgressBar();
                return;
            }
            this.SessionId = sessionid;

            // Удаляем loader
            //DeleteLoaderPackagesAndSheetsR();

            filesInfo.Clean();
            bw.DoWork += DoWork;
            bw.ProgressChanged += ProgressChanged;
            bw.RunWorkerCompleted += RunWorkerCompleted;
            //statusLabel.Text = "Загрузка B2P файлов итогового собеседования...";
            bw.RunWorkerAsync(properFiles);

            /* Процессинг
             * ---------
             * ----------
             */
        }

        private List<string> PreCheckAllFiles(List<string> allFiles)
        {
            List<string> properFiles = new List<string>();

            int i = 0;
            int total = allFiles.Count;
            ProgressBarLabel("Проверка выполняется...");

            ProgressBarTotalStep(1);

            foreach (var file in allFiles)
            {
                // отображаем текущий файл
                ProgressBarLabel(this.fileName);

                this.finalInterview = new FinalInterview();
                // предварительная проверка файла
                if (PreCheckFileBroken(file) == true)
                {
                    // если файл не очень, то берём следующий
                    continue;
                }
                // проверка на статус
                FinalInterview.FileStatus checkStatus = FinalInterview.FileStatus.NotValidXML;
                try
                {
                    checkStatus = this.finalInterview.GetFileStatus(file);
                }
                catch (Exception)
                {
                    checkStatus = FinalInterview.FileStatus.NotValidXML;
                }
                switch (checkStatus)
                {
                    case FinalInterview.FileStatus.ConditionExported:
                        filesInfo.Add(file, FileStatus.ConditionExported, "Файл не был заполнен на уровне ОО! Загрузка файла невозможна!");
                        break;
                    case FinalInterview.FileStatus.ConditionReexport:
                        filesInfo.Add(file, FileStatus.ConditionReexport, "Файл не был заполнен на уровне ОО! Файл экспортирован/выгружен повторно. Загрузка файла невозможна!");
                        break;
                    case FinalInterview.FileStatus.ConditionSavedNotChecked:
                        filesInfo.Add(file, FileStatus.ConditionSavedNotChecked, "Файл сохранен без проверки на уровне ОО! Загрузка файла невозможна!");
                        break;
                    case FinalInterview.FileStatus.ConditionSavedWithErrors:
                        filesInfo.Add(file, FileStatus.ConditionSavedWithErrors, "Файл сохранён с ошибками после проверки! Загрузка файла невозможна!");
                        break;
                    case FinalInterview.FileStatus.ConditionSavedNoErrors:
                        filesInfo.Add(file, FileStatus.ConditionSavedNoErrors, "Файл сохранён после проверки (без ошибок). Готов к загрузке.");
                        break;
                    case FinalInterview.FileStatus.AlreadyLoaded:
                        filesInfo.Add(file, FileStatus.Success, "Уже были загружены все бланки. Данные будут перезаписаны.");
                        break;
                    default:
                        filesInfo.Add(file, FileStatus.NotValidXML, "Нарушена целостность B2P-файла. Загрузка файла невозможна!");
                        break;
                }
                // предварительная проверка файла
                // Только со статусом проверено и без ошибок
                //if (this.finalInterview.GetFileStatus(file) != FinalInterview.FileStatus.ConditionSavedNoErrors)
                //{
                //    continue;
                //}
                if (checkStatus != FinalInterview.FileStatus.NotValidXML)
                {
                    properFiles.Add(file);
                }
                int percent = (i * 100) / total;
                ProgressBarLineStep(percent);
                i += 1;
            }
            return properFiles;
        }

        private bool PreCheckFileBroken(string file)
        {
            bool error = false;
            this.fileName = Path.GetFileNameWithoutExtension(file);
            Match match = Regex.Match(this.fileName, @"^(\d{8})-(\d{2})-(\d{6})-(\d{3}|\d{3}(.*))-(\d{8})$");
            if (!match.Success)
            {
                filesInfo.Add(file, FileStatus.WrongName, "Файл имеет не корректный формат имени! Загрузка файла невозможна!");
                error = true;
            }
            if (!this.finalInterview.TryCheckFileNameDate(file, match.Groups[match.Groups.Count - 1].Value.ToString()))
            {
                filesInfo.Add(file, FileStatus.WrongName, "Файл имеет не корректный формат имени! Загрузка файла невозможна!");
                error = true;
            }
            // проверка на десериализацию
            bool checkSerialise = false;
            try
            {
                checkSerialise = this.finalInterview.TryDeserializeHWPackageBinary(file);
            }
            catch (Exception)
            {
                checkSerialise = false;
            }
            if (checkSerialise == false)
            {
                filesInfo.Add(file, FileStatus.NotValidXML, Message);
                error = true;
            }
            // проверка на версию
            bool checkVersion = false;
            try
            {
                checkVersion = this.finalInterview.CheckFileVersion(file, Properties.Settings.Default.Base);
            }
            catch (Exception)
            {
                checkVersion = false;
            }
            if (checkVersion == false)
            {
                filesInfo.Add(file, FileStatus.WrongVersion, "Файл создан несовместимой версией HandWriter. Загрузка файла невозможна!");
                error = true;
            }

            if (!CheckHasAnyFinished(file))
            {
                filesInfo.Add(file, FileStatus.NotFinished, "Файл не содержит ни одного явившегося участника на экзамен!");
                error = true;
            }
            return error;
        }

        private bool CheckCRC(string filename)
        {
            if (this.finalInterview.CheckCRC2(filename))
                return true;
            else
                return false;
        }

        private bool CheckHasAnyFinished(string filename)
        {
            FinalInterviewPackage package = new FinalInterview().ParseFile(filename);
            if (package == null)
            {
                CommonRepository.GetLogger().Error(string.Format("Парсинг файла не прошёл для: {0}", filename));
                return false;
            }
            foreach (var item in package.GetParticipantDatas())
            {
                if (item == null)
                {
                    CommonRepository.GetLogger().Error(string.Format("Получение данных не прошло для: {0}", filename));
                    return false;
                }
                // извлекаем всех кто закончил экзамен
                IEnumerable<ParticipantData> participantDatas = item.Where(i => i.HasnotFinished == false);
                // если null или нет таких вообще ни одного
                if (participantDatas == null || (participantDatas != null && !participantDatas.Any()))
                {
                    CommonRepository.GetLogger().Error(string.Format("Нет участников окончивших экзамен для: {0}", filename));
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckHasAuditoriesValidFile(HwPackage package)
        {
            foreach (var page in package.pages)
            {
                foreach (var block in page.blocks)
                {
                    if (Regex.Match(block.BlockName, @"Аудитория\d{2}").Success)
                    {
                        if (!string.IsNullOrWhiteSpace(block.Value) && Regex.Match(block.Value, @"^(\d+|[Нн])$").Success)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool ProcessFile(string fileName, out string currentFileInfo)
        {
            bool success = true;
            currentFileInfo = "";
            // парсим файл
            FinalInterviewPackage finalInterviewPackage = this.finalInterview.ParseFile(fileName);
            this.fileName = finalInterviewPackage.GetHeaderData().FileName;
            List<ParticipantData[]> blockpages = finalInterviewPackage.GetParticipantDatas();
            CommonData commonData = finalInterviewPackage.GetCommonData();
            // забираем ошибки для логирования
            List<string> errors = this.finalInterview.GetErrors();
            if (errors.Count != 0)
            {
                CommonRepository.GetLogger().Error(errors);
            }
            // Дополняем ParticipantsInfo данными по участникам, которых(данных) нет в xml
            GetParticipantsInfo(ref blockpages);
            // проверяем на уже загруженные
            var alreadyImported = CheckForAlreadyImported(commonData, ref blockpages);
            if (alreadyImported == null)
            {
                currentFileInfo = string.Format("ошибка сверки файла с БД!");
                success = false;
                return success;
            }
            if (alreadyImported.Item1 != 0 && alreadyImported.Item1 == alreadyImported.Item2)
            {
                CommonRepository.GetLogger().Error(string.Format("{0} уже загружены для файла {1}: {2}", alreadyImported.Item2, this.fileName, alreadyImported.Item3));
                currentFileInfo = string.Format("Повторная загрузка файла, данные успешно обновлены. Загружены участники: {0} ", alreadyImported.Item2);
                //success = false;
                //return success;
            }
            if (alreadyImported.Item1 != 0 && alreadyImported.Item2 != 0 && alreadyImported.Item1 > alreadyImported.Item2)
            {
                currentFileInfo = string.Format("Участников уже загружено: {0}; ", alreadyImported.Item2);
            }
            // Переходим к созданию таблиц для данного считанного файла
            this.participantsLoaded = 0;
            CreateTablesProcessing(commonData, blockpages);
            // обновляем Condition = 20
            UpdatePackagesSuccessCondition(commonData);
            currentFileInfo += string.Format("Загружено успешно: {0} ", this.participantsLoaded);
            return success;
        }

        private Tuple<int, int, string> CheckForAlreadyImported(CommonData commonData, ref List<ParticipantData[]> blockpages)
        {
            Tuple<int, int, string> result = null;
            int counter = 0;
            int totalCounter = 0;
            StringBuilder sb = new StringBuilder();
            DataConnection dc = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    var query = db.GetTable<sht_Sheets_R>().SchemaName("dbo")
                        .Where(sr => sr.ExamDate == commonData.ExamDate.ToString("yyyy.MM.dd")
                        && sr.TestTypeCode == 9 && sr.SubjectCode == 20)
                        .Select(s => new { s.ExamDate, s.ParticipantID, s.FileName });
                    // считаем всего участников для загрузки
                    foreach (var bp in blockpages)
                    {
                        foreach (var pdata in bp)
                        {
                            if (pdata != null && pdata.SheetId != null && pdata.SheetId != Guid.Empty && !pdata.HasnotFinished)
                            {
                                totalCounter += 1;
                            }
                        }
                    }
                    // считаем совпавших с sht_sheets_AB
                    foreach (var partic in query)
                    {
                        foreach (var bp in blockpages)
                        {
                            foreach (var pdata in bp)
                            {
                                if (pdata != null && partic.ParticipantID == pdata.ParticipantId &&
                                    partic.ExamDate == commonData.ExamDate.ToString("yyyy.MM.dd") &&
                                    partic.FileName == this.fileName)
                                {
                                    sb.Append("SheetID: ")
                                        .Append(pdata.SheetId.ToString()).Append(" - ").Append(pdata.Surname)
                                        .Append(" ").Append(pdata.Name).Append(" ").Append(pdata.Secondname)
                                        .Append(" Документ: ").Append(pdata.DocumentSeries).Append(" ").Append(pdata.DocumentNumber)
                                        .Append(Environment.NewLine);
                                    // помечаем как пуст чтобы не использовать позже в процессинге
                                    //pdata.ParticipantId = Guid.Empty;
                                    counter += 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Fatal(status);
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
            result = new Tuple<int, int, string>(totalCounter, counter, sb.ToString());
            return result;
        }

        private void GetParticipantsInfo(ref List<ParticipantData[]> blockpages)
        {
            DataConnection dc = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    var query = db.GetTable<rbd_Participant>().SchemaName("dbo")
                        .Select(p => new { p.ParticipantID, p.Surname, p.Name, p.SecondName, p.DocumentSeries, p.DocumentNumber, p.Sex });
                    foreach (var partic in query)
                    {
                        foreach (var bp in blockpages)
                        {
                            foreach (var pdata in bp)
                            {
                                if (pdata != null && partic.ParticipantID.Equals(pdata.ParticipantId))
                                {
                                    pdata.Surname = partic.Surname;
                                    pdata.Name = partic.Name;
                                    pdata.Secondname = partic.SecondName;
                                    pdata.DocumentSeries = partic.DocumentSeries;
                                    pdata.DocumentNumber = partic.DocumentNumber;
                                    pdata.Sex = partic.Sex;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
        }

        /// <summary>
        /// Удаляем loader.sht_Sheets_R и loader.sht_Packages
        /// </summary>
        private void DeleteLoaderPackagesAndSheetsR()
        {
            DataConnection dc = null;
            GIA_DB db = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    db.BeginTransaction();
                    db.GetTable<sht_Sheets_R>().SchemaName("loader")
                        .Delete();
                    db.GetTable<sht_Package>().SchemaName("loader")
                        .Delete();
                    db.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
                db.RollbackTransaction();
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
        }
        private void CreateTablesProcessing(CommonData commonData, List<ParticipantData[]> blockpages)
        {
            int i = 1;
            foreach (ParticipantData[] page in blockpages)
            {
                ProcessSinglePage(commonData, page, i);
                i += 1;
            }
        }
        private void ProcessSinglePage(CommonData commonData, ParticipantData[] page, int pageIndex)
        {
            int i = 1;
            foreach (var participantData in page)
            {
                // если есть что-то, то скорее всего есть участник с результатами
                if (!participantData.ParticipantId.Equals(Guid.Empty))
                {
                    ProcessSingleParticipant(commonData, participantData, pageIndex, i);
                    i += 1;
                }
            }
        }

        private void ProcessSingleParticipant(CommonData commonData, ParticipantData participantData, int pageIndex, int participantIndex)
        {
            DataConnection dc = null;
            // убрана проверка признака, потому что по непонятной причине он начал проставляться произвольным образом
            //bool notFinished = participantData.AuditoryCode.Equals("Н") || participantData.AuditoryCode.Equals("н");
            bool notFinished = participantData.HasnotFinished;
            if (participantData.SheetId.Equals(Guid.Empty))
            {
                return;
            }
            GIA_DB db = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    db.BeginTransaction();
                    // так будем различать новое и старое: если true, то либо записи нет вообще, либо sheetid из файла указывает на старую запись с другой датой и именем файла
                    bool oldOrNotExisting = true;
                    Guid newPackageId = Guid.NewGuid();
                    Guid newSheetId = Guid.NewGuid();
                    string newBarcode = this.barcodeGenerator.GetNextBarcode();
                    // ищем существующие записи в бланках dbo
                    IEnumerable<Guid> sheetRIds = db.sht_Sheets_R.SchemaName("dbo")
                        .Where(sr => sr.FileName == this.fileName
                        && sr.ParticipantID == participantData.ParticipantId
                        && sr.SubjectCode == 20
                        && sr.TestTypeCode == 9
                        && sr.ExamDate == commonData.ExamDate.ToString("yyyy.MM.dd"))
                        .Select(srr => srr.SheetID);
                    // уже загружали
                    if (sheetRIds.Count() > 0)
                    {
                        newSheetId = sheetRIds.FirstOrDefault();
                        oldOrNotExisting = false;
                    }
                    // если неявился, то досвидос(удаляем лишние бланки sht_Sheets_R)
                    if (notFinished == true)
                    {
                        // если даты и имена не совпадают, не удаляем старые бланки
                        if (oldOrNotExisting == true)
                        {
                            db.CommitTransaction();
                            return;
                        }
                        // проставляем неявку
                        db.GetTable<sht_Sheets_R>().SchemaName("loader")
                            .Where(r => r.SheetID == newSheetId && r.ExamDate.Equals(commonData.ExamDate.ToString("yyyy.MM.dd"))
                            && r.SubjectCode == commonData.SubjectCode && r.TestTypeCode == 9)
                            .Set((n) => n.Reserve07, "1")
                            .Update();
                        db.GetTable<sht_Sheets_R>().SchemaName("dbo")
                            .Where(r => r.SheetID == newSheetId && r.ExamDate.Equals(commonData.ExamDate.ToString("yyyy.MM.dd"))
                            && r.SubjectCode == commonData.SubjectCode && r.TestTypeCode == 9)
                            .Set((n) => n.Reserve07, "1")
                            .Update();
                        db.CommitTransaction();
                        return;
                    }
                    newPackageId = InsertPackage(commonData, participantData, oldOrNotExisting, db);
                    InsertOrUpdateSheetsR(commonData, participantData, notFinished, db, ref newSheetId, newPackageId, oldOrNotExisting, ref newBarcode);
                    // обновляем данные или вставляем новый бланк AB
                    InsertOrUpdateSheetsAB(commonData, participantData, notFinished, db, newSheetId, newPackageId, newBarcode);
                    // обновляем данные или вставляем новый бланк D
                    InsertOrUpdateSheetsD(commonData, participantData, notFinished, db, newSheetId, newPackageId, newBarcode);
                    // обновляем данные или вставляем новый бланк C
                    InsertOrUpdateSheetsC(commonData, participantData, notFinished, db, newSheetId, newPackageId, newBarcode);
                    // зачёт и итоговый балл
                    InsertOrUpdateMarksAB(commonData, participantData, db, newSheetId);
                    // Если не закончил!
                    if (notFinished)
                    {
                        for (int mark = 0; mark < participantData.Cval.Length - 1; mark++)
                        {
                            // заносим записи с нулём
                            InsertFinalMarksD(commonData, participantData, db, mark, "0", newSheetId, oldOrNotExisting);
                        }
                    }
                    else
                    {
                        // если закончил!
                        // сначала вставляем критерии
                        InsertDs(commonData, participantData, pageIndex, participantIndex, db, newSheetId);
                        // теперь вставляем промежуточные ответы
                        InsertCs(commonData, participantData, pageIndex, participantIndex, db, newSheetId);
                    }
                    // коммитим транзакцию
                    db.CommitTransaction();
                }
                this.participantsLoaded += 1;
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Fatal(status);
                db.RollbackTransaction();
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
        }

        private void InsertCs(CommonData commonData, ParticipantData participantData, int pageIndex, int participantIndex, GIA_DB db, Guid newSheetId)
        {
            List<sht_Marks_C> marks_Cs = new List<sht_Marks_C>();
            List<sht_FinalMarks_C> finalmarks_Cs = new List<sht_FinalMarks_C>();
            for (int answer = 0; answer < participantData.Dval.Length; answer++)
            {
                string answerValue = string.Empty;
                // проверяем на лажу в оценке
                if (string.IsNullOrEmpty(participantData.Dval[answer]) || !participantData.Dval[answer].Any(c => char.IsDigit(c)))
                {
                    answerValue = "0";
                }
                else
                {
                    answerValue = participantData.Dval[answer];
                }
                marks_Cs.Add(new sht_Marks_C()
                {
                    REGION = commonData.Region,
                    MarkID = Guid.NewGuid(),
                    SheetFK = newSheetId,
                    ProtocolFile = this.fileName,
                    ProtocolCode = pageIndex.ToString(),
                    ProtocolCRC = commonData.PackageId.ToString(),
                    ThirdCheck = false,
                    RowNumber = participantIndex,
                    TaskNumber = answer + 1,
                    MarkValue = answerValue
                });
                finalmarks_Cs.Add(new sht_FinalMarks_C()
                {
                    REGION = commonData.Region,
                    MarkID = Guid.NewGuid(),
                    SheetFK = newSheetId,
                    TaskNumber = answer + 1,
                    MarkValue = answerValue
                });
            }
            db.sht_Marks_C.SchemaName("loader")
            .Merge()
            .Using(marks_Cs)
            .On((target, source) => target.SheetFK == source.SheetFK &&
                                    target.TaskNumber == source.TaskNumber &&
                                    target.ProtocolFile == source.ProtocolFile)
            .UpdateWhenMatched()
            .InsertWhenNotMatched()
            .Merge();
            db.sht_FinalMarks_C.SchemaName("loader")
            .Merge()
            .Using(finalmarks_Cs)
            .On((target, source) => target.SheetFK == source.SheetFK &&
                                    target.TaskNumber == source.TaskNumber)
            .UpdateWhenMatched()
            .InsertWhenNotMatched()
            .Merge();
        }

        private void InsertDs(CommonData commonData, ParticipantData participantData, int pageIndex, int participantIndex, GIA_DB db, Guid newSheetId)
        {
            List<sht_Marks_D> marks_Ds = new List<sht_Marks_D>();
            List<sht_FinalMarks_D> finalmarks_Ds = new List<sht_FinalMarks_D>();
            for (int mark = 0; mark < participantData.Cval.Length; mark++)
            {
                string markValue = string.Empty;
                // проверяем на лажу в оценке
                if (string.IsNullOrEmpty(participantData.Cval[mark]) || !participantData.Cval[mark].Any(c => char.IsDigit(c)))
                {
                    markValue = "0";
                }
                else
                {
                    markValue = participantData.Cval[mark];
                }
                marks_Ds.Add(new sht_Marks_D()
                {
                    REGION = commonData.Region,
                    MarkID = Guid.NewGuid(),
                    SheetFK = newSheetId,
                    ProtocolFile = this.fileName,
                    ProtocolCode = pageIndex.ToString(),
                    ProtocolCRC = commonData.PackageId.ToString(),
                    ThirdCheck = false,
                    RowNumber = participantIndex,
                    TaskNumber = mark + 1,
                    MarkValue = markValue
                });
                finalmarks_Ds.Add(new sht_FinalMarks_D()
                {
                    REGION = commonData.Region,
                    MarkID = Guid.NewGuid(),
                    SheetFK = newSheetId,
                    TaskNumber = mark + 1,
                    MarkValue = markValue
                });
            }
            db.sht_Marks_D.SchemaName("loader")
            .Merge()
            .Using(marks_Ds)
            .On((target, source) => target.SheetFK == source.SheetFK &&
                                    target.TaskNumber == source.TaskNumber &&
                                    target.ProtocolFile == source.ProtocolFile)
            .UpdateWhenMatched()
            .InsertWhenNotMatched()
            .Merge();
            db.sht_FinalMarks_D.SchemaName("loader")
            .Merge()
            .Using(finalmarks_Ds)
            .On((target, source) => target.SheetFK == source.SheetFK &&
                                    target.TaskNumber == source.TaskNumber)
            .UpdateWhenMatched()
            .InsertWhenNotMatched()
            .Merge();
        }

        private void InsertFinalMarksD(CommonData commonData, ParticipantData participantData, GIA_DB db, int mark, string markValue, Guid newSheetId, bool oldOrEmpty)
        {
            // если лажа, то просто вставляем новое
            if (oldOrEmpty)
            {
                // insert, т.к. нету ничего
                db.sht_FinalMarks_D.SchemaName("loader").Insert(() => new sht_FinalMarks_D
                {
                    REGION = commonData.Region,
                    MarkID = Guid.NewGuid(),
                    SheetFK = newSheetId,
                    TaskNumber = mark + 1,
                    MarkValue = markValue
                });
            }
            else
            {
                // ищем существующие записи 
                IEnumerable<Guid> finalMarkDIds = db.sht_FinalMarks_D.SchemaName("loader")
                .Where(fmd => fmd.SheetFK == newSheetId && fmd.TaskNumber == mark + 1)
                .Select(sabb => sabb.MarkID);
                if (finalMarkDIds.Count() == 0)
                {
                    // insert, т.к. нету ничего
                    db.sht_FinalMarks_D.SchemaName("loader").Insert(() => new sht_FinalMarks_D
                    {
                        REGION = commonData.Region,
                        MarkID = Guid.NewGuid(),
                        SheetFK = newSheetId,
                        TaskNumber = mark + 1,
                        MarkValue = markValue
                    });
                    return;
                }
                // если больше одной записи, это плохо, это дубль
                if (finalMarkDIds.Count() > 1)
                {
                    CommonRepository.GetLogger().Error(string.Format("Более одной записи sht_FinalMarks_D для sheetfk: {0}", participantData.SheetId));
                    // пофиг, берём первую попавшуюся (может быть удалять?)
                }
                Guid finalMarkDId = finalMarkDIds.FirstOrDefault();
                if (finalMarkDId != null && !finalMarkDId.Equals(Guid.Empty))
                {
                    // update, т.к. нашли MarkID
                    db.sht_FinalMarks_D.SchemaName("loader")
                        .Where(fmd => fmd.SheetFK == newSheetId && fmd.MarkID == finalMarkDId)
                        .Set((s) => s.MarkValue, markValue)
                        .Update();
                }
            }
        }

        private void InsertOrUpdateSheetsD(CommonData commonData, ParticipantData participantData, bool notFinished, GIA_DB db, Guid newSheetId, Guid newPackageId, string newBarcode)
        {
            List<sht_Sheets_D> sht_Sheets_Ds = new List<sht_Sheets_D>();
            var sheet_D = new sht_Sheets_D()
            {
                REGION = commonData.Region,
                SheetID = newSheetId,
                PackageFK = newPackageId,
                FileName = this.fileName,
                RegionCode = commonData.Region,
                DepartmentCode = this.fileName.Substring(0, 4),
                TestTypeCode = 9,
                SubjectCode = commonData.SubjectCode,
                ExamDate = commonData.ExamDate.ToString("yyyy.MM.dd"),
                StationCode = commonData.SchoolCode,
                AuditoriumCode = notFinished ? "" : participantData.AuditoryCode,
                SheetCode = int.Parse(newBarcode.Substring(5, 7)),
                IsEmpty = false,
                ImageNumber = 1,
                VariantCode = notFinished ? 0 : participantData.VariantCode,
                Condition = 20,
                ProjectBatchID = -1,
                Reserve01 = participantData.ExpertSurname,
                Barcode = newBarcode.ReplaceAt(2, '0')
            };
            sht_Sheets_Ds.Add(sheet_D);
            db.sht_Sheets_D.SchemaName("loader")
            .Merge()
            .Using(sht_Sheets_Ds)
            .On((target, source) => target.SheetID == source.SheetID &&
                                    target.PackageFK == source.PackageFK &&
                                    target.FileName == source.FileName &&
                                    target.ExamDate == source.ExamDate)
            .UpdateWhenMatched()
            .InsertWhenNotMatched()
            .Merge();
        }

        private void InsertOrUpdateSheetsC(CommonData commonData, ParticipantData participantData, bool notFinished, GIA_DB db, Guid newSheetId, Guid newPackageId, string newBarcode)
        {
            List<sht_Sheets_C> sht_Sheets_Cs = new List<sht_Sheets_C>();
            var sheet_C = new sht_Sheets_C()
            {
                REGION = commonData.Region,
                SheetID = newSheetId,
                PackageFK = newPackageId,
                FileName = this.fileName,
                RegionCode = commonData.Region,
                DepartmentCode = this.fileName.Substring(0, 4),
                TestTypeCode = 9,
                SubjectCode = commonData.SubjectCode,
                ExamDate = commonData.ExamDate.ToString("yyyy.MM.dd"),
                StationCode = commonData.SchoolCode,
                AuditoriumCode = notFinished ? "" : participantData.AuditoryCode,
                SheetCode = int.Parse(newBarcode.Substring(5, 7)),
                IsEmpty = false,
                ImageNumber = 1,
                VariantCode = notFinished ? 0 : participantData.VariantCode,
                Condition = 20,
                ProjectBatchID = -1,
                Reserve01 = participantData.ExpertSurname,
                Barcode = newBarcode.ReplaceAt(2, '2')
            };
            sht_Sheets_Cs.Add(sheet_C);
            db.sht_Sheets_C.SchemaName("loader")
            .Merge()
            .Using(sht_Sheets_Cs)
            .On((target, source) => target.SheetID == source.SheetID &&
                                    target.PackageFK == source.PackageFK &&
                                    target.FileName == source.FileName &&
                                    target.ExamDate == source.ExamDate)
            .UpdateWhenMatched()
            .InsertWhenNotMatched()
            .Merge();
        }

        private Guid InsertPackage(CommonData commonData, ParticipantData participantData, bool oldOrNotExisting, GIA_DB db)
        {
            Guid result = Guid.NewGuid();
            // ищем существующие записи 
            IEnumerable<Guid> packagesIds = db.sht_Packages.SchemaName("loader")
                .Where(pa => pa.ExamDate == commonData.ExamDate.ToString("yyyy.MM.dd")
                && pa.FileName == this.fileName
                && pa.SubjectCode == 20
                && pa.TestTypeCode == 9
                )
                .Select(paa => paa.PackageID);
            // уже есть, юзаем этот пакет
            if (packagesIds.Count() == 1)
            {
                result = packagesIds.FirstOrDefault();
            }
            // если больше одной записи, это плохо, это дубль
            else
            if (packagesIds.Count() > 1)
            {
                CommonRepository.GetLogger().Error(string.Format("Более одной записи sht_Packages для FileName: {0}", this.fileName));
                // пофиг, берём первую попавшуюся (может быть удалять?)
            }
            else
            if (packagesIds.Count() == 0)
            {
                // ищем существующие записи dbo
                packagesIds = db.sht_Packages.SchemaName("dbo")
                    .Where(pa => pa.ExamDate == commonData.ExamDate.ToString("yyyy.MM.dd")
                              && pa.FileName == this.fileName
                              && pa.SubjectCode == 20
                              && pa.TestTypeCode == 9
                           )
                    .Select(paa => paa.PackageID);
                if (packagesIds.Count() == 0)
                {
                    // insert, т.к. нету ничего
                    db.sht_Packages.SchemaName("loader").Insert(() => new sht_Package
                    {
                        REGION = commonData.Region,
                        PackageID = result,
                        FileName = this.fileName,
                        RegionCode = commonData.Region,
                        DepartmentCode = this.fileName.Substring(0, 4),
                        TestTypeCode = 9,
                        SubjectCode = commonData.SubjectCode,
                        ExamDate = commonData.ExamDate.ToString("yyyy.MM.dd"),
                        StationCode = commonData.SchoolCode,
                        AuditoriumCode = "-1",
                        Condition = 20,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        SheetsCount = 0,
                        IsExported = true,
                        ProjectBatchID = -1
                    });
                }
                else
                {
                    // найдено в dbo, вставляем такую же в loader(необходимо для правильной работы конвертовалки
                    result = packagesIds.FirstOrDefault();
                    // insert, т.к. нету ничего
                    db.sht_Packages.SchemaName("loader").Insert(() => new sht_Package
                    {
                        REGION = commonData.Region,
                        PackageID = result,
                        FileName = this.fileName,
                        RegionCode = commonData.Region,
                        DepartmentCode = this.fileName.Substring(0, 4),
                        TestTypeCode = 9,
                        SubjectCode = commonData.SubjectCode,
                        ExamDate = commonData.ExamDate.ToString("yyyy.MM.dd"),
                        StationCode = commonData.SchoolCode,
                        AuditoriumCode = "-1",
                        Condition = 20,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        SheetsCount = 0,
                        IsExported = true,
                        ProjectBatchID = -1
                    });
                }
            }
            commonData.PackageId = result;
            return result;
        }
        private void InsertOrUpdateSheetsR(CommonData commonData, ParticipantData participantData, bool notFinished, GIA_DB db, ref Guid newSheetId, Guid newPackageId, bool oldOrEmpty, ref string newBarcode)
        {
            // ищем существующие записи 
            IEnumerable<Tuple<Guid, string>> sheetRIds = db.sht_Sheets_R.SchemaName("loader")
                .Where(sar => sar.ParticipantID == participantData.ParticipantId
                && sar.ExamDate == commonData.ExamDate.ToString("yyyy.MM.dd")
                && sar.SubjectCode == 20
                && sar.TestTypeCode == 9
                && sar.FileName == this.fileName)
                .Select(sarr => new Tuple<Guid, string>(sarr.SheetID, sarr.Barcode));
            // если больше одной записи, это плохо, это дубль
            if (sheetRIds.Count() > 1)
            {
                CommonRepository.GetLogger().Error(string.Format("Более одной записи sht_Sheets_R для sheetid: {0}", participantData.SheetId));
                // пофиг, берём первую попавшуюся (может быть удалять?)
            }
            // если лажа, то просто вставляем новое
            if (oldOrEmpty && sheetRIds.Count() == 0)
            {
                // ограничение - нельзя в замыкании использовать ref аргумент, обходим это локальной переменной
                Guid newestSheetID = newSheetId;
                string newestBarcode = newBarcode;
                // insert, т.к. нету ничего
                db.sht_Sheets_R.SchemaName("loader").Insert(() => new sht_Sheets_R
                {
                    REGION = commonData.Region,
                    SheetID = newestSheetID,
                    PackageFK = newPackageId,
                    FileName = this.fileName,
                    Surname = participantData.Surname,
                    Name = participantData.Name,
                    SecondName = participantData.Secondname,
                    Sex = participantData.Sex ? 1 : 0,
                    DocumentSeries = participantData.DocumentSeries,
                    DocumentNumber = participantData.DocumentNumber,
                    ParticipantID = participantData.ParticipantId,
                    RegionCode = commonData.Region,
                    DepartmentCode = this.fileName.Substring(0, 4),
                    TestTypeCode = 9,
                    SubjectCode = commonData.SubjectCode,
                    ExamDate = commonData.ExamDate.ToString("yyyy.MM.dd"),
                    StationCode = commonData.SchoolCode,
                    SchoolCode = commonData.SchoolCode,
                    AuditoriumCode = notFinished ? "" : participantData.AuditoryCode,
                    ImageNumber = 1,
                    VariantCode = notFinished ? 0 : participantData.VariantCode,
                    HasSignature = true,
                    Condition = 20,
                    ProjectBatchID = -1,
                    Reserve07 = notFinished ? "1" : "0",
                    Barcode = newestBarcode,
                    Reserve01 = newestBarcode.Substring(5, 7)
                });
            }
            else
            {
                newSheetId = sheetRIds.FirstOrDefault().Item1;
                newBarcode = sheetRIds.FirstOrDefault().Item2;
                if (newSheetId != null && !newSheetId.Equals(Guid.Empty))
                {
                    // ограничение - нельзя в замыкании использовать ref аргумент, обходим это локальной переменной
                    Guid newestSheetID = newSheetId;
                    // update, т.к. нашли sheetid
                    db.GetTable<sht_Sheets_R>().SchemaName("loader")
                    .Where(r => r.SheetID == newestSheetID)
                    .Set((n) => n.AuditoriumCode, notFinished ? "" : participantData.AuditoryCode)
                    .Set((n) => n.Surname, participantData.Surname)
                    .Set((n) => n.Name, participantData.Name)
                    .Set((n) => n.SecondName, participantData.Secondname)
                    .Set((n) => n.Sex, participantData.Sex ? 1 : 0)
                    .Set((n) => n.DocumentSeries, participantData.DocumentSeries)
                    .Set((n) => n.DocumentNumber, participantData.DocumentNumber)
                    .Set((n) => n.VariantCode, notFinished ? 0 : participantData.VariantCode)
                    .Set((n) => n.HasSignature, true)
                    .Set((n) => n.Condition, 20)
                    .Set((n) => n.Reserve07, notFinished ? "1" : "0")
                    .Update();
                }
            }
        }
        private void InsertOrUpdateSheetsAB(CommonData commonData, ParticipantData participantData, bool notFinished, GIA_DB db, Guid newSheetId, Guid newPackageId, string newBarcode)
        {
            List<sht_Sheets_AB> sht_Sheets_ABs = new List<sht_Sheets_AB>();
            var sheet_AB = new sht_Sheets_AB()
            {
                REGION = commonData.Region,
                SheetID = newSheetId,
                PackageFK = newPackageId,
                FileName = this.fileName,
                RegionCode = commonData.Region,
                DepartmentCode = this.fileName.Substring(0, 4),
                TestTypeCode = 9,
                SubjectCode = commonData.SubjectCode,
                ExamDate = commonData.ExamDate.ToString("yyyy.MM.dd"),
                StationCode = commonData.SchoolCode,
                AuditoriumCode = notFinished ? "" : participantData.AuditoryCode,
                ImageNumber = 1,
                VariantCode = notFinished ? 0 : participantData.VariantCode,
                HasSignature = true,
                Condition = 20,
                ProjectBatchID = -1,
                Reserve02 = participantData.Property == 7 ? "22" : "0",
                Barcode = newBarcode.ReplaceAt(2, '1')
            };
            sht_Sheets_ABs.Add(sheet_AB);
            db.sht_Sheets_AB.SchemaName("loader")
            .Merge()
            .Using(sht_Sheets_ABs)
            .On((target, source) => target.SheetID == source.SheetID &&
                                    target.PackageFK == source.PackageFK &&
                                    target.FileName == source.FileName &&
                                    target.ExamDate == source.ExamDate)
            .UpdateWhenMatched()
            .InsertWhenNotMatched()
            .Merge();
        }

        /// <summary>
        /// Изменение или добавление новой записи в sht_marks_AB для зачёта или итоговой оценки
        /// </summary>
        /// <param name="commonData"></param>
        /// <param name="participantData"></param>
        /// <param name="db"></param>
        private static void InsertOrUpdateMarksAB(CommonData commonData, ParticipantData participantData, GIA_DB db, Guid newSheetId)
        {
            List<sht_Marks_AB> marks_ABs = new List<sht_Marks_AB>();
            string answerValue = string.Empty;
            // проверяем на лажу в зачёте
            if (string.IsNullOrEmpty(participantData.CheckMark) || !participantData.CheckMark.Any(c => char.IsDigit(c)))
            {
                answerValue = "0";
            }
            else
            {
                answerValue = participantData.CheckMark;
            }
            var mark_AB = new sht_Marks_AB()
            {
                REGION = commonData.Region,
                MarkID = Guid.NewGuid(),
                SheetFK = newSheetId,
                TaskTypeCode = 0,
                TaskNumber = 1,
                AnswerValue = answerValue
            };
            marks_ABs.Add(mark_AB);
            // проверяем на лажу в итоговом
            if (string.IsNullOrEmpty(participantData.FinalMark) || !participantData.FinalMark.Any(c => char.IsDigit(c)))
            {
                answerValue = "0";
            }
            else
            {
                answerValue = participantData.FinalMark;
            }
            mark_AB = new sht_Marks_AB()
            {
                REGION = commonData.Region,
                MarkID = Guid.NewGuid(),
                SheetFK = newSheetId,
                TaskTypeCode = 1,
                TaskNumber = 1,
                AnswerValue = answerValue
            };
            marks_ABs.Add(mark_AB);
            db.sht_Marks_AB.SchemaName("loader")
                .Merge()
                .Using(marks_ABs)
                .On((target, source) => target.SheetFK == source.SheetFK &&
                                        target.TaskTypeCode == source.TaskTypeCode &&
                                        target.TaskNumber == source.TaskNumber)
                .UpdateWhenMatched()
                .InsertWhenNotMatched()
            .Merge();
        }

        private void UpdatePackagesSuccessCondition(CommonData commonData)
        {
            DataConnection dc = null;
            GIA_DB db = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    db.BeginTransaction();
                    // дейтим статус в sht_Packages в 20, типа загрузили
                    db.GetTable<sht_Package>().SchemaName("loader")
                    .Where(p => p.PackageID == commonData.PackageId &&
                    p.FileName == this.fileName &&
                    p.ExamDate == commonData.ExamDate.ToString("yyyy.MM.dd") &&
                    p.SubjectCode == commonData.SubjectCode)
                    .Set((n) => n.Condition, Convert.ToInt32(FinalInterview.FileStatus.Success))
                    .Set((n) => n.UpdateTime, DateTime.Now)
                    .Update();
                    db.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Fatal(status);
                db.RollbackTransaction();
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
        }

        public int GetParticipantsLoaded()
        {
            return this.participantsLoaded;
        }

        public List<TableInfo> MakeInfoData()
        {
            var resultData = new List<TableInfo>();
            foreach (var item in this.filesInfo.GetList())
            {
                TableInfo ti = new TableInfo();
                ti.Name = item.Filename;
                ti.Description = item.StatusMessage;
                ti.Status = "";
                resultData.Add(ti);
            }
            return resultData;
        }
    }
}
