using System;
using System.Windows;
using WordKiller.Scripts;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphList : ViewModelDocumentChanges, IParagraphData
{
    public string Type { get => "List"; }

    string data;

    public string Data { get => data; set => SetPropertyDocument(ref data, value); }

    string description;

    public string Description { get => description; set => SetPropertyDocument(ref description, value); }

    public Visibility DescriptionVisibility
    {
        get => Visibility.Visible;
    }
    public ParagraphList(string description, string data)
    {
        this.description = description;
        this.data = data;
    }

    public ParagraphList()
    {
        if(Properties.Settings.Default.AutoHeader)
        {
            description = UIHelper.FindResourse(Type);
        }
        else
        {
            description = string.Empty;
        }
        data = string.Empty;
    }
}
