using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.DataTypes.ParagraphData.Sections;
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
            if (paragraphTree.SelectedItem is ParagraphTable paragraphTable)
            {
                int rows = paragraphTable.TableData.Rows;
                CountRowOrColumn(textBox, ref rows);
                gridTable.RowDefinitions.Clear();
                for (int i = 0; i < rows; i++)
                {
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
            if (paragraphTree.SelectedItem is ParagraphTable paragraphTable)
            {
                int columns = paragraphTable.TableData.Columns;
                CountRowOrColumn(textBox, ref columns);
                gridTable.ColumnDefinitions.Clear();
                for (int i = 0; i < columns; i++)
                {
                    gridTable.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100 / columns, type: GridUnitType.Star) });
                }
                paragraphTable.TableData.Columns = columns;
                UpdateTable();
            }
        }
    }

    void CountRowOrColumn(TextBox textBox, ref int count)
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
        if (paragraphTree.SelectedItem is ParagraphTable paragraphTable)
        {
            gridTable.Children.Clear();
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
                    gridTable.Children.Add(textBox);
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

    bool EnableDragDrop(DragEventArgs e)
    {
        DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
        if (dragDropInfo == null && e.Data.GetData(DataFormats.FileDrop) != null && (paragraphTree.SelectedItem is ParagraphPicture || paragraphTree.SelectedItem is ParagraphCode || paragraphTree.SelectedItem is ParagraphAppendix))
        {
            return true;
        }
        return false;
    }

    void Win_DragEnter(object sender, DragEventArgs e)
    {
        e.Handled = true;
        e.Effects = DragDropEffects.None;
        if (EnableDragDrop(e))
        {
            viewModel.VisibilityDrag = Visibility.Visible;
        }
    }

    void Win_DragOver(object sender, DragEventArgs e)
    {
        e.Handled = true;
        e.Effects = DragDropEffects.None;
        if (EnableDragDrop(e))
        {
            viewModel.VisibilityDrag = Visibility.Visible;
        }
    }

    void Win_DragLeave(object sender, DragEventArgs e)
    {
        e.Handled = true;
        e.Effects = DragDropEffects.None;
        if (EnableDragDrop(e))
        {
            viewModel.VisibilityDrag = Visibility.Collapsed;
        }
    }

    void PictureBox_DragEnter(object sender, DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            viewModel.VisibilityDrag = Visibility.Visible;
        }
    }

    void PictureBox_DragOver(object sender, DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            viewModel.VisibilityDrag = Visibility.Visible;
        }
    }

    void PictureBox_DragLeave(object sender, DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            viewModel.VisibilityDrag = Visibility.Collapsed;
        }
    }

    void PictureBox_Drop(object sender, DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string path = (data as string[])[0];
                if (path.Length > 0)
                {
                    string nameFile = Path.GetFileNameWithoutExtension(path);
                    if (paragraphTree.SelectedItem is ParagraphPicture paragraphPicture)
                    {
                        System.Drawing.Bitmap bitmap;
                        try
                        {
                            bitmap = new(path);
                        }
                        catch
                        {
                            return;
                        }
                        paragraphPicture.Bitmap = bitmap;
                        viewModel.Document.MainImage = paragraphPicture.BitmapImage;
                        paragraphPicture.Description = nameFile;
                    }
                    else if (paragraphTree.SelectedItem is ParagraphCode paragraphCode)
                    {
                        FileStream file = new(path, FileMode.Open);
                        StreamReader reader = new(file);
                        string data1 = reader.ReadToEnd();
                        paragraphCode.Description = nameFile;
                        richTextBox.SetText(data1, false);
                    }
                }
            }
            viewModel.VisibilityDrag = Visibility.Collapsed;
        }
    }

    void PictureBox_Drop_Appendix(object sender, DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string path = (data as string[])[0];
                if (path.Length > 0)
                {
                    string nameFile = Path.GetFileNameWithoutExtension(path);
                    if (viewModel.Document.Data.Appendix.Selected is ParagraphPicture paragraphPicture)
                    {
                        System.Drawing.Bitmap bitmap;
                        try
                        {
                            bitmap = new(path);
                        }
                        catch
                        {
                            return;
                        }
                        paragraphPicture.Bitmap = bitmap;
                        paragraphPicture.UpdateBitmapImage();
                        paragraphPicture.Description = nameFile;
                    }
                    else if (viewModel.Document.Data.Appendix.Selected is ParagraphCode paragraphCode)
                    {
                        FileStream file = new(path, FileMode.Open);
                        StreamReader reader = new(file);
                        string data1 = reader.ReadToEnd();
                        paragraphCode.Description = nameFile;
                        paragraphCode.Data = data1;
                    }
                }
            }
            viewModel.VisibilityDrag = Visibility.Collapsed;
        }
    }

    void TreeView_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (paragraphTree.SelectedItem is IParagraphData drag)
            {
                if (drag is not ParagraphTitle && drag is not ParagraphTaskSheet && drag is not ParagraphListOfReferences && drag is not ParagraphAppendix)
                {
                    DragDropEffects finalDropEffect = DragDrop.DoDragDrop(paragraphTree, new DragDropInfo((IParagraphData)paragraphTree.SelectedValue), DragDropEffects.Move);
                    if ((finalDropEffect == DragDropEffects.Move) && (target != null))
                    {
                        CopyItem((IParagraphData)paragraphTree.SelectedValue, target);
                        target = null;
                    }
                }
            }
        }
    }

    void TreeView_DragOver(object sender, DragEventArgs e)
    {
        TreeViewItem TargetItem = UIHelper.GetNearestContainer
                (e.OriginalSource as UIElement);

        DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
        if (dragDropInfo == null || (TargetItem.Header == dragDropInfo.ParagraphData ||
            dragDropInfo.ParagraphData is ParagraphTitle || dragDropInfo.ParagraphData is ParagraphTaskSheet || dragDropInfo.ParagraphData is ParagraphListOfReferences || dragDropInfo.ParagraphData is ParagraphAppendix ||
            TargetItem.Header is ParagraphTitle || TargetItem.Header is ParagraphTaskSheet || TargetItem.Header is ParagraphListOfReferences || TargetItem.Header is ParagraphAppendix ||
            (dragDropInfo.ParagraphData is ParagraphH1 && TargetItem.Header is not ParagraphH1) ||
            (dragDropInfo.ParagraphData is ParagraphH2 && (TargetItem.Header is not ParagraphH2 && TargetItem.Header is not ParagraphH1))))
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

            TreeViewItem TargetItem = UIHelper.GetNearestContainer
                (e.OriginalSource as UIElement);
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

    void CopyItem(IParagraphData _sourceItem, IParagraphData _targetItem)
    {
        if (_targetItem is ParagraphH1 && _sourceItem is ParagraphH1)
        {
            if (MessageBox.Show("Поменять местами «" + _sourceItem.Description.ToString() + "» с «" + _targetItem.Description.ToString() + "»", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int i = viewModel.Document.Data.Paragraphs.IndexOf(_targetItem);
                int f = viewModel.Document.Data.Paragraphs.IndexOf(_sourceItem);
                (viewModel.Document.Data.Paragraphs[i], viewModel.Document.Data.Paragraphs[f]) = (viewModel.Document.Data.Paragraphs[f], viewModel.Document.Data.Paragraphs[i]);
            }
            return;
        }
        else if (_targetItem is ParagraphH2)
        {
            if (_sourceItem is ParagraphH2)
            {
                if (MessageBox.Show("Поменять местами «" + _sourceItem.Description.ToString() + "» с «" + _targetItem.Description.ToString() + "»", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    viewModel.Document.Data.SwapParagraphs(_targetItem, _sourceItem);
                }
                return;
            }
        }
        if (_targetItem is not SectionParagraphs)
        {
            if (_sourceItem is not ParagraphH2 && _sourceItem is not ParagraphH1)
            {
                if (MessageBox.Show("Поменять местами «" + _sourceItem.Description.ToString() + "» с «" + _targetItem.Description.ToString() + "»", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    viewModel.Document.Data.SwapParagraphs(_targetItem, _sourceItem);
                }
                return;
            }
            else
            {
                MessageBox.Show("Так сделать невозможно", "Ошибка", MessageBoxButton.OK);
                return;
            }
        }
        if (MessageBox.Show("Вставить «" + _sourceItem.Description.ToString() + "» в «" + _targetItem.Description.ToString() + "»", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            viewModel.Document.Data.RemoveParagraph(_sourceItem);
            SectionParagraphs section = _targetItem as SectionParagraphs;
            section.AddParagraph(_sourceItem);
        }

    }

    void NewNotComplexObjects_Drop(object sender, DragEventArgs e)
    {
        var data = e.Data.GetData(DataFormats.FileDrop);
        if (data != null)
        {
            foreach (string path in data as string[])
            {
                if (path.Length > 0)
                {
                    string nameFile = Path.GetFileNameWithoutExtension(path);
                    System.Drawing.Bitmap bitmap;
                    try
                    {
                        bitmap = new(path);
                        this.viewModel.Document.Data.AddParagraph(new ParagraphPicture(nameFile, bitmap));
                    }
                    catch
                    {
                        FileStream file = new(path, FileMode.Open);
                        StreamReader reader = new(file);
                        string data1 = reader.ReadToEnd();
                        this.viewModel.Document.Data.AddParagraph(new ParagraphCode(nameFile, data1));
                    }
                }
            }
        }
        viewModel.VisibilityDrag = Visibility.Collapsed;
    }

    void NewNotComplexObjects_DragOver(object sender, DragEventArgs e)
    {
        DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
        if (dragDropInfo != null)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
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
}