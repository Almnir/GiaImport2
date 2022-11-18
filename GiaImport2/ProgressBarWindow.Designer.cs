namespace GiaImport2
{
    partial class ProgressBarWindow
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
            this.pbLabel = new DevExpress.XtraEditors.LabelControl();
            this.progressBarTotal = new DevExpress.XtraEditors.ProgressBarControl();
            this.progressBarLine = new DevExpress.XtraEditors.ProgressBarControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarTotal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarLine.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.progressBarLine);
            this.panelControl1.Controls.Add(this.cancelButton);
            this.panelControl1.Controls.Add(this.pbLabel);
            this.panelControl1.Controls.Add(this.progressBarTotal);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(504, 141);
            this.panelControl1.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(179, 111);
            this.cancelButton.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.cancelButton.LookAndFeel.UseDefaultLookAndFeel = false;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(131, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Отменить";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // pbLabel
            // 
            this.pbLabel.Location = new System.Drawing.Point(156, 12);
            this.pbLabel.Name = "pbLabel";
            this.pbLabel.Size = new System.Drawing.Size(0, 13);
            this.pbLabel.TabIndex = 2;
            // 
            // progressBarTotal
            // 
            this.progressBarTotal.Location = new System.Drawing.Point(12, 31);
            this.progressBarTotal.Name = "progressBarTotal";
            this.progressBarTotal.Size = new System.Drawing.Size(480, 23);
            this.progressBarTotal.TabIndex = 1;
            // 
            // progressBarLine
            // 
            this.progressBarLine.Location = new System.Drawing.Point(12, 59);
            this.progressBarLine.Name = "progressBarLine";
            this.progressBarLine.Size = new System.Drawing.Size(480, 46);
            this.progressBarLine.TabIndex = 4;
            // 
            // ProgressBarWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(504, 141);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressBarWindow";
            this.ShowInTaskbar = false;
            this.Text = "Выполнение";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProgressBarWindow_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarTotal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarLine.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl pbLabel;
        private DevExpress.XtraEditors.ProgressBarControl progressBarTotal;
        private DevExpress.XtraEditors.SimpleButton cancelButton;
        private DevExpress.XtraEditors.ProgressBarControl progressBarLine;
    }
}