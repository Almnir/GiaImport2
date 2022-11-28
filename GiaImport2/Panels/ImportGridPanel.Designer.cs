namespace GiaImport2
{
    partial class ImportGridPanel
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ImportGridControl = new DevExpress.XtraGrid.GridControl();
            this.ImportGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.ImportGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImportGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ImportGridControl
            // 
            this.ImportGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImportGridControl.Location = new System.Drawing.Point(0, 0);
            this.ImportGridControl.MainView = this.ImportGridView;
            this.ImportGridControl.Name = "ImportGridControl";
            this.ImportGridControl.Size = new System.Drawing.Size(630, 443);
            this.ImportGridControl.TabIndex = 0;
            this.ImportGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ImportGridView});
            // 
            // ImportGridView
            // 
            this.ImportGridView.GridControl = this.ImportGridControl;
            this.ImportGridView.Name = "ImportGridView";
            this.ImportGridView.OptionsCustomization.AllowGroup = false;
            this.ImportGridView.OptionsSelection.MultiSelect = true;
            this.ImportGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.ImportGridView.OptionsView.ShowFooter = true;
            this.ImportGridView.OptionsView.ShowGroupPanel = false;
            // 
            // ImportGridPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ImportGridControl);
            this.Name = "ImportGridPanel";
            this.Size = new System.Drawing.Size(630, 443);
            ((System.ComponentModel.ISupportInitialize)(this.ImportGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImportGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl ImportGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView ImportGridView;
    }
}
