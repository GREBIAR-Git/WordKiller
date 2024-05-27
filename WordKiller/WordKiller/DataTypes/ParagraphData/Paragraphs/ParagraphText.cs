using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
internal class ParagraphText : ViewModelDocumentChanges, IParagraphData
{
    string data;

    public ParagraphText()
    {
        data = string.Empty;
    }

    public ParagraphText(string data)
    {
        this.data = data;
    }

    public string Type => "Text";

    public string Data
    {
        get => data;
        set => SetPropertyDocument(ref data, value, "Description");
    }

    public string Description
    {
        get => data.Replace("\r\n", " ");
        set => SetPropertyDocument(ref data, value, "Data");
    }

    public Visibility DescriptionVisibility => Visibility.Collapsed;
}