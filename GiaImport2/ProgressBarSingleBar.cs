using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GiaImport2
{
    public partial class ProgressBarSingleBar : DevExpress.XtraEditors.XtraForm
    {
        private CancellationTokenSource Source;
        public ProgressBarSingleBar() : base()
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

        public LabelControl GetLabel()
        {
            return this.pbLabel;
        }

        private void ProgressBarMarquee_FormClosing(object sender, FormClosingEventArgs e)
        {
            Source.Cancel();
        }
    }
}