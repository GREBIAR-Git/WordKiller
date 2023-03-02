using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphPicture : ViewModelBase, IParagraphData
{
    public string Type { get => "Picture"; }

    string data;

    public string Data { get => data; set => SetProperty(ref data, value); }

    string description;

    public string Description { get => description; set => SetProperty(ref description, value); }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Visible;
    }

    public ParagraphPicture(string description, string data)
    {
        this.description = description;
        this.data = data;
    }

    public ParagraphPicture()
    {
        description = string.Empty;
        data = string.Empty;
    }
}
