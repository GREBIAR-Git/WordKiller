using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WordKiller.DataTypes;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.Scripts;
using WordKiller.Scripts.ForUI;
using WordKiller.Scripts.ImportExport;
using WordKiller.ViewModels;

namespace WordKiller;

public partial class MainWindow : Window
{
    readonly ViewModelMain viewModel;

    readonly WordKillerFile file;

    IParagraphData? target;

    public MainWindow(string[] args)
    {
        //args = new string[] { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\1.wkr" };
        InitializeComponent();
        viewModel = new();
        DataContext = viewModel;
        file = new(saveLogo);
        TitleElements.SaveTitleUIElements(titlePanel);
        HeaderUpdateMI();

        if (args.Length > 0)
        {
            if (args[0].EndsWith(Properties.Settings.Default.Extension) && File.Exists(args[0]))
            {
                OpenFile(args[0]);
            }
            else
            {
                UIHelper.ShowError("1");
            }
        }
        textPanel.ColumnDefinitions[0].Width = new GridLength(Properties.Settings.Default.TreeViewSize, GridUnitType.Star);
        textPanel.ColumnDefinitions[2].Width = new GridLength(100 - Properties.Settings.Default.TreeViewSize, GridUnitType.Star);
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
            if ((IParagraphData)paragraphTree.SelectedItem != null)
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
                int i = viewModel.Data.Paragraphs.IndexOf(_targetItem);
                int f = viewModel.Data.Paragraphs.IndexOf(_sourceItem);
                (viewModel.Data.Paragraphs[i], viewModel.Data.Paragraphs[f]) = (viewModel.Data.Paragraphs[f], viewModel.Data.Paragraphs[i]);
            }
            return;
        }
        else if (_targetItem is ParagraphH2)
        {
            if (_sourceItem is ParagraphH2)
            {
                if (MessageBox.Show("Поменять местами «" + _sourceItem.Description.ToString() + "» с «" + _targetItem.Description.ToString() + "»", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    viewModel.Data.SwapParagraphs(_targetItem, _sourceItem);
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
                    viewModel.Data.SwapParagraphs(_targetItem, _sourceItem);
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
            viewModel.Data.RemoveParagraph(_sourceItem);
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
                        this.viewModel.Data.AddParagraph(new ParagraphPicture(nameFile, bitmap));
                    }
                    catch
                    {
                        FileStream file = new(path, FileMode.Open);
                        StreamReader reader = new(file);
                        string data1 = reader.ReadToEnd();
                        this.viewModel.Data.AddParagraph(new ParagraphCode(nameFile, data1));
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

    void TreeView_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        Properties.Settings.Default.TreeViewSize = textPanel.ColumnDefinitions[0].Width.Value * 100 / textPanel.ActualWidth;
        Properties.Settings.Default.Save();
    }

    // Верхнее меню

    async void Export_MI_Click(object sender, RoutedEventArgs e)
    {
        Report report = new();
        await Task.Run(() =>
            Report.Create(viewModel.Data, viewModel.Export.ExportPDF, viewModel.Export.ExportHTML));
        if (Properties.Settings.Default.CloseWindow)
        {
            UIHelper.WindowClose();
        }
    }

    void WindowBinding_New(object sender, ExecutedRoutedEventArgs e)
    {
        viewModel.Data = file.NewFile(richTextBox);
    }

    void WindowBinding_Open(object sender, ExecutedRoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "wordkiller file (*" + Properties.Settings.Default.Extension + ")|*" + Properties.Settings.Default.Extension + "|All files (*.*)|*.*"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            OpenFile(openFileDialog.FileName);
        }
    }

    void OpenFile(string fileName)
    {
        viewModel.Data = file.OpenFile(fileName);
        HeaderUpdate();
        paragraphTree.ItemsSource = viewModel.Data.Paragraphs;
        if (paragraphTree.Items.Count > 0)
        {
            //paragraphTree.SelectedIndex = 0;
        }
    }

    void WindowBinding_Save(object sender, ExecutedRoutedEventArgs e)
    {
        file.Save(viewModel.Data);
        HeaderUpdateMI();
    }

    void WindowBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
    {
        file.SaveAs(viewModel.Data);
        HeaderUpdateMI();
    }

    void WindowBinding_Quit(object sender, ExecutedRoutedEventArgs e)
    {
        UIHelper.WindowClose();
    }

    void DocumentType_MI_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        DocumentTypeChange(menuItem);
    }

    void Title_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel.Data.Properties.Title)
        {
            if (viewModel.Data.Paragraphs.Count > 0)
            {
                if (viewModel.Data.Paragraphs[0] is not ParagraphTitle)
                {
                    viewModel.Data.InsertBefore(viewModel.Data.Paragraphs[0], new ParagraphTitle());
                }
            }
            else
            {
                viewModel.Data.AddParagraph(new ParagraphTitle());
            }
        }
        else
        {
            if (viewModel.Data.Paragraphs.Count > 0 && viewModel.Data.Paragraphs[0] is ParagraphTitle)
            {
                viewModel.Data.Paragraphs.RemoveAt(0);
            }
            if (viewModel.Data.Paragraphs.Count > 0)
            {
                if (viewModel.Data.Paragraphs[0] is ParagraphTaskSheet)
                {
                    return;
                }
            }
            richTextBox.Visibility = Visibility.Visible;
        }
    }

    void TaskSheet_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel.Data.Properties.TaskSheet)
        {
            if (viewModel.Data.Paragraphs.Count > 0)
            {
                if (viewModel.Data.Paragraphs[0] is not ParagraphTitle)
                {
                    viewModel.Data.InsertBefore(viewModel.Data.Paragraphs[0], new ParagraphTaskSheet());
                }
                else
                {
                    viewModel.Data.InsertAfter(viewModel.Data.Paragraphs[0], new ParagraphTaskSheet());
                }
            }
            else
            {
                viewModel.Data.AddParagraph(new ParagraphTaskSheet());
            }
        }
        else
        {
            if (viewModel.Data.Paragraphs.Count > 1 && viewModel.Data.Paragraphs[0] is ParagraphTitle && viewModel.Data.Paragraphs[1] is ParagraphTaskSheet)
            {
                viewModel.Data.Paragraphs.RemoveAt(1);
            }
            else if (viewModel.Data.Paragraphs.Count > 0 && viewModel.Data.Paragraphs[0] is ParagraphTaskSheet)
            {
                richTextBox.Visibility = Visibility.Visible;
                viewModel.Data.Paragraphs.RemoveAt(0);
            }
        }

    }

    void DocumentTypeChange(MenuItem menuItem)
    {
        if (menuItem.IsChecked)
        {
            return;
        }
        DefaultDocumentMI.IsChecked = false;
        LaboratoryWorkMI.IsChecked = false;
        PracticeWorkMI.IsChecked = false;
        CourseworkMI.IsChecked = false;
        ControlWorkMI.IsChecked = false;
        ReferatMI.IsChecked = false;
        DiplomaMI.IsChecked = false;
        VKRMI.IsChecked = false;
        menuItem.IsChecked = true;
        HeaderUpdateMI();
    }

    void HeaderUpdateMI()
    {
        if (DefaultDocumentMI.IsChecked)
        {
            if (viewModel.Data.Paragraphs.Count > 0 && viewModel.Data.Paragraphs[0] is ParagraphTitle)
            {
                viewModel.Data.Paragraphs.RemoveAt(0);
            }
            if (viewModel.Data.Paragraphs.Count > 1 && viewModel.Data.Paragraphs[0] is ParagraphTitle && viewModel.Data.Paragraphs[1] is ParagraphTaskSheet)
            {
                viewModel.Data.Paragraphs.RemoveAt(1);
            }
            else if (viewModel.Data.Paragraphs.Count > 0 && viewModel.Data.Paragraphs[0] is ParagraphTaskSheet)
            {
                viewModel.Data.Paragraphs.RemoveAt(0);
            }
            viewModel.Data.Properties.Title = false;
            title.Visibility = Visibility.Collapsed;
            taskSheet.Visibility = Visibility.Collapsed;
            richTextBox.Visibility = Visibility.Visible;
            viewModel.Data.Properties.TaskSheet = false;
            TextHeader("DefaultDocument");
            viewModel.Data.Type = TypeDocument.DefaultDocument;
        }
        else
        {
            title.Visibility = Visibility.Visible;
            viewModel.Data.Properties.Title = true;
            viewModel.Data.Properties.TaskSheet = false;
            taskSheet.Visibility = Visibility.Collapsed;
            if (viewModel.Data.Paragraphs.Count > 0)
            {
                if (viewModel.Data.Paragraphs[0] is not ParagraphTitle)
                {
                    viewModel.Data.InsertBefore(viewModel.Data.Paragraphs[0], new ParagraphTitle());
                }
            }
            else
            {
                viewModel.Data.AddParagraph(new ParagraphTitle());
            }

            if (viewModel.Data.Paragraphs.Count > 1 && viewModel.Data.Paragraphs[0] is ParagraphTitle && viewModel.Data.Paragraphs[1] is ParagraphTaskSheet)
            {
                viewModel.Data.Paragraphs.RemoveAt(1);
            }
            else if (viewModel.Data.Paragraphs.Count > 0 && viewModel.Data.Paragraphs[0] is ParagraphTaskSheet)
            {
                viewModel.Data.Paragraphs.RemoveAt(0);
            }

            if (LaboratoryWorkMI.IsChecked)
            {
                viewModel.Data.Type = TypeDocument.LaboratoryWork;
                TextHeader("LaboratoryWork");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (PracticeWorkMI.IsChecked)
            {
                viewModel.Data.Type = TypeDocument.PracticeWork;
                TextHeader("PracticeWork");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (CourseworkMI.IsChecked)
            {
                taskSheet.Visibility = Visibility.Visible;
                viewModel.Data.Properties.TaskSheet = true;
                viewModel.Data.Type = TypeDocument.Coursework;
                if (viewModel.Data.Paragraphs.Count > 0)
                {
                    if (viewModel.Data.Paragraphs[0] is not ParagraphTitle)
                    {
                        viewModel.Data.InsertBefore(viewModel.Data.Paragraphs[0], new ParagraphTaskSheet());
                    }
                    else
                    {
                        viewModel.Data.InsertAfter(viewModel.Data.Paragraphs[0], new ParagraphTaskSheet());
                    }
                }
                else
                {
                    viewModel.Data.AddParagraph(new ParagraphTaskSheet());
                }
                TextHeader("Coursework");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 4.1 5.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ControlWorkMI.IsChecked)
            {
                viewModel.Data.Type = TypeDocument.ControlWork;
                TextHeader("ControlWork");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ReferatMI.IsChecked)
            {
                viewModel.Data.Type = TypeDocument.Referat;
                TextHeader("Referat");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 0.3 1.3 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (DiplomaMI.IsChecked)
            {
                viewModel.Data.Type = TypeDocument.Diploma;
                TextHeader("DiplomaWork");
                TitleElements.ShowTitleElems(titlePanel, "");
            }
            else if (VKRMI.IsChecked)
            {
                viewModel.Data.Type = TypeDocument.VKR;
                TextHeader("VKR");
                TitleElements.ShowTitleElems(titlePanel, "");
            }
        }
    }

    void HeaderUpdate()
    {
        switch (viewModel.Data.Type)
        {
            case TypeDocument.DefaultDocument:
                DocumentTypeChange(DefaultDocumentMI);
                break;
            case TypeDocument.LaboratoryWork:
                DocumentTypeChange(LaboratoryWorkMI);
                break;
            case TypeDocument.PracticeWork:
                DocumentTypeChange(PracticeWorkMI);
                break;
            case TypeDocument.Coursework:
                DocumentTypeChange(CourseworkMI);
                break;
            case TypeDocument.ControlWork:
                DocumentTypeChange(ControlWorkMI);
                break;
            case TypeDocument.Referat:
                DocumentTypeChange(ReferatMI);
                break;
            case TypeDocument.Diploma:
                DocumentTypeChange(DiplomaMI);
                break;
            case TypeDocument.VKR:
                DocumentTypeChange(VKRMI);
                break;
        }

        HeaderUpdateMI();
    }

    void TextHeader(string type)
    {
        string text = (string)FindResource(type);
        if (file.SavePathExists())
        {
            viewModel.WinTitle = file.SavePath + " — " + text;
        }
        else
        {
            viewModel.WinTitle = "WordKiller — " + text;
        }
    }

    //титульник

    void FacultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (professorComboBox != null)
        {
            TitleElements.UpdateProfessorComboBox(professorComboBox, facultyComboBox);
        }
    }

    void CapsLockFix_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox a = (TextBox)sender;
        if (!string.IsNullOrEmpty(a.Text))
        {
            a.Text = a.Text[..1].ToUpper() + a.Text[1..].ToLower();
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
                if (!item.IsSelected)
                {
                    notComplexObjects.Visibility = Visibility.Collapsed;
                    taskSheetPanel.Visibility = Visibility.Collapsed;
                    titlePanel.Visibility = Visibility.Collapsed;
                    unselectInfo.Visibility = Visibility.Visible;
                }
            }
        }
    }

    void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        richTextBox.AllowDrop = false;
        unselectInfo.Visibility = Visibility.Collapsed;
        if (paragraphTree.SelectedItem is ParagraphTitle)
        {
            notComplexObjects.Visibility = Visibility.Collapsed;
            taskSheetPanel.Visibility = Visibility.Collapsed;
            titlePanel.Visibility = Visibility.Visible;
        }
        else if (paragraphTree.SelectedItem is ParagraphTaskSheet)
        {
            notComplexObjects.Visibility = Visibility.Collapsed;
            taskSheetPanel.Visibility = Visibility.Visible;
            titlePanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            notComplexObjects.Visibility = Visibility.Visible;
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

        if (typeItem.Content.ToString() == "Текст")
        {
            viewModel.Data.AddParagraph(new ParagraphText());
        }
        else if (typeItem.Content.ToString() == "Заголовок")
        {
            viewModel.Data.AddParagraph(new ParagraphH1());
        }
        else if (typeItem.Content.ToString() == "Подзаголовок")
        {
            viewModel.Data.AddParagraph(new ParagraphH2());
        }
        else if (typeItem.Content.ToString() == "Список")
        {
            viewModel.Data.AddParagraph(new ParagraphList());
        }
        else if (typeItem.Content.ToString() == "Картинка")
        {
            viewModel.Data.AddParagraph(new ParagraphPicture());
        }
        else if (typeItem.Content.ToString() == "Таблица")
        {
            viewModel.Data.AddParagraph(new ParagraphTable());
        }
        else if (typeItem.Content.ToString() == "Фрагмент кода")
        {
            viewModel.Data.AddParagraph(new ParagraphCode());
        }
    }

    void ContextMenuDelete_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem != null)
        {
            if (paragraphTree.SelectedItem is IParagraphData item)
            {
                viewModel.Data.RemoveParagraph(item);
                if (paragraphTree.SelectedItem == null)
                {
                    notComplexObjects.Visibility = Visibility.Collapsed;
                    taskSheetPanel.Visibility = Visibility.Collapsed;
                    titlePanel.Visibility = Visibility.Collapsed;
                    unselectInfo.Visibility = Visibility.Visible;
                }
            }
        }
    }

    void ContextMenuInsertAfter_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem == null) return;
        viewModel.Data.InsertAfter(paragraphTree.SelectedItem as IParagraphData, new ParagraphText());
    }

    void ContextMenuInsertBefore_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem == null) return;
        viewModel.Data.InsertBefore(paragraphTree.SelectedItem as IParagraphData, new ParagraphText());
    }
}