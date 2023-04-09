using WordKiller.DataTypes.ParagraphData;

namespace WordKiller.DataTypes
{
    public class DragDropInfo
    {
        public IParagraphData ParagraphData { get; private set; }

        public DragDropInfo(IParagraphData paragraphData)
        {
            ParagraphData = paragraphData;
        }
    }
}
