using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace GiaImport2
{
    public partial class ExportInterviewPanel : DevExpress.XtraEditors.XtraUserControl
    {
        public GridControl GetExportInterviewGrid()
        {
            return this.ExportInterviewGrid;
        }
    
        public GridView GetExportInterviewView()
        {
            return this.ExportInterviewView;
        }
        public ExportInterviewPanel()
        {
            InitializeComponent();
        }
    }
}
