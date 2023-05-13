using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using System.IO;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.Scripts.ForUI;
using WordKiller.Scripts.ReportHelper;

namespace WordKiller.Scripts;
class Report
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

                    main.Document = new Document();
                    Body body = main.Document.AppendChild(new Body());
                    ReportStyles.Init(doc, data.Type);
                    int pageStartNumber = 1;
                    try
                    {
                        if (data.Type != DataTypes.Enums.DocumentType.DefaultDocument && data.Properties.Title)
                        {
                            ReportPageSettings.PageSetup(body, title: true);
                            ReportComplexObjects.TitlePart(doc, data.Type, data.Title);
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
                        ReportComplexObjects.ListOfReferencesPart(doc, data.ListOfReferences, data.Properties.ListOfReferences);
                    }
                    catch
                    {
                        UIHelper.ShowError("7");
                        return false;
                    }

                    try
                    {
                        ReportComplexObjects.AppendixPart(doc, data.Appendix, data.Properties.Appendix);
                    }
                    catch
                    {
                        UIHelper.ShowError("8");
                        return false;
                    }

                    if (data.Properties.PageNumbers)
                    {
                        ReportPageSettings.PageNumber(doc, pageStartNumber);
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
            int h1 = 1;
            int h2 = 1;
            int h2all = 1;
            int l = 1;
            int p = 1;
            int t = 1;
            int c = 1;
            int te = 1;
            Section(data);
            ReportExtras.PageBreak(doc);

            void Paragraph(IParagraphData paragraph)
            {
                string data;
                if (paragraph.Data.Length > 1)
                {
                    data = paragraph.Data.Remove(paragraph.Data.Length - 2, 2);
                }
                else
                {
                    data = paragraph.Data;
                }
                if (paragraph is ParagraphText)
                {
                    ReportText.Text(doc, data, "Текст");
                    te++;
                }
                else if (paragraph is ParagraphH1)
                {
                    string text = data.ToUpper();
                    if (h1 != 1 || h2 != 1 || t != 1 || te != 1 || l != 1 || p != 1 || c != 1)
                    {
                        ReportExtras.PageBreak(doc);
                    }
                    if (text.ToUpper() != "ВВЕДЕНИЕ")
                    {

                        if (numberHeading && text.ToUpper() != "ЗАКЛЮЧЕНИЕ")
                        {
                            text = h1.ToString() + " " + text;
                        }
                        h1++;
                    }
                    ReportText.Text(doc, text, "Раздел");
                    h2 = 1;
                }
                else if (paragraph is ParagraphH2)
                {
                    string text = string.Empty;
                    if (numberHeading)
                    {
                        text += (h1 - 1).ToString() + "." + h2.ToString() + " ";
                    }

                    text += data;
                    ReportText.Text(doc, "\n" + text, "Подраздел");
                    h2all++;
                    h2++;
                }
                else if (paragraph is ParagraphList)
                {
                    ReportList.Create(doc, data);
                    l++;
                }
                else if (paragraph is ParagraphPicture picture)
                {
                    string id = ReportImage.Registration(doc, picture);
                    if (id != null && picture.Bitmap != null)
                    {
                        ReportImage.Create(doc, id, picture.Bitmap);
                    }
                    ReportText.Text(doc, "Рисунок " + p + " – " + paragraph.Description, "Картинка");
                    p++;
                }
                else if (paragraph is ParagraphTable)
                {
                    ParagraphTable paragraphTable = paragraph as ParagraphTable;
                    ReportText.Text(doc, "Таблица " + t + " – " + paragraphTable.Description, "ТекстКТаблице");
                    ReportTable.Create(doc, paragraphTable.TableData);
                    t++;
                }
                else if (paragraph is ParagraphCode)
                {
                    ReportText.Text(doc, paragraph.Description, "РазделПриложение");

                    ReportText.Text(doc, data, "Код");
                    c++;
                }
            }

            void Section(SectionParagraphs sectionParagraphs)
            {
                foreach (IParagraphData paragraph in sectionParagraphs.Paragraphs)
                {
                    if (paragraph is SectionParagraphs)
                    {
                        Paragraph(paragraph);
                        Section(paragraph as SectionParagraphs);
                    }
                    else
                    {
                        if (paragraph is not ParagraphTitle && paragraph is not ParagraphTaskSheet && paragraph is not ParagraphListOfReferences && paragraph is not ParagraphAppendix)
                        {
                            Paragraph(paragraph);
                        }
                    }
                }
            }
        }
    }
}
