using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphTable : ViewModelDocumentChanges, IParagraphData
{
    public string Type { get => "Table"; }

    public string Data { get => description; set => SetPropertyDocument(ref description, value); }

    string description;

    TableData tableData;

    public TableData TableData { get => tableData; set => SetPropertyDocument(ref tableData, value); }

    public string Description { get => description; set => SetPropertyDocument(ref description, value); }

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
