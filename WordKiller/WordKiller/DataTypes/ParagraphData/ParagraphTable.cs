using System;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class ParagraphTable : IParagraphData
{
    public string Type { get => "Table"; }

    public string Data { get => description; set => description = value; }

    string description;

    public TableData TableData { get; set; }

    public string Description { get => description; set => description = value; }

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
