using System.Data;

namespace GiaImport2
{
    public partial class ResultStatisticsControl : DevExpress.XtraEditors.XtraUserControl
    {
        public ResultStatisticsControl(DataTable dataTable, string logText)
        {
            InitializeComponent();
            this.StatisticsGridControl.DataSource = dataTable;
            this.StatisticsGridControl.Refresh();
            this.LogRichEditControl.Text = logText;
        }
    }
}
