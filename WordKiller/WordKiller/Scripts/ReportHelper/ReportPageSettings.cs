using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WordKiller.Scripts.ReportHelper;

public static class ReportPageSettings
{
    public const short cm_to_pt = 567;

    public static void HeaderPageNumber(WordprocessingDocument document, int start = 4, int type = 4)
    {
        MainDocumentPart mainDocumentPart = document.MainDocumentPart;

        Justification justification = new Justification();

        if(type % 3 == 0)
        {
            justification.Val = JustificationValues.Left;
        }
        else if(type % 3 == 1)
        {
            justification.Val = JustificationValues.Center;
        }
        else if(type % 3 == 2)
        {
            justification.Val = JustificationValues.Right;
        }

        if(type<3)
        {
            mainDocumentPart.DeleteParts(mainDocumentPart.FooterParts);

            FooterPart footerPart = mainDocumentPart.AddNewPart<FooterPart>();

            string headerPartId = mainDocumentPart.GetIdOfPart(footerPart);

            footerPart.Footer = GeneratePageNumber<Footer>(justification);
            IEnumerable<SectionProperties> sections = mainDocumentPart.Document.Body.Descendants<SectionProperties>();
            foreach (SectionProperties section in sections)
            {
                section.RemoveAllChildren<FooterReference>();
                section.PrependChild(new FooterReference { Id = headerPartId, Type = HeaderFooterValues.Default });
                section.PrependChild(new PageNumberType { Start = start});
            }
        }
        else
        {
            mainDocumentPart.DeleteParts(mainDocumentPart.HeaderParts);

            HeaderPart headerPart = mainDocumentPart.AddNewPart<HeaderPart>();

            string headerPartId = mainDocumentPart.GetIdOfPart(headerPart);

            headerPart.Header = GeneratePageNumber<Header>(justification);
            IEnumerable<SectionProperties> sections = mainDocumentPart.Document.Body.Descendants<SectionProperties>();
            foreach (SectionProperties section in sections)
            {
                section.RemoveAllChildren<HeaderReference>();
                section.PrependChild(new HeaderReference { Id = headerPartId, Type = HeaderFooterValues.Default });
                section.PrependChild(new PageNumberType { Start = start});
            }
        }

    }

    public static T GeneratePageNumber<T>(Justification justification) where T : OpenXmlElement, new()
    {
        T openXmlElement = new T();

        Paragraph paragraph = new Paragraph(
            new ParagraphProperties(
                justification,
                new SpacingBetweenLines
                {
                    After = "0",
                    Before = "0",
                    Line = "240",
                    LineRule = LineSpacingRuleValues.Auto
                }
            ),
            new Run(new SimpleField { Instruction = "Page" })
        );

        openXmlElement.Append(paragraph);
        return openXmlElement;
    }

    public static void PageSetup(Body body, float top = 2, float right = 1.5f, float bot = 2, float left = 3,
        bool title = false)
    {
        SectionProperties props = new();
        body.AppendChild(props);
        props.AddChild(new PageMargin
        {
            Top = (int)(top * cm_to_pt),
            Right = Convert.ToUInt32(right * cm_to_pt),
            Bottom = (int)(bot * cm_to_pt),
            Left = Convert.ToUInt32(left * cm_to_pt)
        });
        props.AppendChild(new PageSize
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