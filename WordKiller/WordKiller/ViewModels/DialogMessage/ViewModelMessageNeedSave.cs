using System;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels.DialogMessage;

public class ViewModelMessageNeedSave : ViewModelBase
{
    string additionalColor;

    string alternativeColor;

    ICommand? exit;

    string hoverColor;

    string mainColor;

    ICommand? not;

    ICommand? yes;

    public ViewModelMessageNeedSave()
    {
        mainColor = Properties.Settings.Default.MainColor;
        additionalColor = Properties.Settings.Default.AdditionalColor;
        alternativeColor = Properties.Settings.Default.AlternativeColor;
        hoverColor = Properties.Settings.Default.HoverColor;
        Number = -1;
    }

    public int Number { get; set; }

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

    public ICommand Yes
    {
        get
        {
            return yes ??= new RelayCommand(
                obj =>
                {
                    Number = 0;
                    CloseAction();
                });
        }
    }

    public ICommand Not
    {
        get
        {
            return not ??= new RelayCommand(
                obj =>
                {
                    Number = 1;
                    CloseAction();
                });
        }
    }
}