using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using OrelUniverEmbeddedAPI;
using WordKiller.Commands;
using WordKiller.Models;

namespace WordKiller.ViewModels.Settings;

public class ViewModelProfileSettings : ViewModelBase
{
    ICommand? add;

    bool autoInput;

    string course;

    ObservableCollection<string> courseItems;

    string direction;

    ICommand? editPartnersCell;

    string faculty;

    ObservableCollection<string> facultyItems;

    string group;

    ObservableCollection<string> groupItems;
    int selectedCategory;

    string university;

    ObservableCollection<string> universityItems;

    ICommand? updateCourse;

    ICommand? updateFaculty;

    ICommand? updateGroup;

    ICommand? updateYear;

    BindingList<User> users;

    Visibility visibilityAutoInput;

    Visibility visibilityManualInput;

    Visibility visibitityCategoryGeneral;

    Visibility visibitityCategoryUsers;

    string year;

    public ViewModelProfileSettings()
    {
        UniversityItems = [];
        FacultyItems = [];
        CourseItems = [];
        GroupItems = [];
        AutoInput = Properties.Settings.Default.AutoInput;
        VisibitityCategoryUsers = Visibility.Collapsed;
        Users = Properties.Settings.Default.Users;
        Year = Properties.Settings.Default.Year;
        UniversityItems.Add("Орловский Государственный университет имени И.С. Тургенева");
        University = Properties.Settings.Default.University;
        Faculty = Properties.Settings.Default.Faculty;
        Course = Properties.Settings.Default.Course;
        Group = Properties.Settings.Default.Group;
        Direction = Properties.Settings.Default.Direction;
    }

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

    public Visibility VisibitityCategoryGeneral
    {
        get => visibitityCategoryGeneral;
        set => SetProperty(ref visibitityCategoryGeneral, value);
    }

    public Visibility VisibitityCategoryUsers
    {
        get => visibitityCategoryUsers;
        set => SetProperty(ref visibitityCategoryUsers, value);
    }

    public BindingList<User> Users
    {
        get => users;
        set => SetProperty(ref users, value);
    }

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

    public ICommand Add
    {
        get { return add ??= new RelayCommand(obj => { Users.Add(new()); }); }
    }

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

    public Visibility VisibilityManualInput
    {
        get => visibilityManualInput;
        set => SetProperty(ref visibilityManualInput, value);
    }

    public Visibility VisibilityAutoInput
    {
        get => visibilityAutoInput;
        set => SetProperty(ref visibilityAutoInput, value);
    }

    public bool AutoInput
    {
        get => autoInput;
        set
        {
            SetProperty(ref autoInput, value);
            if (autoInput)
            {
                VisibilityManualInput = Visibility.Visible;
                VisibilityAutoInput = Visibility.Collapsed;
            }
            else
            {
                VisibilityManualInput = Visibility.Collapsed;
                VisibilityAutoInput = Visibility.Visible;
            }

            Properties.Settings.Default.AutoInput = autoInput;
            Properties.Settings.Default.Save();
        }
    }

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

    public string University
    {
        get => university;
        set
        {
            SetProperty(ref university, value);
            if (!AutoInput)
            {
                UpdateFaculty.Execute(null);
            }

            Properties.Settings.Default.University = university;
            Properties.Settings.Default.Save();
        }
    }

    public string Faculty
    {
        get => faculty;
        set
        {
            SetProperty(ref faculty, value);
            if (!AutoInput)
            {
                UpdateCourse.Execute(null);
            }

            Properties.Settings.Default.Faculty = faculty;
            Properties.Settings.Default.Save();
        }
    }

    public string Course
    {
        get => course;
        set
        {
            SetProperty(ref course, value);
            if (!AutoInput)
            {
                UpdateGroup.Execute(null);
            }

            Properties.Settings.Default.Course = course;
            Properties.Settings.Default.Save();
        }
    }

    public ICommand UpdateYear
    {
        get { return updateYear ??= new RelayCommand(obj => { Year = DateTime.Today.Year.ToString(); }); }
    }

    public ObservableCollection<string> UniversityItems
    {
        get => universityItems;
        set => SetProperty(ref universityItems, value);
    }

    public ObservableCollection<string> FacultyItems
    {
        get => facultyItems;
        set => SetProperty(ref facultyItems, value);
    }

    public ObservableCollection<string> CourseItems
    {
        get => courseItems;
        set => SetProperty(ref courseItems, value);
    }

    public ObservableCollection<string> GroupItems
    {
        get => groupItems;
        set => SetProperty(ref groupItems, value);
    }

    public ICommand UpdateFaculty
    {
        get
        {
            return updateFaculty ??= new RelayCommand(async obj =>
            {
                OrelUniverAPI.Result<Division>? result = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                FacultyItems.Clear();
                CourseItems.Clear();
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

    public ICommand UpdateCourse
    {
        get
        {
            return updateCourse ??= new RelayCommand(async obj =>
            {
                CourseItems.Clear();
                GroupItems.Clear();
                OrelUniverAPI.Result<Division>? facylty = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                if (GoodResponse(facylty))
                {
                    foreach (Division division in facylty.Response)
                    {
                        if (division.Title == faculty)
                        {
                            OrelUniverAPI.Result<Cours>? result =
                                await OrelUniverAPI.ScheduleGetCourseAsync(division.Id.ToString()); //
                            foreach (Cours cours in result.Response)
                            {
                                CourseItems.Add(cours.Course);
                            }

                            break;
                        }
                    }
                }
            });
        }
    }

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
                            OrelUniverAPI.Result<Cours>? result =
                                await OrelUniverAPI.ScheduleGetCourseAsync(division.Id.ToString());
                            foreach (Cours cours in result.Response)
                            {
                                if (cours.Course == Course)
                                {
                                    OrelUniverAPI.Result<Group>? groups =
                                        await OrelUniverAPI.ScheduleGetGroupsAsync(division.Id.ToString(),
                                            cours.Course);
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

    static bool GoodResponse<T>(OrelUniverAPI.Result<T>? result)
    {
        if (result != null && result.Code == 1 && result.Response != null && result.Response.Count > 0)
        {
            return true;
        }

        return false;
    }
}

/*
//7 ипаит //498 //7286-92PG
OrelUniverAPI.Result<Employee>? result23456 = await OrelUniverAPI.EmployeeGetListAsync();//+
OrelUniverAPI.Result<EmployeeMax>? result1 = await OrelUniverAPI.EmployeeGetAsync("3686");//+
OrelUniverAPI.Result<Division>? result3 = await OrelUniverAPI.GetDivisionsForEmployeesAsync();//+
OrelUniverAPI.Result<EmployeeMin>? result2 = await OrelUniverAPI.ScheduleGetEmployeesAsync("7", "498");//
OrelUniverAPI.Result<Subdivision>? result16 = await OrelUniverAPI.ScheduleGetSubdivisionsAsync("7");//
OrelUniverAPI.Result<Course>? result12 = await OrelUniverAPI.ScheduleGetCourseAsync("7");//
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