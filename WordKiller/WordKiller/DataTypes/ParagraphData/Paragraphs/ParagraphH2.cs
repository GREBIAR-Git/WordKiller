using System;
using System.Windows;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphH2 : SectionH2, IParagraphData
{
    string data;

    public ParagraphH2(string data)
    {
        this.data = data;
    }

    public ParagraphH2()
    {
        data = string.Empty;
    }

    public string Type => "SubHeader";

    public string Data
    {
        get => data;
        set => SetPropertyDocument(ref data, CapsLockHelper.ToCapsLockH2(value), "Description");
    }

    public string Description
    {
        get => data.Replace("\r\n", " ");
        set => SetPropertyDocument(ref data, CapsLockHelper.ToCapsLockH2(value), "Data");
    }

    public Visibility DescriptionVisibility => Visibility.Collapsed;
}