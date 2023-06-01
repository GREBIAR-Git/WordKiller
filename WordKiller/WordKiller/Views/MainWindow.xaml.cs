using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordKiller.Scripts;
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

    void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        UIHelper.TableValidation(e, (TextBox)sender, true);
    }
    void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
            e.Handled = true;
    }

    void NumberValidationTextBoxTemplate(object sender, TextCompositionEventArgs e)
    {
        UIHelper.TableValidation(e, (TextBox)sender);
    }

    void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
        {
            e.Handled = true;
        }
    }

    void TreeView_MouseMove(object sender, MouseEventArgs e)
    {
        TreeViewDragDrop.MouseMove(e, viewModel.Document, paragraphTree);
    }

    void TreeView_DragOver(object sender, DragEventArgs e)
    {
        TreeViewDragDrop.DragOver(e);
    }

    void TreeView_Drop(object sender, DragEventArgs e)
    {
        TreeViewDragDrop.Drop(e);
    }

    void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        richTextBox.KeyProcessing(e);
    }

    void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (SaveHelper.NeedSave)
        {
            if (!viewModel.Document.File.NeedSave(viewModel.Document.Data))
            {
                e.Cancel = true;
            }
        }
    }

    void RichTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (richTextBox.Selection.Text.Length > 0)
        {
            e.Handled = true;
        }
    }

    void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        QuickSearch.TextChanged(S1, richTextBox, viewModel.Document);
    }

    void Button_Next(object sender, RoutedEventArgs e)
    {
        QuickSearch.Next(S1, richTextBox, viewModel.Document);
    }

    void ParagraphTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        QuickSearch.SelectedItemChanged(e, viewModel.Document, richTextBox, paragraphTree);
    }

    void Button_Click(object sender, RoutedEventArgs e)
    {
        books.Items.Refresh();
        electronicResources.Items.Refresh();
    }
}