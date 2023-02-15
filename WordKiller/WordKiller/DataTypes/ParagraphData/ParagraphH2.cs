using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class ParagraphH2 : IParagraphData
{
    public string Type { get => "SubHeader"; }

    string data;

    public string Data { get => data; set => data = value; }

    public string Description { get => data; set => data = value; }


    public ParagraphH2(string data)
    {
        this.data = data;
    }

    public ParagraphH2()
    {
        data = string.Empty;
    }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Collapsed;
    }
}
