using WordKiller.DataTypes.ParagraphData;

namespace WordKiller.DataTypes
{
    public class DragDropInfo
    {
        public IParagraphData paragraphData { get; private set; }

        public DragDropInfo(IParagraphData paragraphData)
        {
            this.paragraphData = paragraphData;
        }
    }
}
