using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.Scripts;
using WordKiller.Scripts.ForUI;
using WordKiller.ViewModels;

namespace WordKiller;

public partial class MainWindow : Window
{
    readonly ViewModelMain viewModel;

    IParagraphData? target;

    public MainWindow(ViewModelDocument document)
    {
        InitializeComponent();
        viewModel = (ViewModelMain)DataContext;
        viewModel.Document = document;
    }

    void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
        if (!e.Handled)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectedText = e.Text;
            string text = textBox.Text;
            int beginningNumber = 0;
            foreach (char number in text)
            {
                if (number == '0')
                {
                    beginningNumber++;
                }
                else
                {
                    break;
                }
            }
            if (beginningNumber > 0)
            {
                text = text[beginningNumber..];
                e.Handled = true;
            }
            if (!string.IsNullOrEmpty(text))
            {
                int count = int.Parse(text);
                if (count > Properties.Settings.Default.MaxRowAndColumn)
                {
                    count = Properties.Settings.Default.MaxRowAndColumn;
                    text = count.ToString();
                    e.Handled = true;
                }
            }
            if (e.Handled)
            {

                textBox.Text = text;
                textBox.SelectionStart = textBox.Text.Length;
            }
        }
    }

    private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Copy ||
            e.Command == ApplicationCommands.Cut ||
            e.Command == ApplicationCommands.Paste)
        {
            e.Handled = true;
        }
    }

    //Drag Drop
    void TreeView_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            IParagraphData? drag = viewModel.Document.Selected;
            if (drag is not null && drag is not ParagraphTitle && drag is not ParagraphTaskSheet && drag is not ParagraphListOfReferences && drag is not ParagraphAppendix)
            {
                DragDropEffects finalDropEffect = DragDrop.DoDragDrop(paragraphTree, new DragDropInfo(drag), DragDropEffects.Move);
                if ((finalDropEffect == DragDropEffects.Move) && (target != null))
                {
                    viewModel.Document.DragDrop(drag, target);
                    target = null;
                }
            }
        }
    }

    void TreeView_DragOver(object sender, DragEventArgs e)
    {
        TreeViewItem TargetItem = UIHelper.GetNearestContainer(e.OriginalSource as UIElement);
        DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
        if (dragDropInfo == null || (TargetItem.Header == dragDropInfo.ParagraphData ||
            dragDropInfo.ParagraphData is ParagraphTitle || dragDropInfo.ParagraphData is ParagraphTaskSheet || dragDropInfo.ParagraphData is ParagraphListOfReferences || dragDropInfo.ParagraphData is ParagraphAppendix ||
            TargetItem.Header is ParagraphTitle || TargetItem.Header is ParagraphTaskSheet || TargetItem.Header is ParagraphListOfReferences || TargetItem.Header is ParagraphAppendix))
        {
            e.Effects = DragDropEffects.None;
        }
        else
        {
            e.Effects = DragDropEffects.Move;
        }
        e.Handled = true;
    }

    void TreeView_Drop(object sender, DragEventArgs e)
    {
        try
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            TreeViewItem TargetItem = UIHelper.GetNearestContainer(e.OriginalSource as UIElement);

            DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
            if (TargetItem != null && dragDropInfo != null)
            {
                target = (IParagraphData)TargetItem.Header;
                e.Effects = DragDropEffects.Move;
            }
        }
        catch (Exception)
        {
        }
    }

    // Главный RichTextBox
    void WindowBinding_Unselect(object sender, ExecutedRoutedEventArgs e)
    {
        if (viewModel.Document.Selected != null)
        {
            if (paragraphTree.ItemContainerGenerator.ContainerFromItem(paragraphTree.SelectedItem) is TreeViewItem item)
            {
                item.IsSelected = false;
            }
        }
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
}