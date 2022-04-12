using GiaImport2.Services;
using System.ComponentModel;

namespace GiaImport2
{
    public partial class ExportInterviewWizard : DevExpress.XtraEditors.XtraForm
    {
        ICommonRepository CommonRepository;

        public ExportInterviewWizard(ICommonRepository commonRepository)
        {
            InitializeComponent();
            this.CommonRepository = commonRepository;
        }

        private void wizardControl1_CancelClick(object sender, CancelEventArgs e)
        {
            this.Close();
        }

        private void wizardControl1_SelectedPageChanged(object sender, DevExpress.XtraWizard.WizardPageChangedEventArgs e)
        {
            if (e.Page == welcomeWizardPage1)
            {
                FormsHelper.ShowStyledMessageBox("sda", CommonRepository.GetCurrentScheme().Version);
            }
        }
    }
}