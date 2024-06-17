using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using WordKiller.Commands;
using WordKiller.Scripts;

namespace WordKiller.ViewModels.Settings;

public class ViewModelPersonalizationSettings : ViewModelBase
{

    ICommand? restoreDefaultSelectedFonts;
    public ICommand RestoreDefaultSelectedFonts
    {
        get
        {
            return restoreDefaultSelectedFonts ??= new RelayCommand(
                obj =>
                {
                    SelectedFonts = "Arial";
                    Properties.Settings.Default.SelectedFonts = SelectedFonts;
                    Properties.Settings.Default.Save();
                });
        }
    }

    ICommand? restoreDefaultSelectedFontsRTB;

    public ICommand RestoreDefaultSelectedFontsRTB
    {
        get
        {
            return restoreDefaultSelectedFontsRTB ??= new RelayCommand(
                obj =>
                {
                    SelectedFontsRTB = "Arial";
                    Properties.Settings.Default.SelectedFontsRTB = SelectedFontsRTB;
                    Properties.Settings.Default.Save();
                });
        }
    }


    BindingList<string> availableFonts;


    public BindingList<string> AvailableFonts
    {
        get => availableFonts;
        set => SetProperty(ref availableFonts, value);
    }


    string selectedFonts;

    public string SelectedFonts
    {
        get => selectedFonts;
        set
        {
            SetProperty(ref selectedFonts, value);
            Properties.Settings.Default.SelectedFonts = SelectedFonts;
            Properties.Settings.Default.Save();
        }
    }

    BindingList<string> availableFontsRTB;


    public BindingList<string> AvailableFontsRTB
    {
        get => availableFontsRTB;
        set => SetProperty(ref availableFontsRTB, value);
    }


    string selectedFontsRTB;

    public string SelectedFontsRTB
    {
        get => selectedFontsRTB;
        set
        {
            SetProperty(ref selectedFontsRTB, value);
            Properties.Settings.Default.SelectedFontsRTB = SelectedFontsRTB;
            Properties.Settings.Default.Save();
        }
    }


    string accentColor;

    ICommand? restoreDefaultAccentColor;


    public ICommand RestoreDefaultAccentColor
    {
        get
        {
            return restoreDefaultAccentColor ??= new RelayCommand(
                obj =>
                {
                    AccentColor = "#9c6b33";
                    Properties.Settings.Default.AccentColor = accentColor;
                    Properties.Settings.Default.Save();
                });
        }
    }

    ICommand? closingAccentColor;

    double fontSize;

    double fontSizeRTB;


    int language;

    public ViewModelPersonalizationSettings()
    {
        AvailableFonts = [];
        AvailableFontsRTB = [];


        foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
        {
            AvailableFonts.Add(fontFamily.Source);
            AvailableFontsRTB.Add(fontFamily.Source);
        }

        SelectedFonts = Properties.Settings.Default.SelectedFonts;
        SelectedFontsRTB = Properties.Settings.Default.SelectedFontsRTB;
        fontSize = Properties.Settings.Default.FontSize;
        fontSizeRTB = Properties.Settings.Default.FontSizeRTB;
        language = Properties.Settings.Default.Language;
        accentColor = Properties.Settings.Default.AccentColor;
    }

    public string AccentColor
    {
        get => accentColor;
        set => SetProperty(ref accentColor, value);
    }

    ICommand? restoreDefaultFontSize;

    public ICommand RestoreDefaultFontSize
    {
        get
        {
            return restoreDefaultFontSize ??= new RelayCommand(
                obj =>
                {
                    FontSize = 28;
                    Properties.Settings.Default.FontSize = FontSize;
                    Properties.Settings.Default.Save();
                });
        }
    }

    public double FontSize
    {
        get => fontSize;
        set
        {
            SetProperty(ref fontSize, value);
            double size = 1;
            if (fontSize >= 1)
            {
                size = (int)fontSize;
            }

            fontSize = size;
            Properties.Settings.Default.FontSize = size;
            Properties.Settings.Default.Save();
        }
    }

    ICommand? restoreDefaultFontSizeRTB;


    public ICommand RestoreDefaultFontSizeRTB
    {
        get
        {
            return restoreDefaultFontSizeRTB ??= new RelayCommand(
            obj =>
            {
                FontSizeRTB = 18;
                Properties.Settings.Default.FontSizeRTB = FontSizeRTB;
                Properties.Settings.Default.Save();
            });
        }
    }

    public double FontSizeRTB
    {
        get => fontSizeRTB;
        set
        {
            SetProperty(ref fontSizeRTB, value);
            double size = 1;
            if (fontSizeRTB >= 1)
            {
                size = (int)fontSizeRTB;
            }

            fontSizeRTB = size;
            Properties.Settings.Default.FontSizeRTB = size;
            Properties.Settings.Default.Save();
            //-->UpdateTable();
        }
    }

    public int Language
    {
        get => language;
        set
        {
            SetProperty(ref language, value);
            UIHelper.SelectCulture(Language);
            Properties.Settings.Default.Language = Language;
            Properties.Settings.Default.Save();
        }
    }

    public ICommand ClosingAccentColor
    {
        get
        {
            return closingAccentColor ??= new RelayCommand(
                obj =>
                {
                    Properties.Settings.Default.AccentColor = AccentColor;
                    Properties.Settings.Default.Save();
                });
        }
    }
}