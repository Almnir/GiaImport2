using AdysTech.CredentialManager;
using DevExpress.XtraEditors;
using GiaImport2.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using static GiaImport2.Services.CommonRepository;

namespace GiaImport2
{
    public partial class SettingsWindow : DevExpress.XtraEditors.XtraForm
    {
        ICommonRepository CommonRepository;

        public SettingsWindow(ICommonRepository commonRepository)
        {
            InitializeComponent();
            this.CommonRepository = commonRepository;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CheckConnectionButton_Click(object sender, EventArgs e)
        {
            //FormsHelper.ShowStyledMessageBox("ee", this.CommonRepository.GetCurrentScheme().Version);
            if (CommonRepository.CheckConnection())
            {
                FormsHelper.ShowStyledMessageBox("Проверено", "Соединение успешно!");
            }
            else
            {
                FormsHelper.ShowStyledMessageBox("Внимание!", "Нет соединения!");
            }
        }
        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            var credentials = CommonRepository.GetCredentials();
            if (credentials.UserName != null)
            {
                LoginText.Text = credentials.UserName;
                PasswordText.Text = credentials.Password;
                ServerText.Text = credentials.ServerName;
                DatabaseText.Text = credentials.Database;
            }
            // загрузка из формсеттингов
            if (string.IsNullOrWhiteSpace(Globals.frmSettings.TempDirectoryText ?? ""))
            {
                TmpFolderEdit.Text = Path.GetTempPath();
            }
            else
            {
                TmpFolderEdit.Text = Globals.frmSettings.TempDirectoryText;
            }
            CleanOnExceptionEdit.Enabled = Properties.Settings.Default.PuraSurEraroAktivigi;
            CleanOnExceptionEdit.Checked = Globals.frmSettings.PuraSurEraro;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var strs = new string[] {
                ServerText.Text,
                DatabaseText.Text,
                LoginText.Text,
                PasswordText.Text };
            if (strs.Any(x => String.IsNullOrWhiteSpace(x)))
            {
                FormsHelper.ShowStyledMessageBox("Внимание!", "Не заполнены все поля!");
                return;
            }
            // создаём объект реквизитов
            var cred = new NetworkCredential(LoginText.Text.Trim(), PasswordText.Text.Trim()).ToICredential();
            cred.TargetName = Globals.TARGET_CREDENTIAL;
            cred.Persistance = Persistance.LocalMachine;
            // добавляем атрибуты с дополнительными данными
            cred.Attributes = new Dictionary<string, Object>();
            var server = new SettingsAttributes() { name = "server", value = ServerText.Text.Trim() };
            var database = new SettingsAttributes() { name = "database", value = DatabaseText.Text.Trim() };
            cred.Attributes.Add("ServerAttribute", server);
            cred.Attributes.Add("DatabaseAttribute", database);
            // сохраняем реквизиты
            cred.SaveCredential();
            // сохранение в формсеттинги
            // проверяем заданный подкаталог
            if (!Directory.Exists(TmpFolderEdit.Text.Trim()))
            {
                FormsHelper.ShowStyledMessageBox("Неверный путь для каталога временных файлов!", "Внимание!");
                return;
            }
            else 
            {
                Globals.frmSettings.TempDirectoryText = TmpFolderEdit.Text.Trim();
            }
            Globals.frmSettings.PuraSurEraro = CleanOnExceptionEdit.Checked;

            Globals.frmSettings.Save();
            this.Close();
        }

        private void TmpFolderEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            XtraFolderBrowserDialog openDialog = new XtraFolderBrowserDialog();
            DialogResult userClicked = openDialog.ShowDialog();
            if (userClicked == DialogResult.Cancel)
            {
                return;
            }
            if (userClicked == DialogResult.OK)
            {
                this.TmpFolderEdit.Text = openDialog.SelectedPath;
                this.TmpFolderEdit.ScrollToCaret();
            }
        }
    }

}