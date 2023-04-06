using Microsoft.Win32;
using OrelUniverEmbeddedAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
    DocumentData data;
    readonly WordKillerFile file;

    IParagraphData? target;

    public MainWindow(string[] args)
    {
        //args = new string[] { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\1.wkr" };
        data = new();
        InitializeComponent();
        viewModel = new()
        {
            FontSizeRTB = Properties.Settings.Default.FontSizeRTB,
            MainColor = Properties.Settings.Default.MainColor,
            AdditionalColor = Properties.Settings.Default.AdditionalColor,
            AlternativeColor = Properties.Settings.Default.AlternativeColor,
            HoverColor = Properties.Settings.Default.HoverColor,
            FontSize = Properties.Settings.Default.FontSize,
            WinTitle = "WordKiller",
            TitleOpen = true,
            Title = new(),
            Properties = new()
        };
        DataContext = viewModel;

        if (!WkrExport.IsWordInstall())
        {
            ExportPDF.Visibility = Visibility.Collapsed;
            ExportHTML.Visibility = Visibility.Collapsed;
            ExportPDF.IsChecked = false;
            ExportHTML.IsChecked = false;
        }
        else
        {
            ExportPDF.IsChecked = Properties.Settings.Default.ExportPDF;
            ExportHTML.IsChecked = Properties.Settings.Default.ExportHTML;
        }
        file = new(saveLogo);
        TitleElements.SaveTitleUIElements(titlePanel);
        HeaderUpdateMI();
        UpdateCheckSyntax();
        data.Properties = viewModel.Properties;
        data.Title = viewModel.Title;
        paragraphTree.ItemsSource = data.Paragraphs;

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
        InitSetting();
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
                        FontSize = double.Parse(viewModel.FontSizeRTB),
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
        if (dragDropInfo == null && (paragraphTree.SelectedItem is ParagraphPicture || paragraphTree.SelectedItem is ParagraphCode))
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
                    string nameFile = path.Split('\\').Last();
                    nameFile = nameFile[..nameFile.LastIndexOf('.')];
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
        TreeViewItem TargetItem = GetNearestContainer
                (e.OriginalSource as UIElement);

        DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
        if (dragDropInfo != null && TargetItem.Header != dragDropInfo.paragraphData)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
    }

    void TreeView_Drop(object sender, DragEventArgs e)
    {
        try
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            TreeViewItem TargetItem = GetNearestContainer
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
        if (_sourceItem is ParagraphTitle || _targetItem is ParagraphTitle)
        {
            MessageBox.Show("Так сделать невозможно", "Ошибка", MessageBoxButton.OK);
            return;
        }
        //добавить чтобы главный копировать
        if (_targetItem is ParagraphH1 && _sourceItem is ParagraphH1)
        {
            if (MessageBox.Show("Поменять местами «" + _sourceItem.Description.ToString() + "» с «" + _targetItem.Description.ToString() + "»", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int i = data.Paragraphs.IndexOf(_targetItem);
                int f = data.Paragraphs.IndexOf(_sourceItem);
                (data.Paragraphs[i], data.Paragraphs[f]) = (data.Paragraphs[f], data.Paragraphs[i]);
            }
            return;
        }
        else if (_targetItem is ParagraphH2)
        {
            if (_sourceItem is ParagraphH2)
            {
                if (MessageBox.Show("Поменять местами «" + _sourceItem.Description.ToString() + "» с «" + _targetItem.Description.ToString() + "»", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    data.SwapParagraphs(_targetItem, _sourceItem);
                }
                return;
            }
            else if (_sourceItem is ParagraphH1)
            {
                MessageBox.Show("Так сделать невозможно", "Ошибка", MessageBoxButton.OK);
                return;
            }
        }
        if (_targetItem is not SectionParagraphs)
        {
            if (_sourceItem is not ParagraphH2 && _sourceItem is not ParagraphH1)
            {
                if (MessageBox.Show("Поменять местами «" + _sourceItem.Description.ToString() + "» с «" + _targetItem.Description.ToString() + "»", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    data.SwapParagraphs(_targetItem, _sourceItem);
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
            data.RemoveParagraph(_sourceItem);
            SectionParagraphs section = _targetItem as SectionParagraphs;
            section.AddParagraph(_sourceItem);
        }

    }

    TreeViewItem GetNearestContainer(UIElement element)
    {
        TreeViewItem? container = element as TreeViewItem;
        while ((container == null) && (element != null))
        {
            element = VisualTreeHelper.GetParent(element) as UIElement;
            container = element as TreeViewItem;
        }
        return container;
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
                        this.data.AddParagraph(new ParagraphPicture(nameFile, bitmap));
                    }
                    catch
                    {
                        FileStream file = new(path, FileMode.Open);
                        StreamReader reader = new(file);
                        string data1 = reader.ReadToEnd();
                        this.data.AddParagraph(new ParagraphCode(nameFile, data1));
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

    // Верхнее меню

    async void Export_MI_Click(object sender, RoutedEventArgs e)
    {
        bool exportPDFOn = ExportPDF.IsChecked;
        bool exportHTMLOn = ExportHTML.IsChecked;

        Report report = new();
        await Task.Run(() =>
            Report.Create(data, exportPDFOn, exportHTMLOn));
        if (Properties.Settings.Default.CloseWindow)
        {
            UIHelper.WindowClose();
        }
    }

    void ExportPDF_MI_Click(object sender, RoutedEventArgs e)
    {
        ExportPDF.IsChecked = !ExportPDF.IsChecked;
        Properties.Settings.Default.ExportPDF = ExportPDF.IsChecked;
        Properties.Settings.Default.Save();
    }

    void ExportHTML_MI_Click(object sender, RoutedEventArgs e)
    {
        ExportHTML.IsChecked = !ExportHTML.IsChecked;
        Properties.Settings.Default.ExportHTML = ExportHTML.IsChecked;
        Properties.Settings.Default.Save();
    }

    void WindowBinding_New(object sender, ExecutedRoutedEventArgs e)
    {
        file.NewFile(ref data, richTextBox);
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
        file.OpenFile(fileName, ref data);
        HeaderUpdate();
        viewModel.Properties = data.Properties;
        viewModel.Title = data.Title;
        paragraphTree.ItemsSource = data.Paragraphs;
        if (viewModel.TextOpen)
        {
            if (paragraphTree.Items.Count > 0)
            {
                //paragraphTree.SelectedIndex = 0;
            }
        }
    }

    void WindowBinding_Save(object sender, ExecutedRoutedEventArgs e)
    {
        file.Save(data);
        HeaderUpdateMI();
    }

    void WindowBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
    {
        file.SaveAs(data);
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
        if (data.Properties.Title)
        {
            if (data.Paragraphs.Count > 0)
            {
                if (data.Paragraphs[0] is not ParagraphTitle)
                {
                    data.InsertBefore(data.Paragraphs[0], new ParagraphTitle());
                }
            }
            else
            {
                data.AddParagraph(new ParagraphTitle());
            }
        }
        else
        {
            if (data.Paragraphs.Count > 0 && data.Paragraphs[0] is ParagraphTitle)
            {
                data.Paragraphs.RemoveAt(0);
            }
            if (data.Paragraphs.Count > 0)
            {
                if (data.Paragraphs[0] is ParagraphTaskSheet)
                {
                    return;
                }
            }
            richTextBox.Visibility = Visibility.Visible;
        }
    }

    void TaskSheet_Click(object sender, RoutedEventArgs e)
    {
        if (data.Properties.TaskSheet)
        {
            if (data.Paragraphs.Count > 0)
            {
                if (data.Paragraphs[0] is not ParagraphTitle)
                {
                    data.InsertBefore(data.Paragraphs[0], new ParagraphTaskSheet());
                }
                else
                {
                    data.InsertAfter(data.Paragraphs[0], new ParagraphTaskSheet());
                }
            }
            else
            {
                data.AddParagraph(new ParagraphTaskSheet());
            }
        }
        else
        {
            if (data.Paragraphs.Count > 1 && data.Paragraphs[0] is ParagraphTitle && data.Paragraphs[1] is ParagraphTaskSheet)
            {
                data.Paragraphs.RemoveAt(1);
            }
            else if (data.Paragraphs.Count > 0 && data.Paragraphs[0] is ParagraphTaskSheet)
            {
                richTextBox.Visibility = Visibility.Visible;
                data.Paragraphs.RemoveAt(0);
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
            if (data.Paragraphs.Count > 0 && data.Paragraphs[0] is ParagraphTitle)
            {
                data.Paragraphs.RemoveAt(0);
            }
            if (data.Paragraphs.Count > 1 && data.Paragraphs[0] is ParagraphTitle && data.Paragraphs[1] is ParagraphTaskSheet)
            {
                data.Paragraphs.RemoveAt(1);
            }
            else if (data.Paragraphs.Count > 0 && data.Paragraphs[0] is ParagraphTaskSheet)
            {
                data.Paragraphs.RemoveAt(0);
            }
            data.Properties.Title = false;
            title.Visibility = Visibility.Collapsed;
            taskSheet.Visibility = Visibility.Collapsed;
            richTextBox.Visibility = Visibility.Visible;
            data.Properties.TaskSheet = false;
            TextHeader("DefaultDocument");
            data.Type = TypeDocument.DefaultDocument;
        }
        else
        {
            title.Visibility = Visibility.Visible;
            data.Properties.Title = true;
            data.Properties.TaskSheet = false;
            taskSheet.Visibility = Visibility.Collapsed;
            if (data.Paragraphs.Count > 0)
            {
                if (data.Paragraphs[0] is not ParagraphTitle)
                {
                    data.InsertBefore(data.Paragraphs[0], new ParagraphTitle());
                }
            }
            else
            {
                data.AddParagraph(new ParagraphTitle());
            }

            if (data.Paragraphs.Count > 1 && data.Paragraphs[0] is ParagraphTitle && data.Paragraphs[1] is ParagraphTaskSheet)
            {
                data.Paragraphs.RemoveAt(1);
            }
            else if (data.Paragraphs.Count > 0 && data.Paragraphs[0] is ParagraphTaskSheet)
            {
                data.Paragraphs.RemoveAt(0);
            }

            if (LaboratoryWorkMI.IsChecked)
            {
                data.Type = TypeDocument.LaboratoryWork;
                TextHeader("LaboratoryWork");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (PracticeWorkMI.IsChecked)
            {
                data.Type = TypeDocument.PracticeWork;
                TextHeader("PracticeWork");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (CourseworkMI.IsChecked)
            {
                taskSheet.Visibility = Visibility.Visible;
                data.Properties.TaskSheet = true;
                data.Type = TypeDocument.Coursework;
                if (data.Paragraphs.Count > 0)
                {
                    if (data.Paragraphs[0] is not ParagraphTitle)
                    {
                        data.InsertBefore(data.Paragraphs[0], new ParagraphTaskSheet());
                    }
                    else
                    {
                        data.InsertAfter(data.Paragraphs[0], new ParagraphTaskSheet());
                    }
                }
                else
                {
                    data.AddParagraph(new ParagraphTaskSheet());
                }
                TextHeader("Coursework");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 4.1 5.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ControlWorkMI.IsChecked)
            {
                data.Type = TypeDocument.ControlWork;
                TextHeader("ControlWork");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ReferatMI.IsChecked)
            {
                data.Type = TypeDocument.Referat;
                TextHeader("Referat");
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 0.3 1.3 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (DiplomaMI.IsChecked)
            {
                data.Type = TypeDocument.Diploma;
                TextHeader("DiplomaWork");
                TitleElements.ShowTitleElems(titlePanel, "");
            }
            else if (VKRMI.IsChecked)
            {
                data.Type = TypeDocument.VKR;
                TextHeader("VKR");
                TitleElements.ShowTitleElems(titlePanel, "");
            }
        }
    }

    void HeaderUpdate()
    {
        switch (data.Type)
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

    void NumberHeading_MI_Click(object sender, RoutedEventArgs e)
    {
        viewModel.Properties.NumberHeading = !viewModel.Properties.NumberHeading;
    }

    void CreateNetwork(object sender, RoutedEventArgs e)
    {

    }

    void JoinNetwork(object sender, RoutedEventArgs e)
    {

    }

    void LeaveNetwork(object sender, RoutedEventArgs e)
    {

    }

    void AboutProgramShow(object sender, RoutedEventArgs e)
    {
        AboutProgram aboutProgram = new();
        aboutProgram.Show();
    }

    void DocumentationShow(object sender, RoutedEventArgs e)
    {
        Documentation documentation = new();
        documentation.Show();
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
            IParagraphData item = paragraphTree.SelectedItem as IParagraphData;
            item.Data = richTextBox.GetText();
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

    void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        richTextBox.AllowDrop = false;
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
            data.AddParagraph(new ParagraphText());
        }
        else if (typeItem.Content.ToString() == "Заголовок")
        {
            data.AddParagraph(new ParagraphH1());
        }
        else if (typeItem.Content.ToString() == "Подзаголовок")
        {
            data.AddParagraph(new ParagraphH2());
        }
        else if (typeItem.Content.ToString() == "Список")
        {
            data.AddParagraph(new ParagraphList());
        }
        else if (typeItem.Content.ToString() == "Картинка")
        {
            data.AddParagraph(new ParagraphPicture());
        }
        else if (typeItem.Content.ToString() == "Таблица")
        {
            data.AddParagraph(new ParagraphTable());
        }
        else if (typeItem.Content.ToString() == "Фрагмент кода")
        {
            data.AddParagraph(new ParagraphCode());
        }
    }

    void ContextMenuDelete_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem == null) return;
        data.RemoveParagraph(paragraphTree.SelectedItem as IParagraphData);
    }

    void ContextMenuInsertAfter_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem == null) return;
        data.InsertAfter(paragraphTree.SelectedItem as IParagraphData, new ParagraphText());
    }

    void ContextMenuInsertBefore_Click(object sender, RoutedEventArgs e)
    {
        if (paragraphTree.SelectedItem == null) return;
        data.InsertBefore(paragraphTree.SelectedItem as IParagraphData, new ParagraphText());
    }




    // Рисование
    /*
    void OpenDrawing()
    {
        
        if (viewModel.TitleOpen)
        {
            prevSettings = TitlePageMI;
        }
        else
        {
            prevSettings = DownPanelMI;
        }
        HideElements(prevSettings);
        DrawingPanel.Visibility = Visibility.Visible;
        menuPanel.Visibility = Visibility.Collapsed;
        ParentPanel.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
        ParentPanel.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Star);
    }

    bool isDrawing = false;

    PathFigure currentFigure;*/
    void DrawingMouseDown(object sender, MouseButtonEventArgs e)
    {
        /*
        Mouse.Capture(DrawingTarget);
        isDrawing = true;
        StartFigure(e.GetPosition(DrawingTarget));*/
    }
    /*
    void AddFigurePoint(Point point)
    {
        //   currentFigure.Segments.Add(new LineSegment(point, isStroked: true));
    }

    void EndFigure()
    {
        //currentFigure = null;
    }

    void StartFigure(Point start)
    {
        /*currentFigure = new PathFigure() { StartPoint = start };
        System.Windows.Shapes.Path currentPath =
            new()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Data = new PathGeometry() { Figures = { currentFigure } }
            };
        DrawingTarget.Children.Add(currentPath);*/
    //}

    void DrawingMouseUp(object sender, MouseButtonEventArgs e)
    {
        /*
        AddFigurePoint(e.GetPosition(DrawingTarget));
        EndFigure();
        isDrawing = false;
        Mouse.Capture(null);*/
    }


    void DrawingMouseMove(object sender, MouseEventArgs e)
    {
        /*
        if (!isDrawing)
            return;
        AddFigurePoint(e.GetPosition(DrawingTarget));*/
    }

    // Настройки
    async void InitSetting()
    {
        Universitet.Items.Add("Орловский Государственный университет имени И.С. Тургенева");

        OrelUniverAPI.Result<Division>? divisions = await OrelUniverAPI.GetDivisionsForStudentsAsync();
        if (divisions.Code != 1)
        {
            if (divisions != null && divisions.Response != null)
            {
                foreach (Division division in divisions.Response)
                {
                    Faculty.Items.Add(division.Title);
                }
            }
        }
    }

    void OpenGeneralisSetiings()
    {
        Profile.Visibility = Visibility.Collapsed;
        Personalization.Visibility = Visibility.Collapsed;
        GeneralisSetiings.Visibility = Visibility.Visible;
        Encoding.SelectedIndex = Properties.Settings.Default.NumberEncoding;
        CloseWindow.IsChecked = Properties.Settings.Default.CloseWindow;
        SyntaxChecking.IsChecked = Properties.Settings.Default.SyntaxChecking;
        AutoHeader.IsChecked = Properties.Settings.Default.AutoHeader;
        AssociationWKR.IsChecked = FileAssociation.IsAssociated;
    }

    void OpenPersonalization(object sender, RoutedEventArgs e)
    {
        Profile.Visibility = Visibility.Collapsed;
        Personalization.Visibility = Visibility.Visible;
        GeneralisSetiings.Visibility = Visibility.Collapsed;
        fontSize.Value = int.Parse(Properties.Settings.Default.FontSize);
        fontSizeRTB.Value = int.Parse(Properties.Settings.Default.FontSizeRTB);
        language.SelectedIndex = Properties.Settings.Default.Language;
    }

    async void OpenProfile()
    {
        Profile.Visibility = Visibility.Visible;
        GeneralisSetiings.Visibility = Visibility.Collapsed;
        Personalization.Visibility = Visibility.Collapsed;
        FirstName.Text = Properties.Settings.Default.FirstName;
        LastName.Text = Properties.Settings.Default.LastName;
        MiddleName.Text = Properties.Settings.Default.MiddleName;
        Universitet.Text = Properties.Settings.Default.Universitet;
        Shifr.Text = Properties.Settings.Default.Shifr;
        FirstNameCoop.Text = Properties.Settings.Default.FirstNameCoop;
        LastNameCoop.Text = Properties.Settings.Default.LastNameCoop;
        MiddleNameCoop.Text = Properties.Settings.Default.MiddleNameCoop;
        ShifrCoop.Text = Properties.Settings.Default.ShifrCoop;
        if ((await OrelUniverAPI.ScheduleGetCourseAsync("1")).Code == 1)
        {
            Faculty.Items.Add(Properties.Settings.Default.FacultyString);
            Faculty.SelectedIndex = 0;
            Group.Items.Add(Properties.Settings.Default.GroupString);
            Group.SelectedIndex = 0;
            Cours.Items.Add(Properties.Settings.Default.CourseString);
            Cours.SelectedIndex = 0;
        }
        else
        {
            Faculty.SelectedIndex = Properties.Settings.Default.Faculty;
            Group.SelectedIndex = Properties.Settings.Default.Group;
            Cours.SelectedIndex = Properties.Settings.Default.Course;
        }
    }

    void FirstName_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Properties.Settings.Default.FirstName = textBox.Text;
        Properties.Settings.Default.Save();
    }

    void LastName_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Properties.Settings.Default.LastName = textBox.Text;
        Properties.Settings.Default.Save();
    }

    void MiddleName_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Properties.Settings.Default.MiddleName = textBox.Text;
        Properties.Settings.Default.Save();
    }

    void Universitet_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;
        Properties.Settings.Default.Universitet = comboBox.Items[comboBox.SelectedIndex].ToString();
        Properties.Settings.Default.Save();
    }

    async void Faculty_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;

        OrelUniverAPI.Result<Division>? result6 = await OrelUniverAPI.GetDivisionsForStudentsAsync();
        if (result6.Code != 1)
        {
            foreach (Division division in result6.Response)
            {
                if (division.Title == Faculty.Items[Faculty.SelectedIndex].ToString())
                {
                    Cours.Items.Clear();
                    Group.Items.Clear();
                    OrelUniverAPI.Result<Cours>? result12 = await OrelUniverAPI.ScheduleGetCourseAsync(division.Id.ToString());//
                    foreach (Cours cours in result12.Response)
                    {
                        Cours.Items.Add(cours.Course);
                    }
                }
            }
            if (comboBox.SelectedIndex >= 0)
            {
                Properties.Settings.Default.FacultyString = comboBox.Items[comboBox.SelectedIndex].ToString();
                Properties.Settings.Default.Faculty = comboBox.SelectedIndex;
            }
            Properties.Settings.Default.Save();
        }
    }

    async void Cours_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;

        OrelUniverAPI.Result<Division>? result6 = await OrelUniverAPI.GetDivisionsForStudentsAsync();
        if (result6.Code != 1)
        {
            foreach (Division division in result6.Response)
            {
                if (Faculty.SelectedIndex >= 0 && division.Title == Faculty.Items[Faculty.SelectedIndex].ToString())
                {
                    OrelUniverAPI.Result<Cours>? result12 = await OrelUniverAPI.ScheduleGetCourseAsync(division.Id.ToString());//
                    foreach (Cours cours in result12.Response)
                    {
                        if (Cours.SelectedIndex >= 0 && cours.Course == Cours.Items[Cours.SelectedIndex].ToString())
                        {
                            Group.Items.Clear();
                            OrelUniverAPI.Result<OrelUniverEmbeddedAPI.Group>? result15 = await OrelUniverAPI.ScheduleGetGroupsAsync(division.Id.ToString(), cours.Course.ToString());
                            foreach (OrelUniverEmbeddedAPI.Group groups in result15.Response)
                            {
                                Group.Items.Add(groups.Title);
                            }
                        }
                    }
                }
            }
            if (comboBox.SelectedIndex >= 0)
            {
                Properties.Settings.Default.CourseString = comboBox.Items[comboBox.SelectedIndex].ToString();
                Properties.Settings.Default.Course = comboBox.SelectedIndex;
            }
            Properties.Settings.Default.Save();
        }
    }

    void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;
        if (comboBox.SelectedIndex >= 0)
        {
            Properties.Settings.Default.GroupString = comboBox.Items[comboBox.SelectedIndex].ToString();
            Properties.Settings.Default.Group = comboBox.SelectedIndex;
        }
        Properties.Settings.Default.Save();
    }

    void Shifr_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Properties.Settings.Default.Shifr = textBox.Text;
        Properties.Settings.Default.Save();
    }

    void FirstNameCoop_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Properties.Settings.Default.FirstNameCoop = textBox.Text;
        Properties.Settings.Default.Save();
    }

    void LastNameCoop_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Properties.Settings.Default.LastNameCoop = textBox.Text;
        Properties.Settings.Default.Save();
    }

    void MiddleNameCoop_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Properties.Settings.Default.MiddleNameCoop = textBox.Text;
        Properties.Settings.Default.Save();
    }

    void ShifrCoop_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        Properties.Settings.Default.ShifrCoop = textBox.Text;
        Properties.Settings.Default.Save();
    }

    void OpenProfile(object sender, RoutedEventArgs e)
    {
        OpenProfile();
    }

    void OpenGeneralisSetiings(object sender, RoutedEventArgs e)
    {
        OpenGeneralisSetiings();
    }

    void CloseWindow_Checked(object sender, RoutedEventArgs e)
    {
        if (CloseWindow.IsChecked != null)
        {
            Properties.Settings.Default.CloseWindow = (bool)CloseWindow.IsChecked;
            Properties.Settings.Default.Save();
        }
    }

    void SyntaxChecking_Checked(object sender, RoutedEventArgs e)
    {
        if (SyntaxChecking.IsChecked != null)
        {
            Properties.Settings.Default.SyntaxChecking = (bool)SyntaxChecking.IsChecked;
            Properties.Settings.Default.Save();
            UpdateCheckSyntax();
        }
    }

    void UpdateCheckSyntax()
    {
        richTextBox.SpellCheck.IsEnabled = Properties.Settings.Default.SyntaxChecking;
    }

    void Encoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Properties.Settings.Default.NumberEncoding = Encoding.SelectedIndex;
        Properties.Settings.Default.Save();
    }

    void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        mainPanel.Visibility = Visibility.Collapsed;
        SettingsPanel.Visibility = Visibility.Visible;
        OpenGeneralisSetiings();
    }

    void ExitSettings(object sender, RoutedEventArgs e)
    {
        SettingsPanel.Visibility = Visibility.Collapsed;
        mainPanel.Visibility = Visibility.Visible;
    }

    void AutoHeader_Checked(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.AutoHeader = AutoHeader.IsChecked ?? true;

        Properties.Settings.Default.Save();
    }

    void AssociationWKR_Click(object sender, RoutedEventArgs e)
    {
        bool association = AssociationWKR.IsChecked ?? true;
        if (association)
        {
            if (!FileAssociation.IsRunAsAdmin())
            {
                ProcessStartInfo proc = new()
                {

                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Environment.ProcessPath,
                    Verb = "runas"
                };
                proc.Arguments += "FileAssociation";
                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    UIHelper.ShowError("2");
                }
            }
            else
            {
                FileAssociation.Associate("WordKiller");
            }
        }
        else
        {
            if (!FileAssociation.IsRunAsAdmin())
            {
                ProcessStartInfo proc = new()
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Environment.ProcessPath,
                    Verb = "runas"
                };
                proc.Arguments += "RemoveFileAssociation";
                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    UIHelper.ShowError("2");
                }
            }
            else
            {
                FileAssociation.Remove();
            }
        }
    }

    void ByDefault(object sender, RoutedEventArgs e)
    {
        mainColor.SelectedColor = (Color)ColorConverter.ConvertFromString("#8daacc");
        additionalColor.SelectedColor = (Color)ColorConverter.ConvertFromString("#4a76a8");
        alternativeColor.SelectedColor = (Color)ColorConverter.ConvertFromString("#335e8f");
        hoverColor.SelectedColor = (Color)ColorConverter.ConvertFromString("#b8860b");
        Properties.Settings.Default.MainColor = mainColor.SelectedColor.ToString();
        Properties.Settings.Default.AdditionalColor = additionalColor.SelectedColor.ToString();
        Properties.Settings.Default.AlternativeColor = alternativeColor.SelectedColor.ToString();
        Properties.Settings.Default.HoverColor = hoverColor.SelectedColor.ToString();
        Properties.Settings.Default.Save();
        fontSize.Value = 28;
        fontSizeRTB.Value = 20;
        language.SelectedIndex = 0;
    }

    void MainColor_ContextMenuClosing(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.MainColor = mainColor.SelectedColor.ToString();
        Properties.Settings.Default.Save();
    }

    void AdditionalColor_ContextMenuClosing(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.AdditionalColor = additionalColor.SelectedColor.ToString();
        Properties.Settings.Default.Save();
    }

    void AlternativeColor_ContextMenuClosing(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.AlternativeColor = alternativeColor.SelectedColor.ToString();
        Properties.Settings.Default.Save();
    }

    void HoverColor_ContextMenuClosing(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.HoverColor = hoverColor.SelectedColor.ToString();
        Properties.Settings.Default.Save();
    }

    void FontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        string size = "1";
        if (fontSize.Value >= 1)
        {
            size = ((int)fontSize.Value).ToString();
        }
        viewModel.FontSize = size;
        Properties.Settings.Default.FontSize = size;
        Properties.Settings.Default.Save();
    }

    void FontSizeRTB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        string size = "1";
        if (fontSizeRTB.Value >= 1)
        {
            size = ((int)fontSizeRTB.Value).ToString();
        }
        viewModel.FontSizeRTB = size;
        Properties.Settings.Default.FontSizeRTB = size;
        Properties.Settings.Default.Save();
        UpdateTable();
    }

    void TreeView_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        Properties.Settings.Default.TreeViewSize = textPanel.ColumnDefinitions[0].Width.Value * 100 / textPanel.ActualWidth;
        Properties.Settings.Default.Save();
    }

    void Button_Click(object sender, RoutedEventArgs e)
    {
        ConfigFile.Open();
    }

    void Button_Click_1(object sender, RoutedEventArgs e)
    {
        ConfigFile.Delete();
    }

    void Button_Click_2(object sender, RoutedEventArgs e)
    {
        ConfigFile.DeleteAll();
    }

    void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.SelectCulture(language.SelectedIndex);
        Properties.Settings.Default.Language = language.SelectedIndex;
        Properties.Settings.Default.Save();
    }
}
