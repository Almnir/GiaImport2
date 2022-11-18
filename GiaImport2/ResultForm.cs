using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GiaImport2
{
    public partial class ResultForm : DevExpress.XtraEditors.XtraForm
    {
        public PanelControl GetPanelControl()
        {
            return this.ResultPanelControl;
        }

        public ResultForm(string title)
        {
            InitializeComponent();
            this.Text = title;
        }
    }
}