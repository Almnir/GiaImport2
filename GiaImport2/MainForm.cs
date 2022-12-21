using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraRichEdit.Fields.Expression;
using GiaImport2.Models;
using GiaImport2.Services;
using NLog.Fluent;
using SimpleInjector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace GiaImport2
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly Container DIContainer;
        ICommonRepository CommonRepository;
        IInterviewRepository InterviewRepository;
        private readonly IImportXMLFilesService ImportXMLFilesService;
        ImportGridPanel ImportGridPanel;

        Dictionary<string, FileInfo> LoadedFiles = new Dictionary<string, FileInfo>();

        public MainForm(Container container, ICommonRepository commonRepository, IInterviewRepository interviewRepository, IImportXMLFilesService importXMLFilesService)
        {
            InitializeComponent();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            this.DIContainer = container;
            this.CommonRepository = commonRepository;
            this.InterviewRepository = interviewRepository;
            this.ImportXMLFilesService = importXMLFilesService;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // фоновая панель
            BackgroundPanel backgroundPanel = new BackgroundPanel();
            backgroundPanel.Dock = DockStyle.Fill;
            MainPanel.Controls.Add(backgroundPanel);
            // проверяем, какие странички риббона включить
            var dataBase = CommonRepository.GetCredentials().Database;
            var serverName = CommonRepository.GetCredentials().ServerName;
            RibbonPage page = ribbonControl1.Pages.GetPageByText("Итоговое собеседование");
            page.Visible = false;
            page = ribbonControl1.Pages.GetPageByText("Импорт XML");
            page.Visible = false;
            if (Properties.Settings.Default.Importado == true)
            {
                page = ribbonControl1.Pages.GetPageByText("Импорт XML");
                page.Visible = true;
                ribbonControl1.SelectedPage = page;
            }
            if (Properties.Settings.Default.Intervjuo == true)
            {
                page = ribbonControl1.Pages.GetPageByText("Итоговое собеседование");
                page.Visible = true;
                ribbonControl1.SelectedPage = page;
            }
            if (string.IsNullOrEmpty(dataBase) || string.IsNullOrEmpty(serverName))
            {
                SettingsWindow settingsWindow = DIContainer.GetInstance<SettingsWindow>();
                DialogResult resdlg = settingsWindow.ShowDialog();
                if (resdlg == DialogResult.OK)
                {
                    string error = "";
                    // сверка версий с базой
                    if (CommonRepository.TryCheckVersion(Assembly.GetExecutingAssembly().GetName().Version, Properties.Settings.Default.Importado, Properties.Settings.Default.Intervjuo, out error) == false)
                    {
                        FormsHelper.ShowStyledMessageBox("Ошибка!", error);
                    }
                }
            }
            if (!CommonRepository.CheckDBVersion(dataBase))
            {
                FormsHelper.ShowStyledMessageBox("Внимание!", "Версия БД не соответствует действующей!");
                SettingsWindow settingsWindow = DIContainer.GetInstance<SettingsWindow>();
                DialogResult resdlg = settingsWindow.ShowDialog();
                if (resdlg != DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            string regexPattern = @"_\d+.*$";
            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            if (Directory.Exists(Globals.frmSettings.TempDirectoryText == null ?
                Path.Combine(Path.GetTempPath(), "Tempdir") : Path.Combine(Globals.frmSettings.TempDirectoryText, "Tempdir")))
            {
                var files = Directory.GetFiles(Globals.frmSettings.TempDirectoryText == null ?
                    Path.Combine(Path.GetTempPath(), "Tempdir") : Path.Combine(Globals.frmSettings.TempDirectoryText, "Tempdir"));
                // если найдены уже загруженные файлы Импорта
                if (files != null && files.Count() > 0)
                {
                    // Добавляем панель
                    System.ComponentModel.BindingList<ImportXMLFilesDto> importViews = CreatePanelAndGetBinding();
                    foreach (var file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        string fname = Path.GetFileNameWithoutExtension(file);
                        if (!LoadedFiles.Keys.Contains(file) && !LoadedFiles.Values.Contains(fi) && !regex.IsMatch(fname))
                        {
                            LoadedFiles.Add(file, fi);
                            var ixf = new ImportXMLFilesDto
                            {
                                Name = fi.Name,
                                CreationTime = fi.CreationTime,
                                Length = fi.Length
                            };
                            ImportGridPanel.GetImportGridPanelGrid().Invoke((MethodInvoker)(() =>
                            {
                                importViews.Add(ixf);
                                ImportGridPanel.GetImportGridPanelGrid().Refresh();
                            }));
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(dataBase) && !string.IsNullOrEmpty(serverName))
            {
                this.barStaticItem1.Caption = $"{serverName}/{dataBase}";
            }
            else
            {
                this.barStaticItem1.Caption = "";
            }
            // показываем версию
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Text += " v" + version;
        }

        private async void ExportInterviewButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            if (CommonRepository.CheckConnection() == false)
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "Нет соединения с БД!");
                return;
            }
            var examdates = await InterviewRepository.GetExamDates();
            if (examdates == null || (examdates != null && examdates.Any() == false))
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "Нет данных о датах экзаменов Итогового Собеседования!");
                return;
            }
            splashScreenManager1.CloseWaitForm();
            var exportInterviewWizard = DIContainer.GetInstance<ExportInterviewWizard>();
            exportInterviewWizard.ShowDialog();
            var treeList = exportInterviewWizard.GetTreeList();
            var gridListView = exportInterviewWizard.GetSchoolControlView();
            if (treeList.GetAllCheckedNodes().Count != 0 ||
                exportInterviewWizard.GetSchoolClassesCount() != 0)
            {
                var cancellationToken = new CancellationTokenSource();
                var progressBarWindow = DIContainer.GetInstance<ProgressBarWindow>();
                // панель отображения файлов
                ExportInterviewPanel exportInterviewPanel = new ExportInterviewPanel();
                exportInterviewPanel.Dock = DockStyle.Fill;
                MainPanel.Controls.Clear();
                MainPanel.Controls.Add(exportInterviewPanel);
                ExportFinalInterviewService exportFinalInterviewService = DIContainer.GetInstance<ExportFinalInterviewService>();
                List<Governmentinfo> governmentinfos = exportInterviewWizard.ParticipantsExams;
                System.ComponentModel.BindingList<ExportInterviewDto> exportInterviews = new System.ComponentModel.BindingList<ExportInterviewDto>();
                exportInterviewPanel.GetExportInterviewGrid().DataSource = exportInterviews;

                // отключаем кнопки на риббоне
                FormsHelper.ToggleRibbonButtonsAll(ribbonControl1, false);

                progressBarWindow.SetCancellationToken(cancellationToken);
                progressBarWindow.GetProgressBarTotal().Properties.Step = 1;
                progressBarWindow.GetProgressBarTotal().Properties.PercentView = true;
                progressBarWindow.GetProgressBarTotal().Properties.Maximum = exportInterviewWizard.SchoolClassReturn.Count;
                progressBarWindow.GetProgressBarTotal().Properties.Minimum = 0;
                progressBarWindow.Show();

                exportFinalInterviewService.Init(exportInterviewWizard.ExportPathFolder,
                    (filedto) =>
                    {
                        exportInterviewPanel.GetExportInterviewGrid().Invoke((MethodInvoker)(() =>
                        {
                            exportInterviews.Add(filedto);
                            exportInterviewPanel.GetExportInterviewGrid().Refresh();
                        }));
                    },
                    exportInterviewWizard.ExamDate,
                    (percentage) =>
                    {
                        progressBarWindow.Invoke((MethodInvoker)(() =>
                        {
                            progressBarWindow.GetProgressBarTotal().PerformStep();
                            progressBarWindow.GetProgressBarTotal().Update();
                        }));
                    },
                    (fileName) =>
                    {
                        if (progressBarWindow != null && progressBarWindow.IsDisposed != true)
                        {
                            progressBarWindow.Invoke((MethodInvoker)(() =>
                            {
                                progressBarWindow.GetLabel().Text = fileName;
                                progressBarWindow.GetLabel().Update();
                            }));
                        }
                    },
                    () =>
                    {
                        exportInterviewPanel.GetExportInterviewGrid().Invoke((MethodInvoker)(() =>
                        {
                            exportInterviewPanel.GetExportInterviewView().BestFitColumns();
                        }));
                        if (progressBarWindow != null && progressBarWindow.IsDisposed != true)
                        {
                            progressBarWindow.Invoke((MethodInvoker)(() =>
                            {
                                progressBarWindow.Close();
                            }));
                        }
                        // включаем кнопки на риббоне
                        ribbonControl1.Invoke((MethodInvoker)(() =>
                        {
                            FormsHelper.ToggleRibbonButtonsAll(ribbonControl1, true);
                        }));
                    },
                    exportInterviewWizard.SchoolClassReturn,
                    governmentinfos,
                    exportInterviewWizard.SchoolsOrClassChoice,
                    cancellationToken);
                exportFinalInterviewService.ExportAllExistingData();
            }
        }

        private void SettingsButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SettingsWindow settingsWindow = DIContainer.GetInstance<SettingsWindow>();
            settingsWindow.Show();
            var dataBase = CommonRepository.GetCredentials().Database;
            var serverName = CommonRepository.GetCredentials().ServerName;
            if (!string.IsNullOrEmpty(dataBase) && !string.IsNullOrEmpty(serverName))
            {
                this.barStaticItem1.Caption = $"{serverName}/{dataBase}";
            }
            else
            {
                this.barStaticItem1.Caption = "";
            }
        }

        private void OpenXMLFilesButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraOpenFileDialog openFileDialog = new XtraOpenFileDialog();
            openFileDialog.RestoreDirectory = true;

            openFileDialog.Filter = "Zip Files (.zip)|*.zip|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            openFileDialog.Multiselect = false;

            DialogResult userClicked = openFileDialog.ShowDialog();

            if (userClicked == DialogResult.OK)
            {
                // проверить имена файлов
                if (ImportXMLFilesService.CheckFilesNames(openFileDialog.FileName) == false)
                {
                    MessageBox.Show("Имена файлов должны соответствовать названиям таблиц!");
                    return;
                }
                if (!Directory.Exists(Globals.frmSettings.TempDirectoryText == null ?
                    Path.Combine(Path.GetTempPath(), "Tempdir") : Path.Combine(Globals.frmSettings.TempDirectoryText, "Tempdir")))
                {
                    // создать временный каталог
                    Directory.CreateDirectory(Globals.frmSettings.TempDirectoryText == null ?
                        Path.Combine(Path.GetTempPath(), "Tempdir") : Path.Combine(Globals.frmSettings.TempDirectoryText, "Tempdir"));
                }
                // очистить временный каталог
                ImportXMLFilesService.ClearFiles();
                // Добавляем панель
                System.ComponentModel.BindingList<ImportXMLFilesDto> importViews = CreatePanelAndGetBinding();
                // распаковать выбранный архив во временный каталог
                ImportXMLFilesService.UnpackFiles(openFileDialog.FileName, ribbonControl1,
                    (filedto) =>
                    {
                        ImportGridPanel.GetImportGridPanelGrid().Invoke((MethodInvoker)(() =>
                        {
                            importViews.Add(filedto);
                            ImportGridPanel.GetImportGridPanelGrid().Refresh();
                        }));
                    }
                );
                // сохраняем в конфигурации
                Globals.frmSettings.LastPathText = openFileDialog.FileName;
                Globals.frmSettings.Save();
            }

        }

        private System.ComponentModel.BindingList<ImportXMLFilesDto> CreatePanelAndGetBinding()
        {
            ImportGridPanel = new ImportGridPanel();
            ImportGridPanel.Dock = DockStyle.Fill;
            MainPanel.Controls.Clear();
            MainPanel.Controls.Add(ImportGridPanel);
            System.ComponentModel.BindingList<ImportXMLFilesDto> importViews = new System.ComponentModel.BindingList<ImportXMLFilesDto>();
            ImportGridPanel.GetImportGridPanelGrid().DataSource = importViews;
            GridColumnSummaryItem siTotal = new GridColumnSummaryItem();
            siTotal.SummaryType = SummaryItemType.Count;
            siTotal.DisplayFormat = "Всего: {0}";
            siTotal.FieldName = "CreationTime";
            ImportGridPanel.GetImportGridPanelView().Columns["CreationTime"].Summary.Add(siTotal);
            return importViews;
        }

        private void ImportInterviewButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            if (CommonRepository.GetConnection() == null)
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "Настройки базы данных не установлены!");
                return;
            }
            if (CommonRepository.CheckConnection() == false)
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "Нет соединения с базой данных!");
                return;
            }
            splashScreenManager1.CloseWaitForm();
            XtraFolderBrowserDialog openDialog = new XtraFolderBrowserDialog();

            DialogResult userClicked = openDialog.ShowDialog();

            if (userClicked == DialogResult.Cancel)
            {
                return;
            }
            // проверяем на длину пути
            if (userClicked == DialogResult.OK)
            {
                //try
                //{
                //    statusLabel.Text = openDialog.SelectedPath;
                //}
                //catch (PathTooLongException)
                //{
                //    log.Error("Слишком длинный путь!");
                //    MessageBox.Show("Внимание!", "Слишком длинный путь!");
                //    return;
                //}
                //if (string.IsNullOrEmpty(openDialog.SelectedPath))
                //{
                //    return;
                //}
                //this.metroListView1.Clear();
                //this.metroListView1.Refresh();
                //CancellationTokenSource source = new CancellationTokenSource();
                //ProgressBarWindow pbw = new ProgressBarWindow(source);
                //pbw.SetTitle("Импорт данных выполняется...");
                //ProgressBar pbarTotal = pbw.GetProgressBarTotal();
                //ProgressBar pbarLine = pbw.GetProgressBarLine();

                //XMLImporter xmi = new XMLImporter(openDialog.SelectedPath, this, pbw, source);
                //pbw.FormClosed += (a, e) => { source.Cancel(); };
                //DisableButtons();
                //pbw.Show();
                //pbw.Focus();
                //xmi.ImportAllData();
                //EnableDisableButtons();
            }
        }

        private void ValidateXMLButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            // определим выбранные файлы
            List<ImportXMLFilesDto> checkedFiles = this.GetActualCheckedFiles();
            if (checkedFiles == null)
            {
                FormsHelper.ShowStyledMessageBox("Внимание!", "Ни одного файла не выбрано!");
                return;
            }
            splashScreenManager1.ShowWaitForm();
            if (CommonRepository.GetConnection() == null)
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "Настройки базы данных не установлены!");
                return;
            }
            if (CommonRepository.CheckConnection() == false)
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "Нет соединения с базой данных!");
                return;
            }
            splashScreenManager1.CloseWaitForm();

            ImportXMLFilesService.ValidateFiles(ImportGridPanel, ribbonControl1, checkedFiles,
                (message) =>
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        MessageShowControl.ShowValidationErrors(message);
                    }));
                },
                    (tableinfosErrors) =>
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            ResultWindowWithLog rw = new ResultWindowWithLog();
                            rw.SetTitle("Итого");
                            rw.SetTableData(tableinfosErrors.tableInfos);
                            rw.SetLogData(tableinfosErrors.dependencyErrors);
                            rw.ShowDialog();
                        }));
                    }
                );
        }

        private List<ImportXMLFilesDto> GetActualCheckedFiles()
        {
            List<ImportXMLFilesDto> rows = new List<ImportXMLFilesDto>();
            int[] selectedRows = this.ImportGridPanel.GetImportGridPanelView().GetSelectedRows();
            if (selectedRows.Length == 0) return null;
            for (int i = 0; i < selectedRows.Length; i++)
            {
                int selectedRowHandle = selectedRows[i];
                if (selectedRowHandle >= 0)
                    rows.Add((ImportXMLFilesDto)this.ImportGridPanel.GetImportGridPanelView().GetRow(selectedRowHandle));
            }
            return rows;
        }

        private void ImportXMLFilesButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            // определим выбранные файлы
            List<ImportXMLFilesDto> checkedFiles = this.GetActualCheckedFiles();
            if (checkedFiles == null)
            {
                FormsHelper.ShowStyledMessageBox("Внимание!", "Ни одного файла не выбрано!");
                return;
            }
            splashScreenManager1.ShowWaitForm();
            if (CommonRepository.GetConnection() == null)
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "Настройки базы данных не установлены!");
                return;
            }
            if (CommonRepository.CheckConnection() == false)
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "Нет соединения с базой данных!");
                return;
            }
            if (CommonRepository.CheckIfStoredExist() == false)
            {
                splashScreenManager1.CloseWaitForm();
                FormsHelper.ShowStyledMessageBox("Внимание!", "База данных не содержит необходимых хранимых процедур!");
                return;
            }
            splashScreenManager1.CloseWaitForm();
            // выключаем кнопки на риббоне
            ribbonControl1.Invoke((MethodInvoker)(() =>
            {
                FormsHelper.ToggleRibbonButtonsAll(ribbonControl1, false);
            }));
            BulkManager bm = DIContainer.GetInstance<BulkManager>();
            ImportXMLFilesService.ImportFiles(this.ImportGridPanel, ribbonControl1, bm,checkedFiles,
                () =>
                {
                    if (Globals.frmSettings.PuraSurEraro)
                    {
                        Invoke(new Action(() =>
                        {
                            FormsHelper.ShowStyledMessageBox("Внимание!", "Выполнение прервано! \n Будет произведена очистка временных таблиц!");
                        }));
                        CommonRepository.DeleteLoaderTables();
                        Invoke(new Action(() =>
                        {
                            FormsHelper.ShowStyledMessageBox("Внимание!", "Очистка временных таблиц завершена!");
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            FormsHelper.ShowStyledMessageBox("Внимание!", "Выполнение прервано!");
                        }));
                    }
                    // включаем кнопки на риббоне
                    ribbonControl1.Invoke((MethodInvoker)(() =>
                    {
                        FormsHelper.ToggleRibbonButtonsAll(ribbonControl1, true);
                    }));
                    if (!this.IsDisposed)
                    {
                        Invoke(new Action(() => { this.Focus(); }));
                    }
                },
                (deserializeException) =>
                {
                    ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> errord = deserializeException.errorDict;
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in deserializeException.errorDict)
                    {
                        sb.Append(item.Value.Item1);
                    }
                    Invoke(new Action(() => { MessageShowControl.ShowImportErrors(sb.ToString()); }));
                    // включаем кнопки на риббоне
                    ribbonControl1.Invoke((MethodInvoker)(() =>
                    {
                        FormsHelper.ToggleRibbonButtonsAll(ribbonControl1, true);
                    }));
                    if (!this.IsDisposed)
                    {
                        Invoke(new Action(() => { this.Focus(); }));
                    }
                },
                (bulkException) =>
                {
                    ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> errord = bulkException.errorDict;
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in bulkException.errorDict)
                    {
                        sb.Append(item.Value.Item1);
                    }
                    Invoke(new Action(() => { MessageShowControl.ShowImportErrors(sb.ToString()); }));
                    // включаем кнопки на риббоне
                    ribbonControl1.Invoke((MethodInvoker)(() =>
                    {
                        FormsHelper.ToggleRibbonButtonsAll(ribbonControl1, true);
                    }));
                    if (!this.IsDisposed)
                    {
                        Invoke(new Action(() => { this.Focus(); }));
                    }
                },
                (exception) =>
                {
                    CommonRepository.GetLogger().Error(exception.ToString());
                    Invoke(new Action(() => { MessageShowControl.ShowImportErrors(exception.ToString()); }));
                    // включаем кнопки на риббоне
                    ribbonControl1.Invoke((MethodInvoker)(() =>
                    {
                        FormsHelper.ToggleRibbonButtonsAll(ribbonControl1, true);
                    }));
                    if (!this.IsDisposed)
                    {
                        Invoke(new Action(() => { this.Focus(); }));
                    }
                },
                (importStatistics) =>
                {
                    Invoke(new Action(() =>
                    {
                        string statiscticsall = bm.outLog.ToString();
                        CommonRepository.GetLogger().Info(statiscticsall);
                        var dataStatTable = bm.PrepareStatistics(importStatistics);
                        ResultWindowWithLog rw = new ResultWindowWithLog();
                        rw.SetTableData(dataStatTable);
                        rw.SetLogData(statiscticsall);
                        rw.Focus();
                        rw.ShowDialog();
                    }));
                    Invoke(new Action(() =>
                    {
                        this.Focus();
                    }));
                }
                ,
                () =>
                {
                    if (Globals.frmSettings.PuraSurEraro)
                    {
                        Invoke(new Action(() => {
                            FormsHelper.ShowStyledMessageBox("Внимание!", "Прервано пользователем! \n Будет произведена очистка временных таблиц!");
                        }));
                        CommonRepository.DeleteLoaderTables();
                        Invoke(new Action(() => {
                            FormsHelper.ShowStyledMessageBox("Внимание!", "Очистка временных таблиц завершена!");
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() => {
                            FormsHelper.ShowStyledMessageBox("Внимание!", "Прервано пользователем!");
                        }));
                    }
                    // включаем кнопки на риббоне
                    ribbonControl1.Invoke((MethodInvoker)(() =>
                    {
                        FormsHelper.ToggleRibbonButtonsAll(ribbonControl1, true);
                    }));
                    if (!this.IsDisposed)
                    {
                        Invoke(new Action(() => { this.Focus(); }));
                    }
                }
             );
        }
    }
}
