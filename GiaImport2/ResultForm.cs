using DevExpress.XtraEditors;

namespace GiaImport2
{
    public partial class ResultForm : DevExpress.XtraEditors.XtraForm
    {
        public PanelControl GetPanelControl()
        {
            return this.ResultPanelControl;
        }
        public void SetTitle(string title)
        {
            this.Text = title;
        }
        public ResultForm()
        {
            InitializeComponent();
        }
    }
}