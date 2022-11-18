namespace GiaImport2
{
    partial class SchoolsPanel
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
            this.SchoolsGridControl = new DevExpress.XtraGrid.GridControl();
            this.SchoolsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.SchoolCodeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ShortNameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.SchoolsGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SchoolsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // SchoolsGridControl
            // 
            this.SchoolsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SchoolsGridControl.Location = new System.Drawing.Point(0, 0);
            this.SchoolsGridControl.MainView = this.SchoolsGridView;
            this.SchoolsGridControl.Name = "SchoolsGridControl";
            this.SchoolsGridControl.Size = new System.Drawing.Size(662, 371);
            this.SchoolsGridControl.TabIndex = 0;
            this.SchoolsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.SchoolsGridView});
            // 
            // SchoolsGridView
            // 
            this.SchoolsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.SchoolCodeColumn,
            this.ShortNameColumn});
            this.SchoolsGridView.GridControl = this.SchoolsGridControl;
            this.SchoolsGridView.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.None, "", null, "")});
            this.SchoolsGridView.Name = "SchoolsGridView";
            this.SchoolsGridView.OptionsBehavior.Editable = false;
            this.SchoolsGridView.OptionsBehavior.ReadOnly = true;
            this.SchoolsGridView.OptionsFind.AlwaysVisible = true;
            this.SchoolsGridView.OptionsMenu.EnableFooterMenu = false;
            this.SchoolsGridView.OptionsSelection.MultiSelect = true;
            this.SchoolsGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.SchoolsGridView.OptionsView.ShowFooter = true;
            this.SchoolsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // SchoolCodeColumn
            // 
            this.SchoolCodeColumn.Caption = "Код ОО";
            this.SchoolCodeColumn.FieldName = "SchoolCode";
            this.SchoolCodeColumn.Name = "SchoolCodeColumn";
            this.SchoolCodeColumn.Visible = true;
            this.SchoolCodeColumn.VisibleIndex = 1;
            // 
            // ShortNameColumn
            // 
            this.ShortNameColumn.Caption = "Название";
            this.ShortNameColumn.FieldName = "ShortName";
            this.ShortNameColumn.Name = "ShortNameColumn";
            this.ShortNameColumn.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "ShortName", "{0}")});
            this.ShortNameColumn.Visible = true;
            this.ShortNameColumn.VisibleIndex = 2;
            // 
            // SchoolsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SchoolsGridControl);
            this.Name = "SchoolsPanel";
            this.Size = new System.Drawing.Size(662, 371);
            this.Load += new System.EventHandler(this.SchoolsPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SchoolsGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SchoolsGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl SchoolsGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView SchoolsGridView;
        private DevExpress.XtraGrid.Columns.GridColumn SchoolCodeColumn;
        private DevExpress.XtraGrid.Columns.GridColumn ShortNameColumn;
    }
}
