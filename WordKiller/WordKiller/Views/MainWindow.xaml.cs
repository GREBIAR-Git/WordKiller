using Microsoft.Win32;
using OrelUniverEmbeddedAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WordKiller.DataTypes;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.Scripts;
using WordKiller.Scripts.ForUI;
using WordKiller.Scripts.ImportExport;
using WordKiller.ViewModels;

namespace WordKiller;

public partial class MainWindow : Window
{
    int menuLeftIndex;
    readonly string[] menuNames;
    MenuItem DownPanelMI;

    MenuItem prevSettings;

    ViewModelMain viewModel;

    readonly WordKillerFile file;

    DocumentData data;

    bool clearSubstitution = false;

    IParagraphData? current;

    public MainWindow(string[] args)
    {
        //args = new string[] { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\1.wkr" };
        data = new();
        viewModel = new()
        {
            Displayed = (string)FindResource("Something"),
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
        InitializeComponent();
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
        DownPanelMI = SubstitutionMI;
        prevSettings = DownPanelMI;
        HeaderUpdateMI();
        RefreshMenu(1);
        menuNames = ComboBoxSetup();
        UpdateCheckSyntax();
        data.Properties = viewModel.Properties;
        data.Title = viewModel.Title;
        if (args.Length > 0)
        {
            if (args[0].EndsWith(Properties.Settings.Default.Extension) && File.Exists(args[0]))
            {
                OpenFile(args[0]);
            }
            else
            {
                throw new Exception((string)FindResource("Error1"));
            }
        }
        InitSetting();
        lstTest.ItemsSource = data.Paragraphs;
        InitListBox();
    }

    //Комбобоксы 

    string[] ComboBoxSetup()
    {
        string[] names = new string[(elementPanel.Children.Count) / 2 - 1];
        for (int i = 1; i < (elementPanel.Children.Count) / 2; i++)
        {
            Button button = (Button)elementPanel.Children[i];
            button.Click += Click_ComboBox;
            names[i - 1] = button.Content.ToString();
        }

        for (int i = elementPanel.ColumnDefinitions.Count - 1; i < elementPanel.Children.Count - 1; i++)
        {
            ComboBox comboBox = (ComboBox)elementPanel.Children[i];
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            comboBox.PreviewMouseRightButtonDown += ComboBox_PreviewRightMouseDown;
        }
        return names;
    }

    void Click_ComboBox(object sender, RoutedEventArgs e)
    {
        UnselectComboBoxes();
        Button control = (Button)sender;
        if (control.Content.ToString() == (string)FindResource("Header"))
        {
            TypeSubstitution.SelectedIndex = 0;
        }
        else if (control.Content.ToString() == (string)FindResource("SubHeader"))
        {
            TypeSubstitution.SelectedIndex = 1;
        }
        else if (control.Content.ToString() == (string)FindResource("List"))
        {
            TypeSubstitution.SelectedIndex = 2;
        }
        else if (control.Content.ToString() == (string)FindResource("Picture"))
        {
            TypeSubstitution.SelectedIndex = 3;
        }
        else if (control.Content.ToString() == (string)FindResource("Table"))
        {
            TypeSubstitution.SelectedIndex = 4;
        }
        else if (control.Content.ToString() == (string)FindResource("Code"))
        {
            TypeSubstitution.SelectedIndex = 5;
        }
        richTextBoxSubstitution.Focus();
    }

    bool IsComboBoxSelected()
    {
        if (h1ComboBox.SelectedIndex != -1 || h2ComboBox.SelectedIndex != -1 || lComboBox.SelectedIndex != -1 || pComboBox.SelectedIndex != -1 || tComboBox.SelectedIndex != -1 || cComboBox.SelectedIndex != -1)
            return true;
        return false;
    }

    void UnselectComboBoxes()
    {
        for (int i = elementPanel.ColumnDefinitions.Count - 1; i < elementPanel.Children.Count - 1; i++)
        {
            ComboBox cmbBox = (ComboBox)elementPanel.Children[i];
            cmbBox.SelectedIndex = -1;
        }
    }

    bool IsValidDataEntry()
    {
        if (TypeSubstitution.SelectedIndex != -1)
        {
            if (TypeSubstitution.SelectedIndex != 3 && TypeSubstitution.SelectedIndex != 5 || System.IO.File.Exists(richTextBoxSubstitution.GetText()))
            {
                return true;
            }
        }
        return false;
    }

    void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        UnselectAll();
        UnselectComboBoxes();
    }

    void UnselectAll()
    {
        if (current != null)
        {
            if (current is ParagraphTable)
            {
                panelTable.Visibility = Visibility.Collapsed;
                richTextBoxSubstitution.Visibility = Visibility.Visible;
                ShowPictureBox();
            }
            clearSubstitution = true;
            current = null;
            Displayed((string)FindResource("Something"));
            TypeSubstitution.Visibility = Visibility.Visible;
            TypeSubstitutionOn.Visibility = Visibility.Collapsed;
            TypeSubstitution.SelectedIndex = -1;
            HeaderSubstitution.Text = string.Empty;
            richTextBoxSubstitution.Document.Blocks.Clear();
            clearSubstitution = false;
        }
    }

    void Add_Click(object sender, RoutedEventArgs e)
    {
        if (IsValidDataEntry())
        {
            IParagraphData dataParagraph;
            string nameComboBox;
            if (TypeSubstitution.SelectedIndex == 0)
            {
                dataParagraph = new ParagraphH1(richTextBoxSubstitution.GetText());
                viewModel.H1P.Add(dataParagraph as ParagraphH1);
                nameComboBox = "h1";
            }
            else if (TypeSubstitution.SelectedIndex == 1)
            {
                dataParagraph = new ParagraphH2(richTextBoxSubstitution.GetText());
                viewModel.H2P.Add(dataParagraph as ParagraphH2);
                nameComboBox = "h2";
            }
            else if (TypeSubstitution.SelectedIndex == 2)
            {
                dataParagraph = new ParagraphList(HeaderSubstitution.Text, richTextBoxSubstitution.GetText());
                viewModel.LP.Add(dataParagraph as ParagraphList);
                nameComboBox = "l";
            }
            else if (TypeSubstitution.SelectedIndex == 3)
            {
                dataParagraph = new ParagraphPicture(HeaderSubstitution.Text, richTextBoxSubstitution.GetText());
                viewModel.PP.Add(dataParagraph as ParagraphPicture);
                nameComboBox = "p";
            }
            else if (TypeSubstitution.SelectedIndex == 4)
            {
                dataParagraph = current as ParagraphTable;
                dataParagraph.Description = HeaderSubstitution.Text;
                viewModel.TP.Add(dataParagraph as ParagraphTable);
                nameComboBox = "t";
            }
            else if (TypeSubstitution.SelectedIndex == 5)
            {
                dataParagraph = new ParagraphCode(HeaderSubstitution.Text, richTextBoxSubstitution.GetText());
                viewModel.CP.Add(dataParagraph as ParagraphCode);
                nameComboBox = "c";
            }
            else
            {
                return;
            }
            ComboBox comboBox = UIHelper.FindChild<ComboBox>(Application.Current.MainWindow, nameComboBox + "ComboBox");
            comboBox.SelectedItem = dataParagraph;
            data.Paragraphs.Add(dataParagraph);
            lstTest.Items.Refresh();
        }
    }

    void ComboBoxIndexChange(object sender)
    {
        ComboBox comboBox = (ComboBox)sender;
        if (comboBox.SelectedIndex != -1)
        {
            for (int i = elementPanel.ColumnDefinitions.Count - 1; i < elementPanel.ColumnDefinitions.Count * 2 - 4; i++)
            {
                ComboBox comboBoxToDeselect;
                if (i != elementPanel.Children.IndexOf(comboBox))
                {
                    comboBoxToDeselect = (ComboBox)(elementPanel.Children[i]);
                    comboBoxToDeselect.SelectedIndex = -1;
                }
            }
            current = comboBox.SelectedItem as IParagraphData;
            DataComboBoxToRichBox(current);
        }
    }

    void DataComboBoxToRichBox(IParagraphData paragraphData)
    {
        TypeSubstitutionOn.Text = (string)FindResource(paragraphData.Type);

        HeaderSubstitution.Text = paragraphData.Description;
        richTextBoxSubstitution.SetText(paragraphData.Data);
        TypeUpdate(paragraphData);
    }

    void TypeUpdate(IParagraphData paragraphData)
    {
        if (paragraphData is ParagraphTable)
        {
            richTextBoxSubstitution.Visibility = Visibility.Collapsed;
            panelTable.Visibility = Visibility.Visible;
            HidePictureBox();
            ParagraphTable paragraphTable = paragraphData as ParagraphTable;
            countRows.Text = paragraphTable.TableData.Rows.ToString();
            countColumns.Text = paragraphTable.TableData.Columns.ToString();
            UpdateTable();
        }
        else
        {
            if (paragraphData is ParagraphH1 || paragraphData is ParagraphH2 || paragraphData is ParagraphList)
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Collapsed;
            }
            else if (paragraphData is ParagraphPicture || paragraphData is ParagraphCode)
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Visible;
            }
            panelTable.Visibility = Visibility.Collapsed;
            richTextBoxSubstitution.Visibility = Visibility.Visible;
            ShowPictureBox();
            ImageUpdate();
        }
    }

    void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox_SelectionChanged((ComboBox)sender);
    }

    void ComboBox_SelectionChanged(ComboBox comboBox)
    {
        if (comboBox.SelectedIndex != -1)
        {
            ComboBoxIndexChange(comboBox);
            TypeSubstitution.Visibility = Visibility.Collapsed;
            TypeSubstitutionOn.Visibility = Visibility.Visible;
            InfoDisplayedText(comboBox);
            ImageUpdate();
        }
        else
        {
            UnselectAll();
        }
    }

    void ComboBox_PreviewRightMouseDown(object sender, MouseButtonEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;
        if (IsComboBoxSelected())
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                RemoveParagraph(comboBox.SelectedItem as IParagraphData);
            }
        }
    }

    void RemoveParagraph(IParagraphData removable)
    {
        data.Paragraphs.Remove(removable);
        if (removable is ParagraphH1)
        {
            viewModel.H1P.Remove(removable as ParagraphH1);
        }
        else if (removable is ParagraphH2)
        {
            viewModel.H2P.Remove(removable as ParagraphH2);
        }
        else if (removable is ParagraphList)
        {
            viewModel.LP.Remove(removable as ParagraphList);
        }
        else if (removable is ParagraphPicture)
        {
            viewModel.PP.Remove(removable as ParagraphPicture);
        }
        else if (removable is ParagraphTable)
        {
            viewModel.TP.Remove(removable as ParagraphTable);
        }
        else if (removable is ParagraphCode)
        {
            viewModel.CP.Remove(removable as ParagraphCode);
        }
    }


    void TypeSubstitution_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TypeSubstitution.SelectedIndex != -1)
        {
            if (!IsComboBoxSelected())
            {
                if (TypeSubstitution.SelectedIndex == 0)
                {
                    current = new ParagraphH1();
                }
                else if (TypeSubstitution.SelectedIndex == 1)
                {
                    current = new ParagraphH2();
                }
                else if (TypeSubstitution.SelectedIndex == 2)
                {
                    current = new ParagraphList();
                }
                else if (TypeSubstitution.SelectedIndex == 3)
                {
                    current = new ParagraphPicture();
                }
                else if (TypeSubstitution.SelectedIndex == 4)
                {
                    current = new ParagraphTable();
                }
                else if (TypeSubstitution.SelectedIndex == 5)
                {
                    current = new ParagraphCode();
                }
            }
            if (Properties.Settings.Default.AutoHeader)
            {
                HeaderSubstitution.Text = (string)FindResource(current.Type);
            }
            HeaderSubstitution.Visibility = current.DescriptionVisibility();
            TypeUpdate(current);
        }
        else
        {
            panelTable.Visibility = Visibility.Collapsed;
            richTextBoxSubstitution.Visibility = Visibility.Visible;
            ShowPictureBox();
            ImageUpdate();
        }

        if (IsValidDataEntry())
        {
            add.Visibility = Visibility.Visible;
        }
        else
        {
            add.Visibility = Visibility.Collapsed;
        }
    }

    bool SaveMainText()
    {
        if (current != null)
        {
            current.Data = richTextBoxSubstitution.GetText();
            HeaderSubstitution.Text = current.Description;
            return true;
        }
        return false;
    }

    bool SaveHeader()
    {
        if (current != null)
        {
            current.Description = HeaderSubstitution.Text;
            return true;
        }
        return false;
    }

    void HeaderSubstitution_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (IsComboBoxSelected() && !clearSubstitution)
        {
            SaveHeader();
        }
    }

    void RichTextBoxSubstitution_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (IsComboBoxSelected() && !clearSubstitution)
        {
            add.Visibility = Visibility.Collapsed;
            SaveMainText();
        }
        else if (IsValidDataEntry())
        {
            add.Visibility = Visibility.Visible;
        }
        else
        {
            add.Visibility = Visibility.Collapsed;
        }
        ImageUpdate();
    }

    void RefreshMenu(int difference)
    {
        menuLeftIndex += difference;
        for (int i = 0; i < elementPanel.ColumnDefinitions.Count - 1; i++)
        {
            if (i >= menuLeftIndex && i < menuLeftIndex + 4)
            {
                elementPanel.ColumnDefinitions[i].Width = new GridLength(23.5, GridUnitType.Star);
            }
            else
            {
                elementPanel.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Star);
            }
        }
        RefreshMenuArrows();
    }

    void RefreshMenuArrows()
    {
        if (menuLeftIndex == 1)
        {
            elementPanel.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
        }
        else
        {
            elementPanel.ColumnDefinitions[0].Width = new GridLength(3, GridUnitType.Star);
        }
        if (menuLeftIndex == elementPanel.ColumnDefinitions.Count - 5)
        {
            elementPanel.ColumnDefinitions[^1].Width = new GridLength(0, GridUnitType.Star);
        }
        else
        {
            elementPanel.ColumnDefinitions[^1].Width = new GridLength(3, GridUnitType.Star);
        }
    }

    void ButtonBack_Click(object sender, RoutedEventArgs e)
    {
        if (menuLeftIndex != 1)
        {
            RefreshMenu(-1);
        }
    }

    void ButtonForward_Click(object sender, RoutedEventArgs e)
    {
        if (menuLeftIndex != elementPanel.ColumnDefinitions.Count - 5)
        {
            RefreshMenu(1);
        }
    }

    // Таблица
    void CountRows_TextChanged(object sender, TextChangedEventArgs e)
    {
        ParagraphTable paragraphTable = current as ParagraphTable;
        int rows = paragraphTable.TableData.Rows;
        CountRowOrColumn(countRows, ref rows);
        gridTable.RowDefinitions.Clear();
        for (int i = 0; i < rows; i++)
        {
            gridTable.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100 / rows, type: GridUnitType.Star) });
        }
        paragraphTable.TableData.Rows = rows;
        UpdateTable();
    }

    void CountColumns_TextChanged(object sender, TextChangedEventArgs e)
    {
        ParagraphTable paragraphTable = current as ParagraphTable;
        int columns = paragraphTable.TableData.Columns;
        CountRowOrColumn(countColumns, ref columns);
        gridTable.ColumnDefinitions.Clear();
        for (int i = 0; i < columns; i++)
        {
            gridTable.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100 / columns, type: GridUnitType.Star) });
        }
        paragraphTable.TableData.Columns = columns;
        UpdateTable();
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
        ParagraphTable paragraphTable = current as ParagraphTable;
        gridTable.Children.Clear();
        for (int i = 0; i < paragraphTable.TableData.Rows; i++)
        {
            for (int f = 0; f < paragraphTable.TableData.Columns; f++)
            {
                TextBox textBox = new()
                {
                    Text = paragraphTable.TableData.DataTable[i, f]
                };
                textBox.TextChanged += Cell_TextChanged;
                gridTable.Children.Add(textBox);
                Grid.SetColumn(textBox, f);
                Grid.SetRow(textBox, i);
            }
        }
    }

    void Cell_TextChanged(object sender, TextChangedEventArgs e)
    {
        ParagraphTable paragraphTable = current as ParagraphTable;
        TextBox textBox = (TextBox)sender;
        int row = Grid.GetRow(textBox);
        int column = Grid.GetColumn(textBox);
        paragraphTable.TableData.DataTable[row, column] = textBox.Text;
    }

    //Drag Drop

    bool EnableDragDrop()
    {
        if (current != null)
        {
            if (current is ParagraphH1 || current is ParagraphH2 || current is ParagraphList || current is ParagraphTable)
            {
                return false;
            }
        }
        return true;
    }

    void Win_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        e.Handled = true;

        if (EnableDragDrop())
        {
            singlePB.Visibility = Visibility.Collapsed;
            Color additional = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.AdditionalColor);
            codeRight.Fill = new SolidColorBrush(additional);
            pictureLeft.Fill = new SolidColorBrush(additional);
            textLeft.Foreground = new SolidColorBrush(Colors.Black);
            textRight.Foreground = new SolidColorBrush(Colors.Black);
        }
    }

    void Win_DragOver(object sender, DragEventArgs e)
    {

        e.Effects = DragDropEffects.None;
        e.Handled = true;
        if (EnableDragDrop())
        {
            singlePB.Visibility = Visibility.Collapsed;
            Color additional = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.AdditionalColor);
            codeRight.Fill = new SolidColorBrush(additional);
            pictureLeft.Fill = new SolidColorBrush(additional);
            textLeft.Foreground = new SolidColorBrush(Colors.Black);
            textRight.Foreground = new SolidColorBrush(Colors.Black);
        }
    }

    void Win_DragLeave(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
        if (EnableDragDrop())
        {
            singlePB.Visibility = Visibility.Visible;
            ImageUpdate();
        }
    }

    void PictureBox_DragEnter(object sender, DragEventArgs e)
    {
        if (EnableDragDrop())
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            singlePB.Visibility = Visibility.Collapsed;
        }
    }

    void PictureBox_DragOver(object sender, DragEventArgs e)
    {
        if (EnableDragDrop())
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            Color additional = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.AdditionalColor);
            if (e.GetPosition(pictureBox).X < pictureBox.RenderSize.Width / 2)
            {
                codeRight.Fill = new SolidColorBrush(additional);
                textRight.Foreground = new SolidColorBrush(Colors.Black);
                pictureLeft.Fill = new SolidColorBrush(Colors.Black);
                textLeft.Foreground = new SolidColorBrush(Colors.White);
            }
            else if (e.GetPosition(pictureBox).X > pictureBox.RenderSize.Width / 2)
            {
                codeRight.Fill = new SolidColorBrush(Colors.Black);
                textRight.Foreground = new SolidColorBrush(Colors.White);
                pictureLeft.Fill = new SolidColorBrush(additional);
                textLeft.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                codeRight.Fill = new SolidColorBrush(additional);
                textRight.Foreground = new SolidColorBrush(Colors.Black);
                pictureLeft.Fill = new SolidColorBrush(additional);
                textLeft.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }

    void PictureBox_DragLeave(object sender, DragEventArgs e)
    {
        if (EnableDragDrop())
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            if (e.GetPosition(pictureBox).X < 0 || e.GetPosition(pictureBox).X > pictureBox.RenderSize.Width || e.GetPosition(pictureBox).Y < 0 || e.GetPosition(pictureBox).Y > pictureBox.RenderSize.Height)
            {
                Color additional = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.AdditionalColor);
                singlePB.Visibility = Visibility.Visible;
                codeRight.Fill = new SolidColorBrush(additional);
                pictureLeft.Fill = new SolidColorBrush(additional);
                textLeft.Foreground = new SolidColorBrush(Colors.Black);
                textRight.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                ImageUpdate();
            }
        }
    }

    void PictureBox_Drop(object sender, DragEventArgs e)
    {
        if (EnableDragDrop())
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string path = (data as string[])[0];
                if (path.Length > 0)
                {
                    string nameFile = path.Split('\\').Last();
                    nameFile = nameFile[..nameFile.LastIndexOf('.')];
                    if (e.GetPosition(pictureBox).X < pictureBox.RenderSize.Width / 2)
                    {
                        TypeSubstitution.SelectedIndex = TypeIndex("p");
                        HeaderSubstitution.Text = nameFile;
                        richTextBoxSubstitution.SetText(path);
                    }
                    else
                    {
                        TypeSubstitution.SelectedIndex = TypeIndex("c");
                        HeaderSubstitution.Text = nameFile;
                        richTextBoxSubstitution.SetText(path);
                    }
                }
            }
        }
        ImageUpdate();
    }

    int TypeIndex(string str)
    {
        if (str == "h1")
        {
            return 0;
        }
        else if (str == "h2")
        {
            return 1;
        }
        else if (str == "l")
        {
            return 2;
        }
        else if (str == "p")
        {
            return 3;
        }
        else if (str == "t")
        {
            return 4;
        }
        else if (str == "c")
        {
            return 5;
        }
        return -1;
    }

    // Верхнее меню

    async void Export_MI_Click(object sender, RoutedEventArgs e)
    {
        List<string> titleData = new();
        AddTitleData(ref titleData);
        bool exportPDFOn = ExportPDF.IsChecked;
        bool exportHTMLOn = ExportHTML.IsChecked;

        Report report = new();
        await Task.Run(() =>
            Report.Create(data, data.Properties.PageNumbers, data.Properties.TableOfContents, data.Properties.NumberHeading, titleData.ToArray(), exportPDFOn, exportHTMLOn));

        if (Properties.Settings.Default.CloseWindow)
        {
            UIHelper.WindowClose();
        }
    }

    void AddTitleData(ref List<string> titleData)
    {
        titleData = new List<string>();
        foreach (UIElement control in titlePanel.Children)
        {
            if (control.GetType().ToString() != "System.Windows.Controls.TextBlock")
            {
                if (control.GetType().ToString() == "System.Windows.Controls.TextBox")
                {
                    TextBox f = (TextBox)control;
                    titleData.Add(f.Text);
                }
                else if (control.GetType().ToString() == "System.Windows.Controls.ComboBox")
                {
                    ComboBox c = (ComboBox)control;
                    titleData.Add(c.Text);
                }
            }
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
        file.NewFile(ref data, richTextBox, ref viewModel);
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
        file.OpenFile(fileName, ref data, ref viewModel);
        HeaderUpdate();
        viewModel.Properties = data.Properties;
        viewModel.Title = data.Title;

        if (viewModel.TextOpen)
        {
            if (lstTest.Items.Count > 0)
            {
                lstTest.SelectedIndex = 0;
            }
        }

        lstTest.Items.Refresh();
    }

    void WindowBinding_Save(object sender, ExecutedRoutedEventArgs e)
    {
        file.Save(ref data, ref viewModel);
    }
    void WindowBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
    {
        file.SaveAs(ref data, ref viewModel);
    }

    void WindowBinding_Quit(object sender, ExecutedRoutedEventArgs e)
    {
        UIHelper.WindowClose();
    }

    void View_MI_Click(object sender, RoutedEventArgs e)
    {
        MenuItem ClickMenuItem = (MenuItem)sender;
        if (TitlePageMI.IsChecked)
        {
            HideElements(TitlePageMI);
            ShowElements(ClickMenuItem);
        }
        else if (SubstitutionMI.IsChecked)
        {
            HideElements(SubstitutionMI);
            ShowElements(ClickMenuItem);
        }
        else if (TextMI.IsChecked)
        {
            HideElements(TextMI);
            ShowElements(ClickMenuItem);
        }
    }

    void DocumentType_MI_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        DocumentTypeChange(menuItem);
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
            SwitchPanel.Visibility = Visibility.Collapsed;
            TextHeader((string)FindResource("DefaultDocument"));
            data.Type = TypeDocument.DefaultDocument;
        }
        else
        {
            SwitchPanel.Visibility = Visibility.Visible;
            if (LaboratoryWorkMI.IsChecked)
            {
                data.Type = TypeDocument.LaboratoryWork;
                TextHeader((string)FindResource("LaboratoryWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (PracticeWorkMI.IsChecked)
            {
                data.Type = TypeDocument.PracticeWork;
                TextHeader((string)FindResource("PracticeWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (CourseworkMI.IsChecked)
            {
                data.Type = TypeDocument.Coursework;
                TextHeader((string)FindResource("Coursework"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 4.1 5.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ControlWorkMI.IsChecked)
            {
                data.Type = TypeDocument.ControlWork;
                TextHeader((string)FindResource("ControlWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ReferatMI.IsChecked)
            {
                data.Type = TypeDocument.Referat;
                TextHeader((string)FindResource("Referat"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 0.3 1.3 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (DiplomaMI.IsChecked)
            {
                data.Type = TypeDocument.Diploma;
                TextHeader((string)FindResource("DiplomaWork"));
                TitleElements.ShowTitleElems(titlePanel, "");
            }
            else if (VKRMI.IsChecked)
            {
                data.Type = TypeDocument.VKR;
                TextHeader((string)FindResource("VKR"));
                TitleElements.ShowTitleElems(titlePanel, "");
            }
        }
        HideTitlePanel();
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

        if (DefaultDocumentMI.IsChecked)
        {
            SwitchPanel.Visibility = Visibility.Collapsed;
            TextHeader((string)FindResource("DefaultDocument"));
            data.Type = TypeDocument.DefaultDocument;
        }
        else
        {
            SwitchPanel.Visibility = Visibility.Visible;
            if (LaboratoryWorkMI.IsChecked)
            {
                data.Type = TypeDocument.LaboratoryWork;
                TextHeader((string)FindResource("LaboratoryWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (PracticeWorkMI.IsChecked)
            {
                data.Type = TypeDocument.PracticeWork;
                TextHeader((string)FindResource("PracticeWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (CourseworkMI.IsChecked)
            {
                data.Type = TypeDocument.Coursework;
                TextHeader((string)FindResource("Coursework"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 4.1 5.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ControlWorkMI.IsChecked)
            {
                data.Type = TypeDocument.ControlWork;
                TextHeader((string)FindResource("ControlWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ReferatMI.IsChecked)
            {
                data.Type = TypeDocument.Referat;
                TextHeader((string)FindResource("Referat"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 0.3 1.3 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (DiplomaMI.IsChecked)
            {
                data.Type = TypeDocument.Diploma;
                TextHeader((string)FindResource("DiplomaWork"));
                TitleElements.ShowTitleElems(titlePanel, "");
            }
            else if (VKRMI.IsChecked)
            {
                data.Type = TypeDocument.VKR;
                TextHeader((string)FindResource("VKR"));
                TitleElements.ShowTitleElems(titlePanel, "");
            }
        }
        HideTitlePanel();
    }

    void TextHeader(string type)
    {
        if (file.SavePathExists())
        {
            win.Title = "WordKiller — " + type;
        }
    }

    void NumberHeading_MI_Click(object sender, RoutedEventArgs e)
    {
        viewModel.Properties.NumberHeading = !viewModel.Properties.NumberHeading;
        ImageUpdate();
    }

    void ChangeUser_MI_Click(object sender, RoutedEventArgs e)
    {
        new ChangeUser<ParagraphPicture>().Start(viewModel.PP);
        new ChangeUser<ParagraphCode>().Start(viewModel.CP);
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

    //для отображения чего либо

    void ShowElements(MenuItem MenuItem)
    {
        if (MenuItem != null)
        {
            MenuItem.IsChecked = true;
        }
        UpdateSize(MenuItem);
        if (MenuItem == TitlePageMI)
        {
            titlePanel.Visibility = Visibility.Visible;
            if (DownPanelMI == SubstitutionMI)
            {
                SwitchPanel.Content = (string)FindResource("ToSubstitutions");
            }
            else
            {
                SwitchPanel.Content = (string)FindResource("ToText");
            }
        }
        else
        {
            SwitchPanel.Content = (string)FindResource("ToTitle");
            DownPanelMI = MenuItem;
            downPanel.Visibility = Visibility.Visible;
            if (MenuItem == SubstitutionMI)
            {
                if (IsValidDataEntry())
                {
                    add.Visibility = Visibility.Visible;
                }
                additionalPanel.ColumnDefinitions[1].Width = new GridLength(38, GridUnitType.Star);
                additionalPanel.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
                additionalPanel.ColumnDefinitions[3].Width = new GridLength(20, GridUnitType.Star);
                elementPanel.Visibility = Visibility.Visible;
                toText.Content = (string)FindResource("ToText");
                additionalPanel.Margin = new Thickness(5, 0, 5, 5);
                elementTBl.Visibility = Visibility.Visible;
                Substitution.Visibility = Visibility.Visible;
                richTextBox.Focus();
                Displayed((string)FindResource("Something"));
                SubstitutionPanelRTB.Visibility = Visibility.Visible;
                ImageUpdate();
            }
            else if (MenuItem == TextMI)
            {
                additionalPanel.Margin = new Thickness(5, 2.5, 5, 2.5);
                additionalPanel.ColumnDefinitions[1].Width = new GridLength(22, GridUnitType.Star);
                additionalPanel.ColumnDefinitions[2].Width = new GridLength(36, GridUnitType.Star);
                additionalPanel.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Star);
                richTextBox.Visibility = Visibility.Visible;
                cursorLocationTB.Visibility = Visibility.Visible;
                toText.Content = (string)FindResource("ToSubstitutions");
                elementTBl.Visibility = Visibility.Collapsed;
                TextPanelRTB.Visibility = Visibility.Visible;
                richTextBox.Focus();
                lstTest.Items.Refresh();
                if (lstTest.Items.Count > 0)
                {
                    lstTest.SelectedIndex = 0;
                }

                richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
            }
        }
    }

    void HideElements(MenuItem MenuItem)
    {
        if (MenuItem == TitlePageMI)
        {
            titlePanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            downPanel.Visibility = Visibility.Collapsed;
            if (MenuItem == SubstitutionMI)
            {
                UnselectComboBoxes();
                Substitution.Visibility = Visibility.Visible;
                add.Visibility = Visibility.Collapsed;
                elementPanel.Visibility = Visibility.Collapsed;
                SubstitutionPanelRTB.Visibility = Visibility.Collapsed;
            }
            else if (MenuItem == TextMI)
            {
                for (int i = elementPanel.ColumnDefinitions.Count - 1; i < elementPanel.Children.Count - 1; i++)
                {
                    ComboBox cmbBox = (ComboBox)elementPanel.Children[i];
                    cmbBox.Items.Refresh();
                }
                lstTest.SelectedIndex = -1;
                richTextBox.Document.Blocks.Clear();
                richTextBox.Visibility = Visibility.Collapsed;
                cursorLocationTB.Visibility = Visibility.Collapsed;
                TextPanelRTB.Visibility = Visibility.Collapsed;
            }
        }
        MenuItem.IsChecked = false;
    }

    void HideTitlePanel()
    {
        if (TitlePageMI.IsChecked && DefaultDocumentMI.IsChecked)
        {
            HideElements(TitlePageMI);
            ShowElements(DownPanelMI);
        }
        TitlePageMI.Visibility = DefaultDocumentMI.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
    }

    void UpdateSize(MenuItem MenuItem)
    {
        if (MenuItem == TitlePageMI)
        {
            MainPanel.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Star);
            MainPanel.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Pixel);
            win.MinWidth = 710;
            win.MinHeight = 340;
            win.Height = win.MinHeight;
            win.Width = win.MinWidth;
        }
        else
        {
            MainPanel.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
            MainPanel.RowDefinitions[2].Height = new GridLength(100, GridUnitType.Star);
            win.MinWidth = 710;
            win.MinHeight = 430;
        }
    }

    void Displayed(string something)
    {
        viewModel.Displayed = (string)FindResource("Displayed") + ": " + something;
    }

    void InfoDisplayedText(ComboBox sender)
    {
        int i = elementPanel.Children.IndexOf(sender) - 1 - (elementPanel.ColumnDefinitions.Count - 2);
        Displayed(menuNames[i] + " - " + (sender.Items.IndexOf(sender.SelectedItem) + 1).ToString());
    }

    void ToText_Click(object sender, RoutedEventArgs e)
    {
        if (DownPanelMI == SubstitutionMI)
        {
            HideElements(SubstitutionMI);
            ShowElements(TextMI);
        }
        else
        {
            HideElements(TextMI);
            ShowElements(SubstitutionMI);
        }
    }

    // Главный RichTextBox

    void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (lstTest.SelectedIndex != -1)
        {
            data.Paragraphs.ElementAt(lstTest.SelectedIndex).Data = richTextBox.GetText();
            lstTest.Items.Refresh();
        }
    }

    void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        richTextBox.KeyProcessing(e);
    }

    void InitListBox()
    {
        Style itemContainerStyle = new(typeof(ListBoxItem));
        itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
        itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(LstTest_PreviewMouseLeftButtonDown)));
        itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(LstTest_Drop)));
        lstTest.ItemContainerStyle = itemContainerStyle;
    }

    void LstTest_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        //if (sender is ListBoxItem)
        {
            ListBoxItem draggedItem = sender as ListBoxItem;
            DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
            draggedItem.IsSelected = true;
        }
    }

    void LstTest_Drop(object sender, DragEventArgs e)
    {
        IParagraphData droppedData = e.Data.GetData(e.Data.GetFormats()[0]) as IParagraphData;
        IParagraphData target = ((ListBoxItem)(sender)).DataContext as IParagraphData;
        int removedIdx = lstTest.Items.IndexOf(droppedData);
        int targetIdx = lstTest.Items.IndexOf(target);

        if (removedIdx < targetIdx)
        {
            data.Paragraphs.Insert(targetIdx + 1, droppedData);
            data.Paragraphs.RemoveAt(removedIdx);
        }
        else
        {
            int remIdx = removedIdx + 1;
            if (data.Paragraphs.Count + 1 > remIdx)
            {
                data.Paragraphs.Insert(targetIdx, droppedData);
                data.Paragraphs.RemoveAt(remIdx);
            }
        }
        lstTest.Items.Refresh();
    }

    void LstTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstTest.SelectedIndex != -1)
        {
            int targetIdx = lstTest.SelectedIndex;
            richTextBox.SetText(data.Paragraphs.ElementAt(targetIdx).Data);
        }
    }

    void NewText_Click(object sender, RoutedEventArgs e)
    {
        data.Paragraphs.Add(new ParagraphText(""));
        lstTest.Items.Refresh();
    }

    void LstTest_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.All;
        e.Handled = true;
    }
    void LstTest_MouseDown(object sender, MouseButtonEventArgs e)
    {
        lstTest.UnselectAll();
    }

    void ContextMenuDelete_Click(object sender, RoutedEventArgs e)
    {
        if (lstTest.SelectedIndex == -1) return;
        RemoveParagraph(lstTest.SelectedItem as IParagraphData);
        lstTest.Items.Refresh();
    }


    void ContextMenuInsertAfter_Click(object sender, RoutedEventArgs e)
    {
        if (lstTest.SelectedIndex == -1) return;
        data.Paragraphs.Insert(lstTest.SelectedIndex + 1, new ParagraphText(""));
        lstTest.Items.Refresh();
    }

    void ContextMenuInsertBefore_Click(object sender, RoutedEventArgs e)
    {
        if (lstTest.SelectedIndex == -1) return;
        data.Paragraphs.Insert(lstTest.SelectedIndex, new ParagraphText(""));
        lstTest.Items.Refresh();
    }

    // PictureBox

    void ShowPictureBox()
    {
        pictureBox.Visibility = Visibility.Visible;
        SubstitutionPanelRTB.ColumnDefinitions[1].Width = new GridLength(40, GridUnitType.Star);
    }

    void HidePictureBox()
    {
        pictureBox.Visibility = Visibility.Collapsed;
        SubstitutionPanelRTB.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
    }

    void ImageUpdate()
    {
        if (current != null)
        {
            singlePB.Visibility = Visibility.Visible;
            if (current is ParagraphH1)
            {
                int index = h1ComboBox.SelectedIndex;
                if (index != -1)
                {
                    if (data.Properties.NumberHeading)
                    {
                        DrawText((index + 1).ToString() + " " + viewModel.H1P[index].Data.ToUpper());
                    }
                    else
                    {
                        DrawText(viewModel.H1P[index].Data.ToUpper());
                    }
                }
                else
                {
                    DrawText(FindResource("Header").ToString().ToUpperInvariant());
                }
            }
            else if (current is ParagraphH2)
            {
                int index = h2ComboBox.SelectedIndex;
                if (index != -1)
                {
                    if (data.Properties.NumberHeading)
                    {
                        DrawText("H1." + (index + 1).ToString() + " " + viewModel.H2P[index].Data);
                    }
                    else
                    {
                        DrawText(viewModel.H2P[index].Data);
                    }
                }
                else
                {
                    DrawText((string)FindResource("SubHeader"));
                }
            }
            else if (current is ParagraphList)
            {
                int index = lComboBox.SelectedIndex;
                if (index != -1)
                {
                    string text = string.Empty;
                    int i = 1, level = 0;
                    string before = "0";
                    foreach (string line in viewModel.LP[index].Data.Split('\n'))
                    {
                        int current = Level(line);
                        if (current == 0)
                        {
                            int dot = before.IndexOf('.');
                            if (before.Length > 0)
                            {
                                if (dot != -1)
                                {
                                    if (before.Length > dot)
                                    {
                                        i = int.Parse(before[..dot]);
                                        i++;
                                    }
                                }
                                else
                                {
                                    i = int.Parse(before);
                                    i++;
                                }
                            }
                            text += i + ") " + line + '\n';
                            before = i.ToString();
                            i++;
                        }
                        else
                        {
                            int start = StartLine(line, current);
                            text += NumberOfTabs(current);
                            if (level == current)
                            {
                                string beforem2 = before;
                                if (before.Length > 1)
                                {
                                    beforem2 = before[..^2];
                                }
                                before = beforem2 + "." + i;
                                text += string.Concat(before, ") ", line.AsSpan(start), "\n");
                                i++;
                            }
                            else if (level > current)
                            {
                                string[] numbers = before.Split('.');
                                int endBefore = 0;
                                if (numbers.Length > current)
                                {
                                    i = int.Parse(numbers[current]);
                                    for (int j = 0; j < numbers.Length; j++)
                                    {
                                        if (j == current)
                                        {
                                            break;
                                        }
                                        if (j != 0)
                                        {
                                            endBefore++;
                                        }
                                        endBefore += numbers[j].Length;
                                    }
                                }
                                else
                                {
                                    i = 999;
                                }
                                i++;
                                before = before[..endBefore];
                                text += before + "." + i + ") " + line[start..] + '\n';
                            }
                            else
                            {

                                i = 1;
                                before = before + "." + OverSize(level, current) + i;
                                text += string.Concat(before, ") ", line.AsSpan(start), "\n");
                                i++;
                            }
                        }
                        level = current;
                    }
                    DrawText(text, TextAlignment.Left, 14);
                }
                else
                {
                    DrawText((string)FindResource("List"));
                }
            }
            else if (current is ParagraphPicture)
            {
                string path;
                if (pComboBox.SelectedIndex == -1)
                {
                    path = richTextBoxSubstitution.GetText();
                }
                else
                {
                    path = viewModel.PP[pComboBox.SelectedIndex].Data;
                }
                if (string.IsNullOrWhiteSpace(path))
                {
                    ShowIconPicture((string)FindResource("Unidentified"));
                }
                else if (File.Exists(path))
                {
                    ShowImage(path);
                }
                else
                {
                    ShowIconPicture((string)FindResource("NotFound"));
                }
            }
            else if (current is ParagraphCode)
            {
                string path;
                if (cComboBox.SelectedIndex == -1)
                {
                    path = richTextBoxSubstitution.GetText();
                }
                else
                {
                    path = viewModel.CP[cComboBox.SelectedIndex].Data;
                }

                if (string.IsNullOrWhiteSpace(path))
                {
                    ShowCode((string)FindResource("Unidentified"));
                }
                else if (File.Exists(path))
                {
                    ShowCode(path);
                }
                else
                {
                    ShowCode((string)FindResource("NotFound"));
                }
            }
            else
            {
                pictureBox.Visibility = Visibility.Visible;
                ShowDragDrop();
            }
        }
        else
        {
            pictureBox.Visibility = Visibility.Visible;
            ShowDragDrop();
        }
    }

    void ShowDragDrop()
    {
        mainImage.Visibility = Visibility.Collapsed;
        dragDropImage.Visibility = Visibility.Visible;
        mainImage.Margin = new Thickness(0, 0, 0, 0);
    }

    void ShowIconPicture(string text)
    {
        mainText.Visibility = Visibility.Visible;
        mainImage.Visibility = Visibility.Visible;
        dragDropImage.Visibility = Visibility.Collapsed;
        mainImage.Width = 220;
        mainImage.Height = 100;
        mainImage.Margin = new Thickness(0, 0, 0, 30);
        mainImage.Source = UIHelper.GetImage("pack://application:,,,/Resources/Pictures/Picture.png");
        mainText.Margin = new Thickness(0, 110, 0, 0);
        mainText.Text = text;
    }

    void ShowCode(string text)
    {
        mainText.Visibility = Visibility.Visible;
        mainImage.Visibility = Visibility.Visible;
        dragDropImage.Visibility = Visibility.Collapsed;
        mainImage.Width = 115;
        mainImage.Height = 160;
        mainImage.Margin = new Thickness(0, 0, 0, 30);
        mainImage.Source = UIHelper.GetImage("pack://application:,,,/Resources/Pictures/Code.png");
        mainText.Margin = new Thickness(0, 165, 0, 0);
        mainText.Text = text;
    }

    void ShowImage(string path)
    {
        mainText.Visibility = Visibility.Collapsed;
        mainImage.Visibility = Visibility.Visible;
        dragDropImage.Visibility = Visibility.Collapsed;
        mainImage.Width = Double.NaN;
        mainImage.Height = Double.NaN;
        mainImage.Margin = new Thickness(0, 0, 0, 0);
        var uriSource = new Uri(path, UriKind.Absolute);
        try
        {
            mainImage.Source = new BitmapImage(uriSource);
        }
        catch
        {
        }
        mainText.Margin = new Thickness(0, 0, 0, 0);
    }

    void DrawText(string text, TextAlignment textAlignment = TextAlignment.Center, double fontSize = 20)
    {
        mainText.Visibility = Visibility.Visible;
        mainText.Text = text;
        mainText.TextAlignment = textAlignment;
        //mainText.FontSize = fontSize;
        mainText.Margin = new Thickness(0, 0, 0, 0);
    }

    public static int Level(string str)
    {
        int level = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '!')
            {
                level++;
            }
            else
            {
                break;
            }
        }
        return level;
    }

    string OverSize(int before, int after)
    {
        string overSize = string.Empty;
        for (; before < after - 1; before++)
        {
            overSize += "1.";
        }
        return overSize;
    }

    public static int StartLine(string line, int current)
    {
        int start = 1;
        if (line.Length < current)
        {
            start += current;
        }
        else
        {
            start = current;
        }
        return start;
    }

    string NumberOfTabs(int numberOfTabs)
    {
        string tabs = string.Empty;
        for (int i = 0; i < numberOfTabs; i++)
        {
            tabs += "  ";//\t-лсишком много
        }
        return tabs;
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
        MenuPanel.Visibility = Visibility.Collapsed;
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
    }

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
        richTextBoxSubstitution.SpellCheck.IsEnabled = Properties.Settings.Default.SyntaxChecking;
    }

    void Encoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Properties.Settings.Default.NumberEncoding = Encoding.SelectedIndex;
        Properties.Settings.Default.Save();
    }

    void OpenSettings_Click(object sender, RoutedEventArgs e)
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
        SettingsPanel.Visibility = Visibility.Visible;
        MenuPanel.Visibility = Visibility.Collapsed;
        ParentPanel.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
        ParentPanel.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Star);
        OpenGeneralisSetiings();
    }

    void ExitSettings(object sender, RoutedEventArgs e)
    {
        ShowElements(prevSettings);
        SettingsPanel.Visibility = Visibility.Collapsed;
        MenuPanel.Visibility = Visibility.Visible;
        ParentPanel.RowDefinitions[0].Height = new GridLength(100, GridUnitType.Star);
        ParentPanel.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
    }

    void SwitchPanel_Click(object sender, RoutedEventArgs e)
    {
        if (TitlePageMI.IsChecked)
        {
            ShowElements(DownPanelMI);
            HideElements(TitlePageMI);
        }
        else
        {
            ShowElements(TitlePageMI);
            HideElements(DownPanelMI);
        }
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
                    MessageBox.Show((string)FindResource("Error2"), (string)FindResource("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
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
                    MessageBox.Show((string)FindResource("Error2"), (string)FindResource("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
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
        fontSize.Value = 6;
        fontSizeRTB.Value = 8;
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
    }

    void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.SelectCulture(language.SelectedIndex);
        Properties.Settings.Default.Language = language.SelectedIndex;
        Properties.Settings.Default.Save();
    }
}
