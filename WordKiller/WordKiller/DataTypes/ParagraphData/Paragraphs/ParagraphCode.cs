using System;
using System.Collections.Generic;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphCode : IParagraphData
{
    public string Type { get => "Code"; }

    string data;

    public string Data { get => data; set => data = value; }

    string description;

    public string Description { get => description; set => description = value; }


    public List<IParagraphData>? Paragraphs { get => null; }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Visible;
    }

    public ParagraphCode(string description, string data)
    {
        this.description = description;
        this.data = data;
    }

    public ParagraphCode()
    {
        description = string.Empty;
        data = string.Empty;
    }
}
