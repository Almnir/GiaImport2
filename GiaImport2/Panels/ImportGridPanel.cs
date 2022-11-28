using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
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
    public partial class ImportGridPanel : UserControl
    {
        public GridControl GetImportGridPanelGrid()
        {
            return this.ImportGridControl;
        }

        public GridView GetImportGridPanelView()
        {
            return this.ImportGridView;
        }
        public ImportGridPanel()
        {
            InitializeComponent();
        }
    }
}
