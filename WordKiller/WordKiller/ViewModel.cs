using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WordKiller;
public class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (!Equals(field, newValue))
        {
            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        return false;
    }

    string? winTitle;
    public string? WinTitle { get => winTitle; set => SetProperty(ref winTitle, value); }

    bool tableOfContents;

    public bool TableOfContents { get => tableOfContents; set => SetProperty(ref tableOfContents, value); }

    bool numbering;

    public bool Numbering { get => numbering; set => SetProperty(ref numbering, value); }

    bool numberHeading;
    public bool NumberHeading { get => numberHeading; set => SetProperty(ref numberHeading, value); }

    string titleFaculty;
    public string TitleFaculty { get => titleFaculty; set => SetProperty(ref titleFaculty, value); }

    string titleNumber;
    public string TitleNumber { get => titleNumber; set => SetProperty(ref titleNumber, value); }

    string titleTheme;
    public string TitleTheme { get => titleTheme; set => SetProperty(ref titleTheme, value); }

    string titleDiscipline;
    public string TitleDiscipline { get => titleDiscipline; set => SetProperty(ref titleDiscipline, value); }

    string titleProfessor;
    public string TitleProfessor { get => titleProfessor; set => SetProperty(ref titleProfessor, value); }

    string titleYear;
    public string TitleYear { get => titleYear; set => SetProperty(ref titleYear, value); }

    string titleShifr;
    public string TitleShifr { get => titleShifr; set => SetProperty(ref titleShifr, value); }

    string titleStudents;
    public string TitleStudents { get => titleStudents; set => SetProperty(ref titleStudents, value); }

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
}
