using System;
using System.Windows;
using WordKiller.DataTypes.ParagraphData.Paragraphs;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelTaskSheet : ViewModelDocumentChanges
{
    ParagraphPicture firstPicture;

    bool photo;

    string reportingMaterial;

    ParagraphPicture secondPicture;
    string sourceData;

    string toc;

    Visibility visibitityPhoto;

    Visibility visibitityTitleText;

    public ViewModelTaskSheet()
    {
        firstPicture = new();
        secondPicture = new();
        reportingMaterial = string.Empty;
        toc = string.Empty;
        sourceData = string.Empty;
        photo = false;
        visibitityTitleText = Visibility.Visible;
        visibitityPhoto = Visibility.Collapsed;
    }

    public string SourceData
    {
        get => sourceData;
        set => SetPropertyDocument(ref sourceData, value);
    }

    public string TOC
    {
        get => toc;
        set => SetPropertyDocument(ref toc, value);
    }

    public string ReportingMaterial
    {
        get => reportingMaterial;
        set => SetPropertyDocument(ref reportingMaterial, value);
    }

    public Visibility VisibitityPhoto
    {
        get => visibitityPhoto;
        set => SetPropertyDocument(ref visibitityPhoto, value);
    }

    public Visibility VisibitityTitleText
    {
        get => visibitityTitleText;
        set => SetPropertyDocument(ref visibitityTitleText, value);
    }

    public bool Photo
    {
        get => photo;
        set
        {
            SetPropertyDocument(ref photo, value);
            if (photo)
            {
                VisibitityTitleText = Visibility.Collapsed;
                VisibitityPhoto = Visibility.Visible;
            }
            else
            {
                VisibitityTitleText = Visibility.Visible;
                VisibitityPhoto = Visibility.Collapsed;
            }
        }
    }

    public ParagraphPicture FirstPicture
    {
        get => firstPicture;
        set => SetPropertyDocument(ref firstPicture, value);
    }

    public ParagraphPicture SecondPicture
    {
        get => secondPicture;
        set => SetPropertyDocument(ref secondPicture, value);
    }
}