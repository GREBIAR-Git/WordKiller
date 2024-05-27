using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.Models.Template;
using WordKiller.Scripts.ReportHelper;
using DocumentType = WordKiller.DataTypes.Enums.DocumentType;
using Settings = WordKiller.Properties.Settings;

namespace WordKiller.Scripts;

internal class Report
{
    public static bool Create(DocumentData data, bool exportPDF, bool exportHTML)
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

                    main.Document = new();
                    Body body = main.Document.AppendChild(new Body());
                    ReportStyles.Init(doc, data.Type);
                    int pageStartNumber = 1;
                    try
                    {
                        if (data.Type != DocumentType.DefaultDocument && data.Properties.Title)
                        {
                            foreach (TemplateType templateType in Settings.Default.TemplateTypes)
                            {
                                if (templateType.Type == data.Type)
                                {
                                    if (templateType.NonStandard)
                                    {
                                        ReportComplexObjects.Text(doc, templateType.Lines, data);
                                    }
                                    else
                                    {
                                        ReportPageSettings.PageSetup(body, title: true);
                                        ReportComplexObjects.TitlePart(doc, data.Type, data.Title);
                                    }
                                }
                            }

                            pageStartNumber++;
                        }
                        else
                        {
                            ReportPageSettings.PageSetup(body);
                        }
                    }
                    catch
                    {
                        UIHelper.ShowError("3");
                        return false;
                    }

                    try
                    {
                        if (data.Properties.TaskSheet)
                        {
                            ReportComplexObjects.TaskSheet(doc, data.Title, data.TaskSheet);
                            pageStartNumber++;
                        }
                    }
                    catch
                    {
                        UIHelper.ShowError("4");
                        return false;
                    }

                    try
                    {
                        if (data.Properties.TableOfContents)
                        {
                            ReportTOC.Create(doc);
                            pageStartNumber++;
                        }
                    }
                    catch
                    {
                        UIHelper.ShowError("5");
                        return false;
                    }

                    try
                    {
                        MainPart(doc, data, data.Properties.NumberHeading);
                    }
                    catch
                    {
                        UIHelper.ShowError("6");
                        return false;
                    }

                    try
                    {
                        ReportComplexObjects.ListOfReferencesPart(doc, data.ListOfReferences,
                            data.Properties.ListOfReferences);
                    }
                    catch
                    {
                        UIHelper.ShowError("7");
                        return false;
                    }

                    try
                    {
                        ReportComplexObjects.AppendixPart(doc, data.Appendix, data.Properties.Appendix, data.Type);
                    }
                    catch
                    {
                        UIHelper.ShowError("8");
                        return false;
                    }

                    if (data.Properties.PageNumbers)
                    {
                        foreach (TemplateType templateType in Settings.Default.TemplateTypes)
                        {
                            if (templateType.Type == data.Type)
                            {
                                if (templateType.ManualPageNumbering)
                                {
                                    ReportPageSettings.PageNumber(doc, templateType.StartPageNumber);
                                }
                                else
                                {
                                    ReportPageSettings.PageNumber(doc, pageStartNumber);
                                }
                            }
                        }
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
                UIHelper.ShowError("9");
            }
        }

        return false;
    }

    static void MainPart(WordprocessingDocument doc, DocumentData data, bool numberHeading = true)
    {
        if (data != null)
        {
            Section(data);

            void Paragraph(IParagraphData paragraph)
            {
                string text;
                if (paragraph.Data.Length > 1)
                {
                    text = paragraph.Data.Remove(paragraph.Data.Length - 2, 2);
                }
                else
                {
                    text = paragraph.Data;
                }

                if (paragraph is ParagraphText)
                {
                    ReportText.Text(doc, text, "Текст");
                }
                else if (paragraph is ParagraphList)
                {
                    if (DocumentType.VKR == data.Type)
                    {
                        ReportList.CreateVKR(doc, text);
                    }
                    else
                    {
                        ReportList.Create(doc, text);
                    }
                }
                else if (paragraph is ParagraphCode)
                {
                    ReportText.Text(doc, paragraph.Description, "РазделПриложение");
                    ReportText.Text(doc, text, "Код");
                }
                else if (paragraph is Numbered numbered)
                {
                    if (paragraph is ParagraphH1)
                    {
                        if (numberHeading)
                        {
                            text = numbered.Number + text;
                        }

                        ReportText.Text(doc, text, "Раздел");
                    }
                    else if (paragraph is ParagraphH2)
                    {
                        if (numberHeading)
                        {
                            text = numbered.Number + text;
                        }

                        ReportText.Text(doc, "\n" + text, "Подраздел");
                    }

                    else if (paragraph is ParagraphPicture picture)
                    {
                        string id = ReportImage.Registration(doc, picture);
                        if (id != null && picture.Bitmap != null)
                        {
                            ReportImage.Create(doc, id, picture.Bitmap);
                        }

                        text = "Рисунок " + numbered.Number + " – " + paragraph.Description;
                        ReportText.Text(doc, text, "Картинка");
                    }
                    else if (paragraph is ParagraphTable paragraphTable)
                    {
                        text = "Таблица " + numbered.Number + " – " + paragraphTable.Description;
                        ReportText.Text(doc, text, "ТекстКТаблице");
                        ReportTable.Create(doc, paragraphTable.TableData);
                    }
                }
            }

            void Section(SectionParagraphs section)
            {
                foreach (IParagraphData paragraph in section.Paragraphs)
                {
                    if (paragraph is SectionParagraphs internalSection)
                    {
                        Paragraph(paragraph);
                        Section(internalSection);
                    }
                    else
                    {
                        if (paragraph is not ParagraphTitle && paragraph is not ParagraphTaskSheet &&
                            paragraph is not ParagraphListOfReferences && paragraph is not ParagraphAppendix)
                        {
                            Paragraph(paragraph);
                        }
                    }
                }

                if (section is ParagraphH1)
                {
                    ReportExtras.PageBreak(doc);
                }
            }
        }
    }
}