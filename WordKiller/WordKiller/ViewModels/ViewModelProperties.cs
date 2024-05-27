using System;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelProperties : ViewModelDocumentChanges
{
    bool appendix;

    bool listOfReferences;

    bool numberHeading;

    bool pageNumbers;

    bool tableOfContents;

    bool taskSheet;
    bool title;

    public bool Title
    {
        get => title;
        set => SetPropertyDocument(ref title, value);
    }

    public bool TableOfContents
    {
        get => tableOfContents;
        set => SetPropertyDocument(ref tableOfContents, value);
    }

    public bool PageNumbers
    {
        get => pageNumbers;
        set => SetPropertyDocument(ref pageNumbers, value);
    }

    public bool NumberHeading
    {
        get => numberHeading;
        set => SetPropertyDocument(ref numberHeading, value);
    }

    public bool ListOfReferences
    {
        get => listOfReferences;
        set => SetPropertyDocument(ref listOfReferences, value);
    }

    public bool TaskSheet
    {
        get => taskSheet;
        set => SetPropertyDocument(ref taskSheet, value);
    }

    public bool Appendix
    {
        get => appendix;
        set => SetPropertyDocument(ref appendix, value);
    }
}