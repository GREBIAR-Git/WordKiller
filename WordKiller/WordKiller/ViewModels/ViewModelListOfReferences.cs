using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.Models;
using WordKiller.Scripts.File;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelListOfReferences : ViewModelDocumentChanges
{
    [NonSerialized] ICommand? add;

    bool alphabeticalOrder;

    bool bibliography;

    [NonSerialized] ICommand? editCell;

    bool listSourcesUsed;

    bool openBooks;

    bool openElectronicResources;

    Visibility visibilityBooks;

    Visibility visibilityElectronicResources;

    public ViewModelListOfReferences()
    {
        Books = [];
        ElectronicResources = [];
        bibliography = true;
        listSourcesUsed = false;
        openBooks = true;
        openElectronicResources = false;
        visibilityElectronicResources = Visibility.Collapsed;
        visibilityBooks = Visibility.Visible;
    }

    public ObservableCollection<Book> Books { get; set; }

    public ObservableCollection<ElectronicResource> ElectronicResources { get; set; }

    public bool ListSourcesUsed
    {
        get => listSourcesUsed;
        set => SetPropertyDocument(ref listSourcesUsed, value);
    }

    public bool Bibliography
    {
        get => bibliography;
        set => SetPropertyDocument(ref bibliography, value);
    }

    public Visibility VisibilityElectronicResources
    {
        get => visibilityElectronicResources;
        set => SetProperty(ref visibilityElectronicResources, value);
    }

    public Visibility VisibilityBooks
    {
        get => visibilityBooks;
        set => SetProperty(ref visibilityBooks, value);
    }

    public bool OpenBooks
    {
        get => openBooks;
        set
        {
            SetProperty(ref openBooks, value);
            if (openBooks)
            {
                VisibilityElectronicResources = Visibility.Collapsed;
                VisibilityBooks = Visibility.Visible;
            }
        }
    }

    public bool OpenElectronicResources
    {
        get => openElectronicResources;
        set
        {
            SetProperty(ref openElectronicResources, value);
            if (openElectronicResources)
            {
                VisibilityElectronicResources = Visibility.Visible;
                VisibilityBooks = Visibility.Collapsed;
            }
        }
    }

    public ICommand Add
    {
        get
        {
            return add ??= new RelayCommand(
                obj =>
                {
                    if (OpenBooks)
                    {
                        Books.Add(new());
                    }
                    else
                    {
                        ElectronicResources.Add(new());
                    }

                    SaveHelper.NeedSave = true;
                });
        }
    }

    public bool AlphabeticalOrder
    {
        get => alphabeticalOrder;
        set
        {
            SetProperty(ref alphabeticalOrder, value);
            if (alphabeticalOrder)
            {
                List<ListOfReferencesResources> resours = [];
                foreach (Book book in Books)
                {
                    resours.Add(book);
                }

                foreach (ElectronicResource electronicResource in ElectronicResources)
                {
                    resours.Add(electronicResource);
                }

                resours = [.. resours.OrderBy(x => x.Full)];
                for (int i = 0; i < resours.Count; i++)
                {
                    resours[i].Id = (i + 1).ToString();
                }
            }
            else
            {
                List<ListOfReferencesResources> resours = [];
                foreach (Book book in Books)
                {
                    resours.Add(book);
                }

                foreach (ElectronicResource electronicResource in ElectronicResources)
                {
                    resours.Add(electronicResource);
                }

                for (int i = 0; i < resours.Count; i++)
                {
                    resours[i].Id = (i + 1).ToString();
                }
            }
        }
    }

    public ICommand EditCell
    {
        get { return editCell ??= new RelayCommand(obj => { SaveHelper.NeedSave = true; }); }
    }
}