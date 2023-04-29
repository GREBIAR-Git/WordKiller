using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
internal class ParagraphText : ViewModelBase, IParagraphData
{
    public string Type { get => "Text"; }

    string data;

    public string Data { get => data; set => SetProperty(ref data, value, "Description"); }

    public string Description { get => data.Replace("\r\n", " "); set => SetProperty(ref data, value, "Data"); }

    public Visibility DescriptionVisibility
    {
        get => Visibility.Collapsed;
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
