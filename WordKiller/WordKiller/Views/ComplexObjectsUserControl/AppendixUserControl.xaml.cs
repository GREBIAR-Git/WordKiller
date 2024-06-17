using System.Windows.Controls;
using System.Windows.Input;
using WordKiller.Scripts;

namespace WordKiller.Views.ComplexObjectsUserControl;

/// <summary>
/// Interaction logic for AppendixUserControl.xaml
/// </summary>
public partial class AppendixUserControl : UserControl
{
    public AppendixUserControl()
    {
        InitializeComponent();
    }

    void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
{
    UIHelper.TableValidation(e, (TextBox)sender, true);
}

    void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut ||
            e.Command == ApplicationCommands.Paste)
        {
            e.Handled = true;
        }
    }
}


