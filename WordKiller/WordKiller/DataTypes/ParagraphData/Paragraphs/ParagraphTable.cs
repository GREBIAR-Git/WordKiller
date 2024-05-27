using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphTable : Numbered, IParagraphData
{
    string description;

    TableData tableData;

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

    public TableData TableData
    {
        get => tableData;
        set => SetPropertyDocument(ref tableData, value);
    }

    public string Type => "Table";

    public string Data
    {
        get => description;
        set => SetPropertyDocument(ref description, value);
    }

    public string Description
    {
        get => description;
        set => SetPropertyDocument(ref description, value);
    }

    public Visibility DescriptionVisibility => Visibility.Visible;
}