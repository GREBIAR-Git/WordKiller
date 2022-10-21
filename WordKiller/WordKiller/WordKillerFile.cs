using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WordKiller
{
    class WordKillerFile
    {
        string? savePath;

        readonly SaveLogo saveLogo;

        readonly ItemCollection typeDocument;

        readonly UIElementCollection titleElement;

        readonly Grid elementPanel;

        readonly MainWindow mainWindow;

        readonly RichTextBox richTextBox;
        public WordKillerFile(Image logo, ItemCollection typeDocument, UIElementCollection titleElement, Grid elementPanel, RichTextBox richTextBox, MainWindow mainWindow)
        {
            saveLogo = new(logo);
            this.typeDocument = typeDocument;
            this.titleElement = titleElement;
            this.elementPanel = elementPanel;
            this.richTextBox = richTextBox;
            this.mainWindow = mainWindow;
        }

        public bool SavePathExists()
        {
            return !string.IsNullOrEmpty(savePath);
        }

        public void Save(ViewModel data, DataComboBox dataComboBox)
        {
            if (!string.IsNullOrEmpty(savePath))
            {
                SaveFile(savePath, data, dataComboBox);
            }
            else
            {
                SaveAs(data, dataComboBox);
            }
        }

        public bool SaveAs(ViewModel data, DataComboBox dataComboBox)
        {
            SaveFileDialog saveFileDialog = new()
            {
                OverwritePrompt = true,
                Filter = "wordkiller file (*" + Properties.Settings.Default.Extension + ")|*" + Properties.Settings.Default.Extension + "|All files (*.*)|*.*",
                FileName = "1"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                savePath = saveFileDialog.FileName;
                SaveFile(savePath, data, dataComboBox);
                return true;
            }
            return false;
        }

        void SaveFile(string nameFile, ViewModel data, DataComboBox dataComboBox)
        {
            FileStream fileStream = System.IO.File.Open(nameFile, FileMode.Create);
            StreamWriter output = new(fileStream);
            string save = string.Empty;

            foreach (Control item in typeDocument)
            {
                if (item.GetType() == typeof(MenuItem) && ((MenuItem)item).IsChecked)
                {
                    save += Config.AddSpecialBoth("Menu") + item.Name.ToString() + "!" + data.NumberHeading.ToString() + "\n";
                }
            }
            save += Config.AddSpecialRight("facultyComboBox") + data.TitleFaculty + "\n";
            save += Config.AddSpecialRight("numberTextBox") + data.TitleNumber + "\n";
            save += Config.AddSpecialRight("themeTextBox") + data.TitleTheme + "\n";
            save += Config.AddSpecialRight("disciplineTextBox") + data.TitleDiscipline + "\n";
            save += Config.AddSpecialRight("professorComboBox") + data.TitleProfessor + "\n";
            save += Config.AddSpecialRight("yearTextBox") + data.TitleYear + "\n";
            save += Config.AddSpecialRight("shifrTextBox") + data.TitleShifr + "\n";
            save += Config.AddSpecialRight("studentsTextBox") + data.TitleStudents + "\n";
            foreach (KeyValuePair<string, ElementComboBox> comboBox in dataComboBox.ComboBox)
            {
                save += SaveCombobox(comboBox.Value, comboBox.Key);
            }
            save += Config.AddSpecialBoth("TextStart") + "\n";
            if (data.TextOpen)
            {
                save += RTBox.GetText(richTextBox) + "\n";
            }
            else
            {
                save += dataComboBox.Text + "\n";
            }
            save += Config.AddSpecialBoth("TextEnd") + "\n";
            if (Properties.Settings.Default.NumberEncoding == 0)
            {
                output.Write("0\r\n" + save);
            }
            else if (Properties.Settings.Default.NumberEncoding == 1)
            {
                output.Write("1\r\n" + Encryption.MegaConvertE(save));
            }
            output.Close();
            data.WinTitle = Path.GetFileName(nameFile);
            saveLogo.Show();
        }

        static string SaveCombobox(ElementComboBox comboBox, string name)
        {
            string comboBoxSave = string.Empty;
            for (int i = 0; i < comboBox.Form.Items.Count; i++)
            {
                string dataCombobox = string.Empty;
                if (comboBox.Data[i][1].Contains('\n'))
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

        public void OpenFile(string fileName, ViewModel data, ref DataComboBox dataComboBox)
        {
            dataComboBox.Text = string.Empty;
            savePath = fileName;
            ClearGlobal(ref dataComboBox);
            FileStream file = new(fileName, FileMode.Open);
            StreamReader reader = new(file);
            try
            {
                string text = reader.ReadToEnd();
                if (text[0] == '1' && text[1] == '\r' && text[2] == '\n')
                {
                    text = Encryption.MegaConvertD(text[3..]);
                }
                else if (text[0] == '0' && text[1] == '\r' && text[2] == '\n')
                {
                    text = text[3..];
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
                        foreach (Control f in typeDocument)
                        {
                            if (f.GetType() == typeof(MenuItem) && f.Name == menuItem[0])
                            {
                                mainWindow.WorkChange((MenuItem)f);
                            }
                        }
                        if (menuItem[0] != "DefaultDocumentMI")
                        {
                            foreach (UIElement control in titleElement)
                            {
                                if (control.GetType().ToString() != "System.Windows.Forms.Label")
                                {
                                    controls.Add(control);
                                }
                            }
                        }
                        data.NumberHeading = bool.Parse(menuItem[1]);
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
                            dataComboBox.Text += line + "\n";
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
                            foreach (KeyValuePair<string, ElementComboBox> comboBox in dataComboBox.ComboBox)
                            {
                                if (variable_value[0].StartsWith(comboBox.Key + "ComboBox"))
                                {
                                    comboBox.Value.Form.Items.Add(variable_value[1]);
                                    string dataComboBoxComboBox = variable_value[2].Replace("!@!", "\n");
                                    string[] str = new string[] { variable_value[1], dataComboBoxComboBox };
                                    comboBox.Value.Data.Add(str);
                                    break;
                                }
                            }
                        }
                    }
                    data.WinTitle = Path.GetFileName(fileName);
                }
                if (dataComboBox.Text.Length > 0)
                {
                    dataComboBox.Text = dataComboBox.Text.Remove(dataComboBox.Text.Length - 1);
                }
            }
            catch
            {
                MessageBox.Show("Файл повреждён");
            }
            if (data.TextOpen)
            {
                richTextBox.Document.Blocks.Clear();
                richTextBox.Document.Blocks.Add(new Paragraph(new Run(dataComboBox.Text)));
                richTextBox.CaretPosition = richTextBox.CaretPosition.DocumentEnd;
            }
            reader.Close();
        }

        static bool LoadingOfTwo(string[] variable_value, UIElement control)
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

        public void NewFile(ViewModel data, ref DataComboBox dataComboBox, ref int menuLeftIndex)
        {
            NeedSave(data, dataComboBox);
            data.WinTitle = "WordKiller";
            ClearGlobal(ref dataComboBox);
            menuLeftIndex = 1;
            richTextBox.Document.Blocks.Clear();
            foreach (UIElement control in titleElement)
            {
                if (control.GetType().ToString() == "System.Windows.Forms.TextBox")
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = string.Empty;
                }
            }
        }

        public bool NeedSave(ViewModel data, DataComboBox dataComboBox)
        {
            MessageBoxResult result = MessageBox.Show("Нужно ли сохранить?", "Нужно ли сохранить?", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
            if (result == MessageBoxResult.Yes)
            {
                Save(data, dataComboBox);
                return true;
            }
            return false;
        }

        void ClearGlobal(ref DataComboBox data)
        {
            data.AllClear();
            for (int i = elementPanel.ColumnDefinitions.Count - 1; i < elementPanel.Children.Count - 1; i++)
            {
                ComboBox cmbBox = (ComboBox)elementPanel.Children[i];
                cmbBox.Items.Clear();
            }
        }
    }
}