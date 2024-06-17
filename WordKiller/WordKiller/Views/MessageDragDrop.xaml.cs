using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordKiller.ViewModels.DialogMessage;

namespace WordKiller.Views;

public partial class MessageDragDrop : Window
{
    public MessageDragDrop(Visibility insert = Visibility.Visible, Visibility before = Visibility.Visible,
        Visibility after = Visibility.Visible, Visibility swap = Visibility.Visible)
    {
        InitializeComponent();
        ViewModel = new(insert, before, after, swap);
        if (insert == Visibility.Collapsed && before == Visibility.Collapsed && after == Visibility.Collapsed &&
            swap == Visibility.Collapsed)
        {
            mainText.SetResourceReference(TextBlock.TextProperty, "DragDrop2");
        }

        DataContext = ViewModel;
        if (ViewModel.CloseAction == null)
            ViewModel.CloseAction = Close;
    }

    public ViewModelMessageDragDrop ViewModel { get; set; }

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