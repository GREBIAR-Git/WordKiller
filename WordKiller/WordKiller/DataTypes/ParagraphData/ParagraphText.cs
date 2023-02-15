using System;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
internal class ParagraphText : IParagraphData
{
    public string Type { get => "Text"; }

    string data;

    public string Data { get => data; set => data = value; }

    public string Description { get => data; set => data = value; }

    public ParagraphText(string data)
    {
        this.data = data;
    }
}
