using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels.Settings;

public class ViewModelSettings : ViewModelBase
{
    ViewModelGeneralSettings general;
    public ViewModelGeneralSettings General { get => general; set => SetProperty(ref general, value); }

    ViewModelPersonalizationSettings personalization;
    public ViewModelPersonalizationSettings Personalization { get => personalization; set => SetProperty(ref personalization, value); }

    ViewModelProfileSettings profile;
    public ViewModelProfileSettings Profile { get => profile; set => SetProperty(ref profile, value); }

    ViewModelTemplatesSettings template;
    public ViewModelTemplatesSettings Template { get => template; set => SetProperty(ref template, value); }

    Visibility visibilityGeneral;
    public Visibility VisibilityGeneral { get => visibilityGeneral; set => SetProperty(ref visibilityGeneral, value); }

    Visibility visibilityPersonalization;
    public Visibility VisibilityPersonalization { get => visibilityPersonalization; set => SetProperty(ref visibilityPersonalization, value); }

    Visibility visibilityProfile;
    public Visibility VisibilityProfile { get => visibilityProfile; set => SetProperty(ref visibilityProfile, value); }

    Visibility visibilityTemplates;
    public Visibility VisibilityTemplates { get => visibilityTemplates; set => SetProperty(ref visibilityTemplates, value); }

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
        VisibilityTemplates = Visibility.Collapsed;
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
        VisibilityTemplates = Visibility.Collapsed;
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
        VisibilityTemplates = Visibility.Collapsed;
    }

    ICommand? openTemplates;

    public ICommand OpenTemplates
    {
        get
        {
            return openTemplates ??= new RelayCommand(
            obj =>
            {
                OpenTemplatesSettings();
            });
        }
    }

    void OpenTemplatesSettings()
    {
        VisibilityGeneral = Visibility.Collapsed;
        VisibilityPersonalization = Visibility.Collapsed;
        VisibilityProfile = Visibility.Collapsed;
        VisibilityTemplates = Visibility.Visible;
    }

    public ViewModelSettings()
    {
        VisibilityGeneral = Visibility.Visible;
        VisibilityPersonalization = Visibility.Collapsed;
        VisibilityProfile = Visibility.Collapsed;
        VisibilityTemplates = Visibility.Collapsed;
        general = new();
        personalization = new();
        profile = new();
        template = new();
    }
}
