using System.Windows;
using System.Windows.Controls;
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
}