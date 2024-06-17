using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WordKiller.Scripts;
using WordKiller.ViewModels;

namespace WordKiller.Views.MainUserControl;

/// <summary>
/// Interaction logic for MainUserControl.xaml
/// </summary>
public partial class MainUserControl : UserControl
{
    public MainUserControl()
    {
        InitializeComponent();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        QuickSearch.TextChanged(S1, richTextBox, ((ViewModelMain)DataContext).Document);
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
         UIHelper.TableValidation(e, (TextBox)sender, true);
    }

    private void Button_Next(object sender, RoutedEventArgs e)
    {
        QuickSearch.Next(S1, richTextBox, ((ViewModelMain)DataContext).Document);
    }

    void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut ||
            e.Command == ApplicationCommands.Paste)
        {
            e.Handled = true;
        }
    }

    void TreeView_MouseMove(object sender, MouseEventArgs e)
    {
        TreeViewDragDrop.MouseMove(e, ((ViewModelMain)DataContext).Document, paragraphTree);
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
    
    void RichTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (richTextBox.Selection.Text.Length > 0)
        {
            e.Handled = true;
        }
    }


    void ParagraphTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        QuickSearch.SelectedItemChanged(e, ((ViewModelMain)DataContext).Document, richTextBox, paragraphTree);
    }
}
