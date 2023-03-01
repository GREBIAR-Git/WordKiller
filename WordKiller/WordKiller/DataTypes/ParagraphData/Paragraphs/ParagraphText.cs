using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
internal class ParagraphText : IParagraphData
{
    public string Type { get => "Text"; }

    string data;

    public string Data { get => data; set => data = value; }

    public string Description { get => data; set => data = value; }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Collapsed;
    }

    public ParagraphText()
    {
        data = string.Empty;
    }

    public ParagraphText(string data)
    {
        this.data = data;
    }
}
