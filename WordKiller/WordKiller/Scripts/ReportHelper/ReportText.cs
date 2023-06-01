using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;

namespace WordKiller.Scripts.ReportHelper;

public static class ReportText
{
    public static void Text(WordprocessingDocument doc, string text, int size = 14,
    JustificationValues justify = JustificationValues.Left, bool bold = false,
    int before = 0, int after = 0, float multiplier = 1, float left = 0, float right = 0, float firstLine = 0, bool caps = false, bool tabs = false)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;

        foreach (string line in text.Split('\n'))
        {
            Paragraph paragraph = body.AppendChild(new Paragraph());

            paragraph.AppendChild(new ParagraphProperties());

            paragraph.ParagraphProperties.AddChild(new Justification()
            {
                Val = justify
            });

            if (text.Contains("\t") && tabs)
            {
                paragraph.ParagraphProperties.AddChild(new Tabs(new TabStop()
                {
                    Val = TabStopValues.Right,
                    Position = 8600
                }));
            }

            paragraph.ParagraphProperties.AddChild(new SpacingBetweenLines()
            {
                After = (after * 20).ToString(),
                Before = (before * 20).ToString(),
                Line = (multiplier * 240).ToString(),
                LineRule = LineSpacingRuleValues.Auto,
            });

            paragraph.ParagraphProperties.AddChild(new Indentation()
            {
                Left = ((int)(left * ReportPageSettings.cm_to_pt)).ToString(),
                Right = ((int)(right * ReportPageSettings.cm_to_pt)).ToString(),
                FirstLine = ((int)firstLine * ReportPageSettings.cm_to_pt).ToString()
            });
            string[] words = line.Split(' ');
            for (int i = 0; i < words.Length - 1; i++)
            {
                TextIntoParagraph(doc, words[i] + ' ', paragraph, bold, size, caps);
            }
            if (words.Length - 1 >= 0)
            {
                TextIntoParagraph(doc, words[^1], paragraph, bold, size, caps);
            }
        }
        return;
    }

    static void TextIntoParagraph(WordprocessingDocument doc, string word, Paragraph paragraph, bool bold, int size, bool caps)
    {
        Run run;
        Hyperlink hyperlink;
        if (word.StartsWith("https") || word.StartsWith("http"))
        {
            HyperlinkRelationship relation = doc.MainDocumentPart.AddHyperlinkRelationship
            (new Uri(word, UriKind.RelativeOrAbsolute), true);

            string relationid = relation.Id;

            hyperlink = paragraph.AppendChild(new Hyperlink() { Id = relationid });
            run = hyperlink.AppendChild(new Run(new Text() { Text = word, Space = SpaceProcessingModeValues.Preserve }));
            run.PrependChild(new RunProperties());
        }
        else
        {
            run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text() { Text = word, Space = SpaceProcessingModeValues.Preserve });
            run.PrependChild(new RunProperties());
        }
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
            Val = (size * ReportStyles.pt_to_halfpt).ToString()
        });

        run.RunProperties.AddChild(new Caps()
        {
            Val = caps
        });
    }

    public static void Text(WordprocessingDocument doc, string text, string style)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        Body body = mainPart.Document.Body;

        foreach (string line in text.Split('\n'))
        {
            Paragraph paragraph = body.AppendChild(new Paragraph());
            paragraph.ParagraphProperties = new ParagraphProperties(
                new ParagraphStyleId() { Val = style });
            string[] words = line.Split(' ');
            for (int i = 0; i < words.Length - 1; i++)
            {
                TextIntoParagraph(doc, words[i] + ' ', paragraph);
            }
            if (words.Length - 1 >= 0)
            {
                TextIntoParagraph(doc, words[^1], paragraph);
            }
        }
    }

    public static void TextIntoParagraph(WordprocessingDocument doc, string word, Paragraph paragraph)
    {
        Run run;
        Hyperlink hyperlink;
        if (word.StartsWith("https") || word.StartsWith("http"))
        {
            HyperlinkRelationship relation = doc.MainDocumentPart.AddHyperlinkRelationship
            (new Uri(word, UriKind.RelativeOrAbsolute), true);

            string relationid = relation.Id;

            hyperlink = paragraph.AppendChild(new Hyperlink() { Id = relationid });
            hyperlink.AppendChild(new Run(new Text() { Text = word, Space = SpaceProcessingModeValues.Preserve }));
        }
        else
        {
            run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text() { Text = word, Space = SpaceProcessingModeValues.Preserve });
        }
    }
}
