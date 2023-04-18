using System;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes;

[Serializable]
public class DocumentData : MainSection
{
    public TypeDocument Type { get; set; }

    ViewModelProperties properties;
    public ViewModelProperties Properties { get => properties; set => SetProperty(ref properties, value); }

    ViewModelTitle title;
    public ViewModelTitle Title { get => title; set => SetProperty(ref title, value); }

    ViewModelTaskSheet taskSheet;
    public ViewModelTaskSheet TaskSheet { get => taskSheet; set => SetProperty(ref taskSheet, value); }

    public void Clear()
    {
        Paragraphs.Clear();
        Title = new();
        Properties = new();
    }

    public DocumentData() : base()
    {
        properties = new ViewModelProperties();
        title = new();
        taskSheet = new();
    }
}
