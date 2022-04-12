using GiaImport2.Services;
using System;

namespace GiaImport2
{
    public partial class SettingsWindow : DevExpress.XtraEditors.XtraForm
    {
        ICommonRepository CommonRepository;

        public SettingsWindow(ICommonRepository commonRepository)
        {
            InitializeComponent();
            this.CommonRepository = commonRepository;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CheckConnectionButton_Click(object sender, EventArgs e)
        {
            FormsHelper.ShowStyledMessageBox("ee",this.CommonRepository.GetCurrentScheme().Version);
        }
    }
}