using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;

namespace WordKiller.Scripts.ReportHelper
{
    public static class ReportPageSettings
    {
        public const short cm_to_pt = 567;

        public static void PageNumber(WordprocessingDocument document, int start = 4)
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
                section.PrependChild(new PageNumberType { Start = start });
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

        public static void PageSetup(Body body, float top = 2, float right = 1.5f, float bot = 2, float left = 3, bool title = false)
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
    }
}
