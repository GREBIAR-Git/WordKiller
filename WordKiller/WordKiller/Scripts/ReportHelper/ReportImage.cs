using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace WordKiller.Scripts.ReportHelper;

public static class ReportImage
{
    const short pixel_to_EMU = 9525;

    public static string Registration(WordprocessingDocument doc, ParagraphPicture picture)
    {
        MainDocumentPart mainPart = doc.MainDocumentPart;
        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);

        using (MemoryStream stream = new())
        {
            picture.Bitmap.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            imagePart.FeedData(stream);
        }
        return mainPart.GetIdOfPart(imagePart);
    }

    public static void Create(WordprocessingDocument wordDoc, string relationshipId, Bitmap bitmap)
    {
        int emusPerCm = 360000;
        float maxWidthCm = 16.51f;
        int maxWidthEmus = (int)(maxWidthCm * emusPerCm);

        int iWidth = bitmap.Width;
        int iHeight = bitmap.Height;
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
                         Name = "Рисунок 1"
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
                new ParagraphStyleId() { Val = "Картинка" })
        };

        paragraph.AppendChild(new Run(element));
        wordDoc.MainDocumentPart.Document.Body.AppendChild(paragraph);
    }

    public static void FullScreen(WordprocessingDocument wordDoc, string relationshipId)
    {
        ReportPageSettings.PageSetup(wordDoc.MainDocumentPart.Document.Body, 0, 0, 0, 0, true);

        int iWidth = 7545000;
        int iHeight = 10700000;

        var element =
             new Drawing(
                 new DW.Inline(
                     new DW.Extent() { Cx = iWidth, Cy = iHeight },
                     new DW.Anchor(
                     new DW.WrapNone())
                     { BehindDoc = false },
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
                         Name = "Фото место титульника"
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
        wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
    }
}
