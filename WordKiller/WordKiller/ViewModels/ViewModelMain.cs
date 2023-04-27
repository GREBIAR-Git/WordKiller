using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.Scripts.ForUI;
using WordKiller.ViewModels.Settings;

namespace WordKiller.ViewModels;
public class ViewModelMain : ViewModelBase
{
    ViewModelDocument document;
    public ViewModelDocument Document { get => document; set => SetProperty(ref document, value); }

    ViewModelSettings settings;
    public ViewModelSettings Settings { get => settings; set => SetProperty(ref settings, value); }

    ViewModelResizing resizing;
    public ViewModelResizing Resizing { get => resizing; set => SetProperty(ref resizing, value); }

    public NetworkCommans NetworkCommans { get; set; }

    public HelpCommands HelpCommands { get; set; }

    Visibility visibilityMainPanel;

    public Visibility VisibilityMainPanel { get => visibilityMainPanel; set => SetProperty(ref visibilityMainPanel, value); }

    Visibility visibilitySettingsPanel;

    public Visibility VisibilitySettingsPanel { get => visibilitySettingsPanel; set => SetProperty(ref visibilitySettingsPanel, value); }

    ICommand? openSettings;

    public ICommand OpenSettings
    {
        get
        {
            return openSettings ??= new RelayCommand(
            obj =>
            {
                VisibilityMainPanel = Visibility.Collapsed;
                VisibilitySettingsPanel = Visibility.Visible;
            });
        }
    }

    ICommand? exitSettings;

    public ICommand ExitSettings
    {
        get
        {
            return exitSettings ??= new RelayCommand(obj =>
            {
                VisibilitySettingsPanel = Visibility.Collapsed;
                VisibilityMainPanel = Visibility.Visible;
            });
        }
    }

    ICommand? quit;

    public ICommand Quit
    {
        get
        {
            return quit ??= new RelayCommand(obj =>
            {
                UIHelper.WindowClose();
            });
        }
    }

    ICommand? updatePerformed;

    public ICommand UpdatePerformed
    {
        get
        {
            return updatePerformed ??= new RelayCommand(
            obj =>
            {
                Document.Data.Title.Performed = Settings.Profile.Users;
            });
        }
    }

    Visibility visibilityDrag;
    public Visibility VisibilityDrag { get => visibilityDrag; set => SetProperty(ref visibilityDrag, value); }

    public ViewModelMain()
    {
        Document = new();
        HelpCommands = new();
        NetworkCommans = new();
        settings = new();
        resizing = new();
        VisibilitySettingsPanel = Visibility.Collapsed;
        VisibilityMainPanel = Visibility.Visible;
        VisibilityDrag = Visibility.Collapsed;
    }
}
