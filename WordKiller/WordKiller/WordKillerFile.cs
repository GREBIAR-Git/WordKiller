using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public void Save(ViewModel data, DataComboBox dataComboBox, TablesData tableData)
        {
            if (!string.IsNullOrEmpty(savePath))
            {
                SaveFile(savePath, data, dataComboBox, tableData);
            }
            else
            {
                SaveAs(data, dataComboBox, tableData);
            }
        }

        public bool SaveAs(ViewModel data, DataComboBox dataComboBox, TablesData tableData)
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
                SaveFile(savePath, data, dataComboBox, tableData);
                return true;
            }
            return false;
        }

        void SaveFile(string nameFile, ViewModel data, DataComboBox dataComboBox, TablesData tableData)
        {
            FileStream fileStream = File.Open(nameFile, FileMode.Create);
            StreamWriter output = new(fileStream);
            StringBuilder save = new StringBuilder();

            foreach (Control item in typeDocument)
            {
                if (item.GetType() == typeof(MenuItem) && ((MenuItem)item).IsChecked)
                {
                    save.AppendLine(Config.AddSpecialBoth("Menu") + item.Name + "!" + data.NumberHeading + "!" + data.PageNumbers + "!" + data.TableOfContents);
                    break;
                }
            }
            save.AppendLine(Config.AddSpecialRight("facultyComboBox") + data.TitleFaculty);
            save.AppendLine(Config.AddSpecialRight("numberTextBox") + data.TitleNumber);
            save.AppendLine(Config.AddSpecialRight("themeTextBox") + data.TitleTheme);
            save.AppendLine(Config.AddSpecialRight("disciplineTextBox") + data.TitleDiscipline);
            save.AppendLine(Config.AddSpecialRight("professorComboBox") + data.TitleProfessor);
            save.AppendLine(Config.AddSpecialRight("yearTextBox") + data.TitleYear);
            save.AppendLine(Config.AddSpecialRight("shifrTextBox") + data.TitleShifr);
            save.AppendLine(Config.AddSpecialRight("studentsTextBox") + data.TitleStudents);
            foreach (KeyValuePair<string, ElementComboBox> comboBox in dataComboBox.ComboBox)
            {
                save.Append(SaveCombobox(comboBox.Value, comboBox.Key));
            }
            save.AppendLine(Config.AddSpecialBoth("TextStart"));
            if (data.TextOpen)
            {
                save.AppendLine(RTBox.GetText(richTextBox));
            }
            else
            {
                save.AppendLine(dataComboBox.Text);
            }
            save.AppendLine(Config.AddSpecialBoth("TextEnd"));
            save.AppendLine(Config.AddSpecialBoth("TableStart"));
            for (int j = 0; j < tableData.collection.Count - 1; j++)
            {
                save.Append(tableData.collection[j].Rows + " " + tableData.collection[j].Columns + " ");
                for (int i = 0; i < tableData.collection[j].Rows; i++)
                {
                    for (int f = 0; f < tableData.collection[j].Columns; f++)
                    {
                        save.Append(tableData.collection[j].DataTable[i, f] + " ");
                    }
                }
                save[save.Length - 1] = '\r';
                save.Append('\n');

            }
            save.Append(Config.AddSpecialBoth("TableEnd"));
            if (Properties.Settings.Default.NumberEncoding == 0)
            {
                output.Write("0\r\n" + save);
            }
            else if (Properties.Settings.Default.NumberEncoding == 1)
            {
                output.Write("1\r\n" + Encryption.MegaConvertE(save.ToString()));
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

                comboBoxSave += name + "ComboBox" + Config.AddSpecialBoth(comboBox.Form.Items[i].ToString()) + dataCombobox + "\r\n";
            }
            return comboBoxSave;
        }

        public void OpenFile(string fileName, ViewModel data, ref DataComboBox dataComboBox, ref TablesData tablesData)
        {
            dataComboBox.Text = string.Empty;
            savePath = fileName;
            ClearGlobal(ref dataComboBox, ref tablesData);
            tablesData.InitTable();
            FileStream file = new(fileName, FileMode.Open);
            StreamReader reader = new(file);
            try
            {
                string text = reader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (text[0] == '1' && text[1] == '\r' && text[2] == '\n')
                    {
                        text = Encryption.MegaConvertD(text[3..]);
                    }
                    else if (text[0] == '0' && text[1] == '\r' && text[2] == '\n')
                    {
                        text = text[3..];
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
                    bool readingTables = false;
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
                            data.PageNumbers = bool.Parse(menuItem[2]);
                            data.TableOfContents = bool.Parse(menuItem[3]);
                        }
                        else if (line.StartsWith(Config.AddSpecialBoth("TextStart")))
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
                        else if (line.StartsWith(Config.AddSpecialBoth("TableStart")))
                        {
                            readingTables = true;
                        }
                        else if (readingTables)
                        {
                            if (line.StartsWith(Config.AddSpecialBoth("TableEnd")))
                            {
                                readingText = false;
                            }
                            else
                            {
                                tablesData.AddTable();
                                string[] dataTable = line.Split(" ");
                                tablesData.CurrentData.Rows = int.Parse(dataTable[0]);
                                tablesData.CurrentData.Columns = int.Parse(dataTable[1]);


                                for (int i = 0; i < tablesData.CurrentData.Rows; i++)
                                {
                                    for (int f = 0; f < tablesData.CurrentData.Columns; f++)
                                    {
                                        tablesData.CurrentData.DataTable[i, f] = dataTable[2 + i * tablesData.CurrentData.Columns + f];
                                    }
                                }
                            }
                            readingTables = true;
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
            }
            catch
            {
                MessageBox.Show(MainWindow.FindResourse("Error7"), MainWindow.FindResourse("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
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

        public void NewFile(ViewModel data, ref DataComboBox dataComboBox, ref int menuLeftIndex, TablesData tableData)
        {
            NeedSave(data, dataComboBox, tableData);
            data.WinTitle = "WordKiller";
            ClearGlobal(ref dataComboBox, ref tableData);
            menuLeftIndex = 1;
            richTextBox.Document.Blocks.Clear();
            dataComboBox.Text = string.Empty;
            foreach (UIElement control in titleElement)
            {
                if (control.GetType().ToString() == "System.Windows.Forms.TextBox")
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = string.Empty;
                }
            }
        }

        public bool NeedSave(ViewModel data, DataComboBox dataComboBox, TablesData tableData)
        {
            MessageBoxResult result = MessageBox.Show(MainWindow.FindResourse("Question1"), MainWindow.FindResourse("Question1"), MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
            if (result == MessageBoxResult.Yes)
            {
                Save(data, dataComboBox, tableData);
                return true;
            }
            return false;
        }

        void ClearGlobal(ref DataComboBox data, ref TablesData tablesData)
        {
            tablesData.collection.Clear();
            data.AllClear();
            for (int i = elementPanel.ColumnDefinitions.Count - 1; i < elementPanel.Children.Count - 1; i++)
            {
                ComboBox cmbBox = (ComboBox)elementPanel.Children[i];
                cmbBox.Items.Clear();
            }
        }
    }
}