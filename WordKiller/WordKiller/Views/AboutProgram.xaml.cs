using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller;

public partial class AboutProgram : Window
{
    public AboutProgram()
    {
        InitializeComponent();
        ViewModel = new();
        DataContext = ViewModel;
    }

    public ViewModelAboutProgram ViewModel { get; set; }
}