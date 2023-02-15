using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class ParagraphList : IParagraphData
{
    public string Type { get => "List"; }

    string data;

    public string Data { get => data; set => data = value; }

    string description;

    public string Description { get => description; set => description = value; }

    public ParagraphList(string description, string data)
    {
        this.description = description;
        this.data = data;
    }

    public ParagraphList()
    {
        description = string.Empty;
        data = string.Empty;
    }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Visible;
    }
}
