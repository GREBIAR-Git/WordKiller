using System;
using WordKiller.DataTypes.ParagraphData.Paragraphs;

namespace WordKiller.DataTypes.ParagraphData.Sections;

[Serializable]
public class SectionH1 : SectionParagraphs
{
    public override SectionParagraphs? Current(IParagraphData data)
    {
        if (Last is SectionParagraphs sectionParagraphs)
        {
            if (data is ParagraphH2)
            {
                return null;
            }
            SectionParagraphs sectionParagraphs1 = sectionParagraphs.Current(data);
            if (sectionParagraphs1 is null)
            {
                return sectionParagraphs;
            }
            else
            {
                return sectionParagraphs1;
            }
        }
        return null;
    }
}
