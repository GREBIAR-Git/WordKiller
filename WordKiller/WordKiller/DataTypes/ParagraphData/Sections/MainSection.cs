using System;
using WordKiller.DataTypes.ParagraphData.Paragraphs;

namespace WordKiller.DataTypes.ParagraphData.Sections
{
    [Serializable]
    public class MainSection : SectionParagraphs
    {
        public override SectionParagraphs? Current(IParagraphData data)
        {
            if (Last is SectionParagraphs sectionParagraphs)
            {
                if (data is ParagraphH1)
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

        public void SwapParagraphs(IParagraphData paragraphData1, IParagraphData paragraphData2)
        {
            SectionParagraphs? section1 = FindSection(this, paragraphData1);
            SectionParagraphs? section2 = FindSection(this, paragraphData2);
            int i = section1.Paragraphs.IndexOf(paragraphData1);
            int f = section2.Paragraphs.IndexOf(paragraphData2);
            (section1.Paragraphs[i], section2.Paragraphs[f]) = (section2.Paragraphs[f], section1.Paragraphs[i]);
        }

        public void InsertBefore(IParagraphData old, IParagraphData insert)
        {
            SectionParagraphs? section = FindSection(this, old);
            int i = section.Paragraphs.IndexOf(old);
            section.Paragraphs.Insert(i, insert);
        }

        public void InsertAfter(IParagraphData old, IParagraphData insert)
        {
            SectionParagraphs? section = FindSection(this, old);
            int i = section.Paragraphs.IndexOf(old);
            section.Paragraphs.Insert(i + 1, insert);
        }

        SectionParagraphs? FindSection(SectionParagraphs section, IParagraphData paragraphData)
        {
            foreach (var paragraph in section.Paragraphs)
            {
                if (paragraph == paragraphData)
                {
                    return section;
                }
                else if (paragraph is SectionParagraphs paragraphsChild)
                {
                    SectionParagraphs sectionParagraphs = FindSection(paragraphsChild, paragraphData);
                    if (sectionParagraphs is not null)
                    {
                        return sectionParagraphs;
                    }
                }
            }
            return null;
        }
    }

}
