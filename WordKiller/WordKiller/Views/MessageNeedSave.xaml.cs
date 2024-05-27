using System.Windows;
using WordKiller.ViewModels.DialogMessage;

namespace WordKiller.Views;

public partial class MessageNeedSave : Window
{
    public MessageNeedSave()
    {
        InitializeComponent();
        ViewModel = new();
        DataContext = ViewModel;
        if (ViewModel.CloseAction == null)
            ViewModel.CloseAction = Close;
    }

    public ViewModelMessageNeedSave ViewModel { get; set; }
}