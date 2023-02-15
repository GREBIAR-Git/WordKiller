using System;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelTitle : ViewModelBase
{
    string faculty;
    public string Faculty { get => faculty; set => SetProperty(ref faculty, value); }

    string number;
    public string Number { get => number; set => SetProperty(ref number, value); }

    string theme;
    public string Theme { get => theme; set => SetProperty(ref theme, value); }

    string discipline;
    public string Discipline { get => discipline; set => SetProperty(ref discipline, value); }

    string professor;
    public string Professor { get => professor; set => SetProperty(ref professor, value); }

    string year;
    public string Year { get => year; set => SetProperty(ref year, value); }

    string shifr;
    public string Shifr { get => shifr; set => SetProperty(ref shifr, value); }

    string students;
    public string Students { get => students; set => SetProperty(ref students, value); }

    public ViewModelTitle()
    {
        faculty = "информационных систем и цифровых технологий";
        number = string.Empty;
        theme = string.Empty;
        discipline = string.Empty;
        professor = string.Empty;
        year = "202";
        shifr = string.Empty;
        students = string.Empty;
    }
}
