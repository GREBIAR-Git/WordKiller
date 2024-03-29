﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.DataTypes.ParagraphData.Sections;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelAppendix : MainSection
{
    IParagraphData? selected;
    public IParagraphData? Selected { get => selected; set => SetPropertyDocument(ref selected, value); }

    int addIndex;
    public int AddIndex { get => addIndex; set => SetPropertyDocument(ref addIndex, value); }

    [NonSerialized]
    ICommand? add;
    public ICommand Add
    {
        get
        {
            return add ??= new RelayCommand(obj =>
            {
                if (AddIndex == 0)
                {
                    Paragraphs.Add(new ParagraphPicture());
                }
                else if (AddIndex == 1)
                {
                    Paragraphs.Add(new ParagraphTable());
                }
                else if (AddIndex == 2)
                {
                    Paragraphs.Add(new ParagraphCode());
                }
            });
        }
    }

    [NonSerialized]
    ICommand? resetAddIndex;

    public ICommand ResetAddIndex
    {
        get
        {
            return resetAddIndex ??= new RelayCommand(obj =>
            {
                AddIndex = -1;
            });
        }
    }

    [NonSerialized]
    ICommand? delete;

    public ICommand Delete
    {
        get
        {
            return delete ??= new RelayCommand(obj =>
            {
                Paragraphs.Remove(Selected);
            });
        }
    }

    public ViewModelAppendix()
    {
        addIndex = -1;
    }
}
