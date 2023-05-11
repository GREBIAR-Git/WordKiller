using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WordKiller.Scripts.ReportHelper
{
    internal class ReportExtras
    {
        public static void SectionBreak(WordprocessingDocument doc, bool title = false)
        {
            MainDocumentPart mainPart = doc.MainDocumentPart;
            Body body = mainPart.Document.Body;
            Paragraph paragraph = body.AppendChild(new Paragraph());
            paragraph.AppendChild(new ParagraphProperties(new SectionProperties(new SectionType() { Val = SectionMarkValues.NextPage })));
            ReportPageSettings.PageSetup(body, title: title);
        }

        public static void PageBreak(WordprocessingDocument doc)
        {
            MainDocumentPart mainPart = doc.MainDocumentPart;
            Body body = mainPart.Document.Body;
            body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
        }

        public static void NewLine(Paragraph paragraph)
        {
            Run run = new();
            run.Append(new Break());
            paragraph.AppendChild(run);
        }

        public static void EmptyLines(WordprocessingDocument doc, int number)
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
    }
}
