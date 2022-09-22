using Mammoth;
using System;
using System.IO;
using System.Text;
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

    public async static void ToHTML(string path)
    {
        DocumentConverter converter = new DocumentConverter()
            .AddStyleMap("p[style-name='H1'] => h1:fresh")
            .AddStyleMap("p[style-name='H2'] => h2:fresh"); ;
        string HTMLFilePath = Path.ChangeExtension(path, ".html");
        var result = converter.ConvertToHtml(path);
        string text = result.Value;
        using (FileStream fstream = new(HTMLFilePath, FileMode.Create))
        {
            byte[] buffer = Encoding.Default.GetBytes(text);
            await fstream.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    static void ReleaseObject(object obj)
    {
        try
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
            obj = null;
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
