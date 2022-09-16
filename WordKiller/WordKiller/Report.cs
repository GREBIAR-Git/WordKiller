using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;

namespace WordKiller;
class Report
{
    const short cm_to_pt = 567;

    const byte pt_to_halfpt = 2;

    const short pixel_to_EMU = 9525;

    public void Create(DataComboBox mainPart, bool numbering, bool tableOfContentsOn, int fromNumbering, bool numberHeading, TypeDocument typeDocument, string[] dataTitle)
    {
        using (WordprocessingDocument doc =
            WordprocessingDocument.Create("C:\\Users\\Nikita\\Desktop\\1.docx",
            WordprocessingDocumentType.Document, true))
        {
            MainDocumentPart main = doc.AddMainDocumentPart();
            main.Document = new Document();
            Body body = main.Document.AppendChild(new Body());

            //PageNumber pageNumber = main.Document.AppendChild(new PageNumber());

            InitStyles(doc);

            PageSetup(body);
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

            TableOfContents(doc, tableOfContentsOn, mainPart);
            //try
            //{
            ProcessSpecials(mainPart);
            MainPart(doc, mainPart, numbering, fromNumbering, numberHeading);


            /*}
            catch
            {
                MessageBox.Show("Ошибка в формировании основного текста документа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
            }*/
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

    string SpaceForYear(string year, char spaceCharacter)
    {
        for (int i = 0; i < 4 - year.Length; i++)
        {
            year += spaceCharacter;
        }
        return year;
    }

    void InitStyles(WordprocessingDocument doc)
    {
        StyleDefinitionsPart styleDefinitions = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();

        Styles styles = new Styles();
        styles.Save(styleDefinitions);
        styles = styleDefinitions.Styles;


        styles.Append(
            InitStyle("EmptyLines", justify: JustificationValues.Center));

        styles.Append(
            InitStyle("Simple", justify: JustificationValues.Both, multiplier: 1.5f, firstLine: 1.25f));

        styles.Append(
            InitStyle("H1", justify: JustificationValues.Center, bold: true, after: 8, multiplier: 1.5f, firstLine: 1.5f, caps: true));

        styles.Append(
            InitStyle("H2", justify: JustificationValues.Center, bold: true, after: 8, multiplier: 1.5f, firstLine: 1.5f));

        styles.Append(
            InitStyle("List", justify: JustificationValues.Both, multiplier: 1.5f, left: 1.25f, hanging: 0.63f));

        styles.Append(
            InitStyle("Picture", justify: JustificationValues.Center, after: 8, multiplier: 1.5f));

        styles.Append(
            InitStyle("Code", 12, JustificationValues.Left));
    }

    Style InitStyle(string name, int size = 14,
        JustificationValues justify = JustificationValues.Left, bool bold = false,
        int before = 0, int after = 0, float multiplier = 1, float left = 0, float right = 0, float firstLine = 0, bool caps = false, float hanging = 0)
    {
        var style = new Style()
        {
            Type = StyleValues.Paragraph,
            StyleId = name,
            CustomStyle = true,
            Default = false
        };
        style.Append(new StyleName()
        {
            Val = name
        });

        var styleRunProperties = new StyleRunProperties();
        styleRunProperties.Append(new RunFonts()
        {
            Ascii = "Times New Roman",
            HighAnsi = "Times New Roman"
        });
        styleRunProperties.Append(new FontSize()
        {
            Val = (size * pt_to_halfpt).ToString()
        });
        styleRunProperties.Append(new Caps()
        {
            Val = caps
        });
        if (bold)
        {
            styleRunProperties.AddChild(new Bold());
        }

        ParagraphProperties paragraphProperties = new();
        paragraphProperties.AddChild(new Justification()
        {
            Val = justify
        });

        paragraphProperties.AddChild(new SpacingBetweenLines()
        {
            After = (after * 20).ToString(),
            Before = (before * 20).ToString(),
            Line = (multiplier * 240).ToString(),
            LineRule = LineSpacingRuleValues.Auto
        });

        if (hanging == 0)
        {
            paragraphProperties.AddChild(new Indentation()
            {
                Left = ((int)(left * cm_to_pt)).ToString(),
                Right = ((int)(right * cm_to_pt)).ToString(),
                FirstLine = ((int)(firstLine * cm_to_pt)).ToString(),
            });
        }
        else
        {
            paragraphProperties.AddChild(new Indentation()
            {
                Left = ((int)((left + hanging) * cm_to_pt)).ToString(),
                Right = ((int)(right * cm_to_pt)).ToString(),
                Hanging = ((int)(hanging * cm_to_pt)).ToString(),
            });
        }



        style.Append(styleRunProperties);
        style.Append(paragraphProperties);
        return style;
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

    void TableOfContents(WordprocessingDocument doc, bool on, DataComboBox dataMainPart)
    {
        if (on)
        {
            Run run = Text(doc, "Содержание", justify: JustificationValues.Center, bold: true, multiplier: 1.5f);
            PageBreak(run);
        }
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

    void Orel(WordprocessingDocument doc, string year)
    {
        string text = "Орел, " + year;
        Run run = Text(doc, text, justify: JustificationValues.Center);
        PageBreak(run);
    }

    void MainPart(WordprocessingDocument doc, DataComboBox content, bool numbering, int fromNumbering, bool numberHeading)
    {
        if (content.Text != null)
        {
            ProcessContent(doc, content, numberHeading);
        }
    }

    void ProcessContent(WordprocessingDocument doc, DataComboBox content, bool number = true)
    {
        int h1 = 1;
        int h2 = 1;
        int h2all = 1;
        int l = 1;
        int p = 1;
        int t = 1;
        int c = 1;
        string def = string.Empty;
        for (int i = 0; i < content.Text.Length; i++)
        {
            if (content.Text[i] == Config.specialBefore)
            {
                if (def != string.Empty)
                {
                    Text(doc, def, "Simple");
                    def = string.Empty;
                }
                if (content.Text[i + 1] == 'h')
                {
                    if (content.Text[i + 2] == '1')
                    {
                        i += 2;
                        string text = string.Empty;
                        if (number)
                        {
                            text += h1.ToString() + " ";
                        }
                        text += ProcessSpecial(h1, "h1", content)[0];
                        Text(doc, text, "H1");
                        h1++;
                        h2 = 1;
                    }
                    else if (content.Text[i + 2] == '2')
                    {
                        i += 2;

                        string text = string.Empty;
                        if (number)
                        {
                            text += (h1 - 1).ToString() + "." + h2.ToString() + " ";
                        }

                        text += ProcessSpecial(h2all, "h2", content)[0];
                        Text(doc, text, "H2");
                        h2all++;
                        h2++;
                    }
                }
                else if (content.Text[i + 1] == 'l')
                {
                    i += 1;
                    string[] text = ProcessSpecial(l, "l", content);
                    List(doc, text[0]);
                    l++;
                }
                else if (content.Text[i + 1] == 'p')
                {
                    i += 1;
                    string[] text = ProcessSpecial(p, "p", content);
                    Picture(doc, text[0]);
                    Text(doc, "Рисунок " + p + " – " + text[1], "Picture");
                    p++;
                }
                else if (content.Text[i + 1] == 't')
                {
                    i += 1;
                    string[] text = ProcessSpecial(t, "t", content);
                    //Table(text[0]);
                    t++;
                }
                else if (content.Text[i + 1] == 'c')
                {
                    i += 1;
                    string[] text = ProcessSpecial(c, "c", content);
                    Text(doc, text[1], "H1");
                    FileStream file = new(text[0], FileMode.Open);
                    StreamReader reader = new(file);
                    string data = reader.ReadToEnd();
                    Text(doc, data, "Code");
                    c++;
                }
            }
            else
            {
                def += content.Text[i];
            }
        }
        if (def != string.Empty)
        {
            Text(doc, def, "Simple");
        }


    }

    void List(WordprocessingDocument doc, string items)
    {
        NumberingDefinitionsPart numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart;
        if (numberingPart == null)
        {
            numberingPart = doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("NumberingDefinitionsPart001");
            Numbering element = new Numbering();
            element.Save(numberingPart);
        }

        int abstractNumberId = numberingPart.Numbering.Elements<AbstractNum>().Count() + 1;
        Level abstractLevel = new Level(
            new NumberingFormat() { Val = NumberFormatValues.Decimal },
            new StartNumberingValue() { Val = 1 },
            new LevelText() { Val = "%1)" })
        { LevelIndex = 0 };
        AbstractNum abstractNum1 = new AbstractNum(abstractLevel) { AbstractNumberId = abstractNumberId };

        if (abstractNumberId == 1)
        {
            numberingPart.Numbering.Append(abstractNum1);
        }
        else
        {
            AbstractNum lastAbstractNum = numberingPart.Numbering.Elements<AbstractNum>().Last();
            numberingPart.Numbering.InsertAfter(abstractNum1, lastAbstractNum);
        }

        int numberId = numberingPart.Numbering.Elements<NumberingInstance>().Count() + 1;
        NumberingInstance numberingInstance1 = new NumberingInstance() { NumberID = numberId };
        AbstractNumId abstractNumId1 = new AbstractNumId() { Val = abstractNumberId };
        numberingInstance1.Append(abstractNumId1);

        if (numberId == 1)
        {
            numberingPart.Numbering.Append(numberingInstance1);
        }
        else
        {
            var lastNumberingInstance = numberingPart.Numbering.Elements<NumberingInstance>().Last();
            numberingPart.Numbering.InsertAfter(numberingInstance1, lastNumberingInstance);
        }

        Body body = doc.MainDocumentPart.Document.Body;

        foreach (string item in items.Split('\n'))
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                Paragraph paragraph = body.AppendChild(new Paragraph());

                paragraph.ParagraphProperties = new ParagraphProperties(
                    new NumberingProperties(
                        new NumberingLevelReference() { Val = 0 },
                        new NumberingId() { Val = numberId }),
                    new ParagraphStyleId() { Val = "List" });

                Run run = paragraph.AppendChild(new Run());
                run.AppendChild(new Text(item));
            }
        }
    }

    void Picture(WordprocessingDocument doc, string fileName)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

        using (FileStream stream = new(fileName, FileMode.Open))
        {
            imagePart.FeedData(stream);
        }

        AddImageToBody(doc, mainPart.GetIdOfPart(imagePart), fileName);
    }
    void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId, string fileName)
    {
        int emusPerCm = 360000;
        float maxWidthCm = 16.51f;
        int maxWidthEmus = (int)(maxWidthCm * emusPerCm);

        int iWidth = 0;
        int iHeight = 0;
        using (System.Drawing.Bitmap bmp = new(fileName))
        {
            iWidth = bmp.Width;
            iHeight = bmp.Height;
        }
        iWidth = (int)Math.Round((decimal)iWidth * pixel_to_EMU);
        iHeight = (int)Math.Round((decimal)iHeight * pixel_to_EMU);
        float ratio = iHeight / (float)iWidth;
        if (iWidth > maxWidthEmus)
        {
            iWidth = maxWidthEmus;
            iHeight = (int)(iWidth * ratio);
        }

        var element =
             new Drawing(
                 new DW.Inline(
                     new DW.Extent() { Cx = iWidth, Cy = iHeight },
                     new DW.EffectExtent()
                     {
                         LeftEdge = 0L,
                         TopEdge = 0L,
                         RightEdge = 0L,
                         BottomEdge = 0L
                     },
                     new DW.DocProperties()
                     {
                         Id = (UInt32Value)1U,
                         Name = "Picture 1"
                     },
                     new DW.NonVisualGraphicFrameDrawingProperties(
                         new A.GraphicFrameLocks() { NoChangeAspect = true }),
                     new A.Graphic(
                         new A.GraphicData(
                             new PIC.Picture(
                                 new PIC.NonVisualPictureProperties(
                                     new PIC.NonVisualDrawingProperties()
                                     {
                                         Id = (UInt32Value)0U,
                                         Name = "New Bitmap Image.jpg"
                                     },
                                     new PIC.NonVisualPictureDrawingProperties()),
                                 new PIC.BlipFill(
                                     new A.Blip(
                                         new A.BlipExtensionList(
                                             new A.BlipExtension()
                                             {
                                                 Uri =
                                                   "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                             })
                                     )
                                     {
                                         Embed = relationshipId,
                                         CompressionState = A.BlipCompressionValues.Print
                                     },
                                     new A.Stretch(
                                         new A.FillRectangle())),
                                 new PIC.ShapeProperties(
                                     new A.Transform2D(
                                         new A.Offset() { X = 0L, Y = 0L },
                                         new A.Extents() { Cx = iWidth, Cy = iHeight }),
                                     new A.PresetGeometry(
                                         new A.AdjustValueList()
                                     )
                                     { Preset = A.ShapeTypeValues.Rectangle }))
                         )
                         { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                 )
                 {
                     DistanceFromTop = (UInt32Value)0U,
                     DistanceFromBottom = (UInt32Value)0U,
                     DistanceFromLeft = (UInt32Value)0U,
                     DistanceFromRight = (UInt32Value)0U,
                     EditId = "50D07946"
                 });
        Paragraph paragraph = new()
        {
            ParagraphProperties = new ParagraphProperties(
                new ParagraphStyleId() { Val = "Picture" })
        };
        paragraph.AppendChild(new Run(element));
        wordDoc.MainDocumentPart.Document.Body.AppendChild(paragraph);
    }

    string[] ProcessSpecial(int i, string special, DataComboBox content)
    {
        string[] text = new string[2];
        if (special == "h1")
        {
            text[0] = content.ComboBox["h1"].Data[i - 1][1];
        }
        else if (special == "h2")
        {
            text[0] = content.ComboBox["h2"].Data[i - 1][1];
        }
        else if (special == "l")
        {
            text[0] = content.ComboBox["l"].Data[i - 1][1];
        }
        else if (special == "p")
        {
            text[0] = content.ComboBox["p"].Data[i - 1][1];
            text[1] = content.ComboBox["p"].Data[i - 1][0];
        }
        else if (special == "t")
        {
            text[0] = content.ComboBox["t"].Data[i - 1][1];
        }
        else if (special == "c")
        {
            text[0] = content.ComboBox["c"].Data[i - 1][1];
            text[1] = content.ComboBox["c"].Data[i - 1][0];
        }
        return text;
    }

    void PageSetup(Body body, float top = 2, float right = 1.5f, float bot = 2, float left = 3)
    {
        SectionProperties props = new();
        body.AppendChild(props);
        props.AddChild(new PageMargin()
        {
            Top = (int)(top * cm_to_pt),
            Right = Convert.ToUInt32(right * cm_to_pt),
            Bottom = (int)(bot * cm_to_pt),
            Left = Convert.ToUInt32(left * cm_to_pt)
        });

        props.AppendChild(new PageSize()
        {
            Width = 11907,
            Height = 16839
        });

    }

    void PageBreak(Run run)
    {
        run.AppendChild(new Break() { Type = BreakValues.Page });
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

    Run Text(WordprocessingDocument doc, string text, int size = 14,
        JustificationValues justify = JustificationValues.Left, bool bold = false,
        int before = 0, int after = 0, float multiplier = 1, float left = 0, float right = 0, float firstLine = 0, bool caps = false)
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
            LineRule = LineSpacingRuleValues.Auto,
        });

        paragraph.ParagraphProperties.AddChild(new Indentation()
        {
            Left = ((int)(left * cm_to_pt)).ToString(),
            Right = ((int)(right * cm_to_pt)).ToString(),
            FirstLine = ((int)firstLine * cm_to_pt).ToString()
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
            Val = (size * pt_to_halfpt).ToString()
        });

        run.RunProperties.AddChild(new Caps()
        {
            Val = caps
        });
        return run;
    }

    void Text(WordprocessingDocument doc, string text, string style)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;


        string[] str = text.Split('\n');

        for (int i = 0; i < str.Length - 1; i++)
        {
            Paragraph paragraph = body.AppendChild(new Paragraph());

            paragraph.ParagraphProperties = new ParagraphProperties(
                new ParagraphStyleId() { Val = style });

            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(str[i]));
        }

        if (str.Length - 1 >= 0)
        {
            Paragraph paragraph = body.AppendChild(new Paragraph());

            paragraph.ParagraphProperties = new ParagraphProperties(
                new ParagraphStyleId() { Val = style });

            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(str[str.Length - 1]));
        }
    }
}
