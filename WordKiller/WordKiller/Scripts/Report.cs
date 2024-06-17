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
                    TemplateType? currentTemplate = CurrentTemplate(data.Type);
                    if(currentTemplate == null)
                    {
                        UIHelper.ShowError("11");
                        return false;
                    }
                    try
                    {
                        if (data.Type != DocumentType.DefaultDocument && data.Properties.Title)
                        {
                            if (currentTemplate.NonStandard)
                            {
                                ReportComplexObjects.Text(doc, currentTemplate.Lines, data);
                            }
                            else
                            {
                                ReportPageSettings.PageSetup(body, title: true);
                                ReportComplexObjects.TitlePart(doc, data.Type, data.Title);
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
                        MainPart(doc, data, currentTemplate, data.Properties.NumberHeading);
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
                        if (currentTemplate.ManualPageNumbering)
                        {
                            ReportPageSettings.HeaderPageNumber(doc, currentTemplate.StartPageNumber, currentTemplate.NumberingDesign);
                        }
                        else
                        {
                            ReportPageSettings.HeaderPageNumber(doc, pageStartNumber, currentTemplate.NumberingDesign);
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

    static TemplateType? CurrentTemplate(DocumentType documentType)
    {
        foreach (TemplateType templateType in Settings.Default.TemplateTypes)
        {
            if (templateType.Type == documentType)
            {
                return templateType;
            }
        }
        return null;
    }

    static void MainPart(WordprocessingDocument doc, DocumentData data, TemplateType currentTemplate, bool numberHeading = true)
    {
        bool newPage = true;
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
                        if(!newPage && currentTemplate.H1Enter)
                        {
                            ReportExtras.EmptyLines(doc, 1, "Подраздел");
                        }
                        ReportText.Text(doc, text, "Подраздел");
                    }

                    else if (paragraph is ParagraphPicture picture)
                    {
                        string id = ReportImage.Registration(doc, picture);
                        if (id != null && picture.Bitmap != null)
                        {
                            ReportImage.Create(doc, id, picture.Bitmap);
                        }

                        string type = "Рисунок";

                        if(currentTemplate.ImageDesign == 1)
                        {
                            type = "Рис.";
                        }

                        text = type + " " + numbered.Number + " – " + paragraph.Description;
                        ReportText.Text(doc, text, "Картинка");
                    }
                    else if (paragraph is ParagraphTable paragraphTable)
                    {
                        string type = "Таблица";

                        if(currentTemplate.TableDesign == 1)
                        {
                            type = "Tабл.";
                        }

                        text = type + " " + numbered.Number + " – " + paragraphTable.Description;
                        ReportText.Text(doc, text, "ТекстКТаблице");
                        ReportTable.Create(doc, paragraphTable.TableData);
                    }
                }
                newPage=false;
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
                    newPage=true;
                }
            }
        }
    }
}