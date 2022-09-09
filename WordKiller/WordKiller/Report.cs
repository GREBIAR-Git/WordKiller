using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Windows;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;

namespace WordKiller;

class Report
{
    public void Create(DataComboBox mainPart, bool numbering, bool tableOfContentsOn, int fromNumbering, bool numberHeading, TypeDocument typeDocument, string[] dataTitle)
    {
        using (WordprocessingDocument doc =
            WordprocessingDocument.Create("C:\\Users\\Nikita\\Desktop\\1.docx",
            WordprocessingDocumentType.Document, true))
        {
            MainDocumentPart main = doc.AddMainDocumentPart();
            main.Document = new Document();
            Body body = main.Document.AppendChild(new Body());
            SectionProperties props = new();
            body.AppendChild(props);

            InitStyles(doc);

            PageSetup(props);
            try
            {
                if (typeDocument != TypeDocument.DefaultDocument)
                {
                    dataTitle[dataTitle.Length - 1] = SpaceForYear(dataTitle[dataTitle.Length - 1], '_');
                    TitlePart(doc, typeDocument, dataTitle);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка в формировании титульной страницы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
            }

            TableOfContents(tableOfContentsOn, mainPart);
            try
            {
                ProcessSpecials(mainPart);
                MainPart(mainPart, numbering, fromNumbering, numberHeading);
            }
            catch
            {
                MessageBox.Show("Ошибка в формировании основного текста документа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
            }
            /*if (exportPDF)
            {
                SaveFileDialog saveFileDialog = new()
                {
                    OverwritePrompt = true,
                    Filter = "PDF | *.pdf",
                    FileName = "1"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    doc.ExportAsFixedFormat(saveFileDialog.FileName, WdExportFormat.wdExportFormatPDF);
                }
            }*/
        }
    }

    void InitStyles(WordprocessingDocument doc)
    {
        StyleDefinitionsPart styleDefinitions = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();

        var styles = new Styles();
        styles.Save(styleDefinitions);
        styles = styleDefinitions.Styles;

        // EmptyLines
        var style = new Style()
        {
            Type = StyleValues.Paragraph,
            StyleId = "EmptyLines",
            CustomStyle = true,
            Default = false
        };
        style.Append(new StyleName() { Val = "EmptyLines" });

        var styleRunProperties = new StyleRunProperties();
        styleRunProperties.Append(new RunFonts() { Ascii = "Times New Roman" });
        styleRunProperties.Append(new FontSize() { Val = "28" });

        ParagraphProperties paragraphProperties = new();
        paragraphProperties.AddChild(new Justification()
        {
            Val = JustificationValues.Center
        });

        paragraphProperties.AddChild(new SpacingBetweenLines()
        {
            After = (0 * 20).ToString(),
            Before = (0 * 20).ToString(),
            Line = (1 * 240).ToString(),
            LineRule = LineSpacingRuleValues.Auto
        });

        paragraphProperties.AddChild(new Indentation()
        {
            Left = ((int)(0 * 567)).ToString(),
            Right = ((int)(0 * 567)).ToString(),
            FirstLine = "0"
        });

        style.Append(styleRunProperties);
        style.Append(paragraphProperties);

        styles.Append(style);
    }

    void MainPart(DataComboBox content, bool numbering, int fromNumbering, bool numberHeading)
    {

    }

    void ProcessSpecials(DataComboBox data)
    {
        foreach (string key in data.ComboBox.Keys)
        {
            data.Text = RemoveENDLs(data.Text, Config.specialBefore + key);
        }
    }

    string RemoveENDLs(string text, string symbol)
    {
        string[] str = text.Split(new string[] { symbol }, StringSplitOptions.None);
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i].Length > 0)
            {
                if (str[i][0] == '\n')
                {
                    str[i] = str[i].Remove(0, 1);
                }
                if (str[i].Length > 0 && str[i][str[i].Length - 1] == '\n')
                {
                    str[i] = str[i].Remove(str[i].Length - 1, 1);
                }
            }
        }
        return String.Join(symbol, str);
    }

    void TableOfContents(bool on, DataComboBox dataMainPart)
    {
        if (on)
        {
            /*string text = "Содержание";
            WriteTextWord(text);
            word.Font.AllCaps = 0;
            word.Font.Size = 14;
            word.Font.Bold = 1;
            word.Font.ColorIndex = 0;
            word.Paragraphs.Space15();
            word.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            PageBreak();*/
        }
    }

    void PageSetup(SectionProperties props, float top = 2, float right = 1.5f, float bot = 2, float left = 3)
    {
        props.AddChild(new PageMargin()
        {
            Top = (int)(top * 567f),
            Right = Convert.ToUInt32(right * 567f),
            Bottom = (int)(bot * 567f),
            Left = Convert.ToUInt32(left * 567f)
        });

        props.AppendChild(new PageSize()
        {
            Width = 11907,
            Height = 16839
        });

    }

    void TitlePart(WordprocessingDocument doc, TypeDocument typeDocument, string[] dataTitle)
    {
        Ministry(doc, dataTitle[0]);
        switch (typeDocument)
        {
            case TypeDocument.LaboratoryWork:
                LabPra(doc, "лабораторной", dataTitle);
                break;
            case TypeDocument.PracticalWork:
                LabPra(doc, "практической", dataTitle);
                break;
            case TypeDocument.Coursework:
                Coursework(doc, dataTitle);
                break;
            case TypeDocument.ControlWork:
                ControlWork(doc, dataTitle);
                break;
            case TypeDocument.Referat:
                Referat(doc, dataTitle);
                break;
            case TypeDocument.GraduateWork:
                break;
            case TypeDocument.VKR:
                break;
        }
        Orel(doc, dataTitle[dataTitle.Length - 1]);
    }

    void Referat(WordprocessingDocument doc, string[] dataTitle)
    {
        EmptyLines(doc, 1);

        string text = "Реферат по дисциплине: «" + dataTitle[3] + "»";
        Text(doc, text, justify: JustificationValues.Center);
        text = "Тема: «" + dataTitle[2] + "»";
        Text(doc, text, justify: JustificationValues.Center);
        EmptyLines(doc, 16);

        text = "Выполнил: студент группы 92ПГ";
        Text(doc, text, justify: JustificationValues.Right);
        text = dataTitle[1];
        Text(doc, text, justify: JustificationValues.Right);
        text = "Проверил: ст. преподаватель кафедры физического воспитания";
        Text(doc, text, justify: JustificationValues.Right);
        text = dataTitle[4];
        Text(doc, text, justify: JustificationValues.Right);

        EmptyLines(doc, 6);
    }

    void ControlWork(WordprocessingDocument doc, string[] dataTitle)
    {
        string text = "Контрольная работа";
        Text(doc, text, 16, JustificationValues.Center, true);

        text = "по дисциплине: «" + dataTitle[2] + "»";
        Text(doc, text, justify: JustificationValues.Center);

        EmptyLines(doc, 10);

        text = "Выполнил: " + dataTitle[1];
        Text(doc, text);
        text = "Институт приборостроения, автоматизации и информационных технологий";
        Text(doc, text);
        text = "Направление: 09.03.04 «Программная инженерия»";
        Text(doc, text);
        text = "Группа: 92ПГ";
        Text(doc, text);

        text = "Проверил: " + dataTitle[3];
        Text(doc, text, after: 10);

        EmptyLines(doc, 1);

        text = "Отметка о зачёте: ";
        Text(doc, text);

        text = "Дата: «____» __________ " + dataTitle[4] + "г.";
        Text(doc, text, justify: JustificationValues.Right);

        EmptyLines(doc, 9);
    }

    void Coursework(WordprocessingDocument doc, string[] dataTitle)
    {
        string text = "Работа допущена к защите";
        Text(doc, text, multiplier: 1.5f, left: 9.5f);
        text = "______________Руководитель";
        Text(doc, text, multiplier: 1.5f, left: 9.5f);
        text = "«____»_____________" + dataTitle[dataTitle.Length - 1] + "г.";
        Text(doc, text, multiplier: 1.5f, left: 9.5f);

        EmptyLines(doc, 3);

        text = "КУРСОВАЯ РАБОТА";
        Text(doc, text, justify: JustificationValues.Center, bold: true);

        EmptyLines(doc, 1);

        text = "по дисциплине: «" + dataTitle[4] + "»";
        Text(doc, text, multiplier: 2);

        text = "на тему: «" + dataTitle[3] + "»";
        Text(doc, text, multiplier: 1.5f);

        EmptyLines(doc, 2);

        text = "Студент _________________" + dataTitle[1];
        Text(doc, text, multiplier: 1.5f);
        text = "Шифр " + dataTitle[2];
        Text(doc, text, multiplier: 1.5f);
        text = "Институт приборостроения, автоматизации и информационных технологий";
        Text(doc, text, multiplier: 1.5f);
        text = "Направление: 09.03.04 «Программная инженерия»";
        Text(doc, text, multiplier: 1.5f);
        text = "Группа: 92ПГ";
        Text(doc, text, multiplier: 1.5f);

        text = "Руководитель __________________" + dataTitle[5];
        Text(doc, text, multiplier: 1.5f, after: 12);

        text = "Оценка: «________________»               Дата ______________";
        Text(doc, text);

        EmptyLines(doc, 2);
    }

    void Orel(WordprocessingDocument doc, string year)
    {
        string text = "Орел, " + year;
        Run run = Text(doc, text, justify: JustificationValues.Center);
        PageBreak(run);
    }

    void PageBreak(Run run)
    {
        run.AppendChild(new Break() { Type = BreakValues.Page });
    }

    void LabPra(WordprocessingDocument doc, string type, string[] dataTitle)
    {
        string text = "ОТЧЁТ";
        Text(doc, text, 16, JustificationValues.Center, true);
        text = "По " + type + " работе №" + dataTitle[1];
        Text(doc, text, 16, JustificationValues.Center, after: 10);
        text = "на тему: «" + dataTitle[2] + "»";
        Text(doc, text, justify: JustificationValues.Center);
        text = "по дисциплине: «" + dataTitle[3] + "»";
        Text(doc, text, justify: JustificationValues.Center);
        EmptyLines(doc, 8);
        text = "Выполнили: Музалевский Н.С., Аллянов М.Д.";
        Text(doc, text);
        text = "Институт приборостроения, автоматизации и информационных технологий";
        Text(doc, text);
        text = "Направление: 09.03.04 «Программная инженерия»";
        Text(doc, text);
        text = "Группа: 92ПГ";
        Text(doc, text);

        text = "Проверил: " + dataTitle[4];
        Text(doc, text, after: 10);
        EmptyLines(doc, 1);

        text = "Отметка о зачёте: ";
        Text(doc, text);

        text = "Дата: «____» __________ " + dataTitle[5] + "г.";
        Text(doc, text, justify: JustificationValues.Right);

        EmptyLines(doc, 8);
    }

    void Ministry(WordprocessingDocument doc, string faculty)
    {
        string text = "МИНИСТЕРСТВО НАУКИ И ВЫСШЕГО ОБРАЗОВАНИЯ";
        Text(doc, text, justify: JustificationValues.Center);
        text = "РОССИЙСКОЙ ФЕДЕРАЦИИ";
        Text(doc, text, justify: JustificationValues.Center);
        text = "ФЕДЕРАЛЬНОЕ ГОСУДАРСТВЕННОЕ БЮДЖЕТНОЕ";
        Text(doc, text, justify: JustificationValues.Center);
        text = "ОБРАЗОВАТЕЛЬНОЕ УЧРЕЖДЕНИЕ ВЫСШЕГО ОБРАЗОВАНИЯ";
        Text(doc, text, justify: JustificationValues.Center);
        text = "«ОРЛОВСКИЙ ГОСУДАРСТВЕННЫЙ УНИВЕРСИТЕТ";
        Text(doc, text, justify: JustificationValues.Center);
        text = "ИМЕНИ И.С.ТУРГЕНЕВА»";
        Text(doc, text, justify: JustificationValues.Center);
        EmptyLines(doc, 2);

        text = "Кафедра " + faculty;
        Text(doc, text, justify: JustificationValues.Center);
        EmptyLines(doc, 3);
    }

    void EmptyLines(WordprocessingDocument doc, int number)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;
        Paragraph paragraph = body.AppendChild(new Paragraph());
        paragraph.AppendChild(new ParagraphProperties());

        paragraph.ParagraphProperties = new ParagraphProperties(
            new ParagraphStyleId() { Val = "EmptyLines" });

        Run run = paragraph.AppendChild(new Run());

        for (int i = 0; i < number - 1; i++)
        {
            run.AppendChild(new CarriageReturn());
        }
    }

    string SpaceForYear(string year, char spaceCharacter)
    {
        for (int i = 0; i < 4 - year.Length; i++)
        {
            year += spaceCharacter;
        }
        return year;
    }

    Run Text(WordprocessingDocument doc, string text, int size = 14,
        JustificationValues justify = JustificationValues.Left, bool bold = false,
        int before = 0, int after = 0, float multiplier = 1, float left = 0, float right = 0)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;
        Paragraph paragraph = body.AppendChild(new Paragraph());

        paragraph.AppendChild(new ParagraphProperties());

        paragraph.ParagraphProperties.AddChild(new Justification()
        {
            Val = justify
        });

        paragraph.ParagraphProperties.AddChild(new SpacingBetweenLines()
        {
            After = (after * 20).ToString(),
            Before = (before * 20).ToString(),
            Line = (multiplier * 240).ToString(),
            LineRule = LineSpacingRuleValues.Auto
        });

        paragraph.ParagraphProperties.AddChild(new Indentation()
        {
            Left = ((int)(left * 567)).ToString(),
            Right = ((int)(right * 567)).ToString(),
            FirstLine = "0"
        });

        Run run = paragraph.AppendChild(new Run());

        run.AppendChild(new Text(text));
        run.PrependChild(new RunProperties());

        if (bold)
        {
            run.RunProperties.AddChild(new Bold());
        }

        run.RunProperties.AddChild(new RunFonts()
        {
            Ascii = "Times New Roman",
            HighAnsi = "Times New Roman"
        });

        run.RunProperties.AddChild(new FontSize()
        {
            Val = (size * 2).ToString()
        });
        return run;
    }
}
