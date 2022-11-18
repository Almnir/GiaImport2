using DevExpress.XtraGrid;

namespace GiaImport2
{
    public partial class PreCheckControl : DevExpress.XtraEditors.XtraUserControl
    {
        public GridControl GetGridControl()
        {
            return PreCheckGridControl;
        }
        public PreCheckControl()
        {
            InitializeComponent();
        }
    }
}
