using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels.Settings
{
    public class ViewModelSettings : ViewModelBase
    {
        ViewModelGeneralSettings general;
        public ViewModelGeneralSettings General { get => general; set => SetProperty(ref general, value); }

        ViewModelPersonalizationSettings personalization;
        public ViewModelPersonalizationSettings Personalization { get => personalization; set => SetProperty(ref personalization, value); }

        ViewModelProfileSettings profile;
        public ViewModelProfileSettings Profile { get => profile; set => SetProperty(ref profile, value); }

        Visibility visibilityGeneral;

        public Visibility VisibilityGeneral { get => visibilityGeneral; set => SetProperty(ref visibilityGeneral, value); }

        Visibility visibilityPersonalization;

        public Visibility VisibilityPersonalization { get => visibilityPersonalization; set => SetProperty(ref visibilityPersonalization, value); }

        Visibility visibilityProfile;

        public Visibility VisibilityProfile { get => visibilityProfile; set => SetProperty(ref visibilityProfile, value); }

        ICommand? openGeneral;

        public ICommand OpenGeneral
        {
            get
            {
                return openGeneral ??= new RelayCommand(
                obj =>
                {
                    OpenGeneralSettings();
                });
            }
        }

        public void OpenGeneralSettings()
        {
            VisibilityGeneral = Visibility.Visible;
            VisibilityPersonalization = Visibility.Collapsed;
            VisibilityProfile = Visibility.Collapsed;
        }

        ICommand? openPersonalization;

        public ICommand OpenPersonalization
        {
            get
            {
                return openPersonalization ??= new RelayCommand(
                obj =>
                {
                    OpenPersonalizationSettings();
                });
            }
        }

        void OpenPersonalizationSettings()
        {
            VisibilityGeneral = Visibility.Collapsed;
            VisibilityPersonalization = Visibility.Visible;
            VisibilityProfile = Visibility.Collapsed;
        }

        ICommand? openProfile;

        public ICommand OpenProfile
        {
            get
            {
                return openProfile ??= new RelayCommand(
                obj =>
                {
                    OpenProfileSettings();
                });
            }
        }

        void OpenProfileSettings()
        {
            VisibilityGeneral = Visibility.Collapsed;
            VisibilityPersonalization = Visibility.Collapsed;
            VisibilityProfile = Visibility.Visible;
        }

        public ViewModelSettings()
        {
            general = new();
            personalization = new();
            profile = new();
        }
    }
}
