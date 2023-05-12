using System;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes;

[Serializable]
public class DocumentData : MainSection
{
    public DocumentType Type { get; set; }

    ViewModelProperties properties;
    public ViewModelProperties Properties { get => properties; set => SetPropertyDocument(ref properties, value); }

    ViewModelTitle title;
    public ViewModelTitle Title { get => title; set => SetPropertyDocument(ref title, value); }

    ViewModelTaskSheet taskSheet;
    public ViewModelTaskSheet TaskSheet { get => taskSheet; set => SetPropertyDocument(ref taskSheet, value); }

    ViewModelListOfReferences listOfReferences;
    public ViewModelListOfReferences ListOfReferences { get => listOfReferences; set => SetPropertyDocument(ref listOfReferences, value); }

    ViewModelAppendix appendix;
    public ViewModelAppendix Appendix { get => appendix; set => SetPropertyDocument(ref appendix, value); }

    public DocumentData() : base()
    {
        properties = new();
        title = new();
        taskSheet = new();
        listOfReferences = new();
        appendix = new();
    }
}
