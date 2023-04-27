using OrelUniverEmbeddedAPI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.Models;

namespace WordKiller.ViewModels.Settings;

public class ViewModelProfileSettings : ViewModelBase
{
    int selectedCategory;

    public int SelectedCategory
    {
        get => selectedCategory;
        set
        {
            SetProperty(ref selectedCategory, value);
            if (selectedCategory == 0)
            {
                VisibitityCategoryGeneral = Visibility.Visible;
                VisibitityCategoryUsers = Visibility.Collapsed;
            }
            else if (selectedCategory == 1)
            {
                VisibitityCategoryGeneral = Visibility.Collapsed;
                VisibitityCategoryUsers = Visibility.Visible;
            }
        }
    }

    Visibility visibitityCategoryGeneral;
    public Visibility VisibitityCategoryGeneral { get => visibitityCategoryGeneral; set => SetProperty(ref visibitityCategoryGeneral, value); }

    Visibility visibitityCategoryUsers;
    public Visibility VisibitityCategoryUsers { get => visibitityCategoryUsers; set => SetProperty(ref visibitityCategoryUsers, value); }

    ObservableCollection<User> users;
    public ObservableCollection<User> Users
    {
        get => users;
        set
        {
            SetProperty(ref users, value);
        }
    }

    ICommand? editPartnersCell;
    public ICommand EditPartnersCell
    {
        get
        {
            return editPartnersCell ??= new RelayCommand(obj =>
            {
                Properties.Settings.Default.Users = Users;
                Properties.Settings.Default.Save();
            });
        }
    }

    ICommand? add;

    public ICommand Add
    {
        get
        {
            return add ??= new RelayCommand(obj =>
            {
                Users.Add(new());
            });
        }
    }

    string group;

    public string Group
    {
        get => group;
        set
        {
            SetProperty(ref group, value);

            Properties.Settings.Default.Group = group;
            Properties.Settings.Default.Save();
        }
    }

    string year;
    public string Year
    {
        get => year;
        set
        {
            SetProperty(ref year, value);
            Properties.Settings.Default.Year = year;
            Properties.Settings.Default.Save();
        }
    }

    Visibility visibilityManualInput;
    public Visibility VisibilityManualInput { get => visibilityManualInput; set => SetProperty(ref visibilityManualInput, value); }

    Visibility visibilityAutoInput;
    public Visibility VisibilityAutoInput { get => visibilityAutoInput; set => SetProperty(ref visibilityAutoInput, value); }

    bool autoInput;
    public bool AutoInput
    {
        get => autoInput;
        set
        {
            SetProperty(ref autoInput, value);
            if (autoInput)
            {
                VisibilityManualInput = Visibility.Collapsed;
                VisibilityAutoInput = Visibility.Visible;
            }
            else
            {
                VisibilityManualInput = Visibility.Visible;
                VisibilityAutoInput = Visibility.Collapsed;
            }
            Properties.Settings.Default.AutoInputS = autoInput;
            Properties.Settings.Default.Save();
        }
    }

    string direction;

    public string Direction
    {
        get => direction;
        set
        {
            SetProperty(ref direction, value);
            Properties.Settings.Default.Direction = direction;
            Properties.Settings.Default.Save();
        }
    }

    string universitet;

    public string Universitet
    {
        get => universitet;
        set
        {
            SetProperty(ref universitet, value);
            if (AutoInput)
            {
                UpdateFaculty.Execute(null);
            }
            Properties.Settings.Default.Universitet = universitet;
            Properties.Settings.Default.Save();
        }
    }

    string faculty;

    public string Faculty
    {
        get => faculty;
        set
        {
            SetProperty(ref faculty, value);
            if (AutoInput)
            {
                UpdateCours.Execute(null);
            }
            Properties.Settings.Default.Faculty = faculty;
            Properties.Settings.Default.Save();
        }
    }

    string cours;

    public string Cours
    {
        get => cours;
        set
        {
            SetProperty(ref cours, value);
            if (AutoInput)
            {
                UpdateGroup.Execute(null);
            }
            Properties.Settings.Default.Course = cours;
            Properties.Settings.Default.Save();
        }
    }

    ICommand? updateYear;

    public ICommand UpdateYear
    {
        get
        {
            return updateYear ??= new RelayCommand(obj =>
            {
                Year = DateTime.Today.Year.ToString();
            });
        }
    }

    ObservableCollection<string> universitetItems;
    public ObservableCollection<string> UniversitetItems { get => universitetItems; set => SetProperty(ref universitetItems, value); }

    ObservableCollection<string> facultyItems;
    public ObservableCollection<string> FacultyItems { get => facultyItems; set => SetProperty(ref facultyItems, value); }

    ObservableCollection<string> coursItems;
    public ObservableCollection<string> CoursItems { get => coursItems; set => SetProperty(ref coursItems, value); }

    ObservableCollection<string> groupItems;
    public ObservableCollection<string> GroupItems { get => groupItems; set => SetProperty(ref groupItems, value); }

    ICommand? updateFaculty;

    public ICommand UpdateFaculty
    {
        get
        {
            return updateFaculty ??= new RelayCommand(async obj =>
            {
                OrelUniverAPI.Result<Division>? result = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                FacultyItems.Clear();
                CoursItems.Clear();
                GroupItems.Clear();
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

    ICommand? updateCours;
    public ICommand UpdateCours
    {
        get
        {
            return updateCours ??= new RelayCommand(async obj =>
            {
                CoursItems.Clear();
                GroupItems.Clear();
                OrelUniverAPI.Result<Division>? facylty = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                if (GoodResponse(facylty))
                {
                    foreach (Division division in facylty.Response)
                    {
                        if (division.Title == faculty)
                        {
                            OrelUniverAPI.Result<Cours>? result = await OrelUniverAPI.ScheduleGetCourseAsync(division.Id.ToString());//
                            foreach (Cours cours in result.Response)
                            {
                                CoursItems.Add(cours.Course);
                            }
                            break;
                        }
                    }
                }
            });
        }
    }

    ICommand? updateGroup;
    public ICommand UpdateGroup
    {
        get
        {
            return updateGroup ??= new RelayCommand(async obj =>
            {
                GroupItems.Clear();
                OrelUniverAPI.Result<Division>? facylty = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                if (GoodResponse(facylty))
                {
                    foreach (Division division in facylty.Response)
                    {
                        if (division.Title == faculty)
                        {
                            OrelUniverAPI.Result<Cours>? result = await OrelUniverAPI.ScheduleGetCourseAsync(division.Id.ToString());
                            foreach (Cours cours in result.Response)
                            {
                                if (cours.Course == Cours)
                                {
                                    OrelUniverAPI.Result<Group>? groups = await OrelUniverAPI.ScheduleGetGroupsAsync(division.Id.ToString(), cours.Course.ToString());
                                    foreach (Group group in groups.Response)
                                    {
                                        GroupItems.Add(group.Title);
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            });
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

    void DataGrid_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Properties.Settings.Default.Users = Users;
        Properties.Settings.Default.Save();
    }

    public ViewModelProfileSettings()
    {
        Users = new();
        UniversitetItems = new();
        FacultyItems = new();
        CoursItems = new();
        GroupItems = new();
        AutoInput = Properties.Settings.Default.AutoInputS;
        VisibitityCategoryUsers = Visibility.Collapsed;
        Users = Properties.Settings.Default.Users;
        Users.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
        Year = Properties.Settings.Default.Year;
        UniversitetItems.Add("Орловский Государственный университет имени И.С. Тургенева");
        Universitet = Properties.Settings.Default.Universitet;
        Faculty = Properties.Settings.Default.Faculty;
        Cours = Properties.Settings.Default.Course;
        Group = Properties.Settings.Default.Group;
        Direction = Properties.Settings.Default.Direction;
    }
}

/*
//7 ипаит //498 //7286-92PG
OrelUniverAPI.Result<Employee>? result23456 = await OrelUniverAPI.EmployeeGetListAsync();//+
OrelUniverAPI.Result<EmployeeMax>? result1 = await OrelUniverAPI.EmployeeGetAsync("3686");//+
OrelUniverAPI.Result<Division>? result3 = await OrelUniverAPI.GetDivisionsForEmployeesAsync();//+
OrelUniverAPI.Result<EmployeeMin>? result2 = await OrelUniverAPI.ScheduleGetEmployeesAsync("7", "498");//
OrelUniverAPI.Result<Subdivision>? result16 = await OrelUniverAPI.ScheduleGetSubdivisionsAsync("7");//
OrelUniverAPI.Result<Cours>? result12 = await OrelUniverAPI.ScheduleGetCourseAsync("7");//
OrelUniverAPI.Result<OrelUniverEmbeddedAPI.Group>? result15 = await OrelUniverAPI.ScheduleGetGroupsAsync("7", "4");//
OrelUniverAPI.Result<Division>? result6 = await OrelUniverAPI.GetDivisionsForStudentsAsync();//+
OrelUniverAPI.Result<Building>? result7 = await OrelUniverAPI.ScheduleGetBuildingsAsync();//+
OrelUniverAPI.Result<Cabinets>? result11 = await OrelUniverAPI.ScheduleGetCabinetsAsync("12");//+
OrelUniverAPI.Result<Schedule>? result4 = await OrelUniverAPI.ScheduleGetByEmployeeAsync(14, 10, 2022, "3686");//
OrelUniverAPI.Result<Schedule>? result8 = await OrelUniverAPI.ScheduleGetByCabinetAsync(14, 10, 2022, "12", "715");//
OrelUniverAPI.Result<Schedule>? result10 = await OrelUniverAPI.ScheduleGetByGroupAsync(14, 10, 2022, "7286");//
OrelUniverAPI.Result<Schedule>? result101 = await OrelUniverAPI.ScheduleGetByGroupAsync(15, 10, 2022, "7286");//
OrelUniverAPI.Result<Schedule>? result102 = await OrelUniverAPI.ScheduleGetByGroupAsync(16, 10, 2022, "7286");//
OrelUniverAPI.Result<Schedule>? result103 = await OrelUniverAPI.ScheduleGetByGroupAsync(17, 10, 2022, "7286");//
OrelUniverAPI.Result<Schedule>? result104 = await OrelUniverAPI.ScheduleGetByGroupAsync(18, 10, 2022, "7286");//
OrelUniverAPI.Result<Exam>? result13 = await OrelUniverAPI.ScheduleGetExamsForEmployeeAsync("498");//
OrelUniverAPI.Result<Exam>? result14 = await OrelUniverAPI.ScheduleGetExamsForStudentAsync("7286");//
*/