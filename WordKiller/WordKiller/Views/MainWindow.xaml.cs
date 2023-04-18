using System;
using System.IO;
using System.Linq;
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

    public MainWindow(string[] args)
    {
        //args = new string[] { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\1.wkr" };

        InitializeComponent();
        viewModel = (ViewModelMain)DataContext;

        if (args.Length > 0)
        {
            if (args[0].EndsWith(Properties.Settings.Default.Extension) && File.Exists(args[0]))
            {
                viewModel.Document.Open(args[0]);
            }
            else
            {
                UIHelper.ShowError("1");
            }
        }
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
        if (dragDropInfo == null && e.Data.GetData(DataFormats.FileDrop) != null && (paragraphTree.SelectedItem is ParagraphPicture || paragraphTree.SelectedItem is ParagraphCode))
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
            dragDrop.Visibility = Visibility.Visible;
        }
    }

    void Win_DragOver(object sender, DragEventArgs e)
    {
        e.Handled = true;
        e.Effects = DragDropEffects.None;
        if (EnableDragDrop(e))
        {
            dragDrop.Visibility = Visibility.Visible;
        }
    }

    void Win_DragLeave(object sender, DragEventArgs e)
    {
        e.Handled = true;
        e.Effects = DragDropEffects.None;
        if (EnableDragDrop(e))
        {
            dragDrop.Visibility = Visibility.Collapsed;
        }
    }

    void PictureBox_DragEnter(object sender, DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            dragDrop.Visibility = Visibility.Visible;
        }
    }

    void PictureBox_DragOver(object sender, DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            dragDrop.Visibility = Visibility.Visible;
        }
    }

    void PictureBox_DragLeave(object sender, DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            dragDrop.Visibility = Visibility.Collapsed;
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
                        pictureI.Source = paragraphPicture.BitmapImage;
                        descriptionObject.Text = nameFile;

                    }
                    else if (paragraphTree.SelectedItem is ParagraphCode)
                    {
                        FileStream file = new(path, FileMode.Open);
                        StreamReader reader = new(file);
                        string data1 = reader.ReadToEnd();
                        descriptionObject.Text = nameFile;
                        richTextBox.SetText(data1, false);
                    }
                }
            }
            dragDrop.Visibility = Visibility.Collapsed;
        }
    }

    void TreeView_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (paragraphTree.SelectedItem is IParagraphData drag)
            {
                if (drag is not ParagraphTitle && drag is not ParagraphTaskSheet)
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
            dragDropInfo.ParagraphData is ParagraphTitle || dragDropInfo.ParagraphData is ParagraphTaskSheet || TargetItem.Header is ParagraphTitle || TargetItem.Header is ParagraphTaskSheet ||
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
        //добавить чтобы главный копировать
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
        dragDrop.Visibility = Visibility.Collapsed;
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

    void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (paragraphTree.SelectedItem != null)
        {
            if (paragraphTree.SelectedItem is IParagraphData item)
            {
                item.Data = richTextBox.GetText();
            }
        }
    }

    void DescriptionObject_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (paragraphTree.SelectedItem != null)
        {
            if (paragraphTree.SelectedItem is IParagraphData item)
            {
                item.Description = descriptionObject.Text;
            }
        }
    }

    void WindowBinding_Unselect(object sender, ExecutedRoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem != null)
        {
            if (paragraphTree.ItemContainerGenerator.ContainerFromItem(paragraphTree.SelectedItem) is TreeViewItem item)
            {
                item.IsSelected = false;
                if (!item.IsSelected)
                {
                    viewModel.Document.VisibilityNotComplexObjects = Visibility.Collapsed;
                    taskSheetPanel.Visibility = Visibility.Collapsed;
                    titlePanel.Visibility = Visibility.Collapsed;
                    viewModel.Document.VisibilityUnselectInfo = Visibility.Visible;
                }
            }
        }
    }

    void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        richTextBox.AllowDrop = false;
        viewModel.Document.VisibilityUnselectInfo = Visibility.Collapsed;
        listProcessing.Visibility = Visibility.Collapsed;
        if (paragraphTree.SelectedItem is ParagraphTitle)
        {
            viewModel.Document.VisibilityNotComplexObjects = Visibility.Collapsed;
            taskSheetPanel.Visibility = Visibility.Collapsed;
            titlePanel.Visibility = Visibility.Visible;
        }
        else if (paragraphTree.SelectedItem is ParagraphTaskSheet)
        {
            viewModel.Document.VisibilityNotComplexObjects = Visibility.Collapsed;
            taskSheetPanel.Visibility = Visibility.Visible;
            titlePanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            viewModel.Document.VisibilityNotComplexObjects = Visibility.Visible;
            taskSheetPanel.Visibility = Visibility.Collapsed;
            titlePanel.Visibility = Visibility.Collapsed;
            if (paragraphTree.SelectedItem is IParagraphData data)
            {
                descriptionObject.Visibility = data.DescriptionVisibility();
                if (descriptionObject.IsVisible)
                {
                    descriptionObject.Text = data.Description;
                }

                if (data is ParagraphPicture paragraphPicture)
                {
                    richTextBox.Visibility = Visibility.Collapsed;
                    picture.Visibility = Visibility.Visible;
                    pictureI.Source = paragraphPicture.BitmapImage;
                    tablePanel.Visibility = Visibility.Collapsed;
                }
                else if (data is ParagraphTable paragraphTable)
                {
                    richTextBox.Visibility = Visibility.Collapsed;
                    picture.Visibility = Visibility.Collapsed;
                    tablePanel.Visibility = Visibility.Visible;
                    countRows.Text = paragraphTable.TableData.Rows.ToString();
                    countColumns.Text = paragraphTable.TableData.Columns.ToString();
                    UpdateTable();
                }
                else if (data != null && e.OldValue != e.NewValue)
                {
                    richTextBox.Visibility = Visibility.Visible;
                    picture.Visibility = Visibility.Collapsed;
                    if (data is not ParagraphCode)
                    {
                        richTextBox.SetText(data.Data, richTextBox.SpellCheck.IsEnabled);
                    }
                    else
                    {
                        richTextBox.SetText(data.Data, false);
                        richTextBox.AllowDrop = true;
                    }
                    if (data is ParagraphList)
                    {
                        listProcessing.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }

    void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        richTextBox.KeyProcessing(e);
    }

    void NewNotComplexObjects(object sender, MouseButtonEventArgs e)
    {
        ComboBoxItem typeItem = (ComboBoxItem)e.Source;

        if (typeItem.Content.ToString() == UIHelper.FindResourse("Text"))
        {
            viewModel.Document.Data.AddParagraph(new ParagraphText());
        }
        else if (typeItem.Content.ToString() == UIHelper.FindResourse("Header"))
        {
            viewModel.Document.Data.AddParagraph(new ParagraphH1());
        }
        else if (typeItem.Content.ToString() == UIHelper.FindResourse("SubHeader"))
        {
            viewModel.Document.Data.AddParagraph(new ParagraphH2());
        }
        else if (typeItem.Content.ToString() == UIHelper.FindResourse("List"))
        {
            viewModel.Document.Data.AddParagraph(new ParagraphList());
        }
        else if (typeItem.Content.ToString() == UIHelper.FindResourse("Picture"))
        {
            viewModel.Document.Data.AddParagraph(new ParagraphPicture());
        }
        else if (typeItem.Content.ToString() == UIHelper.FindResourse("Table"))
        {
            viewModel.Document.Data.AddParagraph(new ParagraphTable());
        }
        else if (typeItem.Content.ToString() == UIHelper.FindResourse("Code"))
        {
            viewModel.Document.Data.AddParagraph(new ParagraphCode());
        }
    }

    void ContextMenuDelete_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem != null)
        {
            if (paragraphTree.SelectedItem is IParagraphData item)
            {
                viewModel.Document.Data.RemoveParagraph(item);
                if (paragraphTree.SelectedItem == null)
                {
                    viewModel.Document.VisibilityNotComplexObjects = Visibility.Collapsed;
                    taskSheetPanel.Visibility = Visibility.Collapsed;
                    titlePanel.Visibility = Visibility.Collapsed;
                    viewModel.Document.VisibilityUnselectInfo = Visibility.Visible;
                }
            }
        }
    }

    void ContextMenuInsertAfter_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem == null) return;
        viewModel.Document.Data.InsertAfter(paragraphTree.SelectedItem as IParagraphData, new ParagraphText());
    }

    void ContextMenuInsertBefore_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem == null) return;
        viewModel.Document.Data.InsertBefore(paragraphTree.SelectedItem as IParagraphData, new ParagraphText());
    }

    void Button_Click(object sender, RoutedEventArgs e)
    {
        string tt = richTextBox.GetText();
        string[] lines = tt.Split("\r\n");
        for (int j = 0; j < lines.Length; j++)
        {
            string[] words = lines[j].Split(' ');
            if (words.Length > 0)
            {
                int after = words[0].IndexOf(')');
                int before = words[0].IndexOf('(');
                int numberSeparators = 0;
                if (after != -1 && before == -1)
                {
                    if (words[0].Length == after + 1)
                    {
                        int separator = -1;
                        for (int i = 0; i < words[0].Length; i++)
                        {
                            separator = words[0].IndexOf('.', separator + 1);
                            if (separator != -1)
                            {
                                numberSeparators++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (numberSeparators > 0)
                        {
                            string sep = "";
                            for (int f = 0; f < numberSeparators; f++)
                            {
                                sep += "!";
                            }
                            words[0] = sep;
                        }
                        else
                        {
                            words = words.Skip(1).ToArray();
                        }
                    }
                    else
                    {
                        string txt = words[0].Substring(after + 1);
                        int separator = -1;
                        for (int i = 0; i < words[0].Length; i++)
                        {
                            separator = words[0].IndexOf('.', separator + 1);
                            if (separator != -1)
                            {
                                numberSeparators++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (numberSeparators > 0)
                        {
                            string sep = "";
                            for (int f = 0; f < numberSeparators; f++)
                            {
                                sep += "!";
                            }
                            words[0] = sep + " " + txt;
                        }
                        else
                        {
                            words[0] = txt;
                        }
                    }
                }
            }
            lines[j] = string.Join(" ", words);
        }
        richTextBox.SetText(string.Join("\n", lines));
    }
}