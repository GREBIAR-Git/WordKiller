using System;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels.DialogMessage
{
    public class ViewModelMessageNeedSave : ViewModelBase
    {
        public int Number { get; set; }

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

        ICommand? yes;
        public ICommand Yes
        {
            get
            {
                return yes ??= new RelayCommand(
                obj =>
                {
                    Number = 0;
                    CloseAction();
                });
            }
        }

        ICommand? not;
        public ICommand Not
        {
            get
            {
                return not ??= new RelayCommand(
                obj =>
                {
                    Number = 1;
                    CloseAction();
                });
            }
        }

        public ViewModelMessageNeedSave()
        {
            mainColor = Properties.Settings.Default.MainColor;
            additionalColor = Properties.Settings.Default.AdditionalColor;
            alternativeColor = Properties.Settings.Default.AlternativeColor;
            hoverColor = Properties.Settings.Default.HoverColor;
            Number = -1;
        }
    }
}
