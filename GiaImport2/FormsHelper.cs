using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
using System.Windows.Forms;

namespace GiaImport2
{
    public static class FormsHelper
    {
        /// <summary>
        /// Показать стилизованный простой месседжбокс без возвращаемых значений
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        public static void ShowStyledMessageBox(string caption, string message)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs();
            args.Caption = caption;
            args.Text = message;
            var messageBoxCustomUserLookAndFeel = new UserLookAndFeel(MainForm.ActiveForm)
            {
                SkinName = "Office 2013 Light Gray",
                Style = LookAndFeelStyle.Skin,
                UseDefaultLookAndFeel = false,
                UseWindowsXPTheme = false
            };
            args.LookAndFeel = messageBoxCustomUserLookAndFeel;
            XtraMessageBox.Show(args);
        }

        /// <summary>
        /// Показать стилизованный месседжбокс да-нет и вернуть выбор
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static DialogResult ShowStyledYesNoMessageBox(string caption, string message)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs();
            args.Caption = caption;
            args.Text = message;
            args.Buttons = new DialogResult[] { DialogResult.Yes, DialogResult.No };
            args.Icon = SystemIcons.Question;
            var messageBoxCustomUserLookAndFeel = new UserLookAndFeel(MainForm.ActiveForm)
            {
                SkinName = "Office 2013 Light Gray",
                Style = LookAndFeelStyle.Skin,
                UseDefaultLookAndFeel = false,
                UseWindowsXPTheme = false
            };
            args.LookAndFeel = messageBoxCustomUserLookAndFeel;
            return XtraMessageBox.Show(args);
        }

        /// <summary>
        /// Кастомный пустой фон грида
        /// </summary>
        /// <param name="gridControl"></param>
        /// <param name="gridView"></param>
        public static void CustomDrawEmptyForeground(GridControl gridControl, GridView gridView)
        {
            string searchName = string.Empty;
            //gridView.ActiveFilterCriteria = new BinaryOperator("SubjectName", searchName);

            Font noMatchesFoundTextFont = new Font("Tahoma", 10);
            Font trySearchingAgainTextFont = new Font("Tahoma", 15, FontStyle.Underline);
            Font trySearchingAgainTextFontBold = new Font(trySearchingAgainTextFont, FontStyle.Underline | FontStyle.Bold);
            SolidBrush linkBrush = new SolidBrush(DevExpress.Skins.EditorsSkins.GetSkin(DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveLookAndFeel).Colors["HyperLinkTextColor"]);
            string noMatchesFoundText = "Ничего не найдено.";
            string trySearchingAgainText = "Загрузите или начните добавлять новое.";
            Rectangle noMatchesFoundBounds = Rectangle.Empty;
            Rectangle trySearchingAgainBounds = Rectangle.Empty;
            bool trySearchingAgainBoundsContainCursor = false;
            int offset = 10;

            gridView.CustomDrawEmptyForeground += (s, e) => {
                e.DefaultDraw();
                e.Appearance.Options.UseFont = true;
                e.Appearance.Font = noMatchesFoundTextFont;
                Size size = e.Appearance.CalcTextSize(e.Cache, noMatchesFoundText, e.Bounds.Width).ToSize();
                int x = (e.Bounds.Width - size.Width) / 2;
                int y = e.Bounds.Y + offset;
                noMatchesFoundBounds = new Rectangle(new Point(x, y), size);
                e.Appearance.DrawString(e.Cache, noMatchesFoundText, noMatchesFoundBounds);
                e.Appearance.Font = trySearchingAgainBoundsContainCursor ? trySearchingAgainTextFontBold : trySearchingAgainTextFont;
                size = e.Appearance.CalcTextSize(e.Cache, trySearchingAgainText, e.Bounds.Width).ToSize();
                x = noMatchesFoundBounds.X - (size.Width - noMatchesFoundBounds.Width) / 2;
                y = noMatchesFoundBounds.Bottom + offset;
                size.Width += offset;
                trySearchingAgainBounds = new Rectangle(new Point(x, y), size);
                e.Appearance.DrawString(e.Cache, trySearchingAgainText, trySearchingAgainBounds, linkBrush);
            };
        }
    }
}
