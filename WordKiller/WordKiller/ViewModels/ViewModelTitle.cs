using OrelUniverEmbeddedAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.Models;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelTitle : ViewModelDocumentChanges
{
    ObservableCollection<User> performed;
    public ObservableCollection<User> Performed
    {
        get => performed;
        set
        {
            SetPropertyDocument(ref performed, value);
        }
    }

    public User FirstPerformed()
    {
        if (performed != null)
        {
            for (int i = 0; i < performed.Count; i++)
            {
                if (performed[i].AutoSelected)
                {
                    return performed[i];
                }
            }
        }
        return new();
    }

    public bool OnePerformed()
    {
        int number = 0;
        foreach (User user in performed)
        {
            if (user.AutoSelected)
            {
                number++;
            }
        }
        if (number <= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string AllPerformed()
    {
        List<string> isChecked = new();

        for (int i = 0; i < performed.Count; i++)
        {
            if (performed[i].AutoSelected)
            {
                isChecked.Add(performed[i].Full);
            }
        }
        return string.Join(", ", isChecked);
    }

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
                                                ProfessorItems.Add(professor.LastName + " " + professor.FirstName[..1] + "." + professor.ParentName[..1] + ".");
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
            SetPropertyDocument(ref faculty, value);
            UpdateCathedra.Execute(null);
        }
    }


    string cathedra;
    public string Cathedra
    {
        get => cathedra;
        set
        {
            SetPropertyDocument(ref cathedra, value);
            UpdateProfessor.Execute(null);
        }
    }

    string number;
    public string Number
    {
        get => number;
        set
        {
            SetPropertyDocument(ref number, value);
        }
    }

    string theme;
    public string Theme
    {
        get => theme;
        set
        {
            SetPropertyDocument(ref theme, value[..1].ToUpper() + value[1..].ToLower());
        }
    }

    string discipline;
    public string Discipline
    {
        get => discipline;
        set
        {
            SetPropertyDocument(ref discipline, value[..1].ToUpper() + value[1..].ToLower());
        }
    }

    string professor;
    public string Professor { get => professor; set => SetPropertyDocument(ref professor, value); }

    string rank;
    public string Rank { get => rank; set => SetPropertyDocument(ref rank, value); }

    bool project;
    public bool Project
    {
        get => project;
        set
        {
            SetPropertyDocument(ref project, value);
        }
    }

    bool work;
    public bool Work
    {
        get => work;
        set
        {
            SetPropertyDocument(ref work, value);
        }
    }

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

    [NonSerialized]
    Visibility visibitityType;
    public Visibility VisibitityType { get => visibitityType; set => SetProperty(ref visibitityType, value); }

    Visibility visibitityManualInput;
    public Visibility VisibitityManualInput { get => visibitityManualInput; set => SetProperty(ref visibitityManualInput, value); }

    Visibility visibitityAutoInput;
    public Visibility VisibitityAutoInput { get => visibitityAutoInput; set => SetProperty(ref visibitityAutoInput, value); }

    Visibility visibitityPhoto;
    public Visibility VisibitityPhoto { get => visibitityPhoto; set => SetProperty(ref visibitityPhoto, value); }

    Visibility visibitityTitleText;
    public Visibility VisibitityTitleText { get => visibitityTitleText; set => SetProperty(ref visibitityTitleText, value); }

    bool autoInput;
    public bool AutoInput
    {
        get => autoInput;
        set
        {
            SetProperty(ref autoInput, value);
            if (autoInput)
            {
                VisibitityManualInput = Visibility.Visible;
                VisibitityAutoInput = Visibility.Collapsed;
            }
            else
            {
                VisibitityManualInput = Visibility.Collapsed;
                VisibitityAutoInput = Visibility.Visible;
            }
        }
    }

    bool photo;
    public bool Photo
    {
        get => photo;
        set
        {
            SetPropertyDocument(ref photo, value);
            if (photo)
            {
                VisibitityTitleText = Visibility.Collapsed;
                VisibitityPhoto = Visibility.Visible;
            }
            else
            {
                VisibitityTitleText = Visibility.Visible;
                VisibitityPhoto = Visibility.Collapsed;
            }
        }
    }

    ParagraphPicture picture;
    public ParagraphPicture Picture
    {
        get => picture;
        set
        {
            SetPropertyDocument(ref picture, value);
        }
    }

    public ViewModelTitle()
    {
        FacultyItems = new();
        CathedraItems = new();
        ProfessorItems = new();
        picture = new();
        cathedra = string.Empty;
        number = string.Empty;
        theme = string.Empty;
        discipline = string.Empty;
        professor = string.Empty;
        rank = string.Empty;
        project = false;
        work = true;
        autoInput = false;
        photo = false;
        visibitityTitleText = Visibility.Visible;
        visibitityPhoto = Visibility.Collapsed;
        visibitityManualInput = Visibility.Collapsed;
        visibitityAutoInput = Visibility.Visible;
        UpdateFaculty.Execute(null);
    }
}
