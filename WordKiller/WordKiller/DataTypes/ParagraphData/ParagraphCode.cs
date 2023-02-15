using System;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class ParagraphCode : IParagraphData
{
    public string Type { get => "Code"; }


    string data;


    public string Data { get => data; set => data = value; }

    string description;

    public string Description { get => description; set => description = value; }

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
