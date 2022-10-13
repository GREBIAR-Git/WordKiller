using System.Windows.Controls;
using System.Windows.Documents;

namespace WordKiller
{
    static class RTBox
    {
        public static string GetText(RichTextBox richTextBox)
        {
            string text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            if (text != string.Empty)
            {
                text = text.Replace("\r", "");
                text = text.Remove(text.Length - 1, 1);
            }
            return text;
        }

        public static void SetText(RichTextBox richTextBox, string text)
        {
            FlowDocument allText = new();
            allText.Blocks.Add(new Paragraph(new Run(text)));
            richTextBox.Document = allText;
        }

        public static int GetCaretIndex(RichTextBox r)
        {
            return new TextRange(r.Document.ContentStart, r.CaretPosition).Text.Replace("\r", "").Length;
        }

        public static int GetLineOfCursor(RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart, richTextBox.CaretPosition).Text.Split('\n').Length;
        }

        public static string GetLineAtCursor(RichTextBox richTextBox)
        {
            string[] lines = new TextRange(richTextBox.Document.ContentStart, richTextBox.CaretPosition).Text.Split('\n');
            string last = lines[^1];
            return last;
        }

        public static void SetCaret(RichTextBox richTextBox, int position)
        {
            TextPointer textPointer = richTextBox.Document.ContentStart.GetPositionAtOffset(position);
            if (textPointer == null)
            {
                richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
            }
            else
            {
                richTextBox.CaretPosition = richTextBox.Document.ContentStart.GetPositionAtOffset(position);
            }
        }
    }
}
