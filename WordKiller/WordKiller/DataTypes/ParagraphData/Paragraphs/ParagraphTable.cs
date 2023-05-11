using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphTable : ViewModelBase, IParagraphData
{
    public string Type { get => "Table"; }

    public string Data { get => description; set => SetProperty(ref description, value); }

    string description;

    public TableData TableData { get; set; }

    public string Description { get => description; set => SetProperty(ref description, value); }

    public Visibility DescriptionVisibility
    {
        get => Visibility.Visible;
    }

    public ParagraphTable(string description)
    {
        TableData = new();
        this.description = description;
    }

    public ParagraphTable()
    {
        TableData = new();
        description = string.Empty;
    }
}
