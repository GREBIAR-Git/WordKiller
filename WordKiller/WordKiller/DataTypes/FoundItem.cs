using WordKiller.DataTypes.ParagraphData;

namespace WordKiller.DataTypes;

internal class FoundItem
{
    public IParagraphData Paragraph { get; set; }
    public int Index { get; set; }

    public FoundItem(IParagraphData paragraphData, int index) 
    { 
        Paragraph = paragraphData;
        Index = index;
    }
}
