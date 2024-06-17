using System;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels.DialogMessage;

public class ViewModelMessageNeedSave : ViewModelBase
{
    ICommand? exit;

    ICommand? not;

    ICommand? yes;

    public ViewModelMessageNeedSave()
    {
        Number = -1;
    }

    public int Number { get; set; }

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