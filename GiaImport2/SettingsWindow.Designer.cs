
namespace GiaImport2
{
    partial class SettingsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.CheckConnectionButton = new DevExpress.XtraEditors.SimpleButton();
            this.PasswordText = new DevExpress.XtraEditors.TextEdit();
            this.LoginText = new DevExpress.XtraEditors.TextEdit();
            this.DatabaseText = new DevExpress.XtraEditors.TextEdit();
            this.ServerText = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.CleanOnExceptionEdit = new DevExpress.XtraEditors.CheckEdit();
            this.TempFolderLabel = new DevExpress.XtraEditors.LabelControl();
            this.TmpFolderEdit = new DevExpress.XtraEditors.ButtonEdit();
            this.OkButton = new DevExpress.XtraEditors.SimpleButton();
            this.CancelSettingsButton = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoginText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CleanOnExceptionEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TmpFolderEdit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.CheckConnectionButton);
            this.groupControl1.Controls.Add(this.PasswordText);
            this.groupControl1.Controls.Add(this.LoginText);
            this.groupControl1.Controls.Add(this.DatabaseText);
            this.groupControl1.Controls.Add(this.ServerText);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Location = new System.Drawing.Point(2, 3);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(570, 163);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "Подключение к БД";
            // 
            // CheckConnectionButton
            // 
            this.CheckConnectionButton.Location = new System.Drawing.Point(77, 132);
            this.CheckConnectionButton.Name = "CheckConnectionButton";
            this.CheckConnectionButton.Size = new System.Drawing.Size(488, 23);
            this.CheckConnectionButton.TabIndex = 8;
            this.CheckConnectionButton.Text = "Проверить соединение";
            this.CheckConnectionButton.Click += new System.EventHandler(this.CheckConnectionButton_Click);
            // 
            // PasswordText
            // 
            this.PasswordText.Location = new System.Drawing.Point(77, 105);
            this.PasswordText.Name = "PasswordText";
            this.PasswordText.Properties.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.PasswordText.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.PasswordText.Properties.PasswordChar = '*';
            this.PasswordText.Properties.UseSystemPasswordChar = true;
            this.PasswordText.Size = new System.Drawing.Size(488, 20);
            this.PasswordText.TabIndex = 7;
            // 
            // LoginText
            // 
            this.LoginText.Location = new System.Drawing.Point(77, 79);
            this.LoginText.Name = "LoginText";
            this.LoginText.Size = new System.Drawing.Size(488, 20);
            this.LoginText.TabIndex = 6;
            // 
            // DatabaseText
            // 
            this.DatabaseText.Location = new System.Drawing.Point(77, 53);
            this.DatabaseText.Name = "DatabaseText";
            this.DatabaseText.Size = new System.Drawing.Size(488, 20);
            this.DatabaseText.TabIndex = 5;
            // 
            // ServerText
            // 
            this.ServerText.Location = new System.Drawing.Point(77, 27);
            this.ServerText.Name = "ServerText";
            this.ServerText.Size = new System.Drawing.Size(488, 20);
            this.ServerText.TabIndex = 4;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(5, 108);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(37, 13);
            this.labelControl4.TabIndex = 3;
            this.labelControl4.Text = "Пароль";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(5, 82);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(30, 13);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Логин";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(5, 56);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(65, 13);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "База данных";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(5, 30);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(37, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Сервер";
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.CleanOnExceptionEdit);
            this.groupControl2.Controls.Add(this.TempFolderLabel);
            this.groupControl2.Controls.Add(this.TmpFolderEdit);
            this.groupControl2.Location = new System.Drawing.Point(2, 172);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(570, 73);
            this.groupControl2.TabIndex = 1;
            this.groupControl2.Text = "Прочие настройки";
            // 
            // CleanOnExceptionEdit
            // 
            this.CleanOnExceptionEdit.Location = new System.Drawing.Point(6, 44);
            this.CleanOnExceptionEdit.Name = "CleanOnExceptionEdit";
            this.CleanOnExceptionEdit.Properties.Caption = "Очищать временные таблицы при ошибке";
            this.CleanOnExceptionEdit.Size = new System.Drawing.Size(240, 19);
            this.CleanOnExceptionEdit.TabIndex = 10;
            // 
            // TempFolderLabel
            // 
            this.TempFolderLabel.Location = new System.Drawing.Point(10, 24);
            this.TempFolderLabel.Name = "TempFolderLabel";
            this.TempFolderLabel.Size = new System.Drawing.Size(123, 13);
            this.TempFolderLabel.TabIndex = 9;
            this.TempFolderLabel.Text = "Подкаталог распаковки";
            // 
            // TmpFolderEdit
            // 
            this.TmpFolderEdit.Location = new System.Drawing.Point(139, 21);
            this.TmpFolderEdit.Name = "TmpFolderEdit";
            this.TmpFolderEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.TmpFolderEdit.Properties.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.TmpFolderEdit.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.TmpFolderEdit.Size = new System.Drawing.Size(426, 20);
            this.TmpFolderEdit.TabIndex = 9;
            this.TmpFolderEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.TmpFolderEdit_ButtonClick);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(141, 260);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 2;
            this.OkButton.Text = "Применить";
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CancelSettingsButton
            // 
            this.CancelSettingsButton.Location = new System.Drawing.Point(361, 260);
            this.CancelSettingsButton.Name = "CancelSettingsButton";
            this.CancelSettingsButton.Size = new System.Drawing.Size(75, 23);
            this.CancelSettingsButton.TabIndex = 3;
            this.CancelSettingsButton.Text = "Отменить";
            this.CancelSettingsButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 297);
            this.Controls.Add(this.CancelSettingsButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("SettingsWindow.IconOptions.SvgImage")));
            this.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "SettingsWindow";
            this.Text = "Настройки";
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PasswordText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoginText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DatabaseText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.groupControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CleanOnExceptionEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TmpFolderEdit.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.SimpleButton OkButton;
        private DevExpress.XtraEditors.SimpleButton CancelSettingsButton;
        private DevExpress.XtraEditors.SimpleButton CheckConnectionButton;
        private DevExpress.XtraEditors.TextEdit PasswordText;
        private DevExpress.XtraEditors.TextEdit LoginText;
        private DevExpress.XtraEditors.TextEdit DatabaseText;
        private DevExpress.XtraEditors.TextEdit ServerText;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl TempFolderLabel;
        private DevExpress.XtraEditors.ButtonEdit TmpFolderEdit;
        private DevExpress.XtraEditors.CheckEdit CleanOnExceptionEdit;
    }
}