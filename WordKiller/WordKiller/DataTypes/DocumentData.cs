﻿using System;
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

    ViewModelListOfReferences listOfReferences;
    public ViewModelListOfReferences ListOfReferences { get => listOfReferences; set => SetProperty(ref listOfReferences, value); }

    ViewModelAppendix appendix;
    public ViewModelAppendix Appendix { get => appendix; set => SetProperty(ref appendix, value); }

    public DocumentData() : base()
    {
        properties = new();
        title = new();
        taskSheet = new();
        listOfReferences = new();
        appendix = new();
    }
}
