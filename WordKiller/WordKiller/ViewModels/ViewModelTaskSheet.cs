using System;
using System.Windows;
using WordKiller.DataTypes.ParagraphData.Paragraphs;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelTaskSheet : ViewModelDocumentChanges
{
    string sourceData;

    public string SourceData
    {
        get => sourceData;
        set
        {
            SetPropertyDocument(ref sourceData, value);
        }
    }

    string toc;

    public string TOC
    {
        get => toc;
        set
        {
            SetPropertyDocument(ref toc, value);
        }
    }

    string reportingMaterial;

    public string ReportingMaterial
    {
        get => reportingMaterial;
        set
        {
            SetPropertyDocument(ref reportingMaterial, value);
        }
    }

    Visibility visibitityPhoto;
    public Visibility VisibitityPhoto { get => visibitityPhoto; set => SetPropertyDocument(ref visibitityPhoto, value); }

    Visibility visibitityTitleText;
    public Visibility VisibitityTitleText { get => visibitityTitleText; set => SetPropertyDocument(ref visibitityTitleText, value); }

    bool photo;
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

    ParagraphPicture firstPicture;
    public ParagraphPicture FirstPicture
    {
        get => firstPicture;
        set
        {
            SetPropertyDocument(ref firstPicture, value);
        }
    }

    ParagraphPicture secondPicture;
    public ParagraphPicture SecondPicture
    {
        get => secondPicture;
        set
        {
            SetPropertyDocument(ref secondPicture, value);
        }
    }

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
}
