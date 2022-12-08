namespace GiaImport2
{
    partial class ResultWindowWithLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultWindowWithLog));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.ResultGridControl = new DevExpress.XtraGrid.GridControl();
            this.ResultGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ResultEditControl = new DevExpress.XtraRichEdit.RichEditControl();
            this.OkButton = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).BeginInit();
            this.splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).BeginInit();
            this.splitContainerControl1.Panel2.SuspendLayout();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            this.splitContainerControl1.Panel1.Controls.Add(this.ResultGridControl);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            this.splitContainerControl1.Panel2.Controls.Add(this.ResultEditControl);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(773, 463);
            this.splitContainerControl1.SplitterPosition = 196;
            this.splitContainerControl1.TabIndex = 0;
            // 
            // ResultGridControl
            // 
            this.ResultGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultGridControl.Location = new System.Drawing.Point(0, 0);
            this.ResultGridControl.MainView = this.ResultGridView;
            this.ResultGridControl.Name = "ResultGridControl";
            this.ResultGridControl.Size = new System.Drawing.Size(773, 196);
            this.ResultGridControl.TabIndex = 0;
            this.ResultGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ResultGridView});
            // 
            // ResultGridView
            // 
            this.ResultGridView.GridControl = this.ResultGridControl;
            this.ResultGridView.Name = "ResultGridView";
            this.ResultGridView.OptionsBehavior.Editable = false;
            this.ResultGridView.OptionsBehavior.ReadOnly = true;
            this.ResultGridView.OptionsCustomization.AllowGroup = false;
            this.ResultGridView.OptionsView.ShowGroupPanel = false;
            // 
            // ResultEditControl
            // 
            this.ResultEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
            this.ResultEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultEditControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
            this.ResultEditControl.Location = new System.Drawing.Point(0, 0);
            this.ResultEditControl.Name = "ResultEditControl";
            this.ResultEditControl.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.ResultEditControl.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.ResultEditControl.ReadOnly = true;
            this.ResultEditControl.ShowCaretInReadOnly = false;
            this.ResultEditControl.Size = new System.Drawing.Size(773, 255);
            this.ResultEditControl.TabIndex = 0;
            this.ResultEditControl.Text = "richEditControl1";
            this.ResultEditControl.Views.DraftView.Padding = new DevExpress.Portable.PortablePadding(4, 4, 0, 0);
            this.ResultEditControl.Views.SimpleView.Padding = new DevExpress.Portable.PortablePadding(4, 4, 4, 0);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.OkButton.Location = new System.Drawing.Point(327, 484);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(108, 30);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "Ясно";
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // ResultWindowWithLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 536);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.splitContainerControl1);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ResultWindowWithLog.IconOptions.LargeImage")));
            this.Name = "ResultWindowWithLog";
            this.Text = "ResultWindowWithLog";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).EndInit();
            this.splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).EndInit();
            this.splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResultGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.SimpleButton OkButton;
        private DevExpress.XtraGrid.GridControl ResultGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView ResultGridView;
        private DevExpress.XtraRichEdit.RichEditControl ResultEditControl;
    }
}