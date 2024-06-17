using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Windows;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphH1 : SectionH1, IParagraphData
{
    string data;

    public ParagraphH1(string data)
    {
        this.data = data;
    }

    public ParagraphH1()
    {
        data = string.Empty;
    }

    public string Type => "Header";

    public string Data
    {
        get => data;
        set => SetPropertyDocument(ref data, CapsLockHelper.ToCapsLockH1(value), "Description");
    }

    public string Description
    {
        get => data.Replace("\r\n", " ");
        set => SetPropertyDocument(ref data, CapsLockHelper.ToCapsLockH1(value), "Data");
    }

    public Visibility DescriptionVisibility => Visibility.Collapsed;
}