using System.Windows;

namespace WordKiller.DataTypes.ParagraphData
{
    public interface IParagraphData
    {
        public string Type { get; }

        public string Data { get; set; }

        public string Description { get; set; }

        public Visibility DescriptionVisibility();
    }
}
