using System;
using System.Windows;
using WordKiller.ViewModels.DialogMessage;

namespace WordKiller.Views;

public partial class MessageNeedSave : Window
{
    public ViewModelMessageNeedSave ViewModel { get; set; }
    public MessageNeedSave()
    {
        InitializeComponent();
        ViewModel = new ViewModelMessageNeedSave();
        DataContext = ViewModel;
        if (ViewModel.CloseAction == null)
            ViewModel.CloseAction = new Action(this.Close);
    }
}
