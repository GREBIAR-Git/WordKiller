using System;
using System.Windows;
using WordKiller.Properties;
using WordKiller.Scripts;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphList : ViewModelDocumentChanges, IParagraphData
{
    string data;

    string description;

    public ParagraphList(string description, string data)
    {
        this.description = description;
        this.data = data;
    }

    public ParagraphList()
    {
        if (Settings.Default.AutoHeader)
        {
            description = UIHelper.FindResourse(Type);
        }
        else
        {
            description = string.Empty;
        }

        data = string.Empty;
    }

    public string Type => "List";

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