using OrelUniverEmbeddedAPI;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelTitle : ViewModelBase
{
    [NonSerialized]
    ObservableCollection<string> facultyItems;
    public ObservableCollection<string> FacultyItems { get => facultyItems; set => SetProperty(ref facultyItems, value); }
    [NonSerialized]
    ObservableCollection<string> cathedraItems;
    public ObservableCollection<string> CathedraItems { get => cathedraItems; set => SetProperty(ref cathedraItems, value); }
    [NonSerialized]
    ObservableCollection<string> professorItems;
    public ObservableCollection<string> ProfessorItems { get => professorItems; set => SetProperty(ref professorItems, value); }


    bool GoodResponse<T>(OrelUniverAPI.Result<T>? result)
    {
        if (result != null && result.Code == 1 && result.Response != null && result.Response.Count > 0)
        {
            return true;
        }
        return false;
    }

    [NonSerialized]
    ICommand? updateFaculty;

    public ICommand UpdateFaculty
    {
        get
        {
            return updateFaculty ??= new RelayCommand(
            async obj =>
            {
                OrelUniverAPI.Result<Division>? result = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                FacultyItems.Clear();
                CathedraItems.Clear();
                ProfessorItems.Clear();
                if (GoodResponse(result))
                {
                    foreach (var item in result.Response)
                    {
                        facultyItems.Add(item.Title);
                    }
                }
            });
        }
    }

    [NonSerialized]
    ICommand? updateCathedra;

    public ICommand UpdateCathedra
    {
        get
        {
            return updateCathedra ??= new RelayCommand(
            async obj =>
            {
                CathedraItems.Clear();
                ProfessorItems.Clear();
                OrelUniverAPI.Result<Division>? facylty = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                if (GoodResponse(facylty))
                {
                    foreach (Division division in facylty.Response)
                    {
                        if (division.Title == Faculty)
                        {
                            OrelUniverAPI.Result<Subdivision>? result = await OrelUniverAPI.ScheduleGetSubdivisionsAsync(division.Id.ToString());//
                            if (GoodResponse(result))
                            {
                                foreach (var item in result.Response)
                                {
                                    CathedraItems.Add(item.Title);
                                }
                            }
                            break;
                        }
                    }
                }
            });
        }
    }
    [NonSerialized]
    ICommand? updateProfessor;

    public ICommand UpdateProfessor
    {
        get
        {
            return updateProfessor ??= new RelayCommand(
            async obj =>
            {
                ProfessorItems.Clear();
                OrelUniverAPI.Result<Division>? facylty = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                if (GoodResponse(facylty))
                {
                    foreach (Division division in facylty.Response)
                    {
                        if (division.Title == Faculty)
                        {
                            OrelUniverAPI.Result<Subdivision>? result = await OrelUniverAPI.ScheduleGetSubdivisionsAsync(division.Id.ToString());//
                            if (GoodResponse(result))
                            {
                                foreach (var subdivision in result.Response)
                                {
                                    if (subdivision.Title == Cathedra)
                                    {
                                        OrelUniverAPI.Result<EmployeeMin>? result1 = await OrelUniverAPI.ScheduleGetEmployeesAsync(division.Id.ToString(), subdivision.Id.ToString());
                                        if (GoodResponse(result1))
                                        {
                                            foreach (var professor in result1.Response)
                                            {
                                                ProfessorItems.Add(professor.LastName + " " + professor.FirstName.Substring(0, 1) + "." + professor.ParentName.Substring(0, 1) + ".");
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            });
        }
    }

    [NonSerialized]
    ICommand? updateRank;

    public ICommand UpdateRank
    {
        get
        {
            return updateRank ??= new RelayCommand(
            async obj =>
            {
                OrelUniverAPI.Result<Division>? facylty = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                if (GoodResponse(facylty))
                {
                    foreach (Division division in facylty.Response)
                    {
                        if (division.Title == Faculty)
                        {
                            OrelUniverAPI.Result<Subdivision>? result = await OrelUniverAPI.ScheduleGetSubdivisionsAsync(division.Id.ToString());//
                            if (GoodResponse(result))
                            {
                                foreach (var subdivision in result.Response)
                                {
                                    if (subdivision.Title == Cathedra)
                                    {
                                        OrelUniverAPI.Result<EmployeeMin>? result1 = await OrelUniverAPI.ScheduleGetEmployeesAsync(division.Id.ToString(), subdivision.Id.ToString());
                                        if (GoodResponse(result1))
                                        {
                                            string[] words = professor.Split(' ');
                                            if (words.Length > 0)
                                            {
                                                foreach (var professor in result1.Response)
                                                {
                                                    if (professor.LastName == words[0])
                                                    {
                                                        OrelUniverAPI.Result<EmployeeMax>? result2 = await OrelUniverAPI.EmployeeGetAsync(professor.Id.ToString());
                                                        if (result2.Response.Count > 0)
                                                        {
                                                            foreach (var prof in result2.Response)
                                                            {
                                                                if (prof.TitleDivision == Cathedra)
                                                                {
                                                                    Rank = prof.NamePosition;
                                                                    break;
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }

                                }
                            }
                            break;
                        }
                    }
                }


            });
        }
    }

    string faculty;
    public string Faculty
    {
        get => faculty;
        set
        {
            SetProperty(ref faculty, value);
            UpdateCathedra.Execute(null);
        }
    }


    string cathedra;
    public string Cathedra
    {
        get => cathedra;
        set
        {
            SetProperty(ref cathedra, value);
            UpdateProfessor.Execute(null);
        }
    }

    string number;
    public string Number
    {
        get => number;
        set
        {
            SetProperty(ref number, value);
        }
    }

    string theme;
    public string Theme
    {
        get => theme;
        set
        {
            SetProperty(ref theme, value[..1].ToUpper() + value[1..].ToLower());
        }
    }

    string discipline;
    public string Discipline
    {
        get => discipline;
        set
        {
            SetProperty(ref discipline, value[..1].ToUpper() + value[1..].ToLower());
        }
    }

    string professor;
    public string Professor { get => professor; set => SetProperty(ref professor, value); }

    string rank;
    public string Rank { get => rank; set => SetProperty(ref rank, value); }

    string shifr;
    public string Shifr { get => shifr; set => SetProperty(ref shifr, value); }

    string students;
    public string Students { get => students; set => SetProperty(ref students, value); }

    [NonSerialized]
    Visibility visibitityFaculty;
    public Visibility VisibitityFaculty { get => visibitityFaculty; set => SetProperty(ref visibitityFaculty, value); }

    [NonSerialized]
    Visibility visibitityPerformed;
    public Visibility VisibitityPerformed { get => visibitityPerformed; set => SetProperty(ref visibitityPerformed, value); }

    [NonSerialized]
    Visibility visibitityNumber;
    public Visibility VisibitityNumber { get => visibitityNumber; set => SetProperty(ref visibitityNumber, value); }

    [NonSerialized]
    Visibility visibitityTheme;
    public Visibility VisibitityTheme { get => visibitityTheme; set => SetProperty(ref visibitityTheme, value); }

    [NonSerialized]
    Visibility visibitityDiscipline;
    public Visibility VisibitityDiscipline { get => visibitityDiscipline; set => SetProperty(ref visibitityDiscipline, value); }

    [NonSerialized]
    Visibility visibitityProfessor;
    public Visibility VisibitityProfessor { get => visibitityProfessor; set => SetProperty(ref visibitityProfessor, value); }

    [NonSerialized]
    Visibility visibitityRank;
    public Visibility VisibitityRank { get => visibitityRank; set => SetProperty(ref visibitityRank, value); }

    public ViewModelTitle()
    {
        FacultyItems = new();
        CathedraItems = new();
        ProfessorItems = new();
        cathedra = string.Empty;
        number = string.Empty;
        theme = string.Empty;
        discipline = string.Empty;
        professor = string.Empty;
        shifr = string.Empty;
        students = string.Empty;
        rank = string.Empty;
        UpdateFaculty.Execute(null);
    }
}
