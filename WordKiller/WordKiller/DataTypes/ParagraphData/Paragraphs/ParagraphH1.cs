using System;
using System.Windows;
using WordKiller.DataTypes.ParagraphData.Sections;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphH1 : SectionH1, IParagraphData
{
    public string Type { get => "Header"; }

    string data;

    public string Data { get => data; set => data = value; }

    public string Description { get => data; set => data = value; }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Collapsed;
    }

    public ParagraphH1(string data)
    {
        this.data = data;
    }
    public ParagraphH1()
    {
        data = string.Empty;
    }
}
