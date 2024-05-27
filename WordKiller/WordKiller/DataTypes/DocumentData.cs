using System;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes;

[Serializable]
public class DocumentData : MainSection
{
    ViewModelAppendix appendix;

    ViewModelListOfReferences listOfReferences;

    ViewModelProperties properties;

    ViewModelTaskSheet taskSheet;

    ViewModelTitle title;

    public DocumentData()
    {
        properties = new();
        title = new();
        taskSheet = new();
        listOfReferences = new();
        appendix = new();
    }

    public DocumentType Type { get; set; }

    public ViewModelProperties Properties
    {
        get => properties;
        set => SetPropertyDocument(ref properties, value);
    }

    public ViewModelTitle Title
    {
        get => title;
        set => SetPropertyDocument(ref title, value);
    }

    public ViewModelTaskSheet TaskSheet
    {
        get => taskSheet;
        set => SetPropertyDocument(ref taskSheet, value);
    }

    public ViewModelListOfReferences ListOfReferences
    {
        get => listOfReferences;
        set => SetPropertyDocument(ref listOfReferences, value);
    }

    public ViewModelAppendix Appendix
    {
        get => appendix;
        set => SetPropertyDocument(ref appendix, value);
    }
}