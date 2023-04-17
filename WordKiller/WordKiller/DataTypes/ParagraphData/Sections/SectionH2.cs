using System;

namespace WordKiller.DataTypes.ParagraphData.Sections;

[Serializable]
public class SectionH2 : SectionParagraphs
{
    public override SectionParagraphs? Current(IParagraphData data)
    {
        if (Last is SectionParagraphs sectionParagraphs)
        {
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
