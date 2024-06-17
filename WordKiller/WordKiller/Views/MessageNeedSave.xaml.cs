using System.Windows;
using System.Windows.Input;
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

    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        
        if(e.ChangedButton == MouseButton.Left)
            DragMove();
    }


    void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
