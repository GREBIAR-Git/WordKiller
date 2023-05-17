using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WordKiller.Scripts.ReportHelper;

public static class ReportTOC
{
    public static void Create(WordprocessingDocument doc)
    {
        ReportPageSettings.PageSetup(doc.MainDocumentPart.Document.Body, title: true);
        var sdtBlock = new SdtBlock
        {
            InnerXml = GetTOC("Содержание", 14)
        };
        doc.MainDocumentPart.Document.Body.AppendChild(sdtBlock);

        var settingsPart = doc.MainDocumentPart.AddNewPart<DocumentSettingsPart>();
        settingsPart.Settings = new Settings { BordersDoNotSurroundFooter = new BordersDoNotSurroundFooter() { Val = true } };

        settingsPart.Settings.Append(new UpdateFieldsOnOpen() { Val = true });

        ReportExtras.SectionBreak(doc);
    }

    static string GetTOC(string title, int titleFontSize)
    {
        return $@"
            <w:sdt>
                <w:sdtPr>
                    <w:id w:val=""-493258456"" />
                    <w:docPartObj>
                        <w:docPartGallery w:val=""Table of Contents"" />
                        <w:docPartUnique />
                    </w:docPartObj>
                </w:sdtPr>
                <w:sdtContent>
                    <w:p w:rsidR=""00095C65"" w:rsidRDefault=""00095C65"">
                        <w:pPr>
                            <w:jc w:val=""center"" /> 
                        </w:pPr>
                        <w:r>
                            <w:rPr>
                                <w:b /> 
                                <w:caps w:val=""true"" />  
                                <w:rFonts w:ascii=""Courier New"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                <w:sz w:val=""{titleFontSize * 2}"" /> 
                                <w:szCs w:val=""{titleFontSize * 2}"" /> 
                            </w:rPr>
                            <w:t>{title}</w:t>
                        </w:r>
                    </w:p>
                    <w:p w:rsidR=""00095C65"" w:rsidRDefault=""00095C65"">
                        <w:r>
                            <w:rPr>
                                <w:b />
                                <w:bCs />
                                <w:noProof />
                            </w:rPr>
                            <w:fldChar w:fldCharType=""begin"" />
                        </w:r>
                        <w:r>
                            <w:rPr>
                                <w:b />
                                <w:bCs />
                                <w:noProof />
                            </w:rPr>
                            <w:instrText xml:space=""preserve""> TOC \o ""1-3"" \h \z \u </w:instrText>
                        </w:r>
                        <w:r>
                            <w:rPr>
                                <w:b />
                                <w:bCs />
                                <w:noProof />
                            </w:rPr>
                            <w:fldChar w:fldCharType=""separate"" />
                        </w:r>
                        <w:r>
                            <w:rPr>
                                <w:caps w:val=""true"" />  
                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                <w:sz w:val=""{titleFontSize * 2}"" /> 
                                <w:szCs w:val=""{titleFontSize * 2}"" /> 
                                <w:noProof />
                            </w:rPr>
                            <w:t>No table of contents entries found.</w:t>
                        </w:r>
                        <w:r>
                            <w:rPr>
                                <w:b />
                                <w:bCs />
                                <w:noProof />
                            </w:rPr>
                            <w:fldChar w:fldCharType=""end"" />
                        </w:r>
                    </w:p>
                </w:sdtContent>
            </w:sdt>
                ";
    }
}
