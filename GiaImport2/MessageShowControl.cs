using System;
using System.Collections.Concurrent;
using System.Text;

namespace GiaImport2
{
    public static class MessageShowControl
    {
        public static void ShowValidationErrors(ConcurrentDictionary<string, string> filesErrors)
        {
            MessageForm frm = new MessageForm();
            frm.SetTitle("Результаты проверки");
            //frm.SetStyling(MessageForm.EnumMessageStyle.Error);
            //frm.SetContent("Файлы, не прошедшие проверку и причина ошибки");
            StringBuilder fileErrorText = new StringBuilder();
            foreach (var fe in filesErrors)
            {
                fileErrorText.Append(string.Format("{0} - {1}", fe.Key, fe.Value)).Append(Environment.NewLine);
            }
            frm.SetExtendedContent(fileErrorText.ToString());
            frm.ShowDialog();
        }
        public static void ShowValidationErrors(string v)
        {
            MessageForm frm = new MessageForm();
            frm.SetTitle("Результаты проверки");
            frm.SetExtendedContent(v.ToString());
            frm.ShowDialog();
        }

        internal static void ShowImportErrors(string v)
        {
            MessageForm frm = new MessageForm();
            frm.SetTitle("Ошибки импорта");
            frm.SetExtendedContent(v.ToString());
            frm.ShowDialog();
        }
    }
}
