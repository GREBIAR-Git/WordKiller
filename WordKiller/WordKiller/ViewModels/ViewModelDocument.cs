using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.Scripts;
using WordKiller.Scripts.ForUI;
using WordKiller.Scripts.ImportExport;

namespace WordKiller.ViewModels
{
    public class ViewModelDocument : ViewModelBase
    {
        DocumentData data;
        public DocumentData Data { get => data; set => SetProperty(ref data, value); }

        readonly WordKillerFile file;

        ViewModelExport export;
        public ViewModelExport Export { get => export; set => SetProperty(ref export, value); }

        string? winTitle;
        public string? WinTitle { get => winTitle; set => SetProperty(ref winTitle, value); }

        Visibility visibilityTitleMI;
        public Visibility VisibilityTitleMI { get => visibilityTitleMI; set => SetProperty(ref visibilityTitleMI, value); }

        Visibility visibilityTaskSheetMI;
        public Visibility VisibilityTaskSheetMI { get => visibilityTaskSheetMI; set => SetProperty(ref visibilityTaskSheetMI, value); }

        Visibility visibilityRTB;
        public Visibility VisibilityNotComplexObjects
        {
            get => visibilityRTB;
            set
            {
                if (SetProperty(ref visibilityRTB, value))
                {
                    if (VisibilityNotComplexObjects == Visibility.Collapsed)
                    {
                        VisibilityUnselectInfo = Visibility.Visible;
                    }
                }
            }
        }

        Visibility visibilityUnselectInfo;

        public Visibility VisibilityUnselectInfo
        {
            get => visibilityUnselectInfo;
            set
            {
                SetProperty(ref visibilityUnselectInfo, value);
            }
        }
        // осталось только 2 клик на меню item пофиксить
        //тыкаем было false -> стало true
        //было true -> стало false


        bool defaultDocument;
        public bool DefaultDocument
        {
            get => defaultDocument;
            set
            {
                if (value)
                {
                    if (!defaultDocument)
                    {
                        DocumentTypeFalse();
                        SetProperty(ref defaultDocument, value);

                        VisibilityTitleMI = Visibility.Collapsed;
                        DeleteTitle();
                        VisibilityTaskSheetMI = Visibility.Collapsed;
                        DeleteTaskSheet();

                        TextHeader("DefaultDocument");
                        Data.Type = TypeDocument.DefaultDocument;
                    }
                }
                else if (defaultDocument)
                {
                    SetProperty(ref defaultDocument, value);
                }
            }
        }

        bool coursework;
        public bool Coursework
        {
            get => coursework;
            set
            {
                if (value)
                {
                    if (!coursework)
                    {
                        DocumentTypeFalse();
                        SetProperty(ref coursework, value);

                        NoDefaultDocument();

                        AddTaskSheet();
                        Data.Type = TypeDocument.Coursework;
                        TextHeader("Coursework");
                        TitleElements.ShowTitleElems("0.0 1.0 0.1 1.1 4.1 5.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
                    }
                }
                else if (coursework)
                {
                    SetProperty(ref coursework, value);
                }
            }
        }

        bool laboratoryWork;
        public bool LaboratoryWork
        {
            get => laboratoryWork;
            set
            {
                if (value)
                {
                    if (!laboratoryWork)
                    {
                        DocumentTypeFalse();
                        SetProperty(ref laboratoryWork, value);

                        NoDefaultDocument();

                        Data.Type = TypeDocument.LaboratoryWork;
                        TextHeader("LaboratoryWork");
                        TitleElements.ShowTitleElems("0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
                    }
                }
                else if (laboratoryWork)
                {
                    SetProperty(ref laboratoryWork, value);
                }
            }
        }

        bool practiceWork;
        public bool PracticeWork
        {
            get => practiceWork;
            set
            {
                if (value)
                {
                    if (!practiceWork)
                    {
                        DocumentTypeFalse();
                        SetProperty(ref practiceWork, value);

                        NoDefaultDocument();

                        Data.Type = TypeDocument.PracticeWork;
                        TextHeader("PracticeWork");
                        TitleElements.ShowTitleElems("0.0 1.0 2.1 3.1 0.3 1.3 0.4 1.4 0.6 1.6 0.7 1.7");
                    }
                }
                else if (practiceWork)
                {
                    SetProperty(ref practiceWork, value);
                }
            }
        }

        bool controlWork;
        public bool ControlWork
        {
            get => controlWork;
            set
            {
                if (value)
                {
                    if (!controlWork)
                    {
                        DocumentTypeFalse();
                        SetProperty(ref controlWork, value);

                        NoDefaultDocument();

                        Data.Type = TypeDocument.ControlWork;
                        TextHeader("ControlWork");
                        TitleElements.ShowTitleElems("0.0 1.0 0.1 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
                    }
                }
                else if (controlWork)
                {
                    SetProperty(ref controlWork, value);
                }
            }
        }

        bool referat;
        public bool Referat
        {
            get => referat;
            set
            {
                if (value)
                {
                    if (!referat)
                    {
                        DocumentTypeFalse();
                        SetProperty(ref referat, value);

                        NoDefaultDocument();

                        Data.Type = TypeDocument.Referat;
                        TextHeader("Referat");
                        TitleElements.ShowTitleElems("0.0 1.0 0.1 0.3 1.3 1.1 0.4 1.4 0.6 1.6 0.7 1.7");
                    }
                }
                else if (referat)
                {
                    SetProperty(ref referat, value);
                }
            }
        }

        bool diploma;
        public bool Diploma
        {
            get => diploma;
            set
            {
                if (value)
                {
                    if (!diploma)
                    {
                        DocumentTypeFalse();
                        SetProperty(ref diploma, value);

                        NoDefaultDocument();

                        Data.Type = TypeDocument.Diploma;
                        TextHeader("DiplomaWork");
                        TitleElements.ShowTitleElems("");
                    }
                }
                else if (diploma)
                {
                    SetProperty(ref diploma, value);
                }
            }
        }

        bool vkr;
        public bool VKR
        {
            get => vkr;
            set
            {
                if (value)
                {
                    if (!vkr)
                    {
                        DocumentTypeFalse();
                        SetProperty(ref vkr, value);

                        NoDefaultDocument();

                        Data.Type = TypeDocument.VKR;
                        TextHeader("VKR");
                        TitleElements.ShowTitleElems("");
                    }
                }
                else if (vkr)
                {
                    SetProperty(ref vkr, value);
                }
            }
        }

        void DocumentTypeFalse()
        {
            DefaultDocument = false;
            Coursework = false;
            LaboratoryWork = false;
            PracticeWork = false;
            ControlWork = false;
            Referat = false;
            Diploma = false;
            VKR = false;
        }

        void NoDefaultDocument()
        {
            AddTitle();
            VisibilityTaskSheetMI = Visibility.Collapsed;
            DeleteTaskSheet();
        }

        void HeaderUpdate()
        {
            switch (Data.Type)
            {
                case TypeDocument.DefaultDocument:
                    DefaultDocument = true;
                    break;
                case TypeDocument.LaboratoryWork:
                    LaboratoryWork = true;
                    break;
                case TypeDocument.PracticeWork:
                    PracticeWork = true;
                    break;
                case TypeDocument.Coursework:
                    Coursework = true;
                    break;
                case TypeDocument.ControlWork:
                    ControlWork = true;
                    break;
                case TypeDocument.Referat:
                    Referat = true;
                    break;
                case TypeDocument.Diploma:
                    Diploma = true;
                    break;
                case TypeDocument.VKR:
                    VKR = true;
                    break;
            }
        }

        void TextHeader(string type)
        {
            string text = UIHelper.FindResourse(type);
            if (file.SavePathExists())
            {
                WinTitle = file.SavePath + " — " + text;
            }
            else
            {
                WinTitle = "WordKiller — " + text;
            }
        }

        ICommand? exportFile;

        public ICommand ExportFile
        {
            get
            {
                return exportFile ??= new RelayCommand(
                async obj =>
                {
                    Report report = new();
                    await Task.Run(() =>
                        Report.Create(Data, Export.ExportPDF, Export.ExportHTML));
                    if (Properties.Settings.Default.CloseWindow)
                    {
                        UIHelper.WindowClose();
                    }
                });
            }
        }

        ICommand? newFile;

        public ICommand NewFile
        {
            get
            {
                return newFile ??= new RelayCommand(
                obj =>
                {
                    Data = file.NewFile();
                    VisibilityNotComplexObjects = Visibility.Collapsed;
                });
            }
        }


        ICommand? openFile;

        public ICommand OpenFile
        {
            get
            {
                return openFile ??= new RelayCommand(
                obj =>
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Filter = "wordkiller file (*" + Properties.Settings.Default.Extension + ")|*" + Properties.Settings.Default.Extension + "|All files (*.*)|*.*"
                    };
                    if (openFileDialog.ShowDialog() == true)
                    {
                        Open(openFileDialog.FileName);
                    }
                });
            }
        }

        public void Open(string fileName)
        {
            DocumentTypeFalse();
            Data = file.OpenFile(fileName);
            HeaderUpdate();
            /*if (paragraphTree.Items.Count > 0)
            {
                //paragraphTree.SelectedIndex = 0;
            }*/
        }

        ICommand? saveFile;

        public ICommand SaveFile
        {
            get
            {
                return saveFile ??= new RelayCommand(
                obj =>
                {
                    file.Save(Data);
                    HeaderUpdateSave();
                });
            }
        }

        ICommand? saveAsFile;

        public ICommand SaveAsFile
        {
            get
            {
                return saveAsFile ??= new RelayCommand(
                obj =>
                {
                    file.SaveAs(Data);
                    HeaderUpdateSave();
                });
            }
        }

        void HeaderUpdateSave()
        {
            switch (Data.Type)
            {
                case TypeDocument.DefaultDocument:
                    TextHeader("DefaultDocument");
                    break;
                case TypeDocument.LaboratoryWork:
                    TextHeader("LaboratoryWork");
                    break;
                case TypeDocument.PracticeWork:
                    TextHeader("PracticeWork");
                    break;
                case TypeDocument.Coursework:
                    TextHeader("Coursework");
                    break;
                case TypeDocument.ControlWork:
                    TextHeader("ControlWork");
                    break;
                case TypeDocument.Referat:
                    TextHeader("Referat");
                    break;
                case TypeDocument.Diploma:
                    TextHeader("DiplomaWork");
                    break;
                case TypeDocument.VKR:
                    TextHeader("VKR");
                    break;
            }
        }

        ICommand? titleMI;

        public ICommand TitleMI
        {
            get
            {
                return titleMI ??= new RelayCommand(obj =>
                {
                    if (Data.Properties.Title)
                    {
                        AddTitle();
                    }
                    else
                    {
                        DeleteTitle();
                    }
                });
            }
        }

        ICommand? taskSheetMI;

        public ICommand TaskSheetMI
        {
            get
            {
                return taskSheetMI ??= new RelayCommand(obj =>
                {
                    if (Data.Properties.TaskSheet)
                    {
                        AddTaskSheet();
                    }
                    else
                    {
                        DeleteTaskSheet();
                    }
                });
            }
        }

        void AddTaskSheet()
        {
            VisibilityTaskSheetMI = Visibility.Visible;
            Data.Properties.TaskSheet = true;
            if (Data.Paragraphs.Count > 0)
            {
                if (Data.Paragraphs[0] is not ParagraphTitle)
                {
                    Data.InsertBefore(Data.Paragraphs[0], new ParagraphTaskSheet());
                }
                else
                {
                    Data.InsertAfter(Data.Paragraphs[0], new ParagraphTaskSheet());
                }
            }
            else
            {
                Data.AddParagraph(new ParagraphTaskSheet());
            }
        }

        void DeleteTaskSheet()
        {
            Data.Properties.TaskSheet = false;
            if (Data.Paragraphs.Count > 1 && Data.Paragraphs[0] is ParagraphTitle && Data.Paragraphs[1] is ParagraphTaskSheet)
            {
                Data.Paragraphs.RemoveAt(1);
            }
            else if (Data.Paragraphs.Count > 0 && Data.Paragraphs[0] is ParagraphTaskSheet)
            {
                Data.Paragraphs.RemoveAt(0);
            }
            if (Data.Paragraphs.Count == 0)
            {
                VisibilityNotComplexObjects = Visibility.Collapsed;
            }
        }

        void AddTitle()
        {
            VisibilityTitleMI = Visibility.Visible;
            Data.Properties.Title = true;
            if (Data.Paragraphs.Count > 0)
            {
                if (Data.Paragraphs[0] is not ParagraphTitle)
                {
                    Data.InsertBefore(Data.Paragraphs[0], new ParagraphTitle());
                }
            }
            else
            {
                Data.AddParagraph(new ParagraphTitle());
            }
        }

        void DeleteTitle()
        {
            Data.Properties.Title = false;

            if (Data.Paragraphs.Count > 0 && Data.Paragraphs[0] is ParagraphTitle)
            {
                Data.Paragraphs.RemoveAt(0);
            }
            if (Data.Paragraphs.Count == 0)
            {
                VisibilityNotComplexObjects = Visibility.Collapsed;
            }
        }

        ICommand? defaultDocument_Click;

        public ICommand DefaultDocument_Click
        {
            get
            {
                return defaultDocument_Click ??= new RelayCommand(
                obj =>
                {
                    if (!DefaultDocument)
                    {
                        DefaultDocument = true;
                    }
                });
            }
        }

        ICommand? coursework_Click;

        public ICommand Coursework_Click
        {
            get
            {
                return coursework_Click ??= new RelayCommand(
                obj =>
                {
                    if (!Coursework)
                    {
                        Coursework = true;
                    }
                });
            }
        }

        ICommand? laboratoryWork_Click;

        public ICommand LaboratoryWork_Click
        {
            get
            {
                return laboratoryWork_Click ??= new RelayCommand(
                obj =>
                {
                    if (!LaboratoryWork)
                    {
                        LaboratoryWork = true;
                    }
                });
            }
        }

        ICommand? practiceWork_Click;

        public ICommand PracticeWork_Click
        {
            get
            {
                return practiceWork_Click ??= new RelayCommand(
                obj =>
                {
                    if (!PracticeWork)
                    {
                        PracticeWork = true;
                    }
                });
            }
        }

        ICommand? controlWork_Click;

        public ICommand ControlWork_Click
        {
            get
            {
                return controlWork_Click ??= new RelayCommand(
                obj =>
                {
                    if (!ControlWork)
                    {
                        ControlWork = true;
                    }
                });
            }
        }

        ICommand? referat_Click;

        public ICommand Referat_Click
        {
            get
            {
                return referat_Click ??= new RelayCommand(
                obj =>
                {
                    if (!Referat)
                    {
                        Referat = true;
                    }
                });
            }
        }

        ICommand? diploma_Click;

        public ICommand Diploma_Click
        {
            get
            {
                return diploma_Click ??= new RelayCommand(
                obj =>
                {
                    if (!Diploma)
                    {
                        Diploma = true;
                    }
                });
            }
        }

        ICommand? vkr_Click;

        public ICommand VKR_Click
        {
            get
            {
                return vkr_Click ??= new RelayCommand(
                obj =>
                {
                    if (!VKR)
                    {
                        VKR = true;
                    }
                });
            }
        }
        public ViewModelDocument()
        {
            VisibilityNotComplexObjects = Visibility.Collapsed;
            VisibilityTitleMI = Visibility.Collapsed;
            VisibilityTaskSheetMI = Visibility.Collapsed;
            WinTitle = "WordKiller";
            data = new();
            file = new();
            export = new();
            DefaultDocument = true;
        }
    }
}