namespace GiaImport2
{
    partial class ExportInterviewPanel
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
            this.ExportInterviewGrid = new DevExpress.XtraGrid.GridControl();
            this.ExportInterviewView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.GovernmentColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SchoolColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.FileNameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.FileSizeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.FileCRCColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ExportInterviewGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExportInterviewView)).BeginInit();
            this.SuspendLayout();
            // 
            // ExportInterviewGrid
            // 
            this.ExportInterviewGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExportInterviewGrid.Location = new System.Drawing.Point(0, 0);
            this.ExportInterviewGrid.MainView = this.ExportInterviewView;
            this.ExportInterviewGrid.Name = "ExportInterviewGrid";
            this.ExportInterviewGrid.Size = new System.Drawing.Size(450, 288);
            this.ExportInterviewGrid.TabIndex = 0;
            this.ExportInterviewGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ExportInterviewView});
            // 
            // ExportInterviewView
            // 
            this.ExportInterviewView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.GovernmentColumn,
            this.SchoolColumn,
            this.FileNameColumn,
            this.FileSizeColumn,
            this.FileCRCColumn});
            this.ExportInterviewView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.ExportInterviewView.GridControl = this.ExportInterviewGrid;
            this.ExportInterviewView.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, "Итого:", null, "")});
            this.ExportInterviewView.Name = "ExportInterviewView";
            this.ExportInterviewView.OptionsBehavior.Editable = false;
            this.ExportInterviewView.OptionsBehavior.ReadOnly = true;
            this.ExportInterviewView.OptionsDetail.ShowDetailTabs = false;
            this.ExportInterviewView.OptionsFind.AllowFindPanel = false;
            this.ExportInterviewView.OptionsHint.ShowCellHints = false;
            this.ExportInterviewView.OptionsHint.ShowColumnHeaderHints = false;
            this.ExportInterviewView.OptionsHint.ShowFooterHints = false;
            this.ExportInterviewView.OptionsMenu.EnableFooterMenu = false;
            this.ExportInterviewView.OptionsMenu.EnableGroupPanelMenu = false;
            this.ExportInterviewView.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.ExportInterviewView.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.False;
            this.ExportInterviewView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.ExportInterviewView.OptionsSelection.UseIndicatorForSelection = false;
            this.ExportInterviewView.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.ExportInterviewView.OptionsView.ShowFooter = true;
            this.ExportInterviewView.OptionsView.ShowIndicator = false;
            // 
            // GovernmentColumn
            // 
            this.GovernmentColumn.Caption = "ОИВ";
            this.GovernmentColumn.FieldName = "GovernmentName";
            this.GovernmentColumn.Name = "GovernmentColumn";
            this.GovernmentColumn.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "GovernmentName", "Итого: {0} файлов")});
            this.GovernmentColumn.Visible = true;
            this.GovernmentColumn.VisibleIndex = 0;
            // 
            // SchoolColumn
            // 
            this.SchoolColumn.Caption = "ОО";
            this.SchoolColumn.FieldName = "SchoolName";
            this.SchoolColumn.Name = "SchoolColumn";
            this.SchoolColumn.Visible = true;
            this.SchoolColumn.VisibleIndex = 1;
            // 
            // FileNameColumn
            // 
            this.FileNameColumn.Caption = "Имя файла";
            this.FileNameColumn.FieldName = "FileName";
            this.FileNameColumn.Name = "FileNameColumn";
            this.FileNameColumn.Visible = true;
            this.FileNameColumn.VisibleIndex = 2;
            // 
            // FileSizeColumn
            // 
            this.FileSizeColumn.Caption = "Размер файла";
            this.FileSizeColumn.FieldName = "FileSize";
            this.FileSizeColumn.Name = "FileSizeColumn";
            this.FileSizeColumn.Visible = true;
            this.FileSizeColumn.VisibleIndex = 3;
            // 
            // FileCRCColumn
            // 
            this.FileCRCColumn.Caption = "CRC";
            this.FileCRCColumn.FieldName = "FileCRC";
            this.FileCRCColumn.Name = "FileCRCColumn";
            this.FileCRCColumn.Visible = true;
            this.FileCRCColumn.VisibleIndex = 4;
            // 
            // ExportInterviewPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ExportInterviewGrid);
            this.Name = "ExportInterviewPanel";
            this.Size = new System.Drawing.Size(450, 288);
            ((System.ComponentModel.ISupportInitialize)(this.ExportInterviewGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExportInterviewView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl ExportInterviewGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView ExportInterviewView;
        private DevExpress.XtraGrid.Columns.GridColumn GovernmentColumn;
        private DevExpress.XtraGrid.Columns.GridColumn SchoolColumn;
        private DevExpress.XtraGrid.Columns.GridColumn FileNameColumn;
        private DevExpress.XtraGrid.Columns.GridColumn FileSizeColumn;
        private DevExpress.XtraGrid.Columns.GridColumn FileCRCColumn;
    }
}
