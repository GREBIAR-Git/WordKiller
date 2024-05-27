using WordKiller.DataTypes.ParagraphData;

namespace WordKiller.DataTypes;

public class DragDropInfo(IParagraphData paragraphData)
{
    public IParagraphData ParagraphData { get; private set; } = paragraphData;
}