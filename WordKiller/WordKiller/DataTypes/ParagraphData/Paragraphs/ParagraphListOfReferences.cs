using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphListOfReferences : IParagraphData
{
    public string Type => "ListOfReferences";

    public string Data { get => "ListOfReferences"; set => throw new NotImplementedException(); }
    public string Description { get => "ListOfReferences"; set => throw new NotImplementedException(); }

    public Visibility DescriptionVisibility
    {
        get => Visibility.Collapsed;
    }
}
