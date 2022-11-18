namespace GiaImport2
{
    partial class ResultStatisticsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.StatisticsGridControl = new DevExpress.XtraGrid.GridControl();
            this.StatisticsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
            this.LogRichEditControl = new DevExpress.XtraRichEdit.RichEditControl();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatisticsGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatisticsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.LogRichEditControl);
            this.layoutControl1.Controls.Add(this.StatisticsGridControl);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(591, 0, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(558, 375);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.simpleSeparator1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(558, 375);
            this.Root.TextVisible = false;
            // 
            // StatisticsGridControl
            // 
            this.StatisticsGridControl.Location = new System.Drawing.Point(12, 12);
            this.StatisticsGridControl.MainView = this.StatisticsGridView;
            this.StatisticsGridControl.Name = "StatisticsGridControl";
            this.StatisticsGridControl.Size = new System.Drawing.Size(534, 173);
            this.StatisticsGridControl.TabIndex = 4;
            this.StatisticsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.StatisticsGridView});
            // 
            // StatisticsGridView
            // 
            this.StatisticsGridView.GridControl = this.StatisticsGridControl;
            this.StatisticsGridView.Name = "StatisticsGridView";
            this.StatisticsGridView.OptionsBehavior.Editable = false;
            this.StatisticsGridView.OptionsBehavior.ReadOnly = true;
            this.StatisticsGridView.OptionsCustomization.AllowGroup = false;
            this.StatisticsGridView.OptionsMenu.EnableGroupPanelMenu = false;
            this.StatisticsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.StatisticsGridControl;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(538, 177);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // simpleSeparator1
            // 
            this.simpleSeparator1.AllowHotTrack = false;
            this.simpleSeparator1.Location = new System.Drawing.Point(0, 177);
            this.simpleSeparator1.Name = "simpleSeparator1";
            this.simpleSeparator1.Size = new System.Drawing.Size(538, 1);
            // 
            // LogRichEditControl
            // 
            this.LogRichEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogRichEditControl.Location = new System.Drawing.Point(109, 190);
            this.LogRichEditControl.Name = "LogRichEditControl";
            this.LogRichEditControl.Options.AutoCorrect.DetectUrls = false;
            this.LogRichEditControl.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.LogRichEditControl.Options.SpellChecker.AutoDetectDocumentCulture = false;
            this.LogRichEditControl.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.LogRichEditControl.ReadOnly = true;
            this.LogRichEditControl.ShowCaretInReadOnly = false;
            this.LogRichEditControl.Size = new System.Drawing.Size(437, 173);
            this.LogRichEditControl.TabIndex = 5;
            this.LogRichEditControl.Text = "richEditControl1";
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.LogRichEditControl;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 178);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(538, 177);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(93, 13);
            // 
            // ResultStatisticsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "ResultStatisticsControl";
            this.Size = new System.Drawing.Size(558, 375);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatisticsGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatisticsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraRichEdit.RichEditControl LogRichEditControl;
        private DevExpress.XtraGrid.GridControl StatisticsGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView StatisticsGridView;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.SimpleSeparator simpleSeparator1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
