using GiaImport2.Services;
using SimpleInjector;
using System;
using System.Windows.Forms;

namespace GiaImport2
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly Container DIContainer;

        public MainForm(Container container)
        {
            InitializeComponent();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            this.DIContainer = container;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            BackgroundPanel backgroundPanel = new BackgroundPanel();
            backgroundPanel.Dock = DockStyle.Fill;
            MainPanel.Controls.Add(backgroundPanel);
        }

        private void ExportInterviewButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (DatabaseHelper.CheckConnection() == false)
            {
                FormsHelper.ShowStyledMessageBox("Внимание!", "Нет соединения с БД!");
                return;
            }
            var exportInterviewWizard = DIContainer.GetInstance<ExportInterviewWizard>();
            exportInterviewWizard.Show();
        }

        private void SettingsButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //SettingsWindow settingsWindow = new SettingsWindow();
            SettingsWindow settingsWindow = DIContainer.GetInstance<SettingsWindow>();
            settingsWindow.Show();
        }

        private void OpenXMLFilesButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //FormsHelper.ShowStyledMessageBox("fff", CommonRepository.GetCurrentScheme().Version);
        }
    }
}
