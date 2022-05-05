using DevExpress.XtraEditors.Controls;
using GiaImport2.Services;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace GiaImport2
{
    public partial class ExportInterviewWizard : DevExpress.XtraEditors.XtraForm
    {
        IInterviewRepository InterviewRepository;

        public ExportInterviewWizard(IInterviewRepository interviewRepository)
        {
            InitializeComponent();
            this.InterviewRepository = interviewRepository;
        }

        private void wizardControl1_CancelClick(object sender, CancelEventArgs e)
        {
            splashScreenManager1.CloseWaitForm();
            this.Close();
        }

        private async void wizardControl1_SelectedPageChanged(object sender, DevExpress.XtraWizard.WizardPageChangedEventArgs e)
        {
            if (e.Page == welcomeWizardPage1)
            {
                splashScreenManager1.ShowWaitForm();
                var examDates = await InterviewRepository.GetExamDates();
                if (examDates != null)
                {
                    ComboBoxItemCollection itemsCollection = ExamDatesCombo.Properties.Items;
                    itemsCollection.BeginUpdate();
                    try
                    {
                        foreach (var item in examDates)
                            itemsCollection.Add(item);
                    }
                    finally
                    {
                        itemsCollection.EndUpdate();
                    }
                    ExamDatesCombo.SelectedIndex = 0;
                }
                
                splashScreenManager1.CloseWaitForm();
            }
        }
    }
}