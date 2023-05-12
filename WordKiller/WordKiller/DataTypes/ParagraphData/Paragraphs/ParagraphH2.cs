using System;
using System.Windows;
using WordKiller.DataTypes.ParagraphData.Sections;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphH2 : SectionH2, IParagraphData
{
    public string Type { get => "SubHeader"; }

    string data;

    public string Data { get => data; set => SetPropertyDocument(ref data, value, "Description"); }

    public string Description { get => data.Replace("\r\n", " "); set => SetPropertyDocument(ref data, value, "Data"); }

    public Visibility DescriptionVisibility
    {
        get => Visibility.Collapsed;
    }

    public ParagraphH2(string data)
    {
        this.data = data;
    }

    public ParagraphH2()
    {
        data = string.Empty;
    }
}
