using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphAppendix : IParagraphData
{
    public string Type => "Appendix";

    public string Data { get => "Appendix"; set => throw new NotImplementedException(); }
    public string Description { get => "Appendix"; set => throw new NotImplementedException(); }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Collapsed;
    }
}
