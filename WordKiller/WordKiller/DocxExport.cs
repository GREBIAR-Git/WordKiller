using System;
using System.IO;
using Word = Microsoft.Office.Interop.Word;

namespace WordKiller;

static class DocxExport
{
    //только при наличии ворда
    public static void ToPDF(string path)
    {
        object misValue = System.Reflection.Missing.Value;
        String PDFFilePath = Path.ChangeExtension(path, ".pdf");

        Word.Application WORD = new();

        Word.Document doc = WORD.Documents.Open(path);
        doc.Activate();

        doc.SaveAs2(PDFFilePath, Word.WdSaveFormat.wdFormatPDF, misValue, misValue, misValue,
        misValue, misValue, misValue, misValue, misValue, misValue, misValue);

        doc.Close();
        WORD.Quit();

        ReleaseObject(doc);
        ReleaseObject(WORD);
    }

    //нужно починить
    public static void ToHTML(string path)
    {
        Console.WriteLine("does not work");
        /*var sourceDocxFileContent = File.ReadAllBytes(path);
        string HTMLFilePath = Path.ChangeExtension(path, ".html");
        using var memoryStream = new MemoryStream();
        await memoryStream.WriteAsync(sourceDocxFileContent);
        using var wordProcessingDocument = WordprocessingDocument.Open(memoryStream, true);
        HtmlConverterSettings settings1 = new()
        {
            PageTitle = "My Page Title"
        };
        var settings = new WmlToHtmlConverterSettings(settings1);
        var html = WmlToHtmlConverter.ConvertToHtml(wordProcessingDocument, settings);
        var htmlString = html.ToString(SaveOptions.DisableFormatting);
        File.WriteAllText(HTMLFilePath, htmlString, Encoding.UTF8);*/
    }

    static void ReleaseObject(object? obj)
    {
        try
        {
            if (obj != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            GC.Collect();
        }
    }
}