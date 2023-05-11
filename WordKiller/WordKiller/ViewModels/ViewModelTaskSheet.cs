using System;
using System.Windows;
using WordKiller.DataTypes.ParagraphData.Paragraphs;

namespace WordKiller.ViewModels
{
    [Serializable]
    public class ViewModelTaskSheet : ViewModelBase
    {
        string sourceData;

        public string SourceData
        {
            get => sourceData;
            set
            {
                SetProperty(ref sourceData, value);
            }
        }

        string toc;

        public string TOC
        {
            get => toc;
            set
            {
                SetProperty(ref toc, value);
            }
        }

        string reportingMaterial;

        public string ReportingMaterial
        {
            get => reportingMaterial;
            set
            {
                SetProperty(ref reportingMaterial, value);
            }
        }

        Visibility visibitityPhoto;
        public Visibility VisibitityPhoto { get => visibitityPhoto; set => SetProperty(ref visibitityPhoto, value); }

        Visibility visibitityTitleText;
        public Visibility VisibitityTitleText { get => visibitityTitleText; set => SetProperty(ref visibitityTitleText, value); }

        bool photo;
        public bool Photo
        {
            get => photo;
            set
            {
                SetProperty(ref photo, value);
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
                SetProperty(ref firstPicture, value);
            }
        }

        ParagraphPicture secondPicture;
        public ParagraphPicture SecondPicture
        {
            get => secondPicture;
            set
            {
                SetProperty(ref secondPicture, value);
            }
        }

        public ViewModelTaskSheet()
        {
            FirstPicture = new();
            SecondPicture = new();
            ReportingMaterial = string.Empty;
            TOC = string.Empty;
            SourceData = string.Empty;
            Photo = false;
        }
    }
}
