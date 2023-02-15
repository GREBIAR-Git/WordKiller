using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class ParagraphH1 : IParagraphData
{
    public string Type { get => "Header"; }

    string data;

    public string Data { get => data; set => data = value; }

    public string Description { get => data; set => data = value; }

    public ParagraphH1(string data)
    {
        this.data = data;
    }
    public ParagraphH1()
    {
        data = string.Empty;
    }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Collapsed;
    }
}
