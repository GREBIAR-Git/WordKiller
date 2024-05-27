using System;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.DataTypes.ParagraphData.Sections;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelAppendix : MainSection
{
    [NonSerialized] ICommand? add;

    int addIndex;

    [NonSerialized] ICommand? delete;

    [NonSerialized] ICommand? resetAddIndex;

    IParagraphData? selected;

    public ViewModelAppendix()
    {
        addIndex = -1;
    }

    public IParagraphData? Selected
    {
        get => selected;
        set => SetPropertyDocument(ref selected, value);
    }

    public int AddIndex
    {
        get => addIndex;
        set => SetPropertyDocument(ref addIndex, value);
    }

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

    public ICommand ResetAddIndex
    {
        get { return resetAddIndex ??= new RelayCommand(obj => { AddIndex = -1; }); }
    }

    public ICommand Delete
    {
        get { return delete ??= new RelayCommand(obj => { Paragraphs.Remove(Selected); }); }
    }
}