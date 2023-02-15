using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace WordKiller.DataTypes.TypeXAML;

public class RTBox : RichTextBox
{
    public string GetText()
    {
        string text = new TextRange(Document.ContentStart, Document.ContentEnd).Text;
        if (text != string.Empty)
        {
            text = text.Replace("\r", "");
            text = text.Remove(text.Length - 1, 1);
        }
        return text;
    }

    public void SetText(string text)
    {
        Document.Blocks.Clear();
        Paragraph paragraph = new();
        string[] lines = text.Split(' ');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Any(wordByte => wordByte > 127))
            {
                paragraph.Inlines.Add(new Run(lines[i])
                {
                    Language = XmlLanguage.GetLanguage("ru-ru")
                });
            }
            else
            {
                paragraph.Inlines.Add(new Run(lines[i])
                {
                    Language = XmlLanguage.GetLanguage("en-us")
                });
            }
            if (lines.Length - 1 != i)
            {
                paragraph.Inlines.Add(new Run(" ")
                {
                    Language = XmlLanguage.GetLanguage("en-us")
                });
            }
        }
        Document.Blocks.Add(paragraph);
    }

    public int GetCaretIndex()
    {
        return new TextRange(Document.ContentStart, CaretPosition).Text.Replace("\r", "").Length;
    }

    public int GetLineOfCursor()
    {
        return new TextRange(Document.ContentStart, CaretPosition).Text.Split('\n').Length;
    }

    public string GetLineAtCursor()
    {
        string[] lines = new TextRange(Document.ContentStart, CaretPosition).Text.Split('\n');
        string last = lines[^1];
        return last;
    }

    public void SetCaret(int position)
    {
        TextPointer textPointer = Document.ContentStart.GetPositionAtOffset(position);
        if (textPointer == null)
        {
            CaretPosition = CaretPosition.DocumentEnd;
        }
        else
        {
            CaretPosition = Document.ContentStart.GetPositionAtOffset(position);
        }
    }

    public void KeyProcessing(KeyEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            if (e.Key == Key.V)
            {
                Paste();
                SetText(GetText());
                e.Handled = true;
            }
            else if (e.Key == Key.X)
            {
                int idxDelLine = GetLineOfCursor();

                List<string> lines = GetText().Split('\n').ToList();

                Clipboard.SetText(lines.ElementAt(idxDelLine - 1));

                lines.RemoveAt(idxDelLine - 1);

                string text = string.Empty;

                foreach (string line in lines)
                {
                    text += line + "\r\n";
                }

                SetText(text);
            }
        }
    }
}
