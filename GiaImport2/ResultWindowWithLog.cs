using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace GiaImport2
{
    public partial class ResultWindowWithLog : DevExpress.XtraEditors.XtraForm
    {
        public ResultWindowWithLog()
        {
            InitializeComponent();
        }
        public void SetTitle(string title)
        {
            this.Text = title;
        }
        public void SetTableData(DataTable tableData)
        {
            this.ResultGridControl.DataSource = tableData;
            this.ResultGridControl.Refresh();
        }
        public void SetTableData(List<TableInfo> tableData)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            foreach (var td in tableData)
            {
                dt.Rows.Add(new Object[]
                {
                    td.Name,
                    td.Description,
                    td.Status
                });
            }

            this.ResultGridControl.DataSource= dt;
            this.ResultGridControl.Refresh();
        }
        public void SetLogData(string logData)
        {
            this.ResultEditControl.Text = logData;
            this.ResultEditControl.Refresh();
        }
        public void SetLogData(ConcurrentDictionary<string, string> logData)
        {
            if (logData == null || (logData != null && logData.Count == 0))
            {
                this.ResultEditControl.Text = "Нет обнаруженных несоответствий внешних ключей.";
                return;
            }
            foreach (var ld in logData)
            {
                this.ResultEditControl.Text += ld.Value;
            }
            this.ResultEditControl.Refresh();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}