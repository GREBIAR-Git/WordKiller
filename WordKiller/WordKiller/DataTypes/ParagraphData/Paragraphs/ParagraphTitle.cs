using System;
using System.Windows;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs
{
    [Serializable]
    public class ParagraphTitle : IParagraphData
    {
        public string Type => "Title";

        public string Data { get => "Title"; set => throw new NotImplementedException(); }
        public string Description { get => "Title"; set => throw new NotImplementedException(); }

        public Visibility DescriptionVisibility()
        {
            return Visibility.Collapsed;
        }
    }
}
