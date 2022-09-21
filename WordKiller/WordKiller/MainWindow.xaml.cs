using Microsoft.Win32;
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
using System.Windows.Threading;

namespace WordKiller;

public partial class MainWindow : Window
{
    int menuLeftIndex;
    string[] menuLabels;
    MenuItem DownPanelMI;
    DataComboBox data;
    readonly DispatcherTimer saveTimer;
    string saveFileName;
    TypeDocument typeDocument;

    public MainWindow(string[] args)
    {
        InitializeComponent();
        TitleElements.SaveTitleUIElements(titlePanel);
        DownPanelMI = SubstitutionMI;
        TextHeaderUpdate();
        RefreshMenu(1);
        ComboBoxSetup();
        data = new DataComboBox(h1ComboBox, h2ComboBox, lComboBox, pComboBox, tComboBox, cComboBox);
        saveTimer = InitializeTimer();
        if (args.Length > 0)
        {
            if (args[0].EndsWith(Config.extension) && File.Exists(args[0]))
            {
                OpenWordKiller(args[0]);
            }
            else
            {
                throw new Exception("Ошибка открытия файла:\nФайл не найден или формат не поддерживается");
            }
        }
    }

    DispatcherTimer InitializeTimer()
    {
        DispatcherTimer timer = new();
        timer.Tick += new EventHandler(HideSaveLogo);
        timer.Interval = new TimeSpan(0, 0, 2);
        timer.Start();
        return timer;
    }

    void ComboBoxSetup()
    {
        menuLabels = new string[(elementPanel.Children.Count) / 2 - 1];
        for (int i = 1; i < (elementPanel.Children.Count) / 2; i++)
        {
            Button button = (Button)elementPanel.Children[i];
            button.Click += Click_ComboBox;
            menuLabels[i - 1] = button.Content.ToString();
        }

        for (int i = elementPanel.ColumnDefinitions.Count - 1; i < elementPanel.Children.Count - 1; i++)
        {
            ComboBox comboBox = (ComboBox)elementPanel.Children[i];
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            comboBox.PreviewMouseRightButtonDown += ComboBox_PreviewRightMouseDown;
        }
    }

    void Click_ComboBox(object sender, RoutedEventArgs e)
    {
        UnselectComboBoxes();
        Button control = (Button)sender;
        if (control.Content.ToString() == "Раздел")
        {
            DefaultTypeRichBox("h1");
        }
        else if (control.Content.ToString() == "Подраздел")
        {
            DefaultTypeRichBox("h2");
        }
        else if (control.Content.ToString() == "Список")
        {
            DefaultTypeRichBox("l");
        }
        else if (control.Content.ToString() == "Картинка")
        {
            DefaultTypeRichBox("p");
        }
        else if (control.Content.ToString() == "Таблица")
        {
            DefaultTypeRichBox("t");
        }
        else if (control.Content.ToString() == "Код")
        {
            DefaultTypeRichBox("c");
        }
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

    void DefaultTypeRichBox(string type)
    {
        string beginning = Config.AddSpecialBoth(type);
        SetText(beginning + "\n\n" + Config.AddSpecialBoth(Config.content) + "\n");
        richTextBox.Focus();
        SetCaret(richTextBox, beginning.Length + 3);
    }

    void WindowBinding_New(object sender, ExecutedRoutedEventArgs e)
    {
        NeedSave();
        win.Title = "WordKiller";
        ClearGlobal();
        menuLeftIndex = 1;
        data = new DataComboBox(h1ComboBox, h2ComboBox, lComboBox, pComboBox, tComboBox, cComboBox);
        richTextBox.Document.Blocks.Clear();
        if (TextMI.IsChecked)
        {
            UpdateTypeButton();
        }
        foreach (UIElement control in titlePanel.Children)
        {
            if (control.GetType().ToString() == "System.Windows.Forms.TextBox")
            {
                TextBox textBox = (TextBox)control;
                textBox.Text = string.Empty;
            }
        }
    }

    void WindowBinding_Open(object sender, ExecutedRoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "wordkiller file (*" + Config.extension + ")|*" + Config.extension + "|All files (*.*)|*.*"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            OpenWordKiller(openFileDialog.FileName);
        }
    }
    void WindowBinding_Save(object sender, ExecutedRoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(saveFileName))
        {
            SaveWordKiller(saveFileName);
        }
        else
        {
            SaveAs();
        }
    }
    void WindowBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
    {
        SaveAs();
    }

    bool SaveAs()
    {
        SaveFileDialog saveFileDialog = new()
        {
            OverwritePrompt = true,
            Filter = "wordkiller file (*" + Config.extension + ")|*" + Config.extension + "|All files (*.*)|*.*",
            FileName = "1"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            saveFileName = saveFileDialog.FileName;
            SaveWordKiller(saveFileName);
            return true;
        }
        return false;
    }

    void OpenWordKiller(string fileName)
    {
        data.Text = string.Empty;
        saveFileName = fileName;
        ClearGlobal();
        FileStream file = new(fileName, FileMode.Open);
        StreamReader reader = new(file);
        try
        {
            string text = reader.ReadToEnd();
            if (text[0] == '1' && text[1] == '\r' && text[2] == '\n')
            {
                text = Encryption.MegaConvertD(text.Substring(3));
            }
            else if (text[0] == '0' && text[1] == '\r' && text[2] == '\n')
            {
                text = text.Substring(3);
                text = text.Replace("\n", "\r\n");
            }
            for (int i = 1; i < text.Length; i++)
            {
                if (text[i - 1] == '\r')
                {
                    text = text.Remove(i, 1);
                }
            }
            string[] lines = text.Split('\r');

            bool readingText = false;
            List<UIElement> controls = new();
            foreach (string line in lines)
            {
                if (line.StartsWith(Config.AddSpecialBoth("Menu")))
                {
                    string[] menuItem = line.Remove(0, 6).Split('!');
                    foreach (Control f in typeMenuItem.Items)
                    {
                        if (f.GetType() == typeof(MenuItem) && f.Name == menuItem[0])
                        {
                            WorkChange((MenuItem)f);
                        }
                    }
                    if (menuItem[0] != "DefaultDocumentMI")
                    {
                        foreach (UIElement control in titlePanel.Children)
                        {
                            if (control.GetType().ToString() != "System.Windows.Forms.Label")
                            {
                                controls.Add(control);
                            }
                        }
                    }
                    NumberHeadingMI.IsChecked = bool.Parse(menuItem[1]);
                }

                if (line.StartsWith(Config.AddSpecialBoth("TextStart")))
                {
                    readingText = true;
                }
                else if (readingText)
                {
                    if (line.StartsWith(Config.AddSpecialBoth("TextEnd")))
                    {
                        readingText = false;
                    }
                    else
                    {
                        data.Text += line + "\n";
                    }
                }
                else
                {
                    string[] variable_value = line.Split(new char[] { Config.specialBefore, Config.specialAfter });
                    if (variable_value.Length == 2)
                    {
                        for (int i = 0; i < controls.Count; i++)
                        {
                            if (LoadingOfTwo(variable_value, controls[i]))
                            {
                                controls.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    else if (variable_value.Length == 3)
                    {
                        foreach (KeyValuePair<string, ElementComboBox> comboBox in data.ComboBox)
                        {
                            if (variable_value[0].StartsWith(comboBox.Key + "ComboBox"))
                            {
                                comboBox.Value.Form.Items.Add(variable_value[1]);
                                string dataComboBox = variable_value[2].Replace("!@!", "\n");
                                string[] str = new string[] { variable_value[1], dataComboBox };
                                comboBox.Value.Data.Add(str);
                                break;
                            }
                        }
                    }
                }
                win.Title = Path.GetFileName(fileName);
            }
            if (data.Text.Length > 0)
            {
                data.Text = data.Text.Remove(data.Text.Length - 1);
            }
        }
        catch
        {
            MessageBox.Show("Файл повреждён");
        }
        if (DownPanelMI == TextMI)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(data.Text)));
            richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
        }
        reader.Close();
    }

    bool LoadingOfTwo(string[] variable_value, UIElement control)
    {
        if (control.GetType().ToString() == "System.Windows.Controls.TextBox")
        {
            TextBox f = (TextBox)control;
            if (variable_value[0].StartsWith(f.Name))
            {
                f.Text = variable_value[1];
                return true;
            }
        }
        else if (control.GetType().ToString() == "System.Windows.Controls.ComboBox")
        {
            ComboBox f = (ComboBox)control;
            if (variable_value[0].StartsWith(f.Name))
            {
                f.Text = variable_value[1];
                return true;
            }
        }
        return false;
    }

    void SaveWordKiller(string nameFile)
    {
        FileStream fileStream = System.IO.File.Open(nameFile, FileMode.Create);
        StreamWriter output = new(fileStream);
        string save = string.Empty;

        foreach (Control item in typeMenuItem.Items)
        {
            if (item.GetType() == typeof(MenuItem) && ((MenuItem)item).IsChecked)
            {
                save += Config.AddSpecialBoth("Menu") + item.Name.ToString() + "!" + NumberHeadingMI.IsChecked.ToString() + "\n";
            }
        }
        save += Config.AddSpecialRight("facultyComboBox") + facultyComboBox.Text + "\n";
        save += Config.AddSpecialRight("numberTextBox") + numberTextBox.Text + "\n";
        save += Config.AddSpecialRight("themeTextBox") + themeTextBox.Text + "\n";
        save += Config.AddSpecialRight("disciplineTextBox") + disciplineTextBox.Text + "\n";
        save += Config.AddSpecialRight("professorComboBox") + professorComboBox.Text + "\n";
        save += Config.AddSpecialRight("yearTextBox") + yearTextBox.Text + "\n";
        save += Config.AddSpecialRight("shifrTextBox") + shifrTextBox.Text + "\n";
        save += Config.AddSpecialRight("studentsTextBox") + studentsTextBox.Text + "\n";
        foreach (KeyValuePair<string, ElementComboBox> comboBox in data.ComboBox)
        {
            save += SaveCombobox(comboBox.Value, comboBox.Key);
        }
        save += Config.AddSpecialBoth("TextStart") + "\n";
        if (TextMI.IsChecked)
        {
            save += GetText(richTextBox) + "\n";
        }
        else
        {
            save += data.Text + "\n";
        }
        save += Config.AddSpecialBoth("TextEnd") + "\n";
        if (Encoding0MenuItem.IsChecked)
        {
            output.Write("0\r\n" + save);
        }
        else if (Encoding1MenuItem.IsChecked)
        {
            output.Write("1\r\n" + Encryption.MegaConvertE(save));
        }
        output.Close();
        win.Title = Path.GetFileName(nameFile);
        ShowSaveLogo();
    }
    void ShowSaveLogo()
    {
        saveLogo.Visibility = Visibility.Visible;
        saveTimer.Stop();
        saveTimer.Start();
    }

    void HideSaveLogo(object source, EventArgs e)
    {
        saveLogo.Visibility = Visibility.Collapsed;
        saveTimer.Stop();
    }

    string SaveCombobox(ElementComboBox comboBox, string name)
    {
        string comboBoxSave = string.Empty;
        for (int i = 0; i < comboBox.Form.Items.Count; i++)
        {
            string dataCombobox = string.Empty;
            if (comboBox.Data[i][1].Contains("\n"))
            {
                foreach (string str in comboBox.Data[i][1].Split('\n'))
                {
                    dataCombobox += str + "!@!";
                }
            }
            else
            {
                dataCombobox = comboBox.Data[i][1];
            }

            comboBoxSave += name + "ComboBox" + Config.AddSpecialBoth(comboBox.Form.Items[i].ToString()) + dataCombobox + "\n";
        }
        return comboBoxSave;
    }

    void ClearGlobal()
    {
        data = new DataComboBox(h1ComboBox, h2ComboBox, lComboBox, pComboBox, tComboBox, cComboBox);
        for (int i = elementPanel.ColumnDefinitions.Count - 1; i < elementPanel.Children.Count - 1; i++)
        {
            ComboBox cmbBox = (ComboBox)elementPanel.Children[i];
            cmbBox.Items.Clear();
        }
    }

    bool NeedSave()
    {
        MessageBoxResult result = MessageBox.Show("Нужно ли сохранить?", "Нужно ли сохранить?", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
        if (result == MessageBoxResult.Yes)
        {
            if (!string.IsNullOrEmpty(saveFileName))
            {
                SaveWordKiller(saveFileName);
            }
            else
            {
                SaveAs();
            }
            return true;
        }
        return false;
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

    void WorkChange(MenuItem menuItem)
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
            TextHeader("Обычный документ");
            typeDocument = TypeDocument.DefaultDocument;
        }
        else if (LabMI.IsChecked)
        {
            typeDocument = TypeDocument.LaboratoryWork;
            TextHeader("Лабораторная работа");
            TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
        }
        else if (PracticeMI.IsChecked)
        {
            typeDocument = TypeDocument.PracticalWork;
            TextHeader("Практическая работа");
            TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
        }
        else if (CourseworkMI.IsChecked)
        {
            typeDocument = TypeDocument.Coursework;
            TextHeader("Курсовая работа");
            TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 4.1 5.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
        }
        else if (ControlWorkMI.IsChecked)
        {
            typeDocument = TypeDocument.ControlWork;
            TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            TextHeader("Контрольная работа");
        }
        else if (RefMI.IsChecked)
        {
            typeDocument = TypeDocument.Referat;
            TitleElements.ShowTitleElems(titlePanel, "0.0 1.0 0.1 0.3 1.3 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
            TextHeader("Реферат");
        }
        else if (DiplomMI.IsChecked)
        {
            typeDocument = TypeDocument.GraduateWork;
            TextHeader("Дипломная работа");
            TitleElements.ShowTitleElems(titlePanel, "");
        }
        else if (VKRMI.IsChecked)
        {
            typeDocument = TypeDocument.VKR;
            TextHeader("ВКР");
            TitleElements.ShowTitleElems(titlePanel, "");
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
        if (string.IsNullOrEmpty(saveFileName))
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
        }
        else
        {
            DownPanelMI = MenuItem;
            downPanel.Visibility = Visibility.Visible;
            if (MenuItem == SubstitutionMI)
            {
                add.Visibility = Visibility.Visible;
                elementPanel.Visibility = Visibility.Visible;
                toText.Content = "К тексту";
                toText.Margin = new Thickness(2, 0, 5, 5);
                elementTBl.Visibility = Visibility.Visible;
                elementCB.Visibility = Visibility.Collapsed;
                richTextBox.Focus();
                elementTBl.Text = "нечто";
                ShowPictureBox();
                ImageUpdate();
            }
            else if (MenuItem == TextMI)
            {
                cursorLocationTB.Visibility = Visibility.Visible;
                toText.Content = "К подстановкам";
                toText.Margin = new Thickness(2, 5, 5, 5);
                elementTBl.Visibility = Visibility.Collapsed;
                elementCB.Visibility = Visibility.Visible;
                elementCB.SelectedItem = "Весь текст";
                ShowSpecials();
                HidePictureBox();
                richTextBox.Focus();
                SetText(data.Text);
                UpdateTypeButton();
                DownTextUpdate();
                richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
            }
        }
    }

    void SetText(string text)
    {
        FlowDocument allText = new();
        allText.Blocks.Add(new Paragraph(new Run(text)));
        richTextBox.Document = allText;
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
                add.Visibility = Visibility.Collapsed;
                elementPanel.Visibility = Visibility.Collapsed;
            }
            else if (MenuItem == TextMI)
            {
                data.Text = GetText(richTextBox);
                richTextBox.Document.Blocks.Clear();
                cursorLocationTB.Visibility = Visibility.Collapsed;
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

    void ShowPictureBox()
    {
        pictureBox.Visibility = Visibility.Visible;
        panelRichTextBox.ColumnDefinitions[1].Width = new GridLength(40, GridUnitType.Star);
    }

    void HidePictureBox()
    {
        pictureBox.Visibility = Visibility.Collapsed;
        panelRichTextBox.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
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
                elementPanel.ColumnDefinitions[i].Width = new GridLength(23, GridUnitType.Star);
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
            elementPanel.ColumnDefinitions[0].Width = new GridLength(4, GridUnitType.Star);
        }
        if (menuLeftIndex == elementPanel.ColumnDefinitions.Count - 5)
        {
            elementPanel.ColumnDefinitions[elementPanel.ColumnDefinitions.Count - 1].Width = new GridLength(0, GridUnitType.Star);
        }
        else
        {
            elementPanel.ColumnDefinitions[elementPanel.ColumnDefinitions.Count - 1].Width = new GridLength(4, GridUnitType.Star);
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

    string GetText(RichTextBox richTextBox)
    {
        string text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
        if (text != string.Empty)
        {
            text = text.Replace("\r", "");
            text = text.Remove(text.Length - 1, 1);
        }
        return text;
    }

    void Add_Click(object sender, RoutedEventArgs e)
    {
        if (ValidAddInput())
        {
            string str = GetText(richTextBox).Split('\n')[0].Replace(Config.specialBefore.ToString(), "").Replace(Config.specialAfter.ToString(), "");
            string[] text = new string[] { GetText(richTextBox).Split('\n')[1], SplitMainText() };
            AddToComboBox(data.ComboBox[str], text);
        }
    }

    bool ValidAddInput()
    {
        string str = GetText(richTextBox).Split('\n')[0];
        if (GetText(richTextBox).Split('\n').Length >= 4 && GetText(richTextBox).Split('\n')[2] == Config.AddSpecialBoth(Config.content))
        {
            if (str == Config.AddSpecialBoth("h1") || str == Config.AddSpecialBoth("h2"))
            {
                return true;
            }
            else if (str == Config.AddSpecialBoth("l"))
            {
                return true;
            }
            else if (str == Config.AddSpecialBoth("p"))
            {
                if (System.IO.File.Exists(SplitMainText()))
                {
                    return true;
                }
            }
            else if (str == Config.AddSpecialBoth("t"))
            {
                // ???
            }
            else if (str == Config.AddSpecialBoth("c"))
            {
                if (System.IO.File.Exists(SplitMainText()))
                {
                    return true;
                }
            }
        }
        return false;
    }

    string SplitMainText()
    {
        string[] str = GetText(richTextBox).Split('\n');
        string mainText = str[3];
        if (str.Length > 4)
        {
            for (int i = 4; str.Length > i; i++)
            {
                mainText += "\n" + str[i];
            }
        }
        return mainText;
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
            richTextBox.Focus();
            DataComboBoxToRichBox(data.SearchComboBox(comboBox));
        }
    }

    void DataComboBoxToRichBox(ElementComboBox comboBox)
    {
        string type = Config.AddSpecialBoth(data.ComboBox.FirstOrDefault(x => x.Value == comboBox).Key);
        string text = type + "\n" + comboBox.Data[comboBox.Form.SelectedIndex][0] + "\n" + Config.AddSpecialBoth(Config.content) + "\n" + comboBox.Data[comboBox.Form.SelectedIndex][1];
        richTextBox.Document.Blocks.Clear();
        richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        richTextBox.Focus();
        SetCaret(richTextBox, type.Length + comboBox.Data[comboBox.Form.SelectedIndex][0].Length + 3);
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
            LStartText(comboBox);
            elementTBl.Text += (comboBox.Items.IndexOf(comboBox.SelectedItem) + 1).ToString();
        }
        else
        {
            elementTBl.Text = "нечто";
            richTextBox.Document.Blocks.Clear();
        }
        ComboBoxIndexChange(comboBox);
    }

    void LStartText(object sender)
    {
        Control senderControl = (Control)sender;
        int i = elementPanel.Children.IndexOf(senderControl) - 1 - (elementPanel.ColumnDefinitions.Count - 2);
        elementTBl.Text = menuLabels[i] + ": ";
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
                    string[] save = data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex];
                    data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex] = data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex - 1];
                    data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex - 1] = save;
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
                    string[] save = data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex];
                    data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex] = data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex + 1];
                    data.SearchComboBox(comboBox).Data[comboBox.SelectedIndex + 1] = save;
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
                comboBox.Items.RemoveAt(comboBox.SelectedIndex);
                ComboBoxIndexChange(comboBox);
                ComboBox_SelectionChanged(comboBox);
            }
        }
    }

    async void ReadScrollMenuItem_Click(object sender, RoutedEventArgs e)
    {
        List<string> titleData = new();
        AddTitleData(ref titleData);
        if (TextMI.IsChecked)
        {
            data.Text = GetText(richTextBox);
        }
        bool numberingOn = NumberingMI.IsChecked;
        bool tableOfContentsOn = tableOfContents.IsChecked;
        bool headingNumbersOn = NumberHeadingMI.IsChecked;
        bool exportPDFOn = ExportPDF.IsChecked;

        Report report = new();
        await Task.Run(() =>
            report.Create(data, numberingOn, tableOfContentsOn, headingNumbersOn, typeDocument, titleData.ToArray()));

        if (CloseWindow.IsChecked)
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

    void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DownPanelMI == SubstitutionMI)
        {
            if (ComboBoxSelected() && GetText(richTextBox) != string.Empty)
            {
                if (SaveComboBoxData(data.ComboBox["h1"]))
                {
                }
                else if (SaveComboBoxData(data.ComboBox["h2"]))
                {
                }
                else if (SaveComboBoxData(data.ComboBox["l"]))
                {
                }
                else if (SaveComboBoxData(data.ComboBox["p"]))
                {
                }
                else if (SaveComboBoxData(data.ComboBox["t"]))
                {
                }
                else if (SaveComboBoxData(data.ComboBox["c"]))
                {
                }
            }
            ImageUpdate();
        }
        else
        {
            UpdateTypeButton();
            ElementComboBoxUpdate();
        }
    }

    void DownTextUpdate()
    {
        if (GetCaretIndex(richTextBox) > 0)
        {
            string str = new TextRange(richTextBox.Document.ContentStart, richTextBox.CaretPosition).Text;
            int h1Count = Regex.Matches(str, Config.AddSpecialLeft("h1")).Count;
            if (h1Count > 0)
            {
                string h1 = data.ComboBox["h1"].Form.Items[h1Count - 1].ToString();
                string h2 = string.Empty;
                if (str.Substring(str.LastIndexOf(Config.AddSpecialLeft("h1"))).Contains(Config.AddSpecialLeft("h2")))
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
                    string h2 = "До H1 заголовков : " + data.ComboBox["h2"].Form.Items[Regex.Matches(str, Config.AddSpecialLeft("h2")).Count - 1].ToString();
                    cursorLocationTB.Text = h2;
                }
                else
                {
                    cursorLocationTB.Text = "До заголовков";
                }
            }
            cursorLocationTB.Text += CursorPosExtra(str);
        }
        else
        {
            cursorLocationTB.Text = "Начало";
        }
    }

    string CursorPosExtra(string str)
    {
        string extra = "";
        for (int i = 2; i < data.ComboBox.Keys.Count; i++)
        {
            string key = data.ComboBox.Keys.ElementAt(i);
            int count = str.Length - key.Length - 1;
            if (count >= 0 && str.Substring(count).StartsWith(Config.specialBefore + key))
            {
                if (key == "l")
                {
                    extra = "Список";
                }
                else if (key == "p")
                {
                    extra = "Картинка";
                }
                else if (key == "t")
                {
                    extra = "Таблица";
                }
                else if (key == "c")
                {
                    extra = "Код";
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
        //int index = richTextBox.SelectionStart;
        int indexSave = elementCB.SelectedIndex;
        elementCB.Items.Clear();
        elementCB.Items.Add("Весь текст");
        elementCB.Items.Add("До разделов");
        string str = GetText(richTextBox);
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
                str = str.Substring(h1Pos + 1 + 2);
                h1Count++;
            }
            else
            {
                elementCB.Items.Add("h2: " + data.ComboBox["h2"].Form.Items[h2Count]);
                str = str.Substring(h2Pos + 1 + 2);
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
        //richTextBox.SelectionStart = index;
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
        if (comboBox.Data.Count <= (GetText(richTextBox).Length - GetText(richTextBox).Replace(Config.AddSpecialLeft(name), "").Length) / (name.Length + 1))
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


    bool SaveComboBoxData(ElementComboBox comboBox)
    {
        int index = comboBox.Form.SelectedIndex;
        if (index != -1)
        {
            string[] lines = GetText(richTextBox).Split('\n');
            comboBox.Data[index][1] = SplitMainText();
            if (comboBox.Data[index][0] != lines[1])
            {
                comboBox.Data[index][0] = lines[1];
                comboBox.Form.Items[index] = comboBox.Data[index][0];
            }
            comboBox.Form.SelectedIndex = index;
            return true;
        }
        return false;
    }

    void ButtonSpecial_Click(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        int idx = GetCaretIndex(richTextBox);
        if (GetText(richTextBox).Length > 0 && idx > 0 && GetText(richTextBox)[idx - 1] == Config.specialBefore)
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
        string d = GetText(richTextBox);
        if (idx == 0 || (idx == 1 && GetText(richTextBox)[idx - 1] == Config.specialBefore) || (idx >= 2 && GetText(richTextBox)[idx - 2] == '\n'))
        {
            SetText(d.Insert(idx, symbol.ToLower() + "\n"));
            SetCaret(richTextBox, idx + symbol.Length + 3);
        }
        else
        {
            SetText(d.Insert(idx, "\n" + symbol.ToLower() + "\n"));
            SetCaret(richTextBox, idx + symbol.Length + 4);
        }
        richTextBox.Focus();
    }

    int GetCaretIndex(RichTextBox r)
    {
        return new TextRange(r.Document.ContentStart, r.CaretPosition).Text.Replace("\r", "").Length;
    }

    int GetLineOfCursor(RichTextBox richTextBox)
    {
        return new TextRange(richTextBox.Document.ContentStart, richTextBox.CaretPosition).Text.Split('\n').Length;
    }

    string GetLineAtCursor(RichTextBox richTextBox)
    {
        string[] lines = new TextRange(richTextBox.Document.ContentStart, richTextBox.CaretPosition).Text.Split('\n');
        string last = lines[lines.Length - 1];
        return last;
    }

    void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DownPanelMI == SubstitutionMI && ComboBoxSelected())
        {
            int line = GetLineOfCursor(richTextBox);
            string[] lines = GetText(richTextBox).Split('\n');
            int index = GetCaretIndex(richTextBox);
            if (new TextRange(richTextBox.CaretPosition.DocumentStart, richTextBox.CaretPosition.DocumentEnd).Text == richTextBox.Selection.Text && (e.Key == Key.Back || e.Key == Key.Delete))
            {
                lines[1] = "";
                lines[3] = "";
                SetText(lines[0] + "\n" + lines[1] + "\n" + lines[2] + "\n" + lines[3]);
                SetCaret(richTextBox, lines[0].Length + 3);
                e.Handled = true;
            }
            else if ((line == 1 || line == 3 || (line == 2 && richTextBox.Selection.Text.Contains("\n"))) && !(e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right))
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Enter && line == 2 || e.Key == Key.Delete && EndSecondLines(lines, index) ||
                    (e.Key == Key.Back || Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.X) &&
                    (BeginningSecondLines(lines, index) || BeginningFourthLines(lines, index)) && richTextBox.Selection.Text.Length == 0)
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Down && (line == 2 || BeginningSecondLines(lines, index) || EndSecondLines(lines, index)))
            {
                SetCaret(richTextBox, index + lines[1].Length + lines[2].Length + 4);
                e.Handled = true;
            }
            else if (e.Key == Key.Up && (line == 4 || BeginningFourthLines(lines, index)))
            {
                SetCaret(richTextBox, index - lines[1].Length - lines[2].Length);
                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V)
            {
                if (line == 2)
                {
                    if (richTextBox.Selection.Text.Contains("\n"))
                    {
                        e.Handled = true;
                    }
                    else if (Clipboard.GetText().Contains("\n"))
                    {
                        Clipboard.SetText(Clipboard.GetText().Replace("\r", "").Replace('\n', ' '));
                    }
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D)
            {
                if (string.IsNullOrEmpty(lines[1]) && !string.IsNullOrEmpty(lines[3]))
                {
                    lines[1] = lines[3];
                    if (lines.Length > 4)
                    {
                        for (int i = 4; i < lines.Length; i++)
                        {
                            lines[3] += "\n" + lines[i];
                        }
                    }
                    SetText(lines[0] + "\n" + lines[1] + "\n" + lines[2] + "\n" + lines[3]);
                }
                else if (string.IsNullOrEmpty(lines[3]))
                {
                    lines[3] = lines[1];
                    SetText(lines[0] + "\n" + lines[1] + "\n" + lines[2] + "\n" + lines[3]);
                }
                SetCaret(richTextBox, lines[0].Length + lines[1].Length + lines[2].Length + lines[3].Length + 6);
            }
        }
        else
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V)
            {
                Clipboard.SetText(Clipboard.GetText().Replace("\r", "").Replace('\n', ' '));
            }
            if (!CheckPressKey(e.Key, Key.Delete, Key.Back, Key.Enter, Key.Up, Key.Down, Key.Left, Key.Right) && (GetLineAtCursor(richTextBox).Contains(Config.specialBefore) || GetLineAtCursor(richTextBox).Contains(Config.specialAfter)) && !(Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)) // probably this is better than something above that does the same for line 0 and 2
            {
                e.Handled = true;
            }
        }
    }

    bool CheckPressKey(Key press, params Key[] keys)
    {
        foreach (Key key in keys)
        {
            if (press == key)
            {
                return true;
            }
        }
        return false;
    }

    void SetCaret(RichTextBox richTextBox, int position)
    {
        TextPointer textPointer = richTextBox.Document.ContentStart.GetPositionAtOffset(position);
        if (textPointer == null)
        {
            richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
        }
        else
        {
            richTextBox.CaretPosition = richTextBox.Document.ContentStart.GetPositionAtOffset(position);
        }
    }

    bool BeginningSecondLines(string[] lines, int index)
    {
        if (lines[0].Length == index - 1)
        {
            return true;
        }
        return false;
    }

    bool BeginningFourthLines(string[] lines, int index)
    {
        if (lines[0].Length + lines[1].Length + lines[2].Length == index - 3)
        {
            return true;
        }
        return false;
    }

    bool EndSecondLines(string[] lines, int index)
    {
        if (lines[1].Length + lines[0].Length == index - 1)
        {
            return true;
        }
        return false;
    }

    void Encoding_Click(object sender, RoutedEventArgs e)
    {
        Encoding0MenuItem.IsChecked = false;
        Encoding1MenuItem.IsChecked = false;
        MenuItem menuItem = (MenuItem)sender;
        menuItem.IsChecked = true;
    }

    void ChangeUserMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ChangeUser(data.ComboBox["p"]);
        ChangeUser(data.ComboBox["c"]);
    }

    void ChangeUser(ElementComboBox elementComboBox)
    {
        for (int i = 0; i < elementComboBox.Data.Count(); i++)
        {
            if (elementComboBox.Data[i][1].Contains(":\\Users\\"))
            {
                string[] directory = elementComboBox.Data[i][1].Split('\\');
                for (int f = 0; f < directory.Length; f++)
                {
                    if (directory[f] == "Users")
                    {
                        directory[f + 1] = Environment.UserName;
                        break;
                    }
                }
                elementComboBox.Data[i][1] = String.Join("\\", directory);
            }
        }
    }

    void SetAsDefault_Click(object sender, RoutedEventArgs e)
    {
        if (!FileAssociation.IsRunAsAdmin())
        {
            ProcessStartInfo proc = new()
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = System.Reflection.Assembly.GetExecutingAssembly().Location,
                Verb = "runas"
            };
            proc.Arguments += "FileAssociation";
            try
            {
                Process.Start(proc);
            }
            catch
            {
                MessageBox.Show("Мы не можем это сделать на вашем устройстве, обновите ОС", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        else
        {
            FileAssociation.Associate("WordKiller", null);
        }
    }

    void RemoveAsDefault_Click(object sender, RoutedEventArgs e)
    {
        if (!FileAssociation.IsRunAsAdmin())
        {
            ProcessStartInfo proc = new()
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = System.Reflection.Assembly.GetExecutingAssembly().Location,
                Verb = "runas"
            };
            proc.Arguments += "RemoveFileAssociation";
            try
            {
                Process.Start(proc);
            }
            catch
            {
                MessageBox.Show("Мы не можем это сделать на вашем устройстве, обновите ОС", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        else
        {
            FileAssociation.Remove();
        }
    }

    void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (TextMI.IsChecked)
        {
            DownTextUpdate();
        }
    }

    void MouseEnterTypeButton(object sender, MouseEventArgs e)
    {
        Button pb = (Button)sender;
        string name = pb.Name.ToLower();
        int index = Regex.Matches(new TextRange(richTextBox.Document.ContentStart, richTextBox.CaretPosition).Text, Config.AddSpecialLeft(name)).Count;
        if (pb.Visibility == Visibility.Visible && index < data.ComboBox[name].Form.Items.Count && pb.IsMouseOver)
        {
            string str = "Вставить " + data.ComboBox[name].Form.Items[index] + " в " + cursorLocationTB.Text;
            if (Regex.Matches(new TextRange(richTextBox.CaretPosition, richTextBox.CaretPosition.DocumentEnd).Text, Config.AddSpecialLeft(name)).Count > 0)
            {
                str += ", последующие сместить";
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
        string str = string.Empty;
        foreach (char ch in GetText(richTextBox))
        {
            if (ch == '\n')
            {
                break;
            }
            str += ch;
        }
        return str;
    }

    void ImageUpdate()
    {
        string str = TypeRichBox();
        singlePB.Visibility = Visibility.Visible;
        if (str == Config.AddSpecialBoth("h1"))
        {
            dragDropImage.Visibility = Visibility.Collapsed;
            mainImage.Visibility = Visibility.Collapsed;
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
                DrawText("РАЗДЕЛ");
            }
        }
        else if (str == Config.AddSpecialBoth("h2"))
        {
            dragDropImage.Visibility = Visibility.Collapsed;
            mainImage.Visibility = Visibility.Collapsed;
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
                DrawText("Подраздел");
            }
        }
        else if (str == Config.AddSpecialBoth("l"))
        {
            dragDropImage.Visibility = Visibility.Collapsed;
            mainImage.Visibility = Visibility.Collapsed;
            DrawText("Список");
        }
        else if (str == Config.AddSpecialBoth("t"))
        {
            dragDropImage.Visibility = Visibility.Collapsed;
            mainImage.Visibility = Visibility.Collapsed;
            DrawText("Таблица");
        }
        else if (str == Config.AddSpecialBoth("p"))
        {
            dragDropImage.Visibility = Visibility.Collapsed;
            mainImage.Visibility = Visibility.Visible;
            if (pComboBox.SelectedIndex == -1)
            {
                try
                {
                    string path = SplitMainText();
                    if (File.Exists(path))
                    {
                        ShowImage(path);
                    }
                    else
                    {
                        ShowImage("Не найден");
                    }
                }
                catch
                {
                    ShowIconPicture("Не указан");
                }
            }
            else
            {
                string path = data.ComboBox["p"].Data[pComboBox.SelectedIndex][1];
                if (File.Exists(path))
                {
                    ShowImage(path);
                }
                else
                {
                    ShowIconPicture("Не найден");
                }
            }
        }
        else if (str == Config.AddSpecialBoth("c"))
        {
            dragDropImage.Visibility = Visibility.Collapsed;
            mainImage.Visibility = Visibility.Visible;
            if (cComboBox.SelectedIndex == -1)
            {
                try
                {
                    string path = SplitMainText();
                    if (File.Exists(path))
                    {
                        ShowCode(path.Split('\\').Last());
                    }
                    else
                    {
                        ShowCode("Не найден");
                    }
                }
                catch
                {
                    ShowCode("Не указан");
                }
            }
            else
            {
                string path = data.ComboBox["c"].Data[cComboBox.SelectedIndex][1];
                if (File.Exists(path))
                {
                    ShowCode(path.Split('\\').Last());
                }
                else
                {
                    ShowCode("Не найден");
                }
            }
        }
        else
        {
            ShowDragDrop();
        }
    }

    void ShowDragDrop()
    {
        mainImage.Visibility = Visibility.Collapsed;
        dragDropImage.Visibility = Visibility.Visible;
        mainImage.Margin = new Thickness(0, 0, 0, 0);
        var uriSource = new Uri(@"Resources/DragNDrop.png", UriKind.Relative);
        mainImage.Source = new BitmapImage(uriSource);
    }

    void ShowIconPicture(string text)
    {
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

    void DrawText(string text)
    {
        mainText.Text = text;
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
        if (str != Config.AddSpecialBoth("h1") && str != Config.AddSpecialBoth("h2") && str != Config.AddSpecialBoth("l") && str != Config.AddSpecialBoth("t"))
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            if (e.GetPosition(pictureBox).X < pictureBox.RenderSize.Width / 2)
            {
                codeRight.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
                pictureLeft.Fill = new SolidColorBrush(Colors.Black);
                textLeft.Foreground = new SolidColorBrush(Colors.White);
                textRight.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (e.GetPosition(pictureBox).X > pictureBox.RenderSize.Width / 2)
            {
                codeRight.Fill = new SolidColorBrush(Colors.Black);
                textLeft.Foreground = new SolidColorBrush(Colors.Black);
                pictureLeft.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
                textRight.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                codeRight.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
                pictureLeft.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
                textLeft.Foreground = new SolidColorBrush(Colors.Black);
                textRight.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }

    void PictureBox_DragEnter(object sender, DragEventArgs e)
    {
        string str = TypeRichBox();
        if (str != Config.AddSpecialBoth("h1") && str != Config.AddSpecialBoth("h2") && str != Config.AddSpecialBoth("l") && str != Config.AddSpecialBoth("t"))
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            singlePB.Visibility = Visibility.Collapsed;
        }
    }

    void PictureBox_DragLeave(object sender, DragEventArgs e)
    {
        string str = TypeRichBox();
        if (str != Config.AddSpecialBoth("h1") && str != Config.AddSpecialBoth("h2") && str != Config.AddSpecialBoth("l") && str != Config.AddSpecialBoth("t"))
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
            if (e.GetPosition(pictureBox).X < 0 || e.GetPosition(pictureBox).X > pictureBox.RenderSize.Width || e.GetPosition(pictureBox).Y < 0 || e.GetPosition(pictureBox).Y > pictureBox.RenderSize.Height)
            {
                singlePB.Visibility = Visibility.Visible;
                codeRight.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
                pictureLeft.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
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
        if (str != Config.AddSpecialBoth("h1") && str != Config.AddSpecialBoth("h2") && str != Config.AddSpecialBoth("l") && str != Config.AddSpecialBoth("t"))
        {
            singlePB.Visibility = Visibility.Collapsed;
            codeRight.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
            pictureLeft.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
            textLeft.Foreground = new SolidColorBrush(Colors.Black);
            textRight.Foreground = new SolidColorBrush(Colors.Black);
        }
    }

    void Win_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
        string str = TypeRichBox();
        if (str != Config.AddSpecialBoth("h1") && str != Config.AddSpecialBoth("h2") && str != Config.AddSpecialBoth("l") && str != Config.AddSpecialBoth("t"))
        {
            singlePB.Visibility = Visibility.Collapsed;
            codeRight.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
            pictureLeft.Fill = new SolidColorBrush(Color.FromArgb(255, 74, 118, 168));
            textLeft.Foreground = new SolidColorBrush(Colors.Black);
            textRight.Foreground = new SolidColorBrush(Colors.Black);
        }
    }

    void Win_DragLeave(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
        string str = TypeRichBox();
        if (str != Config.AddSpecialBoth("h1") && str != Config.AddSpecialBoth("h2") && str != Config.AddSpecialBoth("l") && str != Config.AddSpecialBoth("t"))
        {
            singlePB.Visibility = Visibility.Visible;
            ImageUpdate();
        }
    }

    void PictureBox_Drop(object sender, DragEventArgs e)
    {
        string str = TypeRichBox();
        if (str != Config.AddSpecialBoth("h1") && str != Config.AddSpecialBoth("h2") && str != Config.AddSpecialBoth("l") && str != Config.AddSpecialBoth("t"))
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string path = (data as string[])[0];
                if (path.Length > 0)
                {
                    string nameFile = path.Split('\\').Last();
                    nameFile = nameFile.Substring(0, nameFile.LastIndexOf('.'));
                    if (e.GetPosition(pictureBox).X < pictureBox.RenderSize.Width / 2)
                    {
                        SetText(Config.AddSpecialBoth("p") + "\n" + nameFile + "\n" + Config.AddSpecialBoth(Config.content) + "\n" + path);
                    }
                    else
                    {
                        SetText(Config.AddSpecialBoth("c") + "\n" + nameFile + "\n" + Config.AddSpecialBoth(Config.content) + "\n" + path);
                    }
                }
            }
        }
        ImageUpdate();
    }

    void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        UnselectComboBoxes();
    }

    void OpenSubjectTracker(object sender, RoutedEventArgs e)
    {

    }

    void CapsLockFix_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox a = (TextBox)sender;
        if (!string.IsNullOrEmpty(a.Text))
        {
            a.Text = a.Text.Substring(0, 1).ToUpper() + a.Text.Substring(1).ToLower();
        }
    }

    void SyntaxChecking_Click(object sender, RoutedEventArgs e)
    {
        if (SyntaxChecking.IsChecked)
        {
            richTextBox.SpellCheck.IsEnabled = true;
        }
        else
        {
            richTextBox.SpellCheck.IsEnabled = false;
        }
    }

    void elementCB_update()
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
            string str = GetText(richTextBox).Substring(start, end - start);

            R2_changeText(str);
        }
    }
    void elementCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        elementCB_update();
    }

    void SwitchRichTextBoxes(object sender, RoutedEventArgs e)
    {
        Visibility tmp = richTextBox.Visibility;
        richTextBox.Visibility = richTextBox2.Visibility;
        richTextBox2.Visibility = tmp;
    }

    int FindParagraphStart(string paragraphType)
    {
        if (paragraphType == "До разделов")
        {
            return 0;
        }

        string searchingFor = Config.specialBefore + paragraphType + "\n";

        string str = GetText(richTextBox);

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
        string str = GetText(richTextBox);

        if (paragraphType == "До разделов")
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
        int cursorPosSave = GetCaretIndex(richTextBox2);

        string special = elementCB.SelectedValue.ToString().Split(':')[0];

        int start = FindParagraphStart(special);
        int end = FindParagraphEnd(special);

        string str = GetText(richTextBox);
        string before = str.Substring(0, start);
        string after = str.Substring(end);
        string str2 = GetText(richTextBox2);

        str = before + str2 + after;

        richTextBox.Document.Blocks.Clear();
        richTextBox.Document.Blocks.Add(new Paragraph(new Run(str)));

        elementCB_update();

        elementCB.SelectedValue = selectedValueSave;
        SetCaret(richTextBox2, cursorPosSave);
    }

    void RichTextBox2_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (GetLineAtCursor(richTextBox2).Contains(Config.specialBefore) || GetLineAtCursor(richTextBox2).Contains(Config.specialAfter))
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
}
