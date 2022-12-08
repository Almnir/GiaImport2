namespace GiaImport2
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::GiaImport2.GiaImportWaitForm), true, true);
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.OpenXMLFilesButton = new DevExpress.XtraBars.BarButtonItem();
            this.ValidateXMLButton = new DevExpress.XtraBars.BarButtonItem();
            this.ImportXMLFilesButton = new DevExpress.XtraBars.BarButtonItem();
            this.ExportInterviewButton = new DevExpress.XtraBars.BarButtonItem();
            this.ImportInterviewButton = new DevExpress.XtraBars.BarButtonItem();
            this.FinishExamButton = new DevExpress.XtraBars.BarButtonItem();
            this.SettingsButton = new DevExpress.XtraBars.BarButtonItem();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.ImportPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.FinalInterviewPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.SettingsPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.RibbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.MainPanel = new DevExpress.XtraEditors.PanelControl();
            this.ribbonPage2 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.DrawGroupsBorderMode = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.OpenXMLFilesButton,
            this.ValidateXMLButton,
            this.ImportXMLFilesButton,
            this.ExportInterviewButton,
            this.ImportInterviewButton,
            this.FinishExamButton,
            this.SettingsButton,
            this.barStaticItem1,
            this.barButtonItem1});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 24;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ImportPage,
            this.FinalInterviewPage,
            this.SettingsPage});
            this.ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ShowMoreCommandsButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Show;
            this.ribbonControl1.ShowQatLocationSelector = false;
            this.ribbonControl1.ShowToolbarCustomizeItem = false;
            this.ribbonControl1.Size = new System.Drawing.Size(837, 115);
            this.ribbonControl1.StatusBar = this.RibbonStatusBar;
            this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // 
            // OpenXMLFilesButton
            // 
            this.OpenXMLFilesButton.Caption = "Открыть XML файлы";
            this.OpenXMLFilesButton.Id = 11;
            this.OpenXMLFilesButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("OpenXMLFilesButton.ImageOptions.Image")));
            this.OpenXMLFilesButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("OpenXMLFilesButton.ImageOptions.LargeImage")));
            this.OpenXMLFilesButton.Name = "OpenXMLFilesButton";
            this.OpenXMLFilesButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.OpenXMLFilesButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OpenXMLFilesButton_ItemClick);
            // 
            // ValidateXMLButton
            // 
            this.ValidateXMLButton.Caption = "Проверить XML файлы";
            this.ValidateXMLButton.Id = 12;
            this.ValidateXMLButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ValidateXMLButton.ImageOptions.Image")));
            this.ValidateXMLButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ValidateXMLButton.ImageOptions.LargeImage")));
            this.ValidateXMLButton.Name = "ValidateXMLButton";
            this.ValidateXMLButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.ValidateXMLButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ValidateXMLButton_ItemClick);
            // 
            // ImportXMLFilesButton
            // 
            this.ImportXMLFilesButton.Caption = "Загрузить XML файлы";
            this.ImportXMLFilesButton.Id = 13;
            this.ImportXMLFilesButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ImportXMLFilesButton.ImageOptions.Image")));
            this.ImportXMLFilesButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ImportXMLFilesButton.ImageOptions.LargeImage")));
            this.ImportXMLFilesButton.Name = "ImportXMLFilesButton";
            this.ImportXMLFilesButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.ImportXMLFilesButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ImportXMLFilesButton_ItemClick);
            // 
            // ExportInterviewButton
            // 
            this.ExportInterviewButton.Caption = "Выгрузить файлы ИС";
            this.ExportInterviewButton.Id = 14;
            this.ExportInterviewButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ExportInterviewButton.ImageOptions.Image")));
            this.ExportInterviewButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ExportInterviewButton.ImageOptions.LargeImage")));
            this.ExportInterviewButton.Name = "ExportInterviewButton";
            this.ExportInterviewButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.ExportInterviewButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ExportInterviewButton_ItemClick);
            // 
            // ImportInterviewButton
            // 
            this.ImportInterviewButton.Caption = "Загрузить файлы ИС";
            this.ImportInterviewButton.Id = 15;
            this.ImportInterviewButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ImportInterviewButton.ImageOptions.Image")));
            this.ImportInterviewButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ImportInterviewButton.ImageOptions.LargeImage")));
            this.ImportInterviewButton.Name = "ImportInterviewButton";
            this.ImportInterviewButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.ImportInterviewButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ImportInterviewButton_ItemClick);
            // 
            // FinishExamButton
            // 
            this.FinishExamButton.Caption = "Завершить экзамен";
            this.FinishExamButton.Id = 16;
            this.FinishExamButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("FinishExamButton.ImageOptions.Image")));
            this.FinishExamButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("FinishExamButton.ImageOptions.LargeImage")));
            this.FinishExamButton.Name = "FinishExamButton";
            this.FinishExamButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // SettingsButton
            // 
            this.SettingsButton.Caption = "Настройки";
            this.SettingsButton.Id = 17;
            this.SettingsButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SettingsButton.ImageOptions.Image")));
            this.SettingsButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SettingsButton.ImageOptions.LargeImage")));
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.SettingsButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SettingsButton_ItemClick);
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "barStaticItem1";
            this.barStaticItem1.Id = 19;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "barButtonItem1";
            this.barButtonItem1.Id = 20;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // ImportPage
            // 
            this.ImportPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ImportPage.Name = "ImportPage";
            this.ImportPage.Text = "Импорт XML";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.ItemLinks.Add(this.OpenXMLFilesButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.ValidateXMLButton);
            this.ribbonPageGroup1.ItemLinks.Add(this.ImportXMLFilesButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "Импорт XML файлов";
            // 
            // FinalInterviewPage
            // 
            this.FinalInterviewPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2});
            this.FinalInterviewPage.Name = "FinalInterviewPage";
            this.FinalInterviewPage.Text = "Итоговое собеседование";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.ExportInterviewButton);
            this.ribbonPageGroup2.ItemLinks.Add(this.ImportInterviewButton);
            this.ribbonPageGroup2.ItemLinks.Add(this.FinishExamButton);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "ribbonPageGroup2";
            // 
            // SettingsPage
            // 
            this.SettingsPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup3});
            this.SettingsPage.Name = "SettingsPage";
            this.SettingsPage.Text = "Настройки";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.SettingsButton);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "ribbonPageGroup3";
            // 
            // RibbonStatusBar
            // 
            this.RibbonStatusBar.ItemLinks.Add(this.barStaticItem1, true);
            this.RibbonStatusBar.Location = new System.Drawing.Point(0, 393);
            this.RibbonStatusBar.Name = "RibbonStatusBar";
            this.RibbonStatusBar.Ribbon = this.ribbonControl1;
            this.RibbonStatusBar.Size = new System.Drawing.Size(837, 28);
            // 
            // MainPanel
            // 
            this.MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainPanel.Location = new System.Drawing.Point(0, 94);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(837, 301);
            this.MainPanel.TabIndex = 1;
            // 
            // ribbonPage2
            // 
            this.ribbonPage2.Name = "ribbonPage2";
            // 
            // ribbonPageGroup4
            // 
            this.ribbonPageGroup4.Name = "ribbonPageGroup4";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 421);
            this.Controls.Add(this.RibbonStatusBar);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.ribbonControl1);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("MainForm.IconOptions.Image")));
            this.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "MainForm";
            this.Text = "Импорт ГИА-9";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ImportPage;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.PanelControl MainPanel;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar RibbonStatusBar;
        private DevExpress.XtraBars.Ribbon.RibbonPage FinalInterviewPage;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
        private DevExpress.XtraBars.Ribbon.RibbonPage SettingsPage;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarButtonItem OpenXMLFilesButton;
        private DevExpress.XtraBars.BarButtonItem ValidateXMLButton;
        private DevExpress.XtraBars.BarButtonItem ImportXMLFilesButton;
        private DevExpress.XtraBars.BarButtonItem ExportInterviewButton;
        private DevExpress.XtraBars.BarButtonItem ImportInterviewButton;
        private DevExpress.XtraBars.BarButtonItem FinishExamButton;
        private DevExpress.XtraBars.BarButtonItem SettingsButton;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
    }
}

