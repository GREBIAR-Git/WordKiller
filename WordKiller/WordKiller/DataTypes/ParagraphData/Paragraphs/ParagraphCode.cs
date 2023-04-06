using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphCode : ViewModelBase, IParagraphData
{
    public string Type { get => "Code"; }

    string data;

    public string Data { get => data; set => SetProperty(ref data, value); }

    string description;

    public string Description { get => description; set => SetProperty(ref description, value); }

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
