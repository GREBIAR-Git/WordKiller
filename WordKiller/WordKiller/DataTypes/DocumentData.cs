using System;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes;

[Serializable]
public class DocumentData : MainSection
{

    public TypeDocument Type { get; set; }

    public ViewModelTitle Title { get; set; }

    public ViewModelProperties Properties { get; set; }

    public void Clear()
    {
        Paragraphs.Clear();
        Title = new();
        Properties = new();
    }

    public DocumentData() : base()
    {
        Properties = new ViewModelProperties();
        Title = new();
    }
}
