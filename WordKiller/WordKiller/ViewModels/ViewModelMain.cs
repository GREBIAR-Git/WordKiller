using System.Collections.ObjectModel;
using WordKiller.DataTypes.ParagraphData.Paragraphs;

namespace WordKiller.ViewModels;
public class ViewModelMain : ViewModelBase
{

    public ViewModelProperties properties;
    public ViewModelProperties Properties { get=>properties; set => SetProperty(ref properties, value); }

    public ViewModelTitle title;
    public ViewModelTitle Title { get => title; set => SetProperty(ref title, value); }

    ObservableCollection<ParagraphH1>? h1p = new();

    public ObservableCollection<ParagraphH1>? H1P { get => h1p; set => SetProperty(ref h1p, value); }

    ObservableCollection<ParagraphH2>? h2p = new();

    public ObservableCollection<ParagraphH2>? H2P { get => h2p; set => SetProperty(ref h2p, value); }

    ObservableCollection<ParagraphList>? lp = new();

    public ObservableCollection<ParagraphList>? LP { get => lp; set => SetProperty(ref lp, value); }

    ObservableCollection<ParagraphPicture>? pp = new();

    public ObservableCollection<ParagraphPicture>? PP { get => pp; set => SetProperty(ref pp, value); }

    ObservableCollection<ParagraphTable>? tp = new();

    public ObservableCollection<ParagraphTable>? TP { get => tp; set => SetProperty(ref tp, value); }

    ObservableCollection<ParagraphCode>? cp = new();

    public ObservableCollection<ParagraphCode>? CP { get => cp; set => SetProperty(ref cp, value); }

    string? winTitle;
    public string? WinTitle { get => winTitle; set => SetProperty(ref winTitle, value); }

    bool titleOpen;
    public bool TitleOpen { get => titleOpen; set => SetProperty(ref titleOpen, value); }

    string displayed;

    public string Displayed { get => displayed; set => SetProperty(ref displayed, value); }

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
