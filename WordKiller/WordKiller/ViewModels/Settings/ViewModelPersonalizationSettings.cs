using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.Scripts;

namespace WordKiller.ViewModels.Settings;

public class ViewModelPersonalizationSettings : ViewModelBase
{
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

    string activeColor;
    public string ActiveColor { get => activeColor; set => SetProperty(ref activeColor, value); }

    double fontSize;
    public double FontSize
    {
        get => fontSize;
        set
        {
            SetProperty(ref fontSize, value);
            double size = 1;
            if (fontSize >= 1)
            {
                size = ((int)fontSize);
            }
            fontSize = size;
            Properties.Settings.Default.FontSize = size;
            Properties.Settings.Default.Save();
        }
    }

    double fontSizeRTB;
    public double FontSizeRTB
    {
        get => fontSizeRTB;
        set
        {
            SetProperty(ref fontSizeRTB, value);
            double size = 1;
            if (fontSizeRTB >= 1)
            {
                size = ((int)fontSizeRTB);
            }
            fontSizeRTB = size;
            Properties.Settings.Default.FontSizeRTB = size;
            Properties.Settings.Default.Save();
            //-->UpdateTable();
        }
    }

    int language;
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

    ICommand? byDefault;

    public ICommand ByDefault
    {
        get
        {
            return byDefault ??= new RelayCommand(
            obj =>
            {
                MainColor = "#8daacc";
                AdditionalColor = "#4a76a8";
                AlternativeColor = "#335e8f";
                HoverColor = "#b8860b";
                ActiveColor = "#ff0000";
                Language = 0;
                FontSize = 28;
                FontSizeRTB = 20;
                Properties.Settings.Default.MainColor = mainColor;
                Properties.Settings.Default.AdditionalColor = additionalColor;
                Properties.Settings.Default.AlternativeColor = alternativeColor;
                Properties.Settings.Default.HoverColor = hoverColor;
                Properties.Settings.Default.ActiveColor = activeColor;
                Properties.Settings.Default.Save();
            });
        }
    }

    ICommand? сlosingMainColor;

    public ICommand ClosingMainColor
    {
        get
        {
            return сlosingMainColor ??= new RelayCommand(
            obj =>
            {
                Properties.Settings.Default.MainColor = MainColor;
                Properties.Settings.Default.Save();
            });
        }
    }

    ICommand? сlosingAdditionalColor;

    public ICommand ClosingAdditionalColor
    {
        get
        {
            return сlosingAdditionalColor ??= new RelayCommand(
            obj =>
            {
                Properties.Settings.Default.AdditionalColor = AdditionalColor;
                Properties.Settings.Default.Save();
            });
        }
    }

    ICommand? сlosingAlternativeColor;

    public ICommand ClosingAlternativeColor
    {
        get
        {
            return сlosingAlternativeColor ??= new RelayCommand(obj =>
            {
                Properties.Settings.Default.AlternativeColor = AlternativeColor;
                Properties.Settings.Default.Save();
            });
        }
    }

    ICommand? сlosingHoverColor;

    public ICommand ClosingHoverColor
    {
        get
        {
            return сlosingHoverColor ??= new RelayCommand(obj =>
            {
                Properties.Settings.Default.HoverColor = HoverColor;
                Properties.Settings.Default.Save();
            });
        }
    }

    ICommand? closingActiveColor;

    public ICommand ClosingActiveColor
    {
        get
        {
            return closingActiveColor ??= new RelayCommand(obj =>
            {
                Properties.Settings.Default.ActiveColor = ActiveColor;
                Properties.Settings.Default.Save();
            });
        }
    }

    public ViewModelPersonalizationSettings()
    {
        mainColor = WordKiller.Properties.Settings.Default.MainColor;
        additionalColor = WordKiller.Properties.Settings.Default.AdditionalColor;
        alternativeColor = WordKiller.Properties.Settings.Default.AlternativeColor;
        hoverColor = WordKiller.Properties.Settings.Default.HoverColor;
        activeColor = WordKiller.Properties.Settings.Default.ActiveColor;
        fontSize = WordKiller.Properties.Settings.Default.FontSize;
        fontSizeRTB = WordKiller.Properties.Settings.Default.FontSizeRTB;
        language = Properties.Settings.Default.Language;
    }
}
