using System;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels
{
    public class ViewModelMessageDragDrop : ViewModelBase
    {
        public int Number { get; set; }

        Visibility visibilityInsert;
        public Visibility VisibilityInsert { get => visibilityInsert; set => SetProperty(ref visibilityInsert, value); }

        Visibility visibilityBefore;
        public Visibility VisibilityBefore { get => visibilityBefore; set => SetProperty(ref visibilityBefore, value); }

        Visibility visibilityAfter;
        public Visibility VisibilityAfter { get => visibilityAfter; set => SetProperty(ref visibilityAfter, value); }

        Visibility visibilitySwap;
        public Visibility VisibilitySwap { get => visibilitySwap; set => SetProperty(ref visibilitySwap, value); }

        string mainColor;

        public string MainColor
        {
            get => mainColor;
            set
            {
                SetProperty(ref mainColor, value);
            }
        }

        string additionalColor;

        public string AdditionalColor { get => additionalColor; set => SetProperty(ref additionalColor, value); }

        string alternativeColor;

        public string AlternativeColor { get => alternativeColor; set => SetProperty(ref alternativeColor, value); }

        string hoverColor;

        public string HoverColor { get => hoverColor; set => SetProperty(ref hoverColor, value); }

        public Action CloseAction { get; set; }

        ICommand? exit;
        public ICommand Exit
        {
            get
            {
                return exit ??= new RelayCommand(
                obj =>
                {
                    Number = -1;
                    CloseAction();
                });
            }
        }

        ICommand? insert;
        public ICommand Insert
        {
            get
            {
                return insert ??= new RelayCommand(
                obj =>
                {
                    Number = 0;
                    CloseAction();
                });
            }
        }

        ICommand? before;
        public ICommand Before
        {
            get
            {
                return before ??= new RelayCommand(
                obj =>
                {
                    Number = 1;
                    CloseAction();
                });
            }
        }

        ICommand? after;
        public ICommand After
        {
            get
            {
                return after ??= new RelayCommand(
                obj =>
                {
                    Number = 2;
                    CloseAction();
                });
            }
        }

        ICommand? swap;
        public ICommand Swap
        {
            get
            {
                return swap ??= new RelayCommand(
                obj =>
                {
                    Number = 3;
                    CloseAction();
                });
            }
        }

        public ViewModelMessageDragDrop(Visibility insert, Visibility before, Visibility after, Visibility swap)
        {
            mainColor = WordKiller.Properties.Settings.Default.MainColor;
            additionalColor = WordKiller.Properties.Settings.Default.AdditionalColor;
            alternativeColor = WordKiller.Properties.Settings.Default.AlternativeColor;
            hoverColor = WordKiller.Properties.Settings.Default.HoverColor;
            Number = -1;
            VisibilityInsert = insert;
            VisibilityBefore = before;
            VisibilityAfter = after;
            VisibilitySwap = swap;
        }
    }
}
