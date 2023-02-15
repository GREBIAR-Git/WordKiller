using System;
using System.Collections.Generic;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes;

[Serializable]
public class DocumentData
{
    public TypeDocument Type { get; set; }

    public ViewModelTitle Title { get; set; }

    public List<IParagraphData> Paragraphs { get; set; }

    public ViewModelProperties Properties { get; set; }


    public DocumentData()
    {
        Properties = new ViewModelProperties();
        Title = new();
        Paragraphs = new List<IParagraphData>();
    }
}
