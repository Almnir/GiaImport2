namespace GiaImport2
{
    partial class PreCheckControl
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
            this.PreCheckGridControl = new DevExpress.XtraGrid.GridControl();
            this.PreCheckGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.NameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DescriptionColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.StatusColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.PreCheckGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreCheckGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // PreCheckGridControl
            // 
            this.PreCheckGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreCheckGridControl.Location = new System.Drawing.Point(0, 0);
            this.PreCheckGridControl.MainView = this.PreCheckGridView;
            this.PreCheckGridControl.Name = "PreCheckGridControl";
            this.PreCheckGridControl.Size = new System.Drawing.Size(701, 406);
            this.PreCheckGridControl.TabIndex = 0;
            this.PreCheckGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.PreCheckGridView});
            // 
            // PreCheckGridView
            // 
            this.PreCheckGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.NameColumn,
            this.DescriptionColumn,
            this.StatusColumn});
            this.PreCheckGridView.GridControl = this.PreCheckGridControl;
            this.PreCheckGridView.Name = "PreCheckGridView";
            this.PreCheckGridView.OptionsCustomization.AllowGroup = false;
            this.PreCheckGridView.OptionsMenu.EnableGroupPanelMenu = false;
            this.PreCheckGridView.OptionsView.ShowGroupPanel = false;
            // 
            // NameColumn
            // 
            this.NameColumn.Caption = "Название";
            this.NameColumn.FieldName = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.Visible = true;
            this.NameColumn.VisibleIndex = 0;
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.Caption = "Описание";
            this.DescriptionColumn.FieldName = "Description";
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.Visible = true;
            this.DescriptionColumn.VisibleIndex = 1;
            // 
            // StatusColumn
            // 
            this.StatusColumn.Caption = "Статус операции";
            this.StatusColumn.FieldName = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.Visible = true;
            this.StatusColumn.VisibleIndex = 2;
            // 
            // PreCheckControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PreCheckGridControl);
            this.Name = "PreCheckControl";
            this.Size = new System.Drawing.Size(701, 406);
            ((System.ComponentModel.ISupportInitialize)(this.PreCheckGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PreCheckGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl PreCheckGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView PreCheckGridView;
        private DevExpress.XtraGrid.Columns.GridColumn NameColumn;
        private DevExpress.XtraGrid.Columns.GridColumn DescriptionColumn;
        private DevExpress.XtraGrid.Columns.GridColumn StatusColumn;
    }
}
