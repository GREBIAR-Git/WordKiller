using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.ViewModels;
using WordKiller.XAMLHelper;

namespace WordKiller.Scripts
{
    class QuickSearch
    {
        static readonly List<FoundItem> foundItems = new();

        static int current = 0;

        public static void TextChanged(TextBox textBox, RTBox richTextBox, ViewModelDocument document)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                foundItems.Clear();
                Init(document.Data, textBox.Text);
                UpdateCurrentIndex(document);
                FindCurrent(textBox, richTextBox, document);
                current++;
            }
            else
            {
                richTextBox.Focus();
                richTextBox.SetCaret(-1);
                textBox.Focus();
            }
        }

        static void Init(SectionParagraphs section, string text)
        {
            foreach (IParagraphData item in section.Paragraphs)
            {
                if (item is not ParagraphTitle && item is not ParagraphTaskSheet && item is not ParagraphListOfReferences && item is not ParagraphAppendix)
                {
                    FindTextInItem(item, text);
                    if (item is SectionParagraphs sectionParagraphs)
                    {
                        Init(sectionParagraphs, text);
                    }
                }
            }
        }

        public static void Next(TextBox textBox, RTBox richTextBox, ViewModelDocument document)
        {
            NextInit(document, textBox.Text);
            FindCurrent(textBox, richTextBox, document);
            current++;
        }

        static void NextInit(ViewModelDocument document, string text)
        {
            if (foundItems.Count > current)
            {
                FoundItem foundItem = foundItems[current];
                foundItems.Clear();
                Init(document.Data, text);
                for (int i = 0; i < foundItems.Count; i++)
                {
                    if (foundItems[i].Paragraph.Data == foundItem.Paragraph.Data && foundItems[i].Index == foundItem.Index)
                    {
                        current = i;
                        return;
                    }
                }
                UpdateCurrentIndex(document);
            }
        }

        static void FindCurrent(TextBox textBox, RTBox richTextBox, ViewModelDocument document)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                if (foundItems.Count > 0)
                {
                    if (document.Selected == null && document.Data.Paragraphs.Count > 0)
                    {
                        document.Selected = document.Data.Paragraphs[0];
                    }
                    SelectFoundItem(textBox, richTextBox, document);
                    return;
                }
            }
            richTextBox.Focus();
            richTextBox.SetCaret(-1);
            textBox.Focus();
        }

        static void FindTextInItem(IParagraphData item, string text)
        {
            int index = item.Data.IndexOf(text, 0);
            while (index != -1)
            {
                foundItems.Add(new FoundItem(item, index));
                index = item.Data.IndexOf(text, index + 1);
            }
        }

        static void SelectFoundItem(TextBox textBox, RTBox richTextBox, ViewModelDocument document)
        {
            if (current < foundItems.Count)
            {
                document.Selected = foundItems[current].Paragraph;
                richTextBox.Focus();
                UIHelper.SelectedWord(textBox.Text, foundItems[current].Index, richTextBox);
                textBox.Focus();
            }
            else
            {
                richTextBox.Focus();
                richTextBox.SetCaret(-1);
                textBox.Focus();
                current = -1;
            }
        }

        static void UpdateCurrentIndex(ViewModelDocument document)
        {
            if (foundItems.Count > 0)
            {
                if (document.Selected == null)
                {
                    current = 0;
                }
                else
                {
                    current = 0;
                    UpdateCurrentIndex(document.Data, document);
                }
            }
        }

        static void UpdateCurrentIndex(SectionParagraphs sectionParagraphs, ViewModelDocument document)
        {
            if (foundItems.Count > 0)
            {
                foreach (IParagraphData paragraphData in sectionParagraphs.Paragraphs)
                {
                    if (paragraphData == document.Selected)
                    {
                        return;
                    }
                    while (foundItems.Count > current && paragraphData == foundItems[current].Paragraph)
                    {
                        current++;
                    }
                    if (paragraphData is SectionParagraphs section)
                    {
                        UpdateCurrentIndex(section, document);
                    }
                    if (current >= foundItems.Count)
                    {
                        return;
                    }
                }
            }
        }

        public static void SelectedItemChanged(RoutedPropertyChangedEventArgs<object> e, ViewModelDocument document, RTBox richTextBox, TreeView treeView)
        {
            document.Selected = (IParagraphData)e.NewValue;
            if (document.Selected != null)
            {
                richTextBox.Focus();
                richTextBox.SetCaret(-1);
                treeView.Focus();
            }
            UpdateCurrentIndex(document);
        }
    }
}
