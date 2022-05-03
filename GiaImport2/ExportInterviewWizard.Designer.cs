
namespace GiaImport2
{
    partial class ExportInterviewWizard
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
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::GiaImport2.GiaImportWaitForm), true, true, true);
            this.wizardControl1 = new DevExpress.XtraWizard.WizardControl();
            this.welcomeWizardPage1 = new DevExpress.XtraWizard.WelcomeWizardPage();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
            this.ExportFolderButton = new DevExpress.XtraEditors.SimpleButton();
            this.ExportPathEdit = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.ExamDatesCombo = new DevExpress.XtraEditors.ComboBoxEdit();
            this.wizardPage1 = new DevExpress.XtraWizard.WizardPage();
            this.checkedListBoxControl1 = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.completionWizardPage1 = new DevExpress.XtraWizard.CompletionWizardPage();
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).BeginInit();
            this.wizardControl1.SuspendLayout();
            this.welcomeWizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExportPathEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExamDatesCombo.Properties)).BeginInit();
            this.wizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // wizardControl1
            // 
            this.wizardControl1.CancelText = "Отменить";
            this.wizardControl1.Controls.Add(this.welcomeWizardPage1);
            this.wizardControl1.Controls.Add(this.wizardPage1);
            this.wizardControl1.Controls.Add(this.completionWizardPage1);
            this.wizardControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardControl1.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.wizardControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.NextText = "&Вперёд >";
            this.wizardControl1.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[] {
            this.welcomeWizardPage1,
            this.wizardPage1,
            this.completionWizardPage1});
            this.wizardControl1.Size = new System.Drawing.Size(663, 379);
            this.wizardControl1.Text = "Параметры экспорта";
            this.wizardControl1.SelectedPageChanged += new DevExpress.XtraWizard.WizardPageChangedEventHandler(this.wizardControl1_SelectedPageChanged);
            this.wizardControl1.CancelClick += new System.ComponentModel.CancelEventHandler(this.wizardControl1_CancelClick);
            // 
            // welcomeWizardPage1
            // 
            this.welcomeWizardPage1.Controls.Add(this.labelControl3);
            this.welcomeWizardPage1.Controls.Add(this.radioGroup1);
            this.welcomeWizardPage1.Controls.Add(this.ExportFolderButton);
            this.welcomeWizardPage1.Controls.Add(this.ExportPathEdit);
            this.welcomeWizardPage1.Controls.Add(this.labelControl2);
            this.welcomeWizardPage1.Controls.Add(this.labelControl1);
            this.welcomeWizardPage1.Controls.Add(this.ExamDatesCombo);
            this.welcomeWizardPage1.IntroductionText = "";
            this.welcomeWizardPage1.Name = "welcomeWizardPage1";
            this.welcomeWizardPage1.ProceedText = "Нажмите \'Вперёд\' чтобы продолжить...";
            this.welcomeWizardPage1.Size = new System.Drawing.Size(446, 247);
            this.welcomeWizardPage1.Text = "Выберите первичные параметры экспорта";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(4, 122);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(64, 13);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "Разделение:";
            // 
            // radioGroup1
            // 
            this.radioGroup1.Location = new System.Drawing.Point(3, 141);
            this.radioGroup1.Name = "radioGroup1";
            this.radioGroup1.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "По школам", true, null, "SchoolsRadioButton"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "По классам", true, null, "ClassesRadioButton")});
            this.radioGroup1.Properties.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.radioGroup1.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.radioGroup1.Size = new System.Drawing.Size(171, 79);
            this.radioGroup1.TabIndex = 5;
            // 
            // ExportFolderButton
            // 
            this.ExportFolderButton.Location = new System.Drawing.Point(364, 82);
            this.ExportFolderButton.Name = "ExportFolderButton";
            this.ExportFolderButton.Size = new System.Drawing.Size(79, 23);
            this.ExportFolderButton.TabIndex = 4;
            this.ExportFolderButton.Text = "Выбрать";
            // 
            // ExportPathEdit
            // 
            this.ExportPathEdit.Location = new System.Drawing.Point(4, 84);
            this.ExportPathEdit.Name = "ExportPathEdit";
            this.ExportPathEdit.Size = new System.Drawing.Size(354, 20);
            this.ExportPathEdit.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(4, 63);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(89, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Путь к выгрузке:";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(4, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(79, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Дата экзамена:";
            // 
            // ExamDatesCombo
            // 
            this.ExamDatesCombo.Location = new System.Drawing.Point(4, 24);
            this.ExamDatesCombo.Name = "ExamDatesCombo";
            this.ExamDatesCombo.Properties.AdvancedModeOptions.SelectionColor = System.Drawing.SystemColors.ActiveCaption;
            this.ExamDatesCombo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ExamDatesCombo.Properties.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.ExamDatesCombo.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ExamDatesCombo.Properties.Sorted = true;
            this.ExamDatesCombo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.ExamDatesCombo.Size = new System.Drawing.Size(112, 20);
            this.ExamDatesCombo.TabIndex = 1;
            // 
            // wizardPage1
            // 
            this.wizardPage1.Controls.Add(this.checkedListBoxControl1);
            this.wizardPage1.DescriptionText = "Выберите элементы разделения";
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.Size = new System.Drawing.Size(631, 236);
            this.wizardPage1.Text = "Экспорт файлов ИС";
            // 
            // checkedListBoxControl1
            // 
            this.checkedListBoxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxControl1.Location = new System.Drawing.Point(0, 0);
            this.checkedListBoxControl1.Name = "checkedListBoxControl1";
            this.checkedListBoxControl1.Size = new System.Drawing.Size(631, 236);
            this.checkedListBoxControl1.TabIndex = 0;
            // 
            // completionWizardPage1
            // 
            this.completionWizardPage1.FinishText = "";
            this.completionWizardPage1.Name = "completionWizardPage1";
            this.completionWizardPage1.ProceedText = "Нажмите \'Завершить\' для начала выгрузки...";
            this.completionWizardPage1.Size = new System.Drawing.Size(446, 247);
            this.completionWizardPage1.Text = "Завершение выбора параметров выгрузки";
            // 
            // ExportInterviewWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 379);
            this.Controls.Add(this.wizardControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportInterviewWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Экспорт файлов ИС";
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).EndInit();
            this.wizardControl1.ResumeLayout(false);
            this.welcomeWizardPage1.ResumeLayout(false);
            this.welcomeWizardPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExportPathEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExamDatesCombo.Properties)).EndInit();
            this.wizardPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraWizard.WizardControl wizardControl1;
        private DevExpress.XtraWizard.WelcomeWizardPage welcomeWizardPage1;
        private DevExpress.XtraWizard.WizardPage wizardPage1;
        private DevExpress.XtraWizard.CompletionWizardPage completionWizardPage1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.RadioGroup radioGroup1;
        private DevExpress.XtraEditors.SimpleButton ExportFolderButton;
        private DevExpress.XtraEditors.TextEdit ExportPathEdit;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckedListBoxControl checkedListBoxControl1;
        private DevExpress.XtraEditors.ComboBoxEdit ExamDatesCombo;
        DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
    }
}