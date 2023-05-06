using System;
using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller.Views
{
    /// <summary>
    /// Логика взаимодействия для MessageDragDrop.xaml
    /// </summary>
    public partial class MessageDragDrop : Window
    {
        public ViewModelMessageDragDrop ViewModel { get; set; }

        public MessageDragDrop(Visibility insert = Visibility.Visible, Visibility before = Visibility.Visible, Visibility after = Visibility.Visible, Visibility swap = Visibility.Visible)
        {
            InitializeComponent();
            ViewModel = new ViewModelMessageDragDrop(insert, before, after, swap);
            if (insert == Visibility.Collapsed && before == Visibility.Collapsed && after == Visibility.Collapsed && swap == Visibility.Collapsed)
            {
                mainText.Text = "Этого сделать невозможно";
            }
            DataContext = ViewModel;
            if (ViewModel.CloseAction == null)
                ViewModel.CloseAction = new Action(this.Close);
        }
    }
}
