using System.ComponentModel;
using System.Windows;
using WordKiller.Scripts.File;
using WordKiller.ViewModels;

namespace WordKiller;

public partial class MainWindow : Window
{
    readonly ViewModelMain viewModel;

    public MainWindow(ViewModelDocument document)
    {
        InitializeComponent();
        viewModel = (ViewModelMain)DataContext;
        viewModel.Document = document;
    }

    void Win_Closing(object sender, CancelEventArgs e)
    {
        if (SaveHelper.NeedSave)
        {
            if (!viewModel.Document.File.NeedSave(viewModel.Document.Data))
            {
                e.Cancel = true;
            }
        }
    }


}