using System;
using System.IO;
using Word = Microsoft.Office.Interop.Word;

namespace WordKiller.Scripts;

static class WkrExport
{
    public static bool IsWordInstall()
    {
        Type? officeType = Type.GetTypeFromProgID("Word.Application");

        if (officeType == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //только при наличии ворда
    public static void ToPDF(string path)
    {
        object misValue = System.Reflection.Missing.Value;
        string PDFFilePath = Path.ChangeExtension(path, ".pdf");

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
    public static void ToHTML(object path)
    {
        Word._Application WORD = new Word.Application();
        Word.Documents doc = WORD.Documents;
        object Unknown = Type.Missing;
        object HTMLFilePath = Path.ChangeExtension((string)path, ".html");
        Word.Document od = doc.Open(ref path, ref Unknown,
                                 ref Unknown, ref Unknown, ref Unknown,
                                 ref Unknown, ref Unknown, ref Unknown,
                                 ref Unknown, ref Unknown, ref Unknown,
                                 ref Unknown, ref Unknown, ref Unknown, ref Unknown);
        object format = Word.WdSaveFormat.wdFormatHTML;



        WORD.ActiveDocument.SaveAs(ref HTMLFilePath, ref format,
                    ref Unknown, ref Unknown, ref Unknown,
                    ref Unknown, ref Unknown, ref Unknown,
                    ref Unknown, ref Unknown, ref Unknown,
                    ref Unknown, ref Unknown, ref Unknown,
                    ref Unknown, ref Unknown);

        WORD.Documents.Close(Word.WdSaveOptions.wdDoNotSaveChanges);

        ReleaseObject(doc);
        ReleaseObject(WORD);
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