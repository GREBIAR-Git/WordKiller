using System;

namespace WordKiller.ViewModels
{
    [Serializable]
    public class ViewModelProperties : ViewModelBase
    {
        bool tableOfContents;

        public bool TableOfContents { get => tableOfContents; set => SetProperty(ref tableOfContents, value); }

        bool pageNumbers;

        public bool PageNumbers { get => pageNumbers; set => SetProperty(ref pageNumbers, value); }

        bool numberHeading;
        public bool NumberHeading { get => numberHeading; set => SetProperty(ref numberHeading, value); }

    }
}
