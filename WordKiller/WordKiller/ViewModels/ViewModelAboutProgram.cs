namespace WordKiller.ViewModels;

public class ViewModelAboutProgram : ViewModelBase
{
    string additionalColor;

    string alternativeColor;

    string hoverColor;
    string mainColor;

    public ViewModelAboutProgram()
    {
        mainColor = Properties.Settings.Default.MainColor;
        additionalColor = Properties.Settings.Default.AdditionalColor;
        alternativeColor = Properties.Settings.Default.AlternativeColor;
        hoverColor = Properties.Settings.Default.HoverColor;
    }

    public string MainColor
    {
        get => mainColor;
        set => SetProperty(ref mainColor, value);
    }

    public string AdditionalColor
    {
        get => additionalColor;
        set => SetProperty(ref additionalColor, value);
    }

    public string AlternativeColor
    {
        get => alternativeColor;
        set => SetProperty(ref alternativeColor, value);
    }

    public string HoverColor
    {
        get => hoverColor;
        set => SetProperty(ref hoverColor, value);
    }
}