using System.Windows;

namespace WordKiller.DataTypes.ParagraphData;

public interface IParagraphData
{
    string Type { get; }

    string Data { get; set; }

    string Description { get; set; }

    Visibility DescriptionVisibility { get; }
}