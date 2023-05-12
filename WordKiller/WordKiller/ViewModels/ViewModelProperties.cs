using System;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelProperties : ViewModelDocumentChanges
{
    bool title;

    public bool Title { get => title; set => SetPropertyDocument(ref title, value); }

    bool tableOfContents;

    public bool TableOfContents { get => tableOfContents; set => SetPropertyDocument(ref tableOfContents, value); }

    bool pageNumbers;

    public bool PageNumbers { get => pageNumbers; set => SetPropertyDocument(ref pageNumbers, value); }

    bool numberHeading;
    public bool NumberHeading { get => numberHeading; set => SetPropertyDocument(ref numberHeading, value); }

    bool listOfReferences;
    public bool ListOfReferences { get => listOfReferences; set => SetPropertyDocument(ref listOfReferences, value); }

    bool taskSheet;
    public bool TaskSheet { get => taskSheet; set => SetPropertyDocument(ref taskSheet, value); }

    bool appendix;
    public bool Appendix { get => appendix; set => SetPropertyDocument(ref appendix, value); }
}
