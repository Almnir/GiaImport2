using DevExpress.XtraEditors;
using System.Threading;

namespace GiaImport2
{
    public partial class ProgressBarWindow : DevExpress.XtraEditors.XtraForm
    {
        private CancellationTokenSource Source;

        public ProgressBarWindow() : base()
        {
            InitializeComponent();
            
        }
        public void SetCancellationToken(CancellationTokenSource source)
        {
            this.Source = source;
        }
        public void SetTitle(string title)
        {
            this.Text = title;
        }

        public ProgressBarControl GetProgressBarTotal()
        {
            return this.progressBarTotal;
        }
        public ProgressBarControl GetProgressBarLine()
        {
            return this.progressBarLine;
        }

        public LabelControl GetLabel()
        {
            return this.pbLabel;
        }

        private void ProgressBarWindow_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (progressBarTotal != null && !progressBarTotal.IsDisposed) progressBarTotal.Dispose();
            if (progressBarLine != null && !progressBarLine.IsDisposed) progressBarLine.Dispose();
        }

        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            Source.Cancel();
        }
    }
}