using WordKiller.DataTypes.ParagraphData;

namespace WordKiller.DataTypes;

internal class FoundItem(IParagraphData paragraphData, int index)
{
    public IParagraphData Paragraph { get; set; } = paragraphData;
    public int Index { get; set; } = index;
}