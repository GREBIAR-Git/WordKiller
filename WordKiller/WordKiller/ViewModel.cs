﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WordKiller
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        string winTitle;
        public string WinTitle { get => winTitle; set => SetProperty(ref winTitle, value); }

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

        bool encoding0;
        public bool Encoding0 { get => encoding0; set => SetProperty(ref encoding0, value); }

        bool encoding1;
        public bool Encoding1 { get => encoding1; set => SetProperty(ref encoding1, value); }
    }
}