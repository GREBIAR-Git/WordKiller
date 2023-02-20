using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using WordKiller.DataTypes;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.Scripts.ForUI;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;

namespace WordKiller.Scripts;
class Report
{
    const short cm_to_pt = 567;

    const byte pt_to_halfpt = 2;

    const short pixel_to_EMU = 9525;

    public static bool Create(DocumentData data, bool numbering, bool tableOfContents, bool numberHeading, string[] dataTitle, bool exportPDF, bool exportHTML)
    {
        SaveFileDialog saveFileDialog = new()
        {
            OverwritePrompt = true,
            Filter = "docx files (*.docx)|*.docx|All files (*.*)|*.*",
            FileName = "1"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            string pathSave = saveFileDialog.FileName;
            try
            {
                using (WordprocessingDocument doc =
                WordprocessingDocument.Create(pathSave,
                WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart main = doc.AddMainDocumentPart();

                    main.Document = new Document();
                    Body body = main.Document.AppendChild(new Body());

                    InitStyles(doc);

                    try
                    {
                        if (data.Type != TypeDocument.DefaultDocument)
                        {
                            PageSetup(body, title: true);
                            dataTitle[^1] = SpaceForYear(dataTitle[^1], '_');
                            TitlePart(doc, data.Type, dataTitle, tableOfContents);
                        }
                        else
                        {
                            if (tableOfContents)
                            {
                                PageSetup(body, title: true);
                            }
                            else
                            {
                                PageSetup(body, title: false);
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show(UIHelper.FindResourse("Error3"), UIHelper.FindResourse("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
                        return false;
                    }

                    try
                    {

                        TableOfContents(doc, tableOfContents);
                    }
                    catch
                    {
                        MessageBox.Show(UIHelper.FindResourse("Error4"), UIHelper.FindResourse("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
                        return false;
                    }

                    try
                    {
                        MainPart(doc, data, numberHeading);
                    }
                    catch
                    {
                        MessageBox.Show(UIHelper.FindResourse("Error5"), UIHelper.FindResourse("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
                        return false;
                    }

                    if (numbering)
                    {
                        PageNumber(doc);
                    }
                }
                if (exportHTML)
                {
                    WkrExport.ToHTML(pathSave);
                }
                if (exportPDF)
                {
                    WkrExport.ToPDF(pathSave);
                }
                return true;
            }
            catch (IOException)
            {
                MessageBox.Show(UIHelper.FindResourse("Error6"), UIHelper.FindResourse("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        return false;
    }

    private static string GetTOC(string title, int titleFontSize)
    {
        return $@"
    <w:sdt>
        <w:sdtPr>
            <w:id w:val=""-493258456"" />
            <w:docPartObj>
                <w:docPartGallery w:val=""Table of Contents"" />
                <w:docPartUnique />
            </w:docPartObj>
        </w:sdtPr>
        <w:sdtContent>
            <w:p w:rsidR=""00095C65"" w:rsidRDefault=""00095C65"">
                <w:pPr>
                    <w:jc w:val=""center"" /> 
                </w:pPr>
                <w:r>
                    <w:rPr>
                        <w:b /> 
                        <w:caps w:val=""true"" />  
                        <w:rFonts w:ascii=""Courier New"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                        <w:sz w:val=""{titleFontSize * 2}"" /> 
                        <w:szCs w:val=""{titleFontSize * 2}"" /> 
                    </w:rPr>
                    <w:t>{title}</w:t>
                </w:r>
            </w:p>
            <w:p w:rsidR=""00095C65"" w:rsidRDefault=""00095C65"">
                <w:r>
                    <w:rPr>
                        <w:b />
                        <w:bCs />
                        <w:noProof />
                    </w:rPr>
                    <w:fldChar w:fldCharType=""begin"" />
                </w:r>
                <w:r>
                    <w:rPr>
                        <w:b />
                        <w:bCs />
                        <w:noProof />
                    </w:rPr>
                    <w:instrText xml:space=""preserve""> TOC \o ""1-3"" \h \z \u </w:instrText>
                </w:r>
                <w:r>
                    <w:rPr>
                        <w:b />
                        <w:bCs />
                        <w:noProof />
                    </w:rPr>
                    <w:fldChar w:fldCharType=""separate"" />
                </w:r>
                <w:r>
                    <w:rPr>
                        <w:caps w:val=""true"" />  
                        <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                        <w:sz w:val=""{titleFontSize * 2}"" /> 
                        <w:szCs w:val=""{titleFontSize * 2}"" /> 
                        <w:noProof />
                    </w:rPr>
                    <w:t>No table of contents entries found.</w:t>
                </w:r>
                <w:r>
                    <w:rPr>
                        <w:b />
                        <w:bCs />
                        <w:noProof />
                    </w:rPr>
                    <w:fldChar w:fldCharType=""end"" />
                </w:r>
            </w:p>
        </w:sdtContent>
    </w:sdt>";
    }

    static void PageNumber(WordprocessingDocument document)
    {
        MainDocumentPart mainDocumentPart = document.MainDocumentPart;

        mainDocumentPart.DeleteParts(mainDocumentPart.HeaderParts);

        HeaderPart headerPart = mainDocumentPart.AddNewPart<HeaderPart>();

        string headerPartId = mainDocumentPart.GetIdOfPart(headerPart);

        GeneratePageNumber(headerPart);
        IEnumerable<SectionProperties> sections = mainDocumentPart.Document.Body.Descendants<SectionProperties>();
        foreach (SectionProperties section in sections)
        {
            section.RemoveAllChildren<HeaderReference>();
            section.PrependChild(new HeaderReference() { Id = headerPartId, Type = HeaderFooterValues.Default });
            section.PrependChild(new PageNumberType { Start = 4 });
        }
    }

    static void GeneratePageNumber(HeaderPart part)
    {
        Header header =
            new(
                new Paragraph(
                    new ParagraphProperties(
                        new ParagraphStyleId()
                        {
                            Val = "Header"
                        },
                        new Justification()
                        {
                            Val = JustificationValues.Center
                        },
                        new SpacingBetweenLines()
                        {
                            After = 0.ToString(),
                            Before = 0.ToString(),
                            Line = 240.ToString(),
                            LineRule = LineSpacingRuleValues.Auto
                        }
                    ),
                    new Run(new SimpleField() { Instruction = "Page" })
            ));
        part.Header = header;
    }

    static void SectionBreak(WordprocessingDocument doc, bool title = false)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;
        Paragraph paragraph = body.AppendChild(new Paragraph());
        paragraph.AppendChild(new ParagraphProperties(new SectionProperties(new SectionType() { Val = SectionMarkValues.NextPage })));
        PageSetup(body, title: title);
    }

    static string SpaceForYear(string year, char spaceCharacter)
    {
        for (int i = 0; i < 4 - year.Length; i++)
        {
            year += spaceCharacter;
        }
        return year;
    }

    static void InitStyles(WordprocessingDocument doc)
    {
        StyleDefinitionsPart styleDefinitions = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();

        Styles styles = new();

        styles.Save(styleDefinitions);
        styles = styleDefinitions.Styles;


        styles.Append(
            InitStyle("EmptyLines", justify: JustificationValues.Center));

        styles.Append(
            InitStyle("Simple", justify: JustificationValues.Both, multiplier: 1.5f, firstLine: 1.25f));

        styles.Append(
            InitStyle("H1", justify: JustificationValues.Center, bold: true, after: 8, multiplier: 1.5f, firstLine: 1.5f, caps: true, outlineLevel: 1));

        styles.Append(
            InitStyle("H2", justify: JustificationValues.Center, bold: true, after: 8, multiplier: 1.5f, firstLine: 1.5f, outlineLevel: 2));

        styles.Append(
            InitStyle("List", justify: JustificationValues.Both, multiplier: 1.5f, left: 1.25f, hanging: 0.63f));

        styles.Append(
            InitStyle("Picture", justify: JustificationValues.Center, after: 8, multiplier: 1.5f));

        styles.Append(
            InitStyle("TableText", justify: JustificationValues.Both, before: 8, multiplier: 1.5f));

        styles.Append(
            InitStyle("Table", justify: JustificationValues.Both, after: 6, multiplier: 1f));

        styles.Append(
            InitStyle("Code", 12, JustificationValues.Left));

    }

    static Style InitStyle(string name, int size = 14,
        JustificationValues justify = JustificationValues.Left, bool bold = false,
        int before = 0, int after = 0, float multiplier = 1, float left = 0, float right = 0, float firstLine = 0, bool caps = false, float hanging = 0, int outlineLevel = 0)
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

        if (outlineLevel != 0)
        {
            paragraphProperties.AddChild(new OutlineLevel()
            {
                Val = outlineLevel - 1
            });
        }

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

    static void TableOfContents(WordprocessingDocument doc, bool on)
    {
        if (on)
        {
            var sdtBlock = new SdtBlock
            {
                InnerXml = GetTOC("Содержание", 14)
            };
            doc.MainDocumentPart.Document.Body.AppendChild(sdtBlock);

            var settingsPart = doc.MainDocumentPart.AddNewPart<DocumentSettingsPart>();
            settingsPart.Settings = new Settings { BordersDoNotSurroundFooter = new BordersDoNotSurroundFooter() { Val = true } };

            settingsPart.Settings.Append(new UpdateFieldsOnOpen() { Val = true });

            SectionBreak(doc);
        }
    }

    static void TitlePart(WordprocessingDocument doc, TypeDocument typeDocument, string[] dataTitle, bool TableOfContents)
    {
        Ministry(doc, dataTitle[0]);
        switch (typeDocument)
        {
            case TypeDocument.LaboratoryWork:
                LabPra(doc, "лабораторной", dataTitle);
                break;
            case TypeDocument.PracticeWork:
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
            case TypeDocument.Diploma:
                break;
            case TypeDocument.VKR:
                break;
        }
        Orel(doc, dataTitle[^1]);
        SectionBreak(doc, TableOfContents);
    }

    static void Referat(WordprocessingDocument doc, string[] dataTitle)
    {
        EmptyLines(doc, 1);

        string text = "Реферат по дисциплине: «" + dataTitle[3] + "»";
        Text(doc, text, justify: JustificationValues.Center);
        text = "Тема: «" + dataTitle[2] + "»";
        Text(doc, text, justify: JustificationValues.Center);
        EmptyLines(doc, 16);

        text = "Выполнил: студент группы " + Properties.Settings.Default.GroupString;
        Text(doc, text, justify: JustificationValues.Right);
        text = dataTitle[1];
        Text(doc, text, justify: JustificationValues.Right);
        text = "Проверил: ст. преподаватель кафедры физического воспитания";
        Text(doc, text, justify: JustificationValues.Right);
        text = dataTitle[4];
        Text(doc, text, justify: JustificationValues.Right);

        EmptyLines(doc, 6);
    }

    static void ControlWork(WordprocessingDocument doc, string[] dataTitle)
    {
        string text = "Контрольная работа";
        Text(doc, text, 16, JustificationValues.Center, true);

        text = "по дисциплине: «" + dataTitle[2] + "»";
        Text(doc, text, justify: JustificationValues.Center);

        EmptyLines(doc, 10);

        text = "Выполнил: " + dataTitle[1];
        Text(doc, text);
        text = Properties.Settings.Default.FacultyString;
        Text(doc, text);
        text = "Направление: 09.03.04 «Программная инженерия»";
        Text(doc, text);
        text = "Группа: " + Properties.Settings.Default.Group;
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



    static void Coursework(WordprocessingDocument doc, string[] dataTitle)
    {
        string text = "Работа допущена к защите";
        Text(doc, text, multiplier: 1.5f, left: 9.5f);
        text = "______________Руководитель";
        Text(doc, text, multiplier: 1.5f, left: 9.5f);
        text = "«____»_____________" + dataTitle[^1] + "г.";
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
        text = Properties.Settings.Default.FacultyString;
        Text(doc, text, multiplier: 1.5f);
        text = "Направление: 09.03.04 «Программная инженерия»";
        Text(doc, text, multiplier: 1.5f);
        text = "Группа: " + Properties.Settings.Default.GroupString;
        Text(doc, text, multiplier: 1.5f);

        text = "Руководитель __________________" + dataTitle[5];
        Text(doc, text, multiplier: 1.5f, after: 12);

        text = "Оценка: «________________»               Дата ______________";
        Text(doc, text);

        EmptyLines(doc, 2);
    }

    static void LabPra(WordprocessingDocument doc, string type, string[] dataTitle)
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
        text = Properties.Settings.Default.FacultyString;
        Text(doc, text);
        text = "Направление: 09.03.04 «Программная инженерия»";
        Text(doc, text);
        text = "Группа: " + Properties.Settings.Default.GroupString;
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

    static void Ministry(WordprocessingDocument doc, string faculty)
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

    static void Orel(WordprocessingDocument doc, string year)
    {
        string text = "Орел, " + year;
        Text(doc, text, justify: JustificationValues.Center);
    }

    static void MainPart(WordprocessingDocument doc, DocumentData data, bool numberHeading)
    {
        if (data != null)
        {
            GenerateMainPart(doc, data, numberHeading);
        }
    }

    static void GenerateMainPart(WordprocessingDocument doc, DocumentData data, bool number = true)
    {
        int h1 = 1;
        int h2 = 1;
        int h2all = 1;
        int l = 1;
        int p = 1;
        int t = 1;
        int c = 1;

        foreach (IParagraphData dataType in data.Paragraphs)
        {
            if (dataType is ParagraphText)
            {
                Text(doc, dataType.Data, "Simple");
            }
            else if (dataType is ParagraphH1)
            {
                string text = dataType.Data.ToUpper();
                if (h1 != 1)
                {
                    PageBreak(doc);
                }
                if (text != "ВВЕДЕНИЕ")
                {

                    if (number && text != "ЗАКЛЮЧЕНИЕ")
                    {
                        text = h1.ToString() + " " + text;
                        h1++;
                    }
                }
                Text(doc, text, "H1");
                h2 = 1;
            }
            else if (dataType is ParagraphH2)
            {
                string text = string.Empty;
                if (number)
                {
                    text += (h1 - 1).ToString() + "." + h2.ToString() + " ";
                }

                text += dataType.Data;
                Text(doc, "\n" + text, "H2");
                h2all++;
                h2++;
            }
            else if (dataType is ParagraphList)
            {
                List(doc, dataType.Data);
                l++;
            }
            else if (dataType is ParagraphPicture)
            {
                Picture(doc, dataType.Data);
                Text(doc, "Рисунок " + p + " – " + dataType.Description, "Picture");
                p++;
            }
            else if (dataType is ParagraphTable)
            {
                ParagraphTable paragraphTable = dataType as ParagraphTable;
                Text(doc, "Таблица " + t + " – " + paragraphTable.Description, "TableText");
                Table(doc, paragraphTable.TableData);
                t++;
            }
            else if (dataType is ParagraphCode)
            {
                Text(doc, dataType.Description, "H1");
                FileStream file = new(dataType.Data, FileMode.Open);
                StreamReader reader = new(file);
                string data1 = reader.ReadToEnd();
                Text(doc, data1, "Code");
                c++;
            }
        }
    }

    public static void Table(WordprocessingDocument doc, TableData dataTable)
    {
        Table dTable = new();
        TableProperties props = new();
        dTable.AppendChild(props);

        for (int i = 0; i < dataTable.Rows; i++)
        {
            TableRow tr = new();
            for (int f = 0; f < dataTable.Columns; f++)
            {
                DataCell(tr, dataTable.Columns, 0, dataTable.DataTable[i, f]);
            }
            dTable.Append(tr);
        }

        var tableWidth = new TableWidth()
        {
            Width = "5000",
            Type = TableWidthUnitValues.Pct
        };
        props.Append(tableWidth);

        EnumValue<BorderValues> borderValues = new(BorderValues.Single);
        TableBorders tableBorders = new(
                             new TopBorder { Val = borderValues, Size = 4 },
                             new BottomBorder { Val = borderValues, Size = 4 },
                             new LeftBorder { Val = borderValues, Size = 4 },
                             new RightBorder { Val = borderValues, Size = 4 },
                             new InsideHorizontalBorder { Val = borderValues, Size = 4 },
                             new InsideVerticalBorder { Val = borderValues, Size = 4 });

        props.Append(tableBorders);

        doc.MainDocumentPart.Document.Body.Append(dTable);
    }

    static void DataCell(TableRow tr, int numberOfСolumns, int idx, string text)
    {
        if (numberOfСolumns > idx)
        {
            TableCell tc = new();
            tc.Append(new Paragraph(new Run(new Text(text)))
            {
                ParagraphProperties = new ParagraphProperties()
                {
                    ParagraphStyleId = new ParagraphStyleId() { Val = "Table" }
                }
            });
            tc.Append(new TableCellProperties());
            tr.Append(tc);
        }
    }

    static void List(WordprocessingDocument doc, string items)
    {
        NumberingDefinitionsPart numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart;
        if (numberingPart == null)
        {
            numberingPart = doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("NumberingDefinitionsPart001");
            Numbering element = new();
            element.Save(numberingPart);
        }

        int abstractNumberId = numberingPart.Numbering.Elements<AbstractNum>().Count() + 1;
        Level[] levels = new Level[9];
        string levelText = string.Empty;
        for (int i = 0; i < 9; i++)
        {
            levelText += "%" + (i + 1);
            levels[i] = new()
            {
                NumberingFormat = new NumberingFormat() { Val = NumberFormatValues.Decimal },
                StartNumberingValue = new StartNumberingValue() { Val = 1 },
                LevelText = new LevelText() { Val = levelText + ")" },
                LevelIndex = i,
                LevelSuffix = new LevelSuffix()
                {
                    Val = LevelSuffixValues.Space
                },
                PreviousParagraphProperties = new PreviousParagraphProperties()
                {
                    Indentation = new Indentation()
                    {
                        Start = ((int)(0.63f * i * 2 * cm_to_pt)).ToString(),
                        Hanging = ((int)(0.63f * i * cm_to_pt)).ToString(),
                    }
                }
            };
            levelText += ".";
        }

        AbstractNum abstractNum1 = new(levels) { AbstractNumberId = abstractNumberId, MultiLevelType = new MultiLevelType() { Val = MultiLevelValues.HybridMultilevel } };
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
        NumberingInstance numberingInstance1 = new() { NumberID = numberId };
        AbstractNumId abstractNumId1 = new() { Val = abstractNumberId };
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
                        new NumberingLevelReference() { Val = MainWindow.Level(item) },
                        new NumberingId() { Val = numberId }),
                    new ParagraphStyleId() { Val = "List" }
                    );

                Run run = paragraph.AppendChild(new Run());
                run.AppendChild(new Text(item[MainWindow.StartLine(item, MainWindow.Level(item))..]));
            }
        }
    }

    static void Picture(WordprocessingDocument doc, string fileName)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

        using (FileStream stream = new(fileName, FileMode.Open))
        {
            imagePart.FeedData(stream);
        }

        AddImageToBody(doc, mainPart.GetIdOfPart(imagePart), fileName);
    }
    static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId, string fileName)
    {
        int emusPerCm = 360000;
        float maxWidthCm = 16.51f;
        int maxWidthEmus = (int)(maxWidthCm * emusPerCm);

        int iWidth = 0;
        int iHeight = 0;
        using (Bitmap bmp = new(fileName))
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

    static void PageSetup(Body body, float top = 2, float right = 1.5f, float bot = 2, float left = 3, bool title = false)
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
        if (title)
        {
            props.PrependChild(new TitlePage());
        }
    }

    static void PageBreak(WordprocessingDocument doc)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;
        body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
    }

    static void EmptyLines(WordprocessingDocument doc, int number)
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

    static Run Text(WordprocessingDocument doc, string text, int size = 14,
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

    static void Text(WordprocessingDocument doc, string text, string style)
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
            run.AppendChild(new Text(str[^1]));
        }
    }
}
