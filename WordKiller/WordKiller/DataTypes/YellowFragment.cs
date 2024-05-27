using System;
using System.Xml.Serialization;
using WordKiller.Scripts;

namespace WordKiller.DataTypes;

[Serializable]
public class YellowFragment
{
    public int index;
    public string? Text { get; set; }

    [XmlIgnore]
    public int Index
    {
        get => index;
        set
        {
            index = value;
            TemplateHelper.NeedSave = true;
        }
    }
}