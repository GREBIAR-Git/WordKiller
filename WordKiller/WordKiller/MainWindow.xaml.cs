﻿using Microsoft.Win32;
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WordKiller;

public partial class MainWindow : Window
{
    int menuLeftIndex;
    readonly string[] menuNames;
    MenuItem DownPanelMI;
    DataComboBox data;
    TypeDocument typeDocument;
    MenuItem prevSettings;

    readonly ViewModel viewModel;

    readonly WordKillerFile file;

    bool clearSubstitution = false;

    TablesData tablesData = new TablesData();

    public MainWindow(string[] args)
    {
        //args = new string[] { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\1.wkr" };
        viewModel = new()
        {
            Displayed = (string)FindResource("Something"),
            FontSize = Properties.Settings.Default.FontSize,
            MainColor = Properties.Settings.Default.MainColor,
            AdditionalColor = Properties.Settings.Default.AdditionalColor,
            AlternativeColor = Properties.Settings.Default.AlternativeColor,
            HoverColor = Properties.Settings.Default.HoverColor,
            WinTitle = "WordKiller",
            TitleYear = "202",
            TitleOpen = true,
        };
        InitializeComponent();
        if (!DocxExport.WordInstall())
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
        DataContext = viewModel;
        App.SelectCulture(Properties.Settings.Default.Language);
        file = new(saveLogo, typeMenuItem.Items, titlePanel.Children, elementPanel, richTextBox, this);
        TitleElements.SaveTitleUIElements(titlePanel);
        DownPanelMI = SubstitutionMI;
        prevSettings = DownPanelMI;
        TextHeaderUpdate();
        RefreshMenu(1);
        menuNames = ComboBoxSetup();
        data = new DataComboBox(h1ComboBox, h2ComboBox, lComboBox, pComboBox, tComboBox, cComboBox);
        UpdateCheckSyntax();
        tablesData.InitTable();
        if (args.Length > 0)
        {
            if (args[0].EndsWith(Properties.Settings.Default.Extension) && File.Exists(args[0]))
            {
                file.OpenFile(args[0], viewModel, ref data, ref tablesData);
            }
            else
            {
                throw new Exception((string)FindResource("Error1"));
            }
        }
        InitSetting();
        UpdateHeadersSubstitution();
        //OpenDrawing();
    }

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



    public static string FindResourse(string key)
    {
        return Application.Current.FindResource(key) as string;
    }

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

    /*async void test()
    {
    //7 ипаит //498 //7286-92PG
        OrelUniverAPI.Result<Employee>? result23456 = await OrelUniverAPI.EmployeeGetListAsync();//+
        OrelUniverAPI.Result<EmployeeMax>? result1 = await OrelUniverAPI.EmployeeGetAsync("3686");//+

        OrelUniverAPI.Result<Division>? result3 = await OrelUniverAPI.GetDivisionsForEmployeesAsync();//+
        OrelUniverAPI.Result<EmployeeMin>? result2 = await OrelUniverAPI.ScheduleGetEmployeesAsync("7", "498");//
        OrelUniverAPI.Result<Subdivision>? result16 = await OrelUniverAPI.ScheduleGetSubdivisionsAsync("7");//
        OrelUniverAPI.Result<Cours>? result12 = await OrelUniverAPI.ScheduleGetCourseAsync("7");//
        OrelUniverAPI.Result<OrelUniverEmbeddedAPI.Group>? result15 = await OrelUniverAPI.ScheduleGetGroupsAsync("7", "4");//

        OrelUniverAPI.Result<Division>? result6 = await OrelUniverAPI.GetDivisionsForStudentsAsync();//+
        OrelUniverAPI.Result<Building>? result7 = await OrelUniverAPI.ScheduleGetBuildingsAsync();//+
        OrelUniverAPI.Result<Cabinets>? result11 = await OrelUniverAPI.ScheduleGetCabinetsAsync("12");//+

        OrelUniverAPI.Result<Schedule>? result4 = await OrelUniverAPI.ScheduleGetByEmployeeAsync(14, 10, 2022, "3686");//
        OrelUniverAPI.Result<Schedule>? result8 = await OrelUniverAPI.ScheduleGetByCabinetAsync(14, 10, 2022, "12", "715");//
        OrelUniverAPI.Result<Schedule>? result10 = await OrelUniverAPI.ScheduleGetByGroupAsync(14, 10, 2022, "7286");//
        OrelUniverAPI.Result<Schedule>? result101 = await OrelUniverAPI.ScheduleGetByGroupAsync(15, 10, 2022, "7286");//
        OrelUniverAPI.Result<Schedule>? result102 = await OrelUniverAPI.ScheduleGetByGroupAsync(16, 10, 2022, "7286");//
        OrelUniverAPI.Result<Schedule>? result103 = await OrelUniverAPI.ScheduleGetByGroupAsync(17, 10, 2022, "7286");//
        OrelUniverAPI.Result<Schedule>? result104 = await OrelUniverAPI.ScheduleGetByGroupAsync(18, 10, 2022, "7286");//

        OrelUniverAPI.Result<Exam>? result13 = await OrelUniverAPI.ScheduleGetExamsForEmployeeAsync("498");//
        OrelUniverAPI.Result<Exam>? result14 = await OrelUniverAPI.ScheduleGetExamsForStudentAsync("7286");//
    }*/

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

    bool ComboBoxSelected()
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

    void WindowBinding_New(object sender, ExecutedRoutedEventArgs e)
    {
        file.NewFile(viewModel, ref data, ref menuLeftIndex, tablesData);
        if (viewModel.TextOpen)
        {
            UpdateTypeButton();
        }
    }

    void WindowBinding_Open(object sender, ExecutedRoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "wordkiller file (*" + Properties.Settings.Default.Extension + ")|*" + Properties.Settings.Default.Extension + "|All files (*.*)|*.*"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            file.OpenFile(openFileDialog.FileName, viewModel, ref data, ref tablesData);
        }
    }
    void WindowBinding_Save(object sender, ExecutedRoutedEventArgs e)
    {
        file.Save(viewModel, data, tablesData);
    }
    void WindowBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
    {
        file.SaveAs(viewModel, data, tablesData);
    }

    void WindowBinding_Quit(object sender, ExecutedRoutedEventArgs e)
    {
        WindowClose();
    }

    void WindowClose()
    {
        Application.Current.Shutdown();
    }

    void Work_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        WorkChange(menuItem);
    }

    public void WorkChange(MenuItem menuItem)
    {
        if (menuItem.IsChecked)
        {
            return;
        }
        DefaultDocumentMI.IsChecked = false;
        LabMI.IsChecked = false;
        PracticeMI.IsChecked = false;
        CourseworkMI.IsChecked = false;
        ControlWorkMI.IsChecked = false;
        RefMI.IsChecked = false;
        DiplomMI.IsChecked = false;
        VKRMI.IsChecked = false;
        menuItem.IsChecked = true;
        TextHeaderUpdate();
    }

    void TextHeaderUpdate()
    {
        if (DefaultDocumentMI.IsChecked)
        {
            SwitchPanel.Visibility = Visibility.Collapsed;
            TextHeader((string)FindResource("DefaultDocument"));
            typeDocument = TypeDocument.DefaultDocument;
        }
        else
        {
            SwitchPanel.Visibility = Visibility.Visible;
            if (LabMI.IsChecked)
            {
                typeDocument = TypeDocument.LaboratoryWork;
                TextHeader((string)FindResource("LaboratoryWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (PracticeMI.IsChecked)
            {
                typeDocument = TypeDocument.PracticalWork;
                TextHeader((string)FindResource("PracticeWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (CourseworkMI.IsChecked)
            {
                typeDocument = TypeDocument.Coursework;
                TextHeader((string)FindResource("Coursework"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 4.1 5.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (ControlWorkMI.IsChecked)
            {
                typeDocument = TypeDocument.ControlWork;
                TextHeader((string)FindResource("ControlWork"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (RefMI.IsChecked)
            {
                typeDocument = TypeDocument.Referat;
                TextHeader((string)FindResource("Referat"));
                TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 0.3 1.3 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            }
            else if (DiplomMI.IsChecked)
            {
                typeDocument = TypeDocument.GraduateWork;
                TextHeader((string)FindResource("DiplomaWork"));
                TitleElements.ShowTitleElems(titlePanel, "");
            }
            else if (VKRMI.IsChecked)
            {
                typeDocument = TypeDocument.VKR;
                TextHeader((string)FindResource("VKR"));
                TitleElements.ShowTitleElems(titlePanel, "");
            }
        }
        HideTitlePanel();
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

    void TextHeader(string type)
    {
        if (file.SavePathExists())
        {
            win.Title = "WordKiller — " + type;
        }
    }

    void FacultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (professorComboBox != null)
        {
            UpdateProfessorComboBox();
        }
    }

    void UpdateProfessorComboBox()
    {
        string str = string.Empty;
        professorComboBox.Items.Clear();
        if (facultyComboBox.SelectedIndex == 0)
        {
            str = "Амелина О.В.!Артёмов А.В.!Валухов В.А.!Волков В.Н.!Гордиенко А.П.!Демидов А.В.!Загородних Н.А.!Захарова О.В.!Конюхова О.В.!Корнаева Е.П.!Королева А.К.!Короткий А.В.!Коськин А.В.!Кравцова Э.А.!Лукьянов П.В.!Лунёв Р.А.!Лыськов О.Э.!Машкова А.Л.!Митин А.А.!Новиков С.В.!Новикова Е.В.!Олькина Е.В.!Преснецова В.Ю.!Раков В.И.!Рыженков Д.В.!Савина О.А.!Санников Д.П.!Сезонов Д.С.!Соков О.А.!Стычук А.А.!Терентьев С.В.!Ужаринский А.Ю.!Фроленкова Л.Ю.!Фролов А.И.!Фролова В.А.!Чижов А.В.!Шатеев Р.В.";
        }
        else if (facultyComboBox.SelectedIndex == 1)
        {
            str = "Бондарева Л.А.!Дрёмин В.В.!Дунаев А.В.!Жидков А.В.!Козлов И.О.!Козлова Л.Д.!Марков В.В.!Матюхин С.И.!Незнанов А.И.!Подмастерьев К.В.!Секаева Ж.А.!Селихов А.В.!Углова Н.В.!Шуплецов В.В.!Яковенко М.В.";
        }
        else if (facultyComboBox.SelectedIndex == 2)
        {
            str = "Белевская Ю.А.!Ерёменко В.Т.!Мишин Д.С.!Пеньков Н.Г!Савва Ю.Б.!Фисун А.П.";
        }
        else if (facultyComboBox.SelectedIndex == 3)
        {
            str = "Батуров Д.П.!Гордон В.А.!Кирсанова О.В.!Матюхин С.И.!Потураева Т.В.!Ромашин С.Н.!Семёнова Г.А.!Фроленкова Л.Ю.!Якушина С.И.";
        }
        else if (facultyComboBox.SelectedIndex == 4)
        {
            str = "Аксёнов К.В.!Багров В.В.!Батенков А.А.!Варгашкин В.Я.!Власова М.А.!Воронина О.А.!Донцов В.М.!Косчинский С.Л.!Лобанова В.А.!Лобода О.А.!Майоров М.В.!Мишин В.В.!Муравьёв А.А.!Плащенков Д.А.!Рязанцев П.Н.!Селихов А.В.!Суздальцев А.И.!Тугарев А.С.!Тютякин А.В.!Филина А.В.";
        }
        else if (facultyComboBox.SelectedIndex == 5)
        {
            str = "Аксёнов К.В.!Гладышев А.В.!Качанов А.Н.!Коренков Д.А.!Королева Т.Г.!Петров Г.Н.!Филина А.В.!Чернышов В.А.";
        }
        foreach (string s in str.Split('!'))
        {
            professorComboBox.Items.Add(s);
        }
    }

    async void ReadScrollMenuItem_Click(object sender, RoutedEventArgs e)
    {
        List<string> titleData = new();
        AddTitleData(ref titleData);
        if (TextMI.IsChecked)
        {
            data.Text = RTBox.GetText(richTextBox);
        }
        bool exportPDFOn = ExportPDF.IsChecked;
        bool exportHTMLOn = ExportHTML.IsChecked;

        Report report = new();
        await Task.Run(() =>
            Report.Create(data, viewModel.PageNumbers, viewModel.TableOfContents, viewModel.NumberHeading, typeDocument, titleData.ToArray(), exportPDFOn, exportHTMLOn, tablesData.collection));

        if (Properties.Settings.Default.CloseWindow)
        {
            WindowClose();
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
                if (ValidAddInput())
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
                elementCB.Visibility = Visibility.Collapsed;
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
                elementCB.Visibility = Visibility.Visible;
                elementCB.SelectedItem = (string)FindResource("AllText");
                ShowSpecials();
                TextPanelRTB.Visibility = Visibility.Visible;
                richTextBox.Focus();
                RTBox.SetText(richTextBox, data.Text);
                UpdateTypeButton();
                DownTextUpdate();
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
                data.Text = RTBox.GetText(richTextBox);
                richTextBox.Document.Blocks.Clear();
                richTextBox.Visibility = Visibility.Collapsed;
                cursorLocationTB.Visibility = Visibility.Collapsed;
                TextPanelRTB.Visibility = Visibility.Collapsed;
                HideSpecials();
            }
        }
        MenuItem.IsChecked = false;
    }

    void ShowSpecials()
    {
        panelTypeInserts.Visibility = Visibility.Visible;
    }

    void HideSpecials()
    {
        panelTypeInserts.Visibility = Visibility.Collapsed;
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

    void Add_Click(object sender, RoutedEventArgs e)
    {
        if (ValidAddInput())
        {
            string[] text = new string[] { HeaderSubstitution.Text, RTBox.GetText(richTextBoxSubstitution) };
            if (TypeSubstitution.SelectedIndex == 4)
            {
                tablesData.AddTable();
            }
            string str = TypeRichBox();
            AddToComboBox(data.ComboBox[str], text);
        }
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

    bool ValidAddInput()
    {
        if (TypeSubstitution.SelectedIndex == 0 || TypeSubstitution.SelectedIndex == 1 || TypeSubstitution.SelectedIndex == 2 || TypeSubstitution.SelectedIndex == 4)
        {
            return true;
        }
        else if (TypeSubstitution.SelectedIndex != -1)
        {
            if (System.IO.File.Exists(RTBox.GetText(richTextBoxSubstitution)))
            {
                return true;
            }
        }
        return false;
    }

    void AddToComboBox(ElementComboBox comboBox, string[] strData)
    {
        if (!comboBox.Form.Items.Contains(strData[0]))
        {
            strData[1] = strData[1].Replace("\r", "");
            comboBox.Data.Add(strData);
            comboBox.Form.Items.Add(strData[0]);
            comboBox.Form.SelectedIndex = comboBox.Form.Items.IndexOf(strData[0]);
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
            DataComboBoxToRichBox(data.SearchComboBox(comboBox));
        }
    }

    void DataComboBoxToRichBox(ElementComboBox comboBox)
    {
        TypeSubstitutionOn.Text = Config.AddSpecialBoth(data.ComboBox.FirstOrDefault(x => x.Value == comboBox).Key);

        HeaderSubstitution.Text = comboBox.Data[comboBox.Form.SelectedIndex][0];
        RTBox.SetText(richTextBoxSubstitution, comboBox.Data[comboBox.Form.SelectedIndex][1]);

        if (TypeSubstitutionOn.Text == "◄t►")
        {
            richTextBoxSubstitution.Visibility = Visibility.Collapsed;
            panelTable.Visibility = Visibility.Visible;
            HidePictureBox();
            tablesData.SelectTable(comboBox.Form.SelectedIndex);
            countRows.Text = tablesData.CurrentData.Rows.ToString();
            countColumns.Text = tablesData.CurrentData.Columns.ToString();
            UpdateTable();
        }
        else
        {
            if (TypeSubstitutionOn.Text == "◄h1►")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Collapsed;
            }
            else if (TypeSubstitutionOn.Text == "◄h2►")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Collapsed;
            }
            else if (TypeSubstitutionOn.Text == "◄l►")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Collapsed;
            }
            else if (TypeSubstitutionOn.Text == "◄p►")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Visible;
            }
            else if (TypeSubstitutionOn.Text == "◄c►")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Visible;
            }
            panelTable.Visibility = Visibility.Collapsed;
            richTextBoxSubstitution.Visibility = Visibility.Visible;
            ShowPictureBox();
        }
    }

    void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;
        ComboBox_SelectionChanged(comboBox);
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

    void Displayed(string something)
    {
        viewModel.Displayed = (string)FindResource("Displayed") + ": " + something;
    }

    void InfoDisplayedText(ComboBox sender)
    {
        int i = elementPanel.Children.IndexOf(sender) - 1 - (elementPanel.ColumnDefinitions.Count - 2);
        Displayed(menuNames[i] + " - " + (sender.Items.IndexOf(sender.SelectedItem) + 1).ToString());
    }

    void ComboBox_PreviewRightMouseDown(object sender, MouseButtonEventArgs e)
    {
        ComboBox comboBox = (ComboBox)sender;
        if (ComboBoxSelected())
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftAlt))
            {
                for (int i = elementPanel.ColumnDefinitions.Count - 1; i < 2 * elementPanel.ColumnDefinitions.Count - 1 - 2; i++)
                {
                    comboBox = (ComboBox)(elementPanel.Children[i]);
                    comboBox.SelectedIndex = -1;
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (comboBox.SelectedIndex > 0)
                {
                    //перемещение выбранного элемента ComboBox вверх
                    if (comboBox.Name == "tComboBox")
                    {
                        tablesData.SwapTable(comboBox.SelectedIndex - 1, comboBox.SelectedIndex);
                    }
                    (data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex - 1], data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex])
                      = (data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex], data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex - 1]);
                    string saveName = comboBox.Items[comboBox.SelectedIndex].ToString();
                    int savef = comboBox.SelectedIndex;
                    comboBox.Items[comboBox.SelectedIndex] = comboBox.Items[comboBox.SelectedIndex - 1];
                    comboBox.Items[savef - 1] = saveName;
                    comboBox.SelectedIndex = savef - 1;
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (comboBox.SelectedIndex < comboBox.Items.Count - 1)
                {
                    //перемещение выбранного элемента ComboBox вниз
                    if (comboBox.Name == "tComboBox")
                    {
                        tablesData.SwapTable(comboBox.SelectedIndex, comboBox.SelectedIndex + 1);
                    }
                    (data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex + 1], data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex]) = (data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex], data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex + 1]);
                    string saveName = comboBox.Items[comboBox.SelectedIndex].ToString();
                    int savef = comboBox.SelectedIndex;
                    comboBox.Items[comboBox.SelectedIndex] = comboBox.Items[comboBox.SelectedIndex + 1];
                    comboBox.Items[savef + 1] = saveName;
                    comboBox.SelectedIndex = savef + 1;
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                data.SearchComboBox(comboBox).Data.RemoveAt(comboBox.SelectedIndex);
                int removeIDX = comboBox.SelectedIndex;
                if (comboBox.Name == "tComboBox")
                {
                    tablesData.DeleteTable(removeIDX);
                }
                comboBox.Items.RemoveAt(comboBox.SelectedIndex);
                ComboBox_SelectionChanged(comboBox);
            }
        }
    }

    bool SaveComboBoxMainText(ElementComboBox comboBox)
    {
        int index = comboBox.Form.SelectedIndex;
        if (index != -1 && !clearSubstitution)
        {
            comboBox.Data[index][1] = RTBox.GetText(richTextBoxSubstitution);
            comboBox.Form.SelectedIndex = index;
            return true;
        }
        return false;
    }

    bool SaveComboBoxHeader(ElementComboBox comboBox)
    {
        int index = comboBox.Form.SelectedIndex;
        if (index != -1 && !clearSubstitution)
        {
            if (comboBox.Data[index][0] != HeaderSubstitution.Text)
            {
                comboBox.Data[index][0] = HeaderSubstitution.Text;
                comboBox.Form.Items[index] = comboBox.Data[index][0];
            }
            comboBox.Form.SelectedIndex = index;
            return true;
        }
        return false;
    }

    void HeaderSubstitution_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ComboBoxSelected())
        {
            int caret = HeaderSubstitution.CaretIndex;
            if (SaveComboBoxHeader(data.ComboBox["h1"]))
            {
            }
            else if (SaveComboBoxHeader(data.ComboBox["h2"]))
            {
            }
            else if (SaveComboBoxHeader(data.ComboBox["l"]))
            {
            }
            else if (SaveComboBoxHeader(data.ComboBox["p"]))
            {
            }
            else if (SaveComboBoxHeader(data.ComboBox["t"]))
            {
            }
            else if (SaveComboBoxHeader(data.ComboBox["c"]))
            {
            }
            HeaderSubstitution.Focus();
            HeaderSubstitution.CaretIndex = caret;
        }
    }
    void RichTextBoxSubstitution_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ComboBoxSelected())
        {
            add.Visibility = Visibility.Collapsed;
            if (SaveComboBoxMainText(data.ComboBox["h1"]))
            {
            }
            else if (SaveComboBoxMainText(data.ComboBox["h2"]))
            {
            }
            else if (SaveComboBoxMainText(data.ComboBox["l"]))
            {
            }
            else if (SaveComboBoxMainText(data.ComboBox["p"]))
            {
            }
            else if (SaveComboBoxMainText(data.ComboBox["t"]))
            {
            }
            else if (SaveComboBoxMainText(data.ComboBox["c"]))
            {
            }
        }
        else if (ValidAddInput())
        {
            add.Visibility = Visibility.Visible;
        }
        else
        {
            add.Visibility = Visibility.Collapsed;
        }
        ImageUpdate();
    }

    void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateTypeButton();
        ElementComboBoxUpdate();
    }

    void DownTextUpdate()
    {
        if (RTBox.GetCaretIndex(richTextBox) > 0)
        {
            string str = new TextRange(richTextBox.Document.ContentStart, richTextBox.CaretPosition).Text;
            int h1Count = Regex.Matches(str, Config.AddSpecialLeft("h1")).Count;
            if (h1Count > 0)
            {
                string h1 = data.ComboBox["h1"].Form.Items[h1Count - 1].ToString();
                string h2 = string.Empty;
                if (str[str.LastIndexOf(Config.AddSpecialLeft("h1"))..].Contains(Config.AddSpecialLeft("h2")))
                {
                    h2 = " : " + data.ComboBox["h2"].Form.Items[Regex.Matches(str, Config.AddSpecialLeft("h2")).Count - 1].ToString();
                }
                cursorLocationTB.Text = h1 + h2;
            }
            else
            {
                int h2Count = Regex.Matches(str, Config.AddSpecialLeft("h2")).Count;
                if (h2Count > 0)
                {
                    string h2 = (string)FindResource("Before") + " H1: " + data.ComboBox["h2"].Form.Items[Regex.Matches(str, Config.AddSpecialLeft("h2")).Count - 1].ToString();
                    cursorLocationTB.Text = h2;
                }
                else
                {
                    cursorLocationTB.Text = (string)FindResource("BeforeHeaders");
                }
            }
            cursorLocationTB.Text += CursorPosExtra(str);
        }
        else
        {
            cursorLocationTB.Text = (string)FindResource("Start");
        }
    }

    string CursorPosExtra(string str)
    {
        string extra = "";
        for (int i = 2; i < data.ComboBox.Keys.Count; i++)
        {
            string key = data.ComboBox.Keys.ElementAt(i);
            int count = str.Length - key.Length - 1;
            if (count >= 0 && str[count..].StartsWith(Config.specialBefore + key))
            {
                if (key == "l")
                {
                    extra = (string)FindResource("List");
                }
                else if (key == "p")
                {
                    extra = (string)FindResource("Picture");
                }
                else if (key == "t")
                {
                    extra = (string)FindResource("Table");
                }
                else if (key == "c")
                {
                    extra = (string)FindResource("Code");
                }
                int pCount = Regex.Matches(str, Config.AddSpecialLeft(key)).Count;
                extra = $"({extra} - {data.ComboBox[key].Form.Items[pCount - 1]})";
                break;
            }
        }
        return extra;
    }

    void ElementComboBoxUpdate()
    {
        int indexSave = elementCB.SelectedIndex;
        elementCB.Items.Clear();
        elementCB.Items.Add((string)FindResource("AllText"));
        elementCB.Items.Add((string)FindResource("BeforeHeaders"));
        string str = RTBox.GetText(richTextBox);
        int h1Count = 0; int h2Count = 0;
        while (str.Contains(Config.specialBefore + "h1") || str.Contains(Config.specialBefore + "h2"))
        {
            int h1Pos = str.IndexOf(Config.specialBefore + "h1");
            h1Pos = h1Pos == -1 ? int.MaxValue : h1Pos;
            int h2Pos = str.IndexOf(Config.specialBefore + "h2");
            h2Pos = h2Pos == -1 ? int.MaxValue : h2Pos;
            if (h1Pos < h2Pos)
            {
                elementCB.Items.Add("h1: " + data.ComboBox["h1"].Form.Items[h1Count]);
                str = str[(h1Pos + 1 + 2)..];
                h1Count++;
            }
            else
            {
                elementCB.Items.Add("h2: " + data.ComboBox["h2"].Form.Items[h2Count]);
                str = str[(h2Pos + 1 + 2)..];
                h2Count++;
            }
        }
        if (indexSave < elementCB.Items.Count && indexSave != -1)
        {
            elementCB.SelectedIndex = indexSave;
        }
        else
        {
            elementCB.SelectedItem = 0;
        }
    }

    void UpdateTypeButton()
    {
        foreach (KeyValuePair<string, ElementComboBox> comboBox in data.ComboBox)
        {
            CountTypeText(comboBox.Value, comboBox.Key);
        }
    }

    void CountTypeText(ElementComboBox comboBox, string name)
    {
        if (comboBox.Data.Count <= (RTBox.GetText(richTextBox).Length - RTBox.GetText(richTextBox).Replace(Config.AddSpecialLeft(name), "").Length) / (name.Length + 1))
        {
            Button button = (Button)panelTypeInserts.FindName(name.ToUpper());
            button.Visibility = Visibility.Collapsed;
        }
        else
        {
            Button button = (Button)panelTypeInserts.FindName(name.ToUpper());
            button.Visibility = Visibility.Visible;
        }
    }

    void ButtonSpecial_Click(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        int idx = RTBox.GetCaretIndex(richTextBox);
        if (RTBox.GetText(richTextBox).Length > 0 && idx > 0 && RTBox.GetText(richTextBox)[idx - 1] == Config.specialBefore)
        {
            AddSpecialSymbol(button.Name.ToLower(), idx);
        }
        else
        {
            AddSpecialSymbol(Config.AddSpecialLeft(button.Name.ToLower()), idx);
        }
    }

    void AddSpecialSymbol(string symbol, int idx)
    {
        string d = RTBox.GetText(richTextBox);
        if (idx == 0 || (idx == 1 && RTBox.GetText(richTextBox)[idx - 1] == Config.specialBefore) || (idx >= 2 && RTBox.GetText(richTextBox)[idx - 2] == '\n'))
        {
            RTBox.SetText(richTextBox, d.Insert(idx, symbol.ToLower() + "\n"));
            RTBox.SetCaret(richTextBox, idx + symbol.Length + 3);
        }
        else
        {
            RTBox.SetText(richTextBox, d.Insert(idx, "\n" + symbol.ToLower() + "\n"));
            RTBox.SetCaret(richTextBox, idx + symbol.Length + 4);
        }
        richTextBox.Focus();
    }

    void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        ProcessingRichTextBox.StartForTextPanel(richTextBox, e);
    }

    void ChangeUserMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ChangeUser.Start(data.ComboBox["p"]);
        ChangeUser.Start(data.ComboBox["c"]);
    }

    void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        DownTextUpdate();
    }

    void MouseEnterTypeButton(object sender, MouseEventArgs e)
    {
        Button pb = (Button)sender;
        string name = pb.Name.ToLower();
        int index = Regex.Matches(new TextRange(richTextBox.Document.ContentStart, richTextBox.CaretPosition).Text, Config.AddSpecialLeft(name)).Count;
        if (pb.Visibility == Visibility.Visible && index < data.ComboBox[name].Form.Items.Count && pb.IsMouseOver)
        {
            string str = (string)FindResource("Paste") + " " + data.ComboBox[name].Form.Items[index] + " " + (string)FindResource("In") + " " + cursorLocationTB.Text;
            if (Regex.Matches(new TextRange(richTextBox.CaretPosition, richTextBox.CaretPosition.DocumentEnd).Text, Config.AddSpecialLeft(name)).Count > 0)
            {
                str += (string)FindResource("OffsetFollowing");
            }
            cursorLocationTB.Text = str;
        }
    }

    void MouseLeaveTypeButton(object sender, MouseEventArgs e)
    {
        DownTextUpdate();
    }

    void Win_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateProfessorComboBox();
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

    string TypeRichBox()
    {
        string type = string.Empty;
        if (TypeSubstitution.Visibility == Visibility.Visible)
        {
            if (TypeSubstitution.SelectedIndex == 0)
            {
                type = "h1";
            }
            else if (TypeSubstitution.SelectedIndex == 1)
            {
                type = "h2";
            }
            else if (TypeSubstitution.SelectedIndex == 2)
            {
                type = "l";
            }
            else if (TypeSubstitution.SelectedIndex == 3)
            {
                type = "p";
            }
            else if (TypeSubstitution.SelectedIndex == 4)
            {
                type = "t";
            }
            else if (TypeSubstitution.SelectedIndex == 5)
            {
                type = "c";
            }
        }
        else
        {
            type = TypeSubstitutionOn.Text.Replace("◄", "").Replace("►", "");
        }
        return type;
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

    void ImageUpdate()
    {
        string str = TypeRichBox();
        singlePB.Visibility = Visibility.Visible;
        if (str == "h1")
        {
            int index = data.ComboBox["h1"].Form.SelectedIndex;
            if (index != -1)
            {
                if (NumberHeadingMI.IsChecked)
                {
                    DrawText((index + 1).ToString() + " " + data.ComboBox["h1"].Data[index][1].ToUpper());
                }
                else
                {
                    DrawText(data.ComboBox["h1"].Data[index][1].ToUpper());
                }
            }
            else
            {
                DrawText(FindResource("Header").ToString().ToUpperInvariant());
            }
        }
        else if (str == "h2")
        {
            int index = data.ComboBox["h2"].Form.SelectedIndex;
            if (index != -1)
            {
                if (NumberHeadingMI.IsChecked)
                {
                    DrawText("H1." + (index + 1).ToString() + " " + data.ComboBox["h2"].Data[index][1]);
                }
                else
                {
                    DrawText(data.ComboBox["h2"].Data[index][1]);
                }
            }
            else
            {
                DrawText((string)FindResource("SubHeader"));
            }
        }
        else if (str == "l")
        {
            int index = data.ComboBox["l"].Form.SelectedIndex;
            if (index != -1)
            {
                string text = string.Empty;
                int i = 1, level = 0;
                string before = "0";
                foreach (string line in data.ComboBox["l"].Data[index][1].Split('\n'))
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
        else if (str == "t")
        {

        }
        else if (str == "p")
        {
            string path;
            if (pComboBox.SelectedIndex == -1)
            {
                path = RTBox.GetText(richTextBoxSubstitution);
            }
            else
            {
                path = data.ComboBox["p"].Data[pComboBox.SelectedIndex][1];
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
        else if (str == "c")
        {
            string path;
            if (cComboBox.SelectedIndex == -1)
            {
                path = RTBox.GetText(richTextBoxSubstitution);
            }
            else
            {
                path = data.ComboBox["c"].Data[cComboBox.SelectedIndex][1];
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
        var uriSource = new Uri(@"Resources/Picture.png", UriKind.Relative);
        mainImage.Source = new BitmapImage(uriSource);
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
        var uriSource = new Uri(@"Resources/Code.png", UriKind.Relative);
        mainImage.Source = new BitmapImage(uriSource);
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
            mainImage.Source = new BitmapImage(new Uri(@"Resources/DragNDrop.png", UriKind.Relative));
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

    void NumberHeadingMI_Click(object sender, RoutedEventArgs e)
    {
        NumberHeadingMI.IsChecked = !NumberHeadingMI.IsChecked;
        ImageUpdate();
    }

    void PictureBox_DragOver(object sender, DragEventArgs e)
    {
        string str = TypeRichBox();
        if (str != "h1" && str != "h2" && str != "l" && str != "t")
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            Color additional = (Color)ColorConverter.ConvertFromString("#" + Properties.Settings.Default.AdditionalColor);
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

    void PictureBox_DragEnter(object sender, DragEventArgs e)
    {
        string str = TypeRichBox();
        if (str != "h1" && str != "h2" && str != "l" && str != "t")
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            singlePB.Visibility = Visibility.Collapsed;
        }
    }

    void PictureBox_DragLeave(object sender, DragEventArgs e)
    {
        string str = TypeRichBox();
        if (str != "h1" && str != "h2" && str != "l" && str != "t")
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            if (e.GetPosition(pictureBox).X < 0 || e.GetPosition(pictureBox).X > pictureBox.RenderSize.Width || e.GetPosition(pictureBox).Y < 0 || e.GetPosition(pictureBox).Y > pictureBox.RenderSize.Height)
            {
                Color additional = (Color)ColorConverter.ConvertFromString("#" + Properties.Settings.Default.AdditionalColor);
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

    void Win_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
        string str = TypeRichBox();
        if (str != "h1" && str != "h2" && str != "l" && str != "t")
        {
            singlePB.Visibility = Visibility.Collapsed;
            Color additional = (Color)ColorConverter.ConvertFromString("#" + Properties.Settings.Default.AdditionalColor);
            codeRight.Fill = new SolidColorBrush(additional);
            pictureLeft.Fill = new SolidColorBrush(additional);
            textLeft.Foreground = new SolidColorBrush(Colors.Black);
            textRight.Foreground = new SolidColorBrush(Colors.Black);
        }
    }

    void Win_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
        string str = TypeRichBox();
        if (str != "h1" && str != "h2" && str != "l" && str != "t")
        {
            singlePB.Visibility = Visibility.Collapsed;
            Color additional = (Color)ColorConverter.ConvertFromString("#" + Properties.Settings.Default.AdditionalColor);
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
        string str = TypeRichBox();
        if (str != "h1" && str != "h2" && str != "l" && str != "t")
        {
            singlePB.Visibility = Visibility.Visible;
            ImageUpdate();
        }
    }

    void PictureBox_Drop(object sender, DragEventArgs e)
    {
        string str = TypeRichBox();
        if (str != "h1" && str != "h2" && str != "l" && str != "t")
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
                        RTBox.SetText(richTextBoxSubstitution, path);
                    }
                    else
                    {
                        TypeSubstitution.SelectedIndex = TypeIndex("c");
                        HeaderSubstitution.Text = nameFile;
                        RTBox.SetText(richTextBoxSubstitution, path);
                    }
                }
            }
        }
        ImageUpdate();
    }

    void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        UnselectAll();
        UnselectComboBoxes();
    }

    void UnselectAll()
    {
        string type = TypeRichBox();
        clearSubstitution = true;
        Displayed((string)FindResource("Something"));
        TypeSubstitution.Visibility = Visibility.Visible;
        TypeSubstitutionOn.Visibility = Visibility.Collapsed;
        if (type == "t")
        {
            tablesData.UnselectedTable();
            panelTable.Visibility = Visibility.Collapsed;
            richTextBoxSubstitution.Visibility = Visibility.Visible;
            ShowPictureBox();
        }
        TypeSubstitution.SelectedIndex = -1;
        HeaderSubstitution.Text = string.Empty;
        richTextBoxSubstitution.Document.Blocks.Clear();
        clearSubstitution = false;
    }

    void CapsLockFix_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox a = (TextBox)sender;
        if (!string.IsNullOrEmpty(a.Text))
        {
            a.Text = a.Text[..1].ToUpper() + a.Text[1..].ToLower();
        }
    }

    void ElementCB_update()
    {
        if (elementCB.SelectedValue == null)
        {
            return;
        }

        if (elementCB.SelectedIndex == 0)
        {
            richTextBox.Visibility = Visibility.Visible;
            richTextBox2.Visibility = Visibility.Collapsed;
        }
        else
        {
            richTextBox.Visibility = Visibility.Collapsed;
            richTextBox2.Visibility = Visibility.Visible;

            string special = elementCB.SelectedValue.ToString().Split(':')[0];

            int start = FindParagraphStart(special);
            int end = FindParagraphEnd(special);
            string str = RTBox.GetText(richTextBox)[start..end];

            R2_changeText(str);
        }
    }

    void ElementCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ElementCB_update();
    }

    void SwitchRichTextBoxes(object sender, RoutedEventArgs e)
    {
        (richTextBox2.Visibility, richTextBox.Visibility)
            = (richTextBox.Visibility, richTextBox2.Visibility);
    }

    int FindParagraphStart(string paragraphType)
    {
        if (paragraphType == (string)FindResource("BeforeHeaders"))
        {
            return 0;
        }

        string searchingFor = Config.specialBefore + paragraphType + "\n";

        string str = RTBox.GetText(richTextBox);

        int count = 0;
        for (int i = 1; i <= elementCB.SelectedIndex; i++)
        {
            if (elementCB.Items[i].ToString().StartsWith(paragraphType))
            {
                count++;
            }
        }

        int index = 0;
        while (count > 0)
        {
            count--;
            index = str.IndexOf(searchingFor, index);
            index += searchingFor.Length;
        }

        return index;
    }

    int FindParagraphEnd(string paragraphType)
    {
        string str = RTBox.GetText(richTextBox);

        if (paragraphType == (string)FindResource("BeforeHeaders"))
        {
            int h1Pos = str.IndexOf(Config.specialBefore + "h1");
            h1Pos = h1Pos == -1 ? int.MaxValue : h1Pos;
            int h2Pos = str.IndexOf(Config.specialBefore + "h2");
            h2Pos = h2Pos == -1 ? int.MaxValue : h2Pos;
            if (h1Pos == h2Pos && h2Pos == int.MaxValue) h1Pos = str.Length; // somehow it works
            return Math.Min(h1Pos, h2Pos);
        }

        string searchingFor = Config.specialBefore + paragraphType + "\n";

        int count = 0;
        for (int i = 1; i <= elementCB.SelectedIndex; i++)
        {
            if (elementCB.Items[i].ToString().StartsWith(paragraphType))
            {
                count++;
            }
        }

        int index = 0;
        while (count > 0)
        {
            count--;
            index = str.IndexOf(searchingFor, index);
            index += searchingFor.Length;
        }

        index = str.IndexOf(searchingFor, index);
        if (index == -1)
        {
            index = str.Length;
        }
        else
        {
            index -= "\n".Length;
        }

        return index;
    }

    void R2_changeText(string str)
    {
        // changes richTextBox2 text WITHOUT calling its TextChanged
        richTextBox2_TextChanged_On = false;

        richTextBox2.Document.Blocks.Clear();
        richTextBox2.Document.Blocks.Add(new Paragraph(new Run(str)));

        richTextBox2_TextChanged_On = true;
    }
    bool richTextBox2_TextChanged_On = true;

    void RichTextBox2_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (richTextBox2_TextChanged_On == false)
        {
            return;
        }

        if (elementCB.SelectedValue == null)
        {
            return;
        }

        object selectedValueSave = elementCB.SelectedValue; // maybe better to use SelectedIndex
        int cursorPosSave = RTBox.GetCaretIndex(richTextBox2);

        string special = elementCB.SelectedValue.ToString().Split(':')[0];

        int start = FindParagraphStart(special);
        int end = FindParagraphEnd(special);

        string str = RTBox.GetText(richTextBox);
        string before = str[..start];
        string after = str[end..];
        string str2 = RTBox.GetText(richTextBox2);

        str = before + str2 + after;

        richTextBox.Document.Blocks.Clear();
        richTextBox.Document.Blocks.Add(new Paragraph(new Run(str)));

        ElementCB_update();

        elementCB.SelectedValue = selectedValueSave;
        RTBox.SetCaret(richTextBox2, cursorPosSave);
    }

    void RichTextBox2_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (RTBox.GetLineAtCursor(richTextBox2).Contains(Config.specialBefore) || RTBox.GetLineAtCursor(richTextBox2).Contains(Config.specialAfter))
        {
            e.Handled = true;
        }
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
        if (Properties.Settings.Default.AutoHeader)
        {
            AutoHeaderVisibilityText.Visibility = Visibility.Visible;
            AutoHeaderVisibility.IsChecked = Properties.Settings.Default.AutoHeaderVisibility;
        }
    }

    void OpenPersonalization(object sender, RoutedEventArgs e)
    {
        Profile.Visibility = Visibility.Collapsed;
        Personalization.Visibility = Visibility.Visible;
        GeneralisSetiings.Visibility = Visibility.Collapsed;
        fontSize.Value = int.Parse(Properties.Settings.Default.FontSize);
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
        richTextBox2.SpellCheck.IsEnabled = Properties.Settings.Default.SyntaxChecking;
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
        Settings.Visibility = Visibility.Visible;
        MenuPanel.Visibility = Visibility.Collapsed;
        ParentPanel.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
        ParentPanel.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Star);
        OpenGeneralisSetiings();
    }

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
        Drawing.Visibility = Visibility.Visible;
        MenuPanel.Visibility = Visibility.Collapsed;
        ParentPanel.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
        ParentPanel.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Star);
    }

    void ExitSettings(object sender, RoutedEventArgs e)
    {
        ShowElements(prevSettings);
        Settings.Visibility = Visibility.Collapsed;
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

    void ExportPDF_Click(object sender, RoutedEventArgs e)
    {
        ExportPDF.IsChecked = !ExportPDF.IsChecked;
        Properties.Settings.Default.ExportPDF = ExportPDF.IsChecked;
        Properties.Settings.Default.Save();
    }

    void ExportHTML_Click(object sender, RoutedEventArgs e)
    {
        ExportHTML.IsChecked = !ExportHTML.IsChecked;
        Properties.Settings.Default.ExportHTML = ExportHTML.IsChecked;
        Properties.Settings.Default.Save();
    }

    void AutoHeader_Checked(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.AutoHeader = AutoHeader.IsChecked ?? true;
        if (Properties.Settings.Default.AutoHeader)
        {
            AutoHeaderVisibilityText.Visibility = Visibility.Visible;
            AutoHeaderVisibility.IsChecked = Properties.Settings.Default.AutoHeaderVisibility;
        }
        else
        {
            AutoHeaderVisibilityText.Visibility = Visibility.Collapsed;
            Properties.Settings.Default.AutoHeaderVisibility = true;
            UpdateHeadersSubstitution();
        }
        Properties.Settings.Default.Save();
    }

    void AutoHeaderVisibility_Checked(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.AutoHeaderVisibility = AutoHeaderVisibility.IsChecked ?? true;
        Properties.Settings.Default.Save();
        UpdateHeadersSubstitution();
    }

    void TypeSubstitution_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Properties.Settings.Default.AutoHeader)
        {
            if (!clearSubstitution)
            {
                ComboBoxItem item = (ComboBoxItem)TypeSubstitution.SelectedItem;
                HeaderSubstitution.Text = item.Content.ToString() + " " + data.ComboBox[TypeRichBox()].Data.GetHashCode();
            }
        }
        string type = TypeRichBox();

        if (type == "t")
        {
            richTextBoxSubstitution.Visibility = Visibility.Collapsed;
            panelTable.Visibility = Visibility.Visible;
            HidePictureBox();
            countRows.Text = tablesData.CurrentData.Rows.ToString();
            countColumns.Text = tablesData.CurrentData.Columns.ToString();
            UpdateTable();
        }
        else
        {
            if (type == "h1")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Collapsed;
            }
            else if (type == "h2")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Collapsed;
            }
            else if (type == "l")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Collapsed;
            }
            else if (type == "p")
            {
                pictureBox.Visibility = Visibility.Visible;
                dragDropImage.Visibility = Visibility.Collapsed;
                mainImage.Visibility = Visibility.Visible;
            }
            else if (type == "c")
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
        if (ValidAddInput())
        {
            add.Visibility = Visibility.Visible;
        }
        else
        {
            add.Visibility = Visibility.Collapsed;
        }
    }
    void UpdateHeadersSubstitution()
    {
        if (Properties.Settings.Default.AutoHeaderVisibility)
        {
            Substitution.RowDefinitions[1].Height = GridLength.Auto;
        }
        else
        {
            Substitution.RowDefinitions[1].Height = new GridLength(0);
        }
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

    void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.SelectCulture(language.SelectedIndex);
        Properties.Settings.Default.Language = language.SelectedIndex;
        Properties.Settings.Default.Save();
    }
    bool isDrawing = false;

    PathFigure currentFigure;
    void DrawingMouseDown(object sender, MouseButtonEventArgs e)
    {
        Mouse.Capture(DrawingTarget);
        isDrawing = true;
        StartFigure(e.GetPosition(DrawingTarget));
    }

    void AddFigurePoint(Point point)
    {
        currentFigure.Segments.Add(new LineSegment(point, isStroked: true));
    }

    void EndFigure()
    {
        currentFigure = null;
    }

    void StartFigure(Point start)
    {
        currentFigure = new PathFigure() { StartPoint = start };


        System.Windows.Shapes.Path currentPath =
            new System.Windows.Shapes.Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                Data = new PathGeometry() { Figures = { currentFigure } }
            };
        DrawingTarget.Children.Add(currentPath);
    }

    void DrawingMouseUp(object sender, MouseButtonEventArgs e)
    {
        AddFigurePoint(e.GetPosition(DrawingTarget));
        EndFigure();
        isDrawing = false;
        Mouse.Capture(null);
    }

    void DrawingMouseMove(object sender, MouseEventArgs e)
    {
        if (!isDrawing)
            return;
        AddFigurePoint(e.GetPosition(DrawingTarget));
    }

    void CountRows_TextChanged(object sender, TextChangedEventArgs e)
    {
        int rows = tablesData.CurrentData.Rows;
        CountRowOrColumn(countRows, ref rows);
        gridTable.RowDefinitions.Clear();
        for (int i = 0; i < rows; i++)
        {
            gridTable.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100 / rows, type: GridUnitType.Star) });
        }
        tablesData.CurrentData.Rows = rows;
        UpdateTable();
    }

    void CountColumns_TextChanged(object sender, TextChangedEventArgs e)
    {
        int columns = tablesData.CurrentData.Columns;
        CountRowOrColumn(countColumns, ref columns);
        gridTable.ColumnDefinitions.Clear();
        for (int i = 0; i < columns; i++)
        {
            gridTable.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100 / columns, type: GridUnitType.Star) });
        }
        tablesData.CurrentData.Columns = columns;
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
        textBox.Text = textBox.Text.Substring(beginningNumber);
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
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    void UpdateTable()
    {
        /*string[,] oldDataTable = dataTable;
        dataTable = new string[rows, columns];

        for (int i = 0; i < oldDataTable.GetLength(0) && i < rows; i++)
        {
            for (int f = 0; f < oldDataTable.GetLength(1) && f < columns; f++)
            {
                dataTable[i, f] = oldDataTable[i, f];
            }
        }*/

        gridTable.Children.Clear();
        for (int i = 0; i < tablesData.CurrentData.Rows; i++)
        {
            for (int f = 0; f < tablesData.CurrentData.Columns; f++)
            {
                TextBox textBox = new TextBox();
                textBox.Text = tablesData.CurrentData.DataTable[i, f];
                textBox.TextChanged += Cell_TextChanged;
                gridTable.Children.Add(textBox);
                Grid.SetColumn(textBox, f);
                Grid.SetRow(textBox, i);
            }
        }
    }

    void Cell_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        int row = Grid.GetRow(textBox);
        int column = Grid.GetColumn(textBox);
        tablesData.CurrentData.DataTable[row, column] = textBox.Text;
    }
}
