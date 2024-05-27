using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels.Settings;

public class ViewModelSettings : ViewModelBase
{
    ViewModelGeneralSettings general;

    ICommand? openGeneral;

    ICommand? openPersonalization;

    ICommand? openProfile;

    ICommand? openTemplates;

    ViewModelPersonalizationSettings personalization;

    ViewModelProfileSettings profile;

    ViewModelTemplatesSettings template;

    Visibility visibilityGeneral;

    Visibility visibilityPersonalization;

    Visibility visibilityProfile;

    Visibility visibilityTemplates;

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

    public ViewModelGeneralSettings General
    {
        get => general;
        set => SetProperty(ref general, value);
    }

    public ViewModelPersonalizationSettings Personalization
    {
        get => personalization;
        set => SetProperty(ref personalization, value);
    }

    public ViewModelProfileSettings Profile
    {
        get => profile;
        set => SetProperty(ref profile, value);
    }

    public ViewModelTemplatesSettings Template
    {
        get => template;
        set => SetProperty(ref template, value);
    }

    public Visibility VisibilityGeneral
    {
        get => visibilityGeneral;
        set => SetProperty(ref visibilityGeneral, value);
    }

    public Visibility VisibilityPersonalization
    {
        get => visibilityPersonalization;
        set => SetProperty(ref visibilityPersonalization, value);
    }

    public Visibility VisibilityProfile
    {
        get => visibilityProfile;
        set => SetProperty(ref visibilityProfile, value);
    }

    public Visibility VisibilityTemplates
    {
        get => visibilityTemplates;
        set => SetProperty(ref visibilityTemplates, value);
    }

    public ICommand OpenGeneral
    {
        get
        {
            return openGeneral ??= new RelayCommand(
                obj => { OpenGeneralSettings(); });
        }
    }

    public ICommand OpenPersonalization
    {
        get
        {
            return openPersonalization ??= new RelayCommand(
                obj => { OpenPersonalizationSettings(); });
        }
    }

    public ICommand OpenProfile
    {
        get
        {
            return openProfile ??= new RelayCommand(
                obj => { OpenProfileSettings(); });
        }
    }

    public ICommand OpenTemplates
    {
        get
        {
            return openTemplates ??= new RelayCommand(
                obj => { OpenTemplatesSettings(); });
        }
    }

    public void OpenGeneralSettings()
    {
        VisibilityGeneral = Visibility.Visible;
        VisibilityPersonalization = Visibility.Collapsed;
        VisibilityProfile = Visibility.Collapsed;
        VisibilityTemplates = Visibility.Collapsed;
    }

    void OpenPersonalizationSettings()
    {
        VisibilityGeneral = Visibility.Collapsed;
        VisibilityPersonalization = Visibility.Visible;
        VisibilityProfile = Visibility.Collapsed;
        VisibilityTemplates = Visibility.Collapsed;
    }

    void OpenProfileSettings()
    {
        VisibilityGeneral = Visibility.Collapsed;
        VisibilityPersonalization = Visibility.Collapsed;
        VisibilityProfile = Visibility.Visible;
        VisibilityTemplates = Visibility.Collapsed;
    }

    void OpenTemplatesSettings()
    {
        VisibilityGeneral = Visibility.Collapsed;
        VisibilityPersonalization = Visibility.Collapsed;
        VisibilityProfile = Visibility.Collapsed;
        VisibilityTemplates = Visibility.Visible;
    }
}