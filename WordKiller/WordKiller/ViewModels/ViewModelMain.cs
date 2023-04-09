using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes;
using WordKiller.ViewModels.Settings;

namespace WordKiller.ViewModels;
public class ViewModelMain : ViewModelBase
{
    DocumentData data;
    public DocumentData Data { get => data; set => SetProperty(ref data, value); }

    ViewModelSettings settings;
    public ViewModelSettings Settings { get => settings; set => SetProperty(ref settings, value); }

    ViewModelExport export;
    public ViewModelExport Export { get => export; set => SetProperty(ref export, value); }

    public NetworkCommans NetworkCommans { get; set; }

    public HelpCommands HelpCommands { get; set; }

    string? winTitle;
    public string? WinTitle { get => winTitle; set => SetProperty(ref winTitle, value); }

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
                settings.OpenGeneralSettings();
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

    public ViewModelMain()
    {
        Data = new();
        winTitle = "WordKiller";
        HelpCommands = new();
        NetworkCommans = new();
        settings = new();
        export = new();
        VisibilitySettingsPanel = Visibility.Collapsed;
        VisibilityMainPanel = Visibility.Visible;
    }
}
