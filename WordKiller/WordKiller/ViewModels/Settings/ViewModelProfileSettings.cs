namespace WordKiller.ViewModels.Settings
{
    public class ViewModelProfileSettings : ViewModelBase
    {
        string firstName;

        public string FirstName
        {
            get => firstName;
            set
            {
                SetProperty(ref firstName, value);
                Properties.Settings.Default.FirstName = firstName;
                Properties.Settings.Default.Save();
            }
        }

        string lastName;

        public string LastName
        {
            get => lastName;
            set
            {
                SetProperty(ref lastName, value);
                Properties.Settings.Default.LastName = lastName;
                Properties.Settings.Default.Save();
            }
        }

        string middleName;

        public string MiddleName
        {
            get => middleName;
            set
            {
                SetProperty(ref middleName, value);
                Properties.Settings.Default.MiddleName = middleName;
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
                /*
                ComboBox comboBox = (ComboBox)sender;
                OrelUniverAPI.Result<Division>? result6 = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                if (result6.Code != 1)
                {
                    foreach (Division division in result6.Response)
                    {
                        if (division.Title == Faculty.Items[Faculty.SelectedIndex].ToString())
                        {
                            Cours.Items.Clear();
                            Group.Items.Clear();
                            OrelUniverAPI.Result<Cours>? result12 = await OrelUniverAPI.ScheduleGetCourseAsync(division.Id.ToString());//
                            foreach (Cours cours in result12.Response)
                            {
                                Cours.Items.Add(cours.Course);
                            }
                        }
                    }
                    if (comboBox.SelectedIndex >= 0)
                    {
                        Properties.Settings.Default.FacultyString = comboBox.Items[comboBox.SelectedIndex].ToString();
                        Properties.Settings.Default.Faculty = comboBox.SelectedIndex;
                    }
                    Properties.Settings.Default.Save();
                }*/
            }
        }

        string cours;

        public string Cours
        {
            get => cours;
            set
            {
                SetProperty(ref cours, value);
                /*ComboBox comboBox = (ComboBox)sender;

                OrelUniverAPI.Result<Division>? result6 = await OrelUniverAPI.GetDivisionsForStudentsAsync();
                if (result6.Code != 1)
                {
                    foreach (Division division in result6.Response)
                    {
                        if (Faculty.SelectedIndex >= 0 && division.Title == Faculty.Items[Faculty.SelectedIndex].ToString())
                        {
                            OrelUniverAPI.Result<Cours>? result12 = await OrelUniverAPI.ScheduleGetCourseAsync(division.Id.ToString());//
                            foreach (Cours cours in result12.Response)
                            {
                                if (Cours.SelectedIndex >= 0 && cours.Course == Cours.Items[Cours.SelectedIndex].ToString())
                                {
                                    Group.Items.Clear();
                                    OrelUniverAPI.Result<Group>? result15 = await OrelUniverAPI.ScheduleGetGroupsAsync(division.Id.ToString(), cours.Course.ToString());
                                    foreach (Group groups in result15.Response)
                                    {
                                        Group.Items.Add(groups.Title);
                                    }
                                }
                            }
                        }
                    }
                    if (comboBox.SelectedIndex >= 0)
                    {
                        Properties.Settings.Default.CourseString = comboBox.Items[comboBox.SelectedIndex].ToString();
                        Properties.Settings.Default.Course = comboBox.SelectedIndex;
                    }
                    Properties.Settings.Default.Save();
                }*/
            }
        }

        string group;

        public string Group
        {
            get => group;
            set
            {
                SetProperty(ref group, value);
                /*ComboBox comboBox = (ComboBox)sender;
                if (comboBox.SelectedIndex >= 0)
                {
                    Properties.Settings.Default.GroupString = comboBox.Items[comboBox.SelectedIndex].ToString();
                    Properties.Settings.Default.Group = comboBox.SelectedIndex;
                }
                Properties.Settings.Default.Save();*/
            }
        }

        string shifr;

        public string Shifr
        {
            get => shifr;
            set
            {
                SetProperty(ref shifr, value);
                Properties.Settings.Default.Shifr = shifr;
                Properties.Settings.Default.Save();
            }
        }

        string firstNameCoop;

        public string FirstNameCoop
        {
            get => firstNameCoop;
            set
            {
                SetProperty(ref firstNameCoop, value);
                Properties.Settings.Default.FirstNameCoop = firstNameCoop;
                Properties.Settings.Default.Save();
            }
        }

        string lastNameCoop;

        public string LastNameCoop
        {
            get => lastNameCoop;
            set
            {
                SetProperty(ref lastNameCoop, value);
                Properties.Settings.Default.LastNameCoop = lastNameCoop;
                Properties.Settings.Default.Save();
            }
        }

        string middleNameCoop;

        public string MiddleNameCoop
        {
            get => middleNameCoop;
            set
            {
                SetProperty(ref middleNameCoop, value);
                Properties.Settings.Default.MiddleNameCoop = middleNameCoop;
                Properties.Settings.Default.Save();
            }
        }

        string shifrCoop;

        public string ShifrCoop
        {
            get => shifrCoop;
            set
            {
                SetProperty(ref shifrCoop, value);
                Properties.Settings.Default.ShifrCoop = shifrCoop;
                Properties.Settings.Default.Save();
            }
        }
        /*
     async void InitSetting()
    {
    Universitet.Items.Add("Орловский Государственный университет имени И.С. Тургенева");

    OrelUniverAPI.Result<Division>? divisions = await OrelUniverAPI.GetDivisionsForStudentsAsync();
    if (divisions.Code != 1)
    {
        if (divisions != null && divisions.Response != null)
        {
            foreach (Division division in divisions.Response)
            {
                Faculty.Items.Add(division.Title);
            }
        }*/
        public ViewModelProfileSettings()
        {
            FirstName = Properties.Settings.Default.FirstName;
            LastName = Properties.Settings.Default.LastName;
            MiddleName = Properties.Settings.Default.MiddleName;
            Universitet = Properties.Settings.Default.Universitet;
            Shifr = Properties.Settings.Default.Shifr;
            FirstNameCoop = Properties.Settings.Default.FirstNameCoop;
            LastNameCoop = Properties.Settings.Default.LastNameCoop;
            MiddleNameCoop = Properties.Settings.Default.MiddleNameCoop;
            ShifrCoop = Properties.Settings.Default.ShifrCoop;
            /*if ((await OrelUniverAPI.ScheduleGetCourseAsync("1")).Code == 1)
            {
                Faculty.Items.Add(Properties.Settings.Default.FacultyString);
                Faculty.SelectedIndex = 0;
                Group.Items.Add(Properties.Settings.Default.GroupString);
                Group.SelectedIndex = 0;
                Cours.Items.Add(Properties.Settings.Default.CourseString);
                Cours.SelectedIndex = 0;
            }
            else
            {
                Faculty.SelectedIndex = Properties.Settings.Default.Faculty;
                Group.SelectedIndex = Properties.Settings.Default.Group;
                Cours.SelectedIndex = Properties.Settings.Default.Course;
            }*/
        }
    }
}
