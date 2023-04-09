using System;

namespace WordKiller.ViewModels
{
    [Serializable]
    public class ViewModelProperties : ViewModelBase
    {
        bool title;

        public bool Title { get => title; set => SetProperty(ref title, value); }

        bool tableOfContents;

        public bool TableOfContents { get => tableOfContents; set => SetProperty(ref tableOfContents, value); }

        bool pageNumbers;

        public bool PageNumbers { get => pageNumbers; set => SetProperty(ref pageNumbers, value); }

        bool numberHeading;
        public bool NumberHeading { get => numberHeading; set => SetProperty(ref numberHeading, value); }

        bool listOfReferences;
        public bool ListOfReferences { get => listOfReferences; set => SetProperty(ref listOfReferences, value); }

        bool taskSheet;
        public bool TaskSheet { get => taskSheet; set => SetProperty(ref taskSheet, value); }
    }
}
