using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace WordKiller.DataTypes.TypeXAML;

public class RTBox : RichTextBox
{

    public RTBox()
    {
        TextFormatter = new PlainTextFormatter();
    }

    public string GetText()
    {
        string text = new TextRange(Document.ContentStart, Document.ContentEnd).Text;
        if (text != string.Empty)
        {
            if (text.Length > 1)
            {
                text = text.Remove(text.Length - 2, 2);
            }
        }
        return text;
    }

    public void SetText(string text, bool language = true)
    {
        Document.Blocks.Clear();
        if (language)
        {
            SpellCheck.IsEnabled = true;
            SetTextWithLanguage(text);
        }
        else
        {
            List<Paragraph> paragraphs = new();
            SpellCheck.IsEnabled = false;
            foreach (string line in text.Split("\r\n"))
            {
                Paragraph paragraph = new();
                paragraph.Inlines.Add(line);
                paragraphs.Add(paragraph);
            }
            Document.Blocks.AddRange(paragraphs);
        }
        UndoLimit = 0;
        UndoLimit = -1;
    }

    void SetTextWithLanguage(string text)
    {
        List<Paragraph> paragraphs = new();
        foreach (string line in text.Split("\r\n"))
        {
            Paragraph paragraph = new();
            string[] word = line.Split(' ');
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i].Any(wordByte => wordByte > 127))
                {
                    paragraph.Inlines.Add(new Run(word[i])
                    {
                        Language = XmlLanguage.GetLanguage("ru-ru")
                    });
                }
                else
                {
                    paragraph.Inlines.Add(new Run(word[i])
                    {
                        Language = XmlLanguage.GetLanguage("en-us")
                    });
                }
                if (word.Length - 1 != i)
                {
                    paragraph.Inlines.Add(new Run(" ")
                    {
                        Language = XmlLanguage.GetLanguage("en-us")
                    });
                }
            }
            paragraphs.Add(paragraph);
        }
        Document.Blocks.AddRange(paragraphs);
    }

    void PerformSpellCheck()
    {
        string text = GetText();
        Document.Blocks.Clear();
        SpellCheck.IsEnabled = true;
        SetTextWithLanguage(text);
    }

    void PerformSpellCheck(string text)
    {
        Document.Blocks.Clear();
        SpellCheck.IsEnabled = true;
        SetTextWithLanguage(text);
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
        TextPointer textStart = Document.ContentStart;
        while (textStart != null &&
               textStart.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.Text)
            textStart = textStart.GetNextContextPosition(LogicalDirection.Forward);
        if (textStart == null)
        {
            CaretPosition = CaretPosition.DocumentEnd;
        }
        else
        {
            CaretPosition = textStart.GetPositionAtOffset(position);
        }
    }

    int GetIntPosition()
    {
        TextPointer pointerPosition = CaretPosition;
        int intPosition = 0;

        TextPointer currentPosition = Document.ContentStart;
        if (currentPosition != null)
        {
            while (currentPosition != null && currentPosition.CompareTo(pointerPosition) != 0)
            {
                intPosition++;
                currentPosition = currentPosition.GetNextInsertionPosition(LogicalDirection.Forward);
            }
        }


        return intPosition;
    }

    void SetIntPosition(int intPosition)
    {
        TextPointer currentPosition = Document.ContentStart;

        for (int i = 1; i <= intPosition; i++)
        {
            if (currentPosition == null)
            {
                break;
            }
            currentPosition = currentPosition.GetNextInsertionPosition(LogicalDirection.Forward);
        }
        if (currentPosition == null)
        {
            CaretPosition = CaretPosition.DocumentEnd;
        }
        else
        {
            CaretPosition = currentPosition;
        }
    }

    public void KeyProcessing(KeyEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            if (e.Key == Key.V)
            {
                using (DeclareChangeBlock())
                {
                    int i = GetIntPosition();
                    i += Clipboard.GetText().Length;
                    Paste();
                    PerformSpellCheck();
                    SetIntPosition(i);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.X)
            {
                using (DeclareChangeBlock())
                {
                    int i = GetIntPosition();
                    int idxDelLine = GetLineOfCursor();

                    List<string> lines = GetText().Split("\r\n").ToList();

                    Clipboard.SetText(lines.ElementAt(idxDelLine - 1));

                    lines.RemoveAt(idxDelLine - 1);

                    string text = string.Empty;

                    foreach (string line in lines)
                    {
                        text += line + "\r\n";
                    }
                    PerformSpellCheck(text);
                    SetIntPosition(i);
                }
            }
        }
    }
}
