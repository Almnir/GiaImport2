namespace GiaImport2
{
    public partial class MessageForm : DevExpress.XtraEditors.XtraForm
    {
        public MessageForm()
        {
            InitializeComponent();
        }
        public void SetTitle(string title)
        {
            this.Text = title;
        }
        public void SetExtendedContent(string content)
        {
            this.MessageEditControl.Text = content;
            this.MessageEditControl.ScrollToCaret();
        }

        private void OkButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}