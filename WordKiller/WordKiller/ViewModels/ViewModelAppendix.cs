using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelAppendix : ViewModelBase
{
    public ObservableCollection<IParagraphData> Appendix { get; set; }

    IParagraphData? selected;
    public IParagraphData? Selected { get => selected; set => SetProperty(ref selected, value); }

    int addIndex;
    public int AddIndex { get => addIndex; set => SetProperty(ref addIndex, value); }

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
                    Appendix.Add(new ParagraphPicture());
                }
                else if (AddIndex == 1)
                {
                    Appendix.Add(new ParagraphTable());
                }
                else if (AddIndex == 2)
                {
                    Appendix.Add(new ParagraphCode());
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
                Appendix.Remove(Selected);
            });
        }
    }

    public ViewModelAppendix()
    {
        AddIndex = -1;
        Appendix = new();
    }
}
