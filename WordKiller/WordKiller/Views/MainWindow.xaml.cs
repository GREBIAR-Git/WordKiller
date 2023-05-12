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

    // Таблица
    void CountRows_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (viewModel.Document.Selected is ParagraphTable paragraphTable)
            {
                int rows = paragraphTable.TableData.Rows;
                CountRowOrColumn(textBox, ref rows);
                gridTable.RowDefinitions.Clear();//tyt
                for (int i = 0; i < rows; i++)
                {
                    //tyt
                    gridTable.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100 / rows, type: GridUnitType.Star) });
                }
                paragraphTable.TableData.Rows = rows;
                UpdateTable();
            }
        }
    }

    void CountColumns_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (viewModel.Document.Selected is ParagraphTable paragraphTable)
            {
                int columns = paragraphTable.TableData.Columns;
                CountRowOrColumn(textBox, ref columns);
                gridTable.ColumnDefinitions.Clear();//tyt
                for (int i = 0; i < columns; i++)
                {
                    //tyt
                    gridTable.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100 / columns, type: GridUnitType.Star) });
                }
                paragraphTable.TableData.Columns = columns;
                UpdateTable();
            }
        }
    }

    static void CountRowOrColumn(TextBox textBox, ref int count)
    {
        int beginningNumber = 0;
        foreach (char number in textBox.Text)
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
        textBox.Text = textBox.Text[beginningNumber..];
        if (!string.IsNullOrEmpty(textBox.Text))
        {
            count = int.Parse(textBox.Text);
            if (count > Properties.Settings.Default.MaxRowAndColumn)
            {
                count = Properties.Settings.Default.MaxRowAndColumn;
                textBox.Text = count.ToString();
            }
        }
        else
        {
            count = 0;
        }
        textBox.SelectionStart = textBox.Text.Length;
    }

    void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    void UpdateTable()
    {
        if (viewModel.Document.Selected is ParagraphTable paragraphTable)
        {
            gridTable.Children.Clear();//tyt
            for (int i = 0; i < paragraphTable.TableData.Rows; i++)
            {
                for (int f = 0; f < paragraphTable.TableData.Columns; f++)
                {
                    TextBox textBox = new()
                    {
                        Text = paragraphTable.TableData.DataTable[i, f],
                        FontSize = viewModel.Settings.Personalization.FontSizeRTB,
                    };
                    textBox.TextChanged += Cell_TextChanged;
                    gridTable.Children.Add(textBox);//tyt
                    Grid.SetColumn(textBox, f);
                    Grid.SetRow(textBox, i);
                }
            }
        }
    }

    void Cell_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (paragraphTree.SelectedItem is ParagraphTable paragraphTable)
        {
            TextBox textBox = (TextBox)sender;
            int row = Grid.GetRow(textBox);
            int column = Grid.GetColumn(textBox);
            paragraphTable.TableData.DataTable[row, column] = textBox.Text;
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