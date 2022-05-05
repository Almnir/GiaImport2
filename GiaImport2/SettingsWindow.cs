using AdysTech.CredentialManager;
using GiaImport2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GiaImport2
{
    public partial class SettingsWindow : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Структура атрибутов для дополнительных данных в реквизитах
        /// </summary>
        [Serializable]
        public struct SettingsAttributes
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CheckConnectionButton_Click(object sender, EventArgs e)
        {
            //FormsHelper.ShowStyledMessageBox("ee", this.CommonRepository.GetCurrentScheme().Version);
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            var credentials = Globals.GetCredentials();
            if (credentials.Item1 != null)
            {
                LoginText.Text = credentials.Item1;
                PasswordText.Text = credentials.Item2;
                ServerText.Text = credentials.Item3;
                DatabaseText.Text = credentials.Item4;
            }
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
            var cred = new NetworkCredential(LoginText.Text.Trim(), PasswordText.Text.Trim(), Globals.TARGET_CREDENTIAL).ToICredential();
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
            this.Close();
        }
    }

}