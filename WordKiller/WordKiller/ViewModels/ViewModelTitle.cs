using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using OrelUniverEmbeddedAPI;
using WordKiller.Commands;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.Models;
using WordKiller.Models.Template;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelTitle : ViewModelDocumentChanges
{
    string cathedra;

    [NonSerialized] ObservableCollection<string> cathedraItems;

    string direction;

    string discipline;

    bool educational;

    string faculty;

    [NonSerialized] ObservableCollection<string> facultyItems;

    string headCathedra;

    string headOrganization;

    string normocontrol;

    string number;

    bool onHeadOrganization;

    BindingList<User> performed;

    bool photo;

    ParagraphPicture picture;

    string practiceLocation;

    bool production;

    string professor;

    [NonSerialized] ObservableCollection<string> professorItems;

    bool project;

    string rank;

    string theme;

    [NonSerialized] ICommand? updateCathedra;

    [NonSerialized] ICommand? updateFaculty;

    [NonSerialized] ICommand? updateProfessor;

    [NonSerialized] ICommand? updateRank;

    [NonSerialized] Visibility visibitityDirection;

    [NonSerialized] Visibility visibitityDiscipline;

    [NonSerialized] Visibility visibitityFaculty;

    [NonSerialized] Visibility visibitityHeadCathedra;

    [NonSerialized] Visibility visibitityHeadOrganization;

    [NonSerialized] Visibility visibitityHeadOrganizationT;

    [NonSerialized] Visibility visibitityNormocontrol;

    [NonSerialized] Visibility visibitityNumber;

    [NonSerialized] Visibility visibitityPerformed;

    Visibility visibitityPhoto;

    [NonSerialized] Visibility visibitityPracticeLocation;

    [NonSerialized] Visibility visibitityPracticeType;

    [NonSerialized] Visibility visibitityProfessor;

    [NonSerialized] Visibility visibitityRank;

    [NonSerialized] Visibility visibitityTheme;

    Visibility visibitityTitleText;

    [NonSerialized] Visibility visibitityType;

    bool work;

    public ViewModelTitle()
    {
        FacultyItems = [];
        CathedraItems = [];
        ProfessorItems = [];
        picture = new();
        faculty = string.Empty;
        direction = string.Empty;
        practiceLocation = string.Empty;
        normocontrol = string.Empty;
        headCathedra = string.Empty;
        headOrganization = string.Empty;
        cathedra = string.Empty;
        number = string.Empty;
        theme = string.Empty;
        discipline = string.Empty;
        professor = string.Empty;
        rank = string.Empty;
        project = false;
        work = true;
        photo = false;
        production = true;
        educational = false;
        onHeadOrganization = true;
        visibitityTitleText = Visibility.Visible;
        visibitityPhoto = Visibility.Collapsed;
        VisibitityHeadOrganizationT = Visibility.Visible;
        UpdateFaculty.Execute(null);
    }

    public BindingList<User> Performed
    {
        get => performed;
        set => SetPropertyDocument(ref performed, value);
    }

    public ObservableCollection<string> FacultyItems
    {
        get => facultyItems;
        set => SetProperty(ref facultyItems, value);
    }

    public ObservableCollection<string> CathedraItems
    {
        get => cathedraItems;
        set => SetProperty(ref cathedraItems, value);
    }

    public ObservableCollection<string> ProfessorItems
    {
        get => professorItems;
        set => SetProperty(ref professorItems, value);
    }

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
                                OrelUniverAPI.Result<Subdivision>? result =
                                    await OrelUniverAPI.ScheduleGetSubdivisionsAsync(division.Id.ToString()); //
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
                                OrelUniverAPI.Result<Subdivision>? result =
                                    await OrelUniverAPI.ScheduleGetSubdivisionsAsync(division.Id.ToString()); //
                                if (GoodResponse(result))
                                {
                                    foreach (var subdivision in result.Response)
                                    {
                                        if (subdivision.Title == Cathedra)
                                        {
                                            OrelUniverAPI.Result<EmployeeMin>? result1 =
                                                await OrelUniverAPI.ScheduleGetEmployeesAsync(division.Id.ToString(),
                                                    subdivision.Id.ToString());
                                            if (GoodResponse(result1))
                                            {
                                                foreach (var professor in result1.Response)
                                                {
                                                    ProfessorItems.Add(professor.LastName + " " +
                                                                       professor.FirstName[..1] + "." +
                                                                       professor.ParentName[..1] + ".");
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
                                OrelUniverAPI.Result<Subdivision>? result =
                                    await OrelUniverAPI.ScheduleGetSubdivisionsAsync(division.Id.ToString()); //
                                if (GoodResponse(result))
                                {
                                    foreach (var subdivision in result.Response)
                                    {
                                        if (subdivision.Title == Cathedra)
                                        {
                                            OrelUniverAPI.Result<EmployeeMin>? result1 =
                                                await OrelUniverAPI.ScheduleGetEmployeesAsync(division.Id.ToString(),
                                                    subdivision.Id.ToString());
                                            if (GoodResponse(result1))
                                            {
                                                string[] words = professor.Split(' ');
                                                if (words.Length > 0)
                                                {
                                                    foreach (var professor in result1.Response)
                                                    {
                                                        if (professor.LastName == words[0])
                                                        {
                                                            OrelUniverAPI.Result<EmployeeMax>? result2 =
                                                                await OrelUniverAPI.EmployeeGetAsync(
                                                                    professor.Id.ToString());
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

    public string Faculty
    {
        get => faculty;
        set
        {
            SetPropertyDocument(ref faculty, value);
            UpdateCathedra.Execute(null);
        }
    }

    public string Cathedra
    {
        get => cathedra;
        set
        {
            SetPropertyDocument(ref cathedra, value);
            UpdateProfessor.Execute(null);
        }
    }

    public string Number
    {
        get => number;
        set => SetPropertyDocument(ref number, value);
    }

    public string Theme
    {
        get => theme;
        set => SetPropertyDocument(ref theme, value[..1].ToUpper() + value[1..].ToLower());
    }

    public string Discipline
    {
        get => discipline;
        set => SetPropertyDocument(ref discipline, value[..1].ToUpper() + value[1..].ToLower());
    }

    public string Professor
    {
        get => professor;
        set => SetPropertyDocument(ref professor, value);
    }

    public string Rank
    {
        get => rank;
        set => SetPropertyDocument(ref rank, value);
    }

    public bool Project
    {
        get => project;
        set => SetPropertyDocument(ref project, value);
    }

    public bool Work
    {
        get => work;
        set => SetPropertyDocument(ref work, value);
    }

    public bool Educational
    {
        get => educational;
        set => SetPropertyDocument(ref educational, value);
    }

    public bool Production
    {
        get => production;
        set => SetPropertyDocument(ref production, value);
    }

    public string PracticeLocation
    {
        get => practiceLocation;
        set => SetPropertyDocument(ref practiceLocation, value);
    }

    public string Direction
    {
        get => direction;
        set => SetPropertyDocument(ref direction, value);
    }

    public string Normocontrol
    {
        get => normocontrol;
        set => SetPropertyDocument(ref normocontrol, value);
    }

    public string HeadCathedra
    {
        get => headCathedra;
        set => SetPropertyDocument(ref headCathedra, value);
    }

    public string HeadOrganization
    {
        get => headOrganization;
        set => SetPropertyDocument(ref headOrganization, value);
    }

    public bool OnHeadOrganization
    {
        get => onHeadOrganization;
        set
        {
            SetPropertyDocument(ref onHeadOrganization, value);
            if (onHeadOrganization)
            {
                VisibitityHeadOrganizationT = Visibility.Visible;
            }
            else
            {
                VisibitityHeadOrganizationT = Visibility.Collapsed;
            }
        }
    }

    public Visibility VisibitityFaculty
    {
        get => visibitityFaculty;
        set => SetProperty(ref visibitityFaculty, value);
    }

    public Visibility VisibitityPerformed
    {
        get => visibitityPerformed;
        set => SetProperty(ref visibitityPerformed, value);
    }

    public Visibility VisibitityNumber
    {
        get => visibitityNumber;
        set => SetProperty(ref visibitityNumber, value);
    }

    public Visibility VisibitityTheme
    {
        get => visibitityTheme;
        set => SetProperty(ref visibitityTheme, value);
    }

    public Visibility VisibitityDiscipline
    {
        get => visibitityDiscipline;
        set => SetProperty(ref visibitityDiscipline, value);
    }

    public Visibility VisibitityProfessor
    {
        get => visibitityProfessor;
        set => SetProperty(ref visibitityProfessor, value);
    }

    public Visibility VisibitityRank
    {
        get => visibitityRank;
        set => SetProperty(ref visibitityRank, value);
    }

    public Visibility VisibitityType
    {
        get => visibitityType;
        set => SetProperty(ref visibitityType, value);
    }

    public Visibility VisibitityPracticeType
    {
        get => visibitityPracticeType;
        set => SetProperty(ref visibitityPracticeType, value);
    }

    public Visibility VisibitityPracticeLocation
    {
        get => visibitityPracticeLocation;
        set => SetProperty(ref visibitityPracticeLocation, value);
    }

    public Visibility VisibitityDirection
    {
        get => visibitityDirection;
        set => SetProperty(ref visibitityDirection, value);
    }

    public Visibility VisibitityNormocontrol
    {
        get => visibitityNormocontrol;
        set => SetProperty(ref visibitityNormocontrol, value);
    }

    public Visibility VisibitityHeadCathedra
    {
        get => visibitityHeadCathedra;
        set => SetProperty(ref visibitityHeadCathedra, value);
    }

    public Visibility VisibitityHeadOrganization
    {
        get => visibitityHeadOrganization;
        set => SetProperty(ref visibitityHeadOrganization, value);
    }

    public Visibility VisibitityHeadOrganizationT
    {
        get => visibitityHeadOrganizationT;
        set => SetProperty(ref visibitityHeadOrganizationT, value);
    }

    public Visibility VisibitityPhoto
    {
        get => visibitityPhoto;
        set => SetProperty(ref visibitityPhoto, value);
    }

    public Visibility VisibitityTitleText
    {
        get => visibitityTitleText;
        set => SetProperty(ref visibitityTitleText, value);
    }

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

    public ParagraphPicture Picture
    {
        get => picture;
        set => SetPropertyDocument(ref picture, value);
    }

    public string GetData(int i)
    {
        if (i == 0)
        {
            return Faculty;
        }
        else if (i == 1)
        {
            return Cathedra;
        }
        else if (i == 2)
        {
            User user = FirstPerformed();
            return user.LastName + " " + user.FirstName + " " + user.MiddleName;
        }
        else if (i == 3)
        {
            return FirstPerformed().Full;
        }
        else if (i == 4)
        {
            return FirstPerformed().AlternateFull;
        }
        else if (i == 5)
        {
            return Professor;
        }
        else if (i == 6)
        {
            return Professor;
        }
        else if (i == 7)
        {
            return Professor;
        }
        else if (i == 8)
        {
            return FirstPerformed().Shifr;
        }
        else if (i == 9)
        {
            return Number;
        }
        else if (i == 10)
        {
            return Theme;
        }
        else if (i == 11)
        {
            return Properties.Settings.Default.Course;
        }
        else if (i == 12)
        {
            return Properties.Settings.Default.Group;
        }
        else if (i == 13)
        {
            return Properties.Settings.Default.Direction;
        }
        else if (i == 14)
        {
            return Direction;
        }
        else if (i == 15)
        {
            return Properties.Settings.Default.Year;
        }
        else if (i == 16)
        {
            return Discipline;
        }
        else if (i == 17)
        {
            return Rank;
        }
        else if (i == 18)
        {
            return "НЕ работает";
        }
        else if (i == 19)
        {
            return PracticeLocation;
        }
        else if (i == 20)
        {
            return Normocontrol;
        }
        else if (i == 21)
        {
            return HeadCathedra;
        }
        else if (i == 22)
        {
            return HeadOrganization;
        }

        return string.Empty;
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
        if (performed != null)
        {
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
        else
        {
            return true;
        }
    }

    public string AllPerformed()
    {
        if (performed != null)
        {
            List<string> isChecked = [];
            for (int i = 0; i < performed.Count; i++)
            {
                if (performed[i].AutoSelected)
                {
                    isChecked.Add(performed[i].Full);
                }
            }

            return string.Join(", ", isChecked);
        }
        else
        {
            return string.Empty;
        }
    }


    bool GoodResponse<T>(OrelUniverAPI.Result<T>? result)
    {
        if (result != null && result.Code == 1 && result.Response != null && result.Response.Count > 0)
        {
            return true;
        }

        return false;
    }

    public void UpdateTitleItems(DocumentType documentType)
    {
        foreach (TemplateType templateType in Properties.Settings.Default.TemplateTypes)
        {
            if (templateType != null && templateType.Type == documentType)
            {
                if (templateType.NonStandard)
                {
                    templateType.Update();

                    VisibitityPerformed = templateType.Visibilities[1];
                    VisibitityNumber = templateType.Visibilities[2];
                    VisibitityTheme = templateType.Visibilities[3];
                    VisibitityDiscipline = templateType.Visibilities[4];
                    if (templateType.Visibilities[5] == Visibility.Visible)
                    {
                        VisibitityFaculty = Visibility.Visible;
                    }
                    else
                    {
                        VisibitityFaculty = templateType.Visibilities[0];
                    }

                    VisibitityProfessor = templateType.Visibilities[5];
                    VisibitityRank = templateType.Visibilities[6];
                    VisibitityType = templateType.Visibilities[7];
                    VisibitityPracticeType = templateType.Visibilities[8];
                    VisibitityPracticeLocation = templateType.Visibilities[9];
                    VisibitityHeadOrganization = templateType.Visibilities[10];
                    VisibitityDirection = templateType.Visibilities[11];
                    VisibitityNormocontrol = templateType.Visibilities[12];
                    VisibitityHeadCathedra = templateType.Visibilities[13];
                }
                else
                {
                    switch (documentType)
                    {
                        case DocumentType.DefaultDocument:
                            VisibitityFaculty = Visibility.Collapsed;
                            VisibitityPerformed = Visibility.Collapsed;
                            VisibitityNumber = Visibility.Collapsed;
                            VisibitityTheme = Visibility.Collapsed;
                            VisibitityDiscipline = Visibility.Collapsed;
                            VisibitityProfessor = Visibility.Collapsed;
                            VisibitityRank = Visibility.Collapsed;
                            VisibitityType = Visibility.Collapsed;
                            VisibitityPracticeType = Visibility.Collapsed;
                            VisibitityPracticeLocation = Visibility.Collapsed;
                            VisibitityHeadOrganization = Visibility.Collapsed;
                            VisibitityDirection = Visibility.Collapsed;
                            VisibitityNormocontrol = Visibility.Collapsed;
                            VisibitityHeadCathedra = Visibility.Collapsed;
                            break;
                        case DocumentType.LaboratoryWork:
                            VisibitityFaculty = Visibility.Visible;
                            VisibitityPerformed = Visibility.Visible;
                            VisibitityNumber = Visibility.Visible;
                            VisibitityTheme = Visibility.Visible;
                            VisibitityDiscipline = Visibility.Visible;
                            VisibitityProfessor = Visibility.Visible;
                            VisibitityRank = Visibility.Collapsed;
                            VisibitityType = Visibility.Collapsed;
                            VisibitityPracticeType = Visibility.Collapsed;
                            VisibitityPracticeLocation = Visibility.Collapsed;
                            VisibitityHeadOrganization = Visibility.Collapsed;
                            VisibitityDirection = Visibility.Collapsed;
                            VisibitityNormocontrol = Visibility.Collapsed;
                            VisibitityHeadCathedra = Visibility.Collapsed;

                            break;
                        case DocumentType.PracticeWork:
                            VisibitityFaculty = Visibility.Visible;
                            VisibitityPerformed = Visibility.Visible;
                            VisibitityNumber = Visibility.Visible;
                            VisibitityTheme = Visibility.Visible;
                            VisibitityDiscipline = Visibility.Visible;
                            VisibitityProfessor = Visibility.Visible;
                            VisibitityRank = Visibility.Collapsed;
                            VisibitityType = Visibility.Collapsed;
                            VisibitityPracticeType = Visibility.Collapsed;
                            VisibitityPracticeLocation = Visibility.Collapsed;
                            VisibitityHeadOrganization = Visibility.Collapsed;
                            VisibitityDirection = Visibility.Collapsed;
                            VisibitityNormocontrol = Visibility.Collapsed;
                            VisibitityHeadCathedra = Visibility.Collapsed;
                            break;
                        case DocumentType.Coursework:
                            VisibitityFaculty = Visibility.Visible;
                            VisibitityPerformed = Visibility.Visible;
                            VisibitityNumber = Visibility.Collapsed;
                            VisibitityTheme = Visibility.Visible;
                            VisibitityDiscipline = Visibility.Visible;
                            VisibitityProfessor = Visibility.Visible;
                            VisibitityRank = Visibility.Collapsed;
                            VisibitityType = Visibility.Visible;
                            VisibitityPracticeType = Visibility.Collapsed;
                            VisibitityPracticeLocation = Visibility.Collapsed;
                            VisibitityHeadOrganization = Visibility.Collapsed;
                            VisibitityDirection = Visibility.Collapsed;
                            VisibitityNormocontrol = Visibility.Collapsed;
                            VisibitityHeadCathedra = Visibility.Collapsed;
                            break;
                        case DocumentType.ControlWork:
                            VisibitityFaculty = Visibility.Visible;
                            VisibitityPerformed = Visibility.Visible;
                            VisibitityNumber = Visibility.Visible;
                            VisibitityTheme = Visibility.Collapsed;
                            VisibitityDiscipline = Visibility.Visible;
                            VisibitityProfessor = Visibility.Visible;
                            VisibitityRank = Visibility.Collapsed;
                            VisibitityType = Visibility.Collapsed;
                            VisibitityPracticeType = Visibility.Collapsed;
                            VisibitityPracticeLocation = Visibility.Collapsed;
                            VisibitityHeadOrganization = Visibility.Collapsed;
                            VisibitityDirection = Visibility.Collapsed;
                            VisibitityNormocontrol = Visibility.Collapsed;
                            VisibitityHeadCathedra = Visibility.Collapsed;
                            break;
                        case DocumentType.Referat:
                            VisibitityFaculty = Visibility.Visible;
                            VisibitityPerformed = Visibility.Visible;
                            VisibitityNumber = Visibility.Visible;
                            VisibitityTheme = Visibility.Visible;
                            VisibitityDiscipline = Visibility.Visible;
                            VisibitityProfessor = Visibility.Visible;
                            VisibitityRank = Visibility.Visible;
                            VisibitityType = Visibility.Collapsed;
                            VisibitityPracticeType = Visibility.Collapsed;
                            VisibitityPracticeLocation = Visibility.Collapsed;
                            VisibitityHeadOrganization = Visibility.Collapsed;
                            VisibitityDirection = Visibility.Collapsed;
                            VisibitityNormocontrol = Visibility.Collapsed;
                            VisibitityHeadCathedra = Visibility.Collapsed;
                            break;
                        case DocumentType.ProductionPractice:
                            VisibitityFaculty = Visibility.Visible;
                            VisibitityPerformed = Visibility.Visible;
                            VisibitityNumber = Visibility.Collapsed;
                            VisibitityTheme = Visibility.Collapsed;
                            VisibitityDiscipline = Visibility.Collapsed;
                            VisibitityProfessor = Visibility.Visible;
                            VisibitityRank = Visibility.Collapsed;
                            VisibitityType = Visibility.Collapsed;
                            VisibitityPracticeType = Visibility.Visible; //loc
                            VisibitityPracticeLocation = Visibility.Visible; //loc
                            VisibitityHeadOrganization = Visibility.Visible; //loc
                            VisibitityDirection = Visibility.Collapsed;
                            VisibitityNormocontrol = Visibility.Collapsed;
                            VisibitityHeadCathedra = Visibility.Collapsed;
                            break;
                        case DocumentType.VKR:
                            VisibitityFaculty = Visibility.Visible;
                            VisibitityPerformed = Visibility.Visible;
                            VisibitityNumber = Visibility.Collapsed;
                            VisibitityTheme = Visibility.Visible;
                            VisibitityDiscipline = Visibility.Collapsed;
                            VisibitityProfessor = Visibility.Visible;
                            VisibitityRank = Visibility.Collapsed;
                            VisibitityType = Visibility.Collapsed;
                            VisibitityPracticeType = Visibility.Collapsed;
                            VisibitityPracticeLocation = Visibility.Collapsed;
                            VisibitityHeadOrganization = Visibility.Collapsed;
                            VisibitityDirection = Visibility.Visible; // loc
                            VisibitityNormocontrol = Visibility.Visible; // loc
                            VisibitityHeadCathedra = Visibility.Visible; //+ update mb loc
                            break;
                    }
                }
            }
        }
    }
}