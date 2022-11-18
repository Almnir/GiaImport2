namespace GiaImport2
{
    partial class SchoolsClassesPanel
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
            this.SchoolClassesTreeList = new DevExpress.XtraTreeList.TreeList();
            this.ShortNameColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)(this.SchoolClassesTreeList)).BeginInit();
            this.SuspendLayout();
            // 
            // SchoolClassesTreeList
            // 
            this.SchoolClassesTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.ShortNameColumn});
            this.SchoolClassesTreeList.CustomizationFormBounds = new System.Drawing.Rectangle(318, 204, 266, 243);
            this.SchoolClassesTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SchoolClassesTreeList.Location = new System.Drawing.Point(0, 0);
            this.SchoolClassesTreeList.Name = "SchoolClassesTreeList";
            this.SchoolClassesTreeList.OptionsBehavior.Editable = false;
            this.SchoolClassesTreeList.OptionsBehavior.ReadOnly = true;
            this.SchoolClassesTreeList.OptionsFilter.ExpandNodesOnFiltering = true;
            this.SchoolClassesTreeList.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.EntireBranch;
            this.SchoolClassesTreeList.OptionsFind.AllowIncrementalSearch = true;
            this.SchoolClassesTreeList.OptionsFind.AlwaysVisible = true;
            this.SchoolClassesTreeList.OptionsFind.ExpandNodesOnIncrementalSearch = true;
            this.SchoolClassesTreeList.OptionsFind.FindNullPrompt = "Введите текст для поиска...";
            this.SchoolClassesTreeList.OptionsSelection.MultiSelect = true;
            this.SchoolClassesTreeList.OptionsSelection.MultiSelectMode = DevExpress.XtraTreeList.TreeListMultiSelectMode.CellSelect;
            this.SchoolClassesTreeList.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
            this.SchoolClassesTreeList.OptionsView.RootCheckBoxStyle = DevExpress.XtraTreeList.NodeCheckBoxStyle.None;
            this.SchoolClassesTreeList.OptionsView.ShowSummaryFooter = true;
            this.SchoolClassesTreeList.Size = new System.Drawing.Size(537, 339);
            this.SchoolClassesTreeList.TabIndex = 0;
            this.SchoolClassesTreeList.CustomColumnSort += new DevExpress.XtraTreeList.CustomColumnSortEventHandler(this.SchoolClassesTreeList_CustomColumnSort);
            this.SchoolClassesTreeList.SelectionChanged += new System.EventHandler(this.SchoolClassesTreeList_SelectionChanged);
            this.SchoolClassesTreeList.CustomDrawColumnHeader += new DevExpress.XtraTreeList.CustomDrawColumnHeaderEventHandler(this.SchoolClassesTreeList_CustomDrawColumnHeader);
            this.SchoolClassesTreeList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SchoolClassesTreeList_MouseUp);
            // 
            // ShortNameColumn
            // 
            this.ShortNameColumn.Caption = "Отметить/снять все";
            this.ShortNameColumn.FieldName = "ShortName";
            this.ShortNameColumn.Name = "ShortNameColumn";
            this.ShortNameColumn.OptionsColumn.AllowEdit = false;
            this.ShortNameColumn.OptionsColumn.AllowMove = false;
            this.ShortNameColumn.OptionsColumn.AllowMoveToCustomizationForm = false;
            this.ShortNameColumn.OptionsColumn.AllowSize = false;
            this.ShortNameColumn.OptionsColumn.AllowSort = true;
            this.ShortNameColumn.OptionsColumn.ReadOnly = true;
            this.ShortNameColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.ShortNameColumn.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.ShortNameColumn.SummaryFooter = DevExpress.XtraTreeList.SummaryItemType.Count;
            this.ShortNameColumn.Visible = true;
            this.ShortNameColumn.VisibleIndex = 0;
            // 
            // SchoolsClassesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SchoolClassesTreeList);
            this.Name = "SchoolsClassesPanel";
            this.Size = new System.Drawing.Size(537, 339);
            this.Load += new System.EventHandler(this.SchoolsClassesPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SchoolClassesTreeList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList SchoolClassesTreeList;
        private DevExpress.XtraTreeList.Columns.TreeListColumn ShortNameColumn;
    }
}
