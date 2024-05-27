using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphCode : ViewModelDocumentChanges, IParagraphData
{
    string data;

    string description;

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

    public string Type => "Code";

    public string Data
    {
        get => data;
        set => SetPropertyDocument(ref data, value);
    }

    public string Description
    {
        get => description;
        set => SetPropertyDocument(ref description, value);
    }

    public Visibility DescriptionVisibility => Visibility.Visible;
}