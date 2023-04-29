using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphTaskSheet : IParagraphData
{
    public string Type => "TaskSheet";

    public string Data { get => "TaskSheet"; set => throw new NotImplementedException(); }
    public string Description { get => "TaskSheet"; set => throw new NotImplementedException(); }

    public Visibility DescriptionVisibility
    {
        get => Visibility.Collapsed;
    }
}
