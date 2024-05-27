using System;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels.DialogMessage;

public class ViewModelMessageDragDrop : ViewModelBase
{
    string additionalColor;

    ICommand? after;

    string alternativeColor;

    ICommand? before;

    ICommand? exit;

    string hoverColor;

    ICommand? insert;

    string mainColor;

    ICommand? swap;

    Visibility visibilityAfter;

    Visibility visibilityBefore;

    Visibility visibilityInsert;

    Visibility visibilitySwap;

    public ViewModelMessageDragDrop(Visibility insert, Visibility before, Visibility after, Visibility swap)
    {
        mainColor = Properties.Settings.Default.MainColor;
        additionalColor = Properties.Settings.Default.AdditionalColor;
        alternativeColor = Properties.Settings.Default.AlternativeColor;
        hoverColor = Properties.Settings.Default.HoverColor;
        Number = -1;
        VisibilityInsert = insert;
        VisibilityBefore = before;
        VisibilityAfter = after;
        VisibilitySwap = swap;
    }

    public int Number { get; set; }

    public Visibility VisibilityInsert
    {
        get => visibilityInsert;
        set => SetProperty(ref visibilityInsert, value);
    }

    public Visibility VisibilityBefore
    {
        get => visibilityBefore;
        set => SetProperty(ref visibilityBefore, value);
    }

    public Visibility VisibilityAfter
    {
        get => visibilityAfter;
        set => SetProperty(ref visibilityAfter, value);
    }

    public Visibility VisibilitySwap
    {
        get => visibilitySwap;
        set => SetProperty(ref visibilitySwap, value);
    }

    public string MainColor
    {
        get => mainColor;
        set => SetProperty(ref mainColor, value);
    }

    public string AdditionalColor
    {
        get => additionalColor;
        set => SetProperty(ref additionalColor, value);
    }

    public string AlternativeColor
    {
        get => alternativeColor;
        set => SetProperty(ref alternativeColor, value);
    }

    public string HoverColor
    {
        get => hoverColor;
        set => SetProperty(ref hoverColor, value);
    }

    public Action CloseAction { get; set; }

    public ICommand Exit
    {
        get
        {
            return exit ??= new RelayCommand(
                obj =>
                {
                    Number = -1;
                    CloseAction();
                });
        }
    }

    public ICommand Insert
    {
        get
        {
            return insert ??= new RelayCommand(
                obj =>
                {
                    Number = 0;
                    CloseAction();
                });
        }
    }

    public ICommand Before
    {
        get
        {
            return before ??= new RelayCommand(
                obj =>
                {
                    Number = 1;
                    CloseAction();
                });
        }
    }

    public ICommand After
    {
        get
        {
            return after ??= new RelayCommand(
                obj =>
                {
                    Number = 2;
                    CloseAction();
                });
        }
    }

    public ICommand Swap
    {
        get
        {
            return swap ??= new RelayCommand(
                obj =>
                {
                    Number = 3;
                    CloseAction();
                });
        }
    }
}