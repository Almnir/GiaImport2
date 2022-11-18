using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace GiaImport2
{
    public partial class SchoolsPanel : DevExpress.XtraEditors.XtraUserControl
    {
        public GridControl GetGridControl()
        {
            return this.SchoolsGridControl;
        }
        public GridView GetGridView()
        {
            return this.SchoolsGridView;
        }

        public SchoolsPanel()
        {
            InitializeComponent();
        }

        private void SchoolsPanel_Load(object sender, System.EventArgs e)
        {
            this.SchoolsGridView.OptionsFind.ParserKind = DevExpress.Data.Filtering.FindPanelParserKind.Or;
        }
    }
}
