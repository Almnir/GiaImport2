using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using GiaImport2.Models;
using GiaImport2.Services;
using MFtcUtils.Helpers;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GiaImport2
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly Container DIContainer;
        ICommonRepository CommonRepository;
        IInterviewRepository InterviewRepository;

        public MainForm(Container container, ICommonRepository commonRepository, IInterviewRepository interviewRepository)
        {
            InitializeComponent();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            this.DIContainer = container;
            this.CommonRepository = commonRepository;
            this.InterviewRepository = interviewRepository;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            BackgroundPanel backgroundPanel = new BackgroundPanel();
            backgroundPanel.Dock = DockStyle.Fill;
            MainPanel.Controls.Add(backgroundPanel);
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
            //FormsHelper.ShowStyledMessageBox("fff", CommonRepository.GetCurrentScheme().Version);
        }

        private void ImportInterviewButton_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
