using System.Windows;
using WordKiller.ViewModels;

namespace WordKiller;

public partial class AboutProgram : Window
{
    public ViewModelAboutProgram ViewModel { get; set; }
    public AboutProgram()
    {
        InitializeComponent();
        ViewModel = new();
        DataContext = ViewModel;
    }
}
