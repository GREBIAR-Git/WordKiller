namespace WordKiller.ViewModels;
public class ViewModelMain : ViewModelBase
{

    public ViewModelProperties properties;
    public ViewModelProperties Properties { get => properties; set => SetProperty(ref properties, value); }

    public ViewModelTitle title;
    public ViewModelTitle Title { get => title; set => SetProperty(ref title, value); }

    string? winTitle;
    public string? WinTitle { get => winTitle; set => SetProperty(ref winTitle, value); }

    bool titleOpen;
    public bool TitleOpen { get => titleOpen; set => SetProperty(ref titleOpen, value); }

    bool substitutionOpen;
    public bool SubstitutionOpen { get => substitutionOpen; set => SetProperty(ref substitutionOpen, value); }

    bool textOpen;
    public bool TextOpen { get => textOpen; set => SetProperty(ref textOpen, value); }

    string mainColor;

    public string MainColor { get => mainColor; set => SetProperty(ref mainColor, value); }

    string additionalColor;

    public string AdditionalColor { get => additionalColor; set => SetProperty(ref additionalColor, value); }

    string alternativeColor;

    public string AlternativeColor { get => alternativeColor; set => SetProperty(ref alternativeColor, value); }

    string hoverColor;

    public string HoverColor { get => hoverColor; set => SetProperty(ref hoverColor, value); }

    string fontSize;

    public string FontSize { get => fontSize; set => SetProperty(ref fontSize, value); }

    string fontSizeRTB;
    public string FontSizeRTB { get => fontSizeRTB; set => SetProperty(ref fontSizeRTB, value); }
}
