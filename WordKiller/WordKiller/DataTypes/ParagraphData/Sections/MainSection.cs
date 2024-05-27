using System;
using WordKiller.DataTypes.ParagraphData.Paragraphs;

namespace WordKiller.DataTypes.ParagraphData.Sections;

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

    public IParagraphData? PrevLevel(SectionParagraphs section, IParagraphData data)
    {
        foreach (IParagraphData paragraph in section.Paragraphs)
        {
            if (paragraph == data)
            {
                return section as IParagraphData;
            }

            if (paragraph is SectionParagraphs section1)
            {
                IParagraphData? paragraphData = PrevLevel(section1, data);
                if (paragraphData != null)
                {
                    return paragraphData;
                }
            }
        }

        return null;
    }

    public void AddToTop(IParagraphData data)
    {
        if (Paragraphs.Count > 1 && Paragraphs[0] is ParagraphTitle && Paragraphs[1] is ParagraphTaskSheet)
        {
            InsertAfter(Paragraphs[1], data);
        }
        else if ((Paragraphs.Count > 0 && Paragraphs[0] is ParagraphTitle) ||
                 (Paragraphs.Count > 0 && Paragraphs[0] is ParagraphTaskSheet))
        {
            InsertAfter(Paragraphs[0], data);
        }
        else
        {
            if (Paragraphs.Count > 0)
            {
                InsertBefore(Paragraphs[0], data);
            }
            else
            {
                Paragraphs.Add(data);
            }
        }
    }

    public void AddToEnd(IParagraphData data)
    {
        if (Paragraphs.Count > 1 && Paragraphs[^2] is ParagraphListOfReferences && Paragraphs[^1] is ParagraphAppendix)
        {
            if (Paragraphs.Count > 2 && Paragraphs[^3] is SectionParagraphs paragraph && data is not ParagraphH1)
            {
                paragraph.AddParagraph(data);
            }
            else
            {
                InsertBefore(Paragraphs[^2], data);
            }
        }
        else if ((Paragraphs.Count > 0 && Paragraphs[^1] is ParagraphListOfReferences) ||
                 (Paragraphs.Count > 0 && Paragraphs[^1] is ParagraphAppendix))
        {
            if (Paragraphs.Count > 1 && Paragraphs[^2] is SectionParagraphs paragraph && data is not ParagraphH1)
            {
                paragraph.AddParagraph(data);
            }
            else
            {
                InsertBefore(Paragraphs[^1], data);
            }
        }
        else
        {
            Paragraphs.Add(data);
        }
    }

    public void SwapParagraphs(IParagraphData paragraphData1, IParagraphData paragraphData2)
    {
        SectionParagraphs? section1 = FindSection(this, paragraphData1);
        SectionParagraphs? section2 = FindSection(this, paragraphData2);
        int i = section1.Paragraphs.IndexOf(paragraphData1);
        int f = section2.Paragraphs.IndexOf(paragraphData2);
        (section1.Paragraphs[i], section2.Paragraphs[f]) = (section2.Paragraphs[f], section1.Paragraphs[i]);
    }

    public void InsertBefore(IParagraphData into, IParagraphData insert)
    {
        SectionParagraphs? section = FindSection(this, into);
        int i = section.Paragraphs.IndexOf(into);
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