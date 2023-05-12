using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphCode : ViewModelDocumentChanges, IParagraphData
{
    public string Type { get => "Code"; }

    string data;

    public string Data
    {
        get => data;
        set => SetPropertyDocument(ref data, value);
    }

    string description;

    public string Description { get => description; set => SetPropertyDocument(ref description, value); }

    public Visibility DescriptionVisibility
    {
        get => Visibility.Visible;
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
