using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WordKiller.DataTypes.ParagraphData.Sections;

[Serializable]
public abstract class SectionParagraphs : Numbered
{
    ObservableCollection<IParagraphData> paragraphs;
    public ObservableCollection<IParagraphData> Paragraphs { get => paragraphs; set => SetPropertyDocument(ref paragraphs, value); }
    protected IParagraphData? First { get => Paragraphs.FirstOrDefault(); }

    protected IParagraphData? Last { get => Paragraphs.LastOrDefault(); }

    public abstract SectionParagraphs? Current(IParagraphData data);

    public void AddParagraph(IParagraphData data)
    {
        SectionParagraphs section = Current(data);
        if (section is null)
        {
            Paragraphs.Add(data);
        }
        else
        {
            section.Paragraphs.Add(data);
        }
    }

    public bool RemoveParagraph(IParagraphData data1)
    {
        foreach (IParagraphData paragraphData in paragraphs)
        {
            if (data1 == paragraphData)
            {
                paragraphs.Remove(data1);
                return true;
            }
            if (paragraphData is SectionH1 sectionH1)
            {
                if (sectionH1.RemoveParagraph(data1))
                {
                    return true;
                }
            }
            else if (paragraphData is SectionH2 sectionH2)
            {
                if (sectionH2.RemoveParagraph(data1))
                {
                    return true;
                }
            }
        }
        return false;
    }


    public SectionParagraphs()
    {
        paragraphs = new ObservableCollection<IParagraphData>();
    }
}
