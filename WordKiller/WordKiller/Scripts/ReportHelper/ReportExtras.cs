using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WordKiller.Scripts.ReportHelper;

internal class ReportExtras
{
    public static void SectionBreak(WordprocessingDocument doc, bool title = false)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;
        Paragraph paragraph = body.AppendChild(new Paragraph());
        paragraph.AppendChild(
            new ParagraphProperties(new SectionProperties(new SectionType { Val = SectionMarkValues.NextPage })));
        ReportPageSettings.PageSetup(body, title: title);
    }

    public static void PageBreak(WordprocessingDocument doc)
    {
        Body body = doc.MainDocumentPart.Document.GetFirstChild<Body>();
        var paras = body.Elements<Paragraph>();
        var paras1 = paras.Last().Elements<Run>().Last();
        paras1.AppendChild(new Break { Type = BreakValues.Page });
    }

    public static void NewLine(Paragraph paragraph)
    {
        Run run = new();
        run.Append(new Break());
        paragraph.AppendChild(run);
    }

    public static void EmptyLines(WordprocessingDocument doc, int number, string style = "EmptyLines")
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;
        Paragraph paragraph = body.AppendChild(new Paragraph());
        paragraph.AppendChild(new ParagraphProperties());

        paragraph.ParagraphProperties = new(
            new ParagraphStyleId { Val = style });

        Run run = paragraph.AppendChild(new Run());

        for (int i = 0; i < number - 1; i++)
        {
            run.AppendChild(new CarriageReturn());
        }
    }
}