using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.ViewInfo;
using GiaImport2.Models;
using System.Drawing;

namespace GiaImport2
{
    public partial class SchoolsClassesPanel : XtraUserControl
    {
        RepositoryItemCheckEdit CheckEdit;

        public TreeList GetTreeList()
        {
            return this.SchoolClassesTreeList;
        }

        public SchoolsClassesPanel()
        {
            InitializeComponent();
        }

        protected void DrawCheckBox(GraphicsCache cache, RepositoryItemCheckEdit edit, Rectangle r, bool cchecked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = cchecked;
            info.Bounds = r;
            info.CalcViewInfo();
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, r);
            painter.Draw(args);
        }
        private bool IsAllSelected(TreeList tree)
        {
            return tree.GetAllCheckedNodes().Count > 0;
        }
        private void EmbeddedCheckBoxChecked(TreeList tree)
        {
            SelectAll(tree);
        }

        class SelectNodeOperation : TreeListOperation
        {
            public override void Execute(TreeListNode node)
            {
                node.Checked = !node.Checked;
            }
        }
        private void SelectAll(TreeList tree)
        {
            tree.BeginUpdate();
            tree.NodesIterator.DoOperation(new SelectNodeOperation());
            tree.EndUpdate();
        }

        private void SchoolsClassesPanel_Load(object sender, System.EventArgs e)
        {
            CheckEdit = (RepositoryItemCheckEdit)SchoolClassesTreeList.RepositoryItems.Add("CheckEdit");
            SchoolClassesTreeList.OptionsSelection.MultiSelect = true;
            SchoolClassesTreeList.OptionsFind.ParserKind = DevExpress.Data.Filtering.FindPanelParserKind.Or;
            //SchoolClassesTreeList.OptionsCustomization.AllowSort = false;
            //SchoolClassesTreeList.Sort(null, SchoolClassesTreeList.Columns[0], SortOrder.Ascending, true);
        }

        private void SchoolClassesTreeList_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
        {
            if (e.Column != null && e.Column.VisibleIndex == 0)
            {
                Rectangle checkRect = new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 3, 12, 12);
                ColumnInfo info = (ColumnInfo)e.ObjectArgs;
                if (info.CaptionRect.Left < 30)
                    info.CaptionRect = new Rectangle(new Point(info.CaptionRect.Left + 15, info.CaptionRect.Top), info.CaptionRect.Size);
                e.Painter.DrawObject(info);

                DrawCheckBox(e.Cache, CheckEdit, checkRect, IsAllSelected(sender as TreeList));
                e.Handled = true;
            }
        }

        private void SchoolClassesTreeList_SelectionChanged(object sender, System.EventArgs e)
        {
            SchoolClassesTreeList.InvalidateColumnPanel();
        }

        private void SchoolClassesTreeList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TreeList tree = sender as TreeList;
            Point pt = new Point(e.X, e.Y);
            TreeListHitInfo hit = tree.CalcHitInfo(pt);
            if (hit.Column != null)
            {
                ColumnInfo info = tree.ViewInfo.ColumnsInfo[hit.Column];
                Rectangle checkRect = new Rectangle(info.Bounds.Left + 3, info.Bounds.Top + 3, 12, 12);
                if (checkRect.Contains(pt))
                {
                    EmbeddedCheckBoxChecked(tree);
                    // хак для отключения дальнейшей обработки клика мышкой и нежелательной сортировки по столбцу
                    DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                }
            }
        }

        private void SchoolClassesTreeList_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            // сортировка по коду школы
            e.Result = ((SchoolClasses)e.RowObject1).SchoolCode - ((SchoolClasses)e.RowObject2).SchoolCode;
        }
    }
}
