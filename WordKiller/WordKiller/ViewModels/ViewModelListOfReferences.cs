using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.Models;

namespace WordKiller.ViewModels
{
    [Serializable]
    public class ViewModelListOfReferences : ViewModelBase
    {
        public ObservableCollection<Book> Books { get; set; }

        public ObservableCollection<ElectronicResource> ElectronicResources { get; set; }

        bool listSourcesUsed;
        public bool ListSourcesUsed
        {
            get => listSourcesUsed;
            set
            {
                SetProperty(ref listSourcesUsed, value);
            }
        }

        bool bibliography;
        public bool Bibliography
        {
            get => bibliography;
            set
            {
                SetProperty(ref bibliography, value);
            }
        }

        Visibility visibilityElectronicResources;
        public Visibility VisibilityElectronicResources { get => visibilityElectronicResources; set => SetProperty(ref visibilityElectronicResources, value); }

        Visibility visibilityBooks;
        public Visibility VisibilityBooks
        {
            get => visibilityBooks;
            set => SetProperty(ref visibilityBooks, value);
        }

        bool openBooks;
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

        bool openElectronicResources;
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

        [NonSerialized]
        ICommand? add;
        public ICommand Add
        {
            get
            {
                return add ??= new RelayCommand(
                obj =>
                {
                    if (OpenBooks)
                    {
                        Books.Add(new Book());
                    }
                    else
                    {
                        ElectronicResources.Add(new ElectronicResource());
                    }
                });
            }
        }

        public ViewModelListOfReferences()
        {
            Books = new();
            ElectronicResources = new();
            Bibliography = true;
            ListSourcesUsed = false;
            OpenBooks = true;
            OpenElectronicResources = false;
        }
    }
}
