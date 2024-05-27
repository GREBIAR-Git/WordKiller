using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using WordKiller.DataTypes;
using WordKiller.Scripts;
using DocumentType = WordKiller.DataTypes.Enums.DocumentType;

namespace WordKiller.Models.Template;

[Serializable]
public class TemplateType
{
    public TemplateType(DocumentType type)
    {
        manualPageNumbering = false;
        startPageNumber = 0;
        Type = type;
        Templates =
        [
            new("Текст", justify: "both", lineSpacing: 1.5f, firstLine: 1.25f),
            new("Раздел", justify: "center", bold: true, after: 8, lineSpacing: 1.5f,
                firstLine: 1.5f),

            new("Подраздел", justify: "center", bold: true, after: 8, lineSpacing: 1.5f,
                firstLine: 1.5f),

            new("Список", justify: "both", lineSpacing: 1.5f, left: 1.25f),
            new("Картинка", justify: "center", after: 8, lineSpacing: 1.5f),
            new("ТекстКТаблице", justify: "both", before: 8, lineSpacing: 1.5f),
            new("Таблица", justify: "both", after: 6, lineSpacing: 1f),
            new("Код", 12)
        ];
    }

    public TemplateType()
    {
        manualPageNumbering = false;
        startPageNumber = 0;
        Templates = [];
    }

    [XmlIgnore]
    public static List<string> Alignments { get; set; } =
    [
        "left",
        "right",
        "center",
        "both"
    ];

    public DocumentType Type { get; set; }

    public List<Line> Lines { get; set; }

    public List<Visibility> Visibilities { get; set; } = []; //14

    public List<YellowFragment> YellowFragment { get; set; } = [];


    public bool nonStandard { get; set; }

    [XmlIgnore]
    public bool NonStandard
    {
        get => nonStandard;
        set
        {
            if (nonStandard != value)
            {
                nonStandard = value;
                if (nonStandard)
                {
                    if (!InitTitle())
                    {
                        nonStandard = false;
                    }
                }

                Update();
                TemplateHelper.NeedSave = true;
                if (!nonStandard)
                {
                    YellowFragment.Clear();
                    Lines.Clear();
                    Visibilities.Clear();
                }
            }
        }
    }

    public bool manualPageNumbering { get; set; }

    [XmlIgnore]
    public bool ManualPageNumbering
    {
        get => manualPageNumbering;
        set
        {
            if (manualPageNumbering != value)
            {
                manualPageNumbering = value;
                TemplateHelper.NeedSave = true;
            }
        }
    }

    public int startPageNumber { get; set; }

    [XmlIgnore]
    public int StartPageNumber
    {
        get => startPageNumber;
        set
        {
            if (startPageNumber != value)
            {
                startPageNumber = value;
                TemplateHelper.NeedSave = true;
            }
        }
    }

    public ObservableCollection<Template> Templates { get; set; }

    bool InitTitle()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "docx files (*.docx)|*.docx|All files (*.*)|*.*"
        };
        bool? result = openFileDialog.ShowDialog();
        if (result == true)
        {
            string fileName = openFileDialog.FileName;
            //string fileName = @"C:\Users\nikit\OneDrive\Рабочий стол\1.docx";

            List<Line> lines = [];
            using (WordprocessingDocument myDoc = WordprocessingDocument.Open(fileName, true))
            {
                IEnumerable<Paragraph> paragraphList = myDoc.MainDocumentPart.Document.Body.Elements()
                    .Where(c => c is Paragraph).Cast<Paragraph>();
                IEnumerable<SectionProperties> sectionProperties = myDoc.MainDocumentPart.Document.Body.ChildElements
                    .Where(c => c is SectionProperties).Cast<SectionProperties>();
                SectionProperties section = null;
                foreach (SectionProperties section1 in sectionProperties)
                {
                    section = section1;
                }

                foreach (Paragraph p in paragraphList)
                {
                    string paragraphInnerText = p.InnerText;
                    IEnumerable<Run> runList = p.ChildElements.Where(c => c is Run).Cast<Run>();
                    List<Line> lines1 = [];
                    if (runList.Any())
                    {
                        string? runProperties1 = null;
                        string runtext = string.Empty;
                        foreach (Run r in runList)
                        {
                            string runInnerText = string.Empty;
                            IEnumerable<TabChar> twaab = r.ChildElements.Where(c => c is TabChar).Cast<TabChar>();
                            IEnumerable<Break> twaab1 = r.ChildElements.Where(c => c is Break).Cast<Break>();
                            foreach (Break twaab3 in twaab1)
                            {
                                if (!string.IsNullOrEmpty(runtext))
                                {
                                    lines1.Add(new(runtext, runProperties1));
                                    runProperties1 = null;
                                    runtext = string.Empty;
                                }

                                if (twaab3.Type == null || twaab3.Type == BreakValues.TextWrapping)
                                {
                                    runInnerText += "\n";
                                }
                                else if (twaab3.Type == BreakValues.Page)
                                {
                                    runInnerText += "\r\n";
                                }
                            }

                            foreach (TabChar twaab2 in twaab)
                            {
                                runInnerText += "\t";
                            }

                            //lines1.Add(new Line(runInnerText));

                            IEnumerable<RunProperties> runPropertiesList =
                                r.ChildElements.Where(c => c is RunProperties).Cast<RunProperties>();
                            string? current = string.Empty;
                            foreach (RunProperties runProperties in runPropertiesList)
                            {
                                current = runProperties.OuterXml;
                                current = Regex.Replace(current, @"<w:lang.*?\/>", "");
                                //lines1.Last().RunProperties = runProperties.OuterXml;
                            }

                            if (current != null && current == "")
                            {
                                current =
                                    "<w:rPr xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"></w:rPr>";
                            }

                            if (runProperties1 == current)
                            {
                                runInnerText += r.InnerText;
                                runtext += runInnerText;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(runtext))
                                {
                                    lines1.Add(new(runtext, runProperties1));
                                }
                                else if (runInnerText == "\n" || runInnerText == "\r\n" || runInnerText.Contains('\t'))
                                {
                                    lines1.Add(new(runInnerText, current));
                                    runInnerText = string.Empty;
                                    runProperties1 = null;
                                }

                                runInnerText += r.InnerText;
                                runtext = runInnerText;
                                runProperties1 = current;
                            }
                        }

                        lines1.Add(new(runtext, runProperties1));
                    }

                    ParagraphProperties main = null;
                    IEnumerable<ParagraphProperties> paragraphPropertiesList =
                        p.ChildElements.Where(r => r is ParagraphProperties).Cast<ParagraphProperties>();
                    foreach (ParagraphProperties paragraphProperties in paragraphPropertiesList)
                    {
                        main = paragraphProperties;
                    }

                    foreach (Line line1 in lines1)
                    {
                        if (section != null)
                        {
                            line1.sectionProperties = section.OuterXml;
                        }
                    }

                    lines.Add(new(true));
                    if (main != null)
                    {
                        lines.Last().ParagraphProperties = main.OuterXml;
                    }

                    lines.AddRange(lines1);
                }
            }

            Lines = lines;
            InitYellowFragments(lines);
        }

        return (bool)result;
    }

    void InitYellowFragments(List<Line> lines)
    {
        YellowFragment.Clear();
        string str = string.Empty;
        foreach (Line line in lines)
        {
            RunProperties runProperties = new(line.RunProperties);
            if (runProperties.Highlight != null && runProperties.Highlight.Val == HighlightColorValues.Yellow &&
                !string.IsNullOrEmpty(line.Text) && line.Text != "\n")
            {
                str += line.Text;
            }
            else
            {
                if (!string.IsNullOrEmpty(str))
                {
                    YellowFragment.Add(new() { Text = str });
                    str = string.Empty;
                }
            }
        }
    }

    public void Update()
    {
        Visibilities.Clear();
        for (int i = 0; i < 14; i++)
        {
            Visibilities.Add(Visibility.Collapsed);
        }

        for (int i = 0; i < YellowFragment.Count; i++)
        {
            if (YellowFragment[i].Index == 0)
            {
                Visibilities[0] = Visibility.Visible;
            }
            else if (YellowFragment[i].Index == 1)
            {
                //Visibilities[0] = Visibility.Visible;
            }
            else if (YellowFragment[i].Index == 2 || YellowFragment[i].Index == 3 || YellowFragment[i].Index == 4)
            {
                Visibilities[1] = Visibility.Visible;
            }
            else if (YellowFragment[i].Index == 5 || YellowFragment[i].Index == 6 || YellowFragment[i].Index == 7)
            {
                Visibilities[5] = Visibility.Visible;
            }
            else if (YellowFragment[i].Index == 9)
            {
                Visibilities[2] = Visibility.Visible;
            }
            else if (YellowFragment[i].Index == 10)
            {
                Visibilities[3] = Visibility.Visible;
            }
            else if (YellowFragment[i].Index == 14)
            {
                Visibilities[11] = Visibility.Visible;
            }
            else if (YellowFragment[i].Index == 16)
            {
                Visibilities[4] = Visibility.Visible;
            }
            else if (YellowFragment[i].Index == 17)
            {
                Visibilities[6] = Visibility.Visible;
            }
        }
    }
}