using Microsoft.Win32;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes;
using WordKiller.DataTypes.Enums;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.DataTypes.ParagraphData.Sections;
using WordKiller.Models.Template;
using WordKiller.Scripts;
using WordKiller.Scripts.ImportExport;
using WordKiller.Views;

namespace WordKiller.ViewModels;

public class ViewModelDocument : ViewModelBase
{
    DocumentData data;
    public DocumentData Data { get => data; set => SetProperty(ref data, value); }

    readonly WordKillerFile file;
    public WordKillerFile File { get => file; init => SetProperty(ref file, value); }

    IParagraphData? selected;
    public IParagraphData? Selected
    {
        get => selected;
        set
        {
            if (SetProperty(ref selected, value))
            {
                if (selected == null)
                {
                    VisibilitY.NotComplexObjects = Visibility.Collapsed;
                    VisibilitY.TitlePanel = Visibility.Collapsed;
                    VisibilitY.TaskSheetPanel = Visibility.Collapsed;
                    VisibilitY.ListOfReferencesPanel = Visibility.Collapsed;
                    VisibilitY.AppendixPanel = Visibility.Collapsed;
                    VisibilitY.UnselectInfo = Visibility.Visible;
                }
                else
                {
                    VisibilitY.UnselectInfo = Visibility.Collapsed;
                    if (selected is ParagraphTitle)
                    {
                        VisibilitY.NotComplexObjects = Visibility.Collapsed;
                        VisibilitY.TitlePanel = Visibility.Visible;
                        VisibilitY.TaskSheetPanel = Visibility.Collapsed;
                        VisibilitY.ListOfReferencesPanel = Visibility.Collapsed;
                        VisibilitY.AppendixPanel = Visibility.Collapsed;
                        Data.Title.UpdateTitleItems(Data.Type);
                    }
                    else if (selected is ParagraphTaskSheet)
                    {
                        VisibilitY.NotComplexObjects = Visibility.Collapsed;
                        VisibilitY.TitlePanel = Visibility.Collapsed;
                        VisibilitY.TaskSheetPanel = Visibility.Visible;
                        VisibilitY.ListOfReferencesPanel = Visibility.Collapsed;
                        VisibilitY.AppendixPanel = Visibility.Collapsed;
                    }
                    else if (selected is ParagraphListOfReferences)
                    {
                        VisibilitY.NotComplexObjects = Visibility.Collapsed;
                        VisibilitY.TitlePanel = Visibility.Collapsed;
                        VisibilitY.TaskSheetPanel = Visibility.Collapsed;
                        VisibilitY.ListOfReferencesPanel = Visibility.Visible;
                        VisibilitY.AppendixPanel = Visibility.Collapsed;
                    }
                    else if (selected is ParagraphAppendix)
                    {
                        VisibilitY.NotComplexObjects = Visibility.Collapsed;
                        VisibilitY.TitlePanel = Visibility.Collapsed;
                        VisibilitY.TaskSheetPanel = Visibility.Collapsed;
                        VisibilitY.ListOfReferencesPanel = Visibility.Collapsed;
                        VisibilitY.AppendixPanel = Visibility.Visible;
                    }
                    else
                    {
                        VisibilitY.NotComplexObjects = Visibility.Visible;
                        VisibilitY.TitlePanel = Visibility.Collapsed;
                        VisibilitY.TaskSheetPanel = Visibility.Collapsed;
                        VisibilitY.ListOfReferencesPanel = Visibility.Collapsed;
                        VisibilitY.AppendixPanel = Visibility.Collapsed;
                        VisibilitY.AutoList = Visibility.Collapsed;
                        if (selected is ParagraphPicture)
                        {
                            VisibilitY.RTBPanel = Visibility.Collapsed;
                            VisibilitY.ImagePanel = Visibility.Visible;
                            VisibilitY.TablePanel = Visibility.Collapsed;
                        }
                        else if (selected is ParagraphTable)
                        {
                            VisibilitY.RTBPanel = Visibility.Collapsed;
                            VisibilitY.ImagePanel = Visibility.Collapsed;
                            VisibilitY.TablePanel = Visibility.Visible;
                        }
                        else
                        {
                            VisibilitY.RTBPanel = Visibility.Visible;
                            VisibilitY.ImagePanel = Visibility.Collapsed;
                            VisibilitY.TablePanel = Visibility.Collapsed;
                            if (selected is ParagraphCode)
                            {
                                AllowDropRTB = true;
                            }
                            else
                            {
                                AllowDropRTB = false;
                                if (selected is ParagraphList)
                                {
                                    VisibilitY.AutoList = Visibility.Visible;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    ICommand? deselect;
    public ICommand Deselect
    {
        get
        {
            return deselect ??= new RelayCommand(obj =>
            {
                Selected = null;
            });
        }
    }

    int addIndex;
    public int AddIndex { get => addIndex; set => SetProperty(ref addIndex, value); }

    ICommand? add;
    public ICommand Add
    {
        get
        {
            return add ??= new RelayCommand(obj =>
            {
                IParagraphData dataToAdd;
                if (AddIndex == 0)
                {
                    dataToAdd = new ParagraphText();
                }
                else if (AddIndex == 1)
                {
                    dataToAdd = new ParagraphH1();
                }
                else if (AddIndex == 2)
                {
                    dataToAdd = new ParagraphH2();
                }
                else if (AddIndex == 3)
                {
                    dataToAdd = new ParagraphList();
                }
                else if (AddIndex == 4)
                {
                    dataToAdd = new ParagraphPicture();
                }
                else if (AddIndex == 5)
                {
                    dataToAdd = new ParagraphTable();
                }
                else if (AddIndex == 6)
                {
                    dataToAdd = new ParagraphCode();
                }
                else
                {
                    return;
                }
                ParagraphToTreeView(dataToAdd, Selected);
                SaveHelper.NeedSave = true;
                UpdatingObjectNumbering();
            });
        }
    }

    public void ParagraphToTreeView(IParagraphData dataToAdd, IParagraphData? Selected)
    {
        if (Selected is ParagraphTitle || Selected is ParagraphTaskSheet || Selected is null)
        {
            Data.AddToTop(dataToAdd);
        }
        else if (Selected is ParagraphListOfReferences || Selected is ParagraphAppendix)
        {
            Data.AddToEnd(dataToAdd);
        }
        else
        {
            if (Selected is SectionH1 paragraphH1)
            {
                if (dataToAdd is SectionH1)
                {
                    Data.InsertAfter(Selected, dataToAdd);
                }
                else
                {
                    paragraphH1.AddParagraph(dataToAdd);
                }
            }
            else if (Selected is SectionH2 paragraphH2)
            {
                if (dataToAdd is SectionH1)
                {
                    IParagraphData? paragraphData = Data.PrevLevel(Data, Selected);
                    if (paragraphData == null)
                    {
                        Data.InsertAfter(Selected, dataToAdd);
                    }
                    else
                    {
                        Data.InsertAfter(paragraphData, dataToAdd);
                    }
                }
                else if (dataToAdd is SectionH2)
                {
                    Data.InsertAfter(Selected, dataToAdd);
                }
                else
                {
                    paragraphH2.AddParagraph(dataToAdd);
                }
            }
            else
            {
                if (dataToAdd is SectionParagraphs)
                {
                    IParagraphData? paragraphData1 = Data.PrevLevel(Data, Selected);
                    if (paragraphData1 == null)
                    {
                        Data.InsertAfter(Selected, dataToAdd);
                    }
                    else
                    {
                        if (paragraphData1 is SectionH1)
                        {
                            Data.InsertAfter(Selected, dataToAdd);
                        }
                        else if (dataToAdd is SectionH2)
                        {
                            Data.InsertAfter(paragraphData1, dataToAdd);
                        }
                        else
                        {
                            IParagraphData? paragraphData2 = Data.PrevLevel(Data, paragraphData1);
                            if (paragraphData2 == null)
                            {
                                Data.InsertAfter(paragraphData1, dataToAdd);
                            }
                            else
                            {
                                Data.InsertAfter(paragraphData2, dataToAdd);
                            }
                        }
                    }
                }
                else
                {
                    Data.InsertAfter(Selected, dataToAdd);
                }
            }
        }
    }

    void UpdatingObjectNumbering()
    {
        int h1 = 1, h2 = 1, p = 1, t = 1;
        UpdatingObjectNumbering(Data, ref h1, ref h2, ref p, ref t);
    }

    void UpdatingObjectNumbering(SectionParagraphs section, ref int h1, ref int h2, ref int p, ref int t)
    {
        foreach (IParagraphData paragraph in section.Paragraphs)
        {
            if (paragraph is Numbered numbered)
            {
                if (paragraph is ParagraphH1 paragraphH1)
                {
                    string data;
                    if (paragraph.Data.Length > 1)
                    {
                        data = paragraph.Data.Remove(paragraph.Data.Length - 2, 2);
                    }
                    else
                    {
                        data = paragraph.Data;
                    }
                    UpdatingObjectNumbering(paragraphH1, ref h1, ref h2, ref p, ref t);
                    if (data.ToUpper() != "ВВЕДЕНИЕ" && data.ToUpper() != "ЗАКЛЮЧЕНИЕ")
                    {
                        numbered.Number = (h1++).ToString() + " ";
                    }
                    else
                    {
                        numbered.Number = string.Empty;
                    }
                    h2 = 1;
                }
                else if (paragraph is ParagraphH2 paragraphH2)
                {
                    UpdatingObjectNumbering(paragraphH2, ref h1, ref h2, ref p, ref t);
                    numbered.Number = h1.ToString() + "." + (h2++).ToString() + " ";
                }
                else if (paragraph is ParagraphPicture)
                {
                    numbered.Number = (p++).ToString();
                }
                else if (paragraph is ParagraphTable)
                {
                    numbered.Number = (t++).ToString();
                }
            }
        }
    }

    public void DragDrop(IParagraphData targetItem, IParagraphData sourceItem)
    {
        MessageDragDrop mgg;
        if (sourceItem is ParagraphH1)
        {
            if (targetItem is ParagraphH1)
            {
                mgg = new(insert: Visibility.Collapsed);
            }
            else if (targetItem is ParagraphH2)
            {
                if (Data.PrevLevel(Data, targetItem) != null)
                {
                    mgg = new(swap: Visibility.Collapsed);
                }
                else
                {
                    mgg = new();
                }
            }
            else
            {
                if (Data.PrevLevel(Data, targetItem) != null)
                {
                    mgg = new(swap: Visibility.Collapsed);
                }
                else
                {
                    mgg = new();
                }
            }
        }
        else if (sourceItem is ParagraphH2)
        {
            if (targetItem is ParagraphH1)
            {
                if (Data.PrevLevel(Data, sourceItem) != null)
                {
                    mgg = new(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                }
                else
                {
                    mgg = new(insert: Visibility.Collapsed);
                }
            }
            else if (targetItem is ParagraphH2)
            {
                mgg = new(insert: Visibility.Collapsed);
            }
            else
            {
                IParagraphData? paragraphData = Data.PrevLevel(Data, targetItem);
                if (paragraphData is SectionH1 || paragraphData is null)
                {
                    mgg = new();
                }
                else
                {
                    mgg = new(swap: Visibility.Collapsed);
                }
            }
        }
        else
        {
            if (targetItem is ParagraphH1)
            {
                if (Data.PrevLevel(Data, sourceItem) != null)
                {
                    mgg = new(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                }
                else
                {
                    mgg = new(insert: Visibility.Collapsed);
                }
            }
            else if (targetItem is ParagraphH2)
            {
                IParagraphData? paragraphData = Data.PrevLevel(Data, sourceItem);
                if (paragraphData is SectionH1 || paragraphData is null)
                {
                    mgg = new(insert: Visibility.Collapsed);
                }
                else
                {
                    mgg = new(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                }
            }
            else
            {
                mgg = new(insert: Visibility.Collapsed);
            }
        }
        mgg.ShowDialog();
        int i = mgg.ViewModel.Number;
        if (i == 0)
        {
            Delete(targetItem);
            ParagraphToTreeView(targetItem, sourceItem);
        }
        else if (i == 1)
        {
            Delete(targetItem);
            InsertToTreeView(targetItem, Data.InsertBefore, sourceItem);
        }
        else if (i == 2)
        {
            Delete(targetItem);
            InsertToTreeView(targetItem, Data.InsertAfter, sourceItem);
        }
        else if (i == 3)
        {
            Data.SwapParagraphs(sourceItem, targetItem);
        }
        else
        {
            return;
        }
        SaveHelper.NeedSave = true;
        UpdatingObjectNumbering();
    }

    public delegate void Insert(IParagraphData into, IParagraphData insert);

    public void InsertToTreeView(IParagraphData dataToAdd, Insert insert, IParagraphData? Selected)
    {
        if (Selected is SectionH1)
        {
            insert(Selected, dataToAdd);
        }
        else if (Selected is SectionH2)
        {
            if (dataToAdd is SectionH1)
            {
                IParagraphData? paragraphData1 = Data.PrevLevel(Data, Selected);
                if (paragraphData1 == null)
                {
                    insert(Selected, dataToAdd);
                }
                else
                {
                    insert(paragraphData1, dataToAdd);
                }
            }
            else
            {
                insert(Selected, dataToAdd);
            }
        }
        else
        {
            if (dataToAdd is SectionH1)
            {
                IParagraphData? paragraphData1 = Data.PrevLevel(Data, Selected);
                if (paragraphData1 == null)
                {
                    insert(Selected, dataToAdd);
                }
                else
                {
                    IParagraphData? paragraphData2 = Data.PrevLevel(Data, paragraphData1);
                    if (paragraphData2 == null)
                    {
                        insert(paragraphData1, dataToAdd);
                    }
                    else
                    {
                        insert(paragraphData2, dataToAdd);
                    }
                }
            }
            else if (dataToAdd is SectionH2)
            {
                IParagraphData? paragraphData1 = Data.PrevLevel(Data, Selected);
                if (paragraphData1 == null)
                {
                    insert(Selected, dataToAdd);
                }
                else
                {
                    if (paragraphData1 is SectionH1)
                    {
                        insert(Selected, dataToAdd);
                    }
                    else
                    {
                        insert(paragraphData1, dataToAdd);
                    }
                }
            }
            else
            {
                insert(Selected, dataToAdd);
            }
        }
    }

    void InsetBefore(IParagraphData dataToAdd)
    {
        if (Selected is ParagraphTitle || Selected is ParagraphTaskSheet || Selected is null)
        {
            Data.AddToTop(dataToAdd);
        }
        else if (Selected is ParagraphListOfReferences || Selected is ParagraphAppendix)
        {
            Data.AddToEnd(dataToAdd);
        }
        else
        {
            InsertToTreeView(dataToAdd, Data.InsertBefore, Selected);
        }
        SaveHelper.NeedSave = true;
        UpdatingObjectNumbering();
    }

    void InsetAfter(IParagraphData dataToAdd)
    {
        if (Selected is ParagraphTitle || Selected is ParagraphTaskSheet)
        {
            Data.AddToTop(dataToAdd);
        }
        else if (Selected is ParagraphListOfReferences || Selected is ParagraphAppendix || Selected is null)
        {
            Data.AddToEnd(dataToAdd);
        }
        else
        {
            InsertToTreeView(dataToAdd, Data.InsertAfter, Selected);
        }
        SaveHelper.NeedSave = true;
        UpdatingObjectNumbering();
    }

    ICommand insertTextBefore;
    public ICommand InsertTextBefore
    {
        get
        {
            return insertTextBefore ??= new RelayCommand(obj =>
            {
                InsetBefore(new ParagraphText());
            });
        }
    }

    ICommand insertTextAfter;
    public ICommand InsertTextAfter
    {
        get
        {
            return insertTextAfter ??= new RelayCommand(obj =>
            {
                InsetAfter(new ParagraphText());
            });
        }
    }

    ICommand insertHeaderBefore;
    public ICommand InsertHeaderBefore
    {
        get
        {
            return insertHeaderBefore ??= new RelayCommand(obj =>
            {
                InsetBefore(new ParagraphH1());
            });
        }
    }

    ICommand insertHeaderAfter;
    public ICommand InsertHeaderAfter
    {
        get
        {
            return insertHeaderAfter ??= new RelayCommand(obj =>
            {
                InsetAfter(new ParagraphH1());
            });
        }
    }

    ICommand insertSubHeaderBefore;
    public ICommand InsertSubHeaderBefore
    {
        get
        {
            return insertSubHeaderBefore ??= new RelayCommand(obj =>
            {
                InsetBefore(new ParagraphH2());
            });
        }
    }

    ICommand insertSubHeaderAfter;
    public ICommand InsertSubHeaderAfter
    {
        get
        {
            return insertSubHeaderAfter ??= new RelayCommand(obj =>
            {
                InsetAfter(new ParagraphH2());
            });
        }
    }

    ICommand insertListBefore;
    public ICommand InsertListBefore
    {
        get
        {
            return insertListBefore ??= new RelayCommand(obj =>
            {
                InsetBefore(new ParagraphList());
            });
        }
    }

    ICommand insertListAfter;
    public ICommand InsertListAfter
    {
        get
        {
            return insertListAfter ??= new RelayCommand(obj =>
            {
                InsetAfter(new ParagraphList());
            });
        }
    }

    ICommand insertPictureBefore;
    public ICommand InsertPictureBefore
    {
        get
        {
            return insertPictureBefore ??= new RelayCommand(obj =>
            {
                InsetBefore(new ParagraphPicture());
            });
        }
    }

    ICommand insertPictureAfter;
    public ICommand InsertPictureAfter
    {
        get
        {
            return insertPictureAfter ??= new RelayCommand(obj =>
            {
                InsetAfter(new ParagraphPicture());
            });
        }
    }

    ICommand insertTableBefore;
    public ICommand InsertTableBefore
    {
        get
        {
            return insertTableBefore ??= new RelayCommand(obj =>
            {
                InsetBefore(new ParagraphTable());
            });
        }
    }

    ICommand insertTableAfter;
    public ICommand InsertTableAfter
    {
        get
        {
            return insertTableAfter ??= new RelayCommand(obj =>
            {
                InsetAfter(new ParagraphTable());
            });
        }
    }

    ICommand insertCodeBefore;
    public ICommand InsertCodeBefore
    {
        get
        {
            return insertCodeBefore ??= new RelayCommand(obj =>
            {
                InsetBefore(new ParagraphCode());
            });
        }
    }

    ICommand insertCodeAfter;
    public ICommand InsertCodeAfter
    {
        get
        {
            return insertCodeAfter ??= new RelayCommand(obj =>
            {
                InsetAfter(new ParagraphCode());
            });
        }
    }

    ICommand? resetAddIndex;
    public ICommand ResetAddIndex
    {
        get
        {
            return resetAddIndex ??= new RelayCommand(obj =>
            {
                AddIndex = -1;
            });
        }
    }

    ICommand? deleteSelected;
    public ICommand DeleteSelected
    {
        get
        {
            return deleteSelected ??= new RelayCommand(
            obj =>
            {
                Delete(Selected);
            });
        }
    }

    bool Prev(SectionParagraphs section, IParagraphData paragraph, ref IParagraphData? prev)
    {
        if (section.Paragraphs.Count > 0 && prev == null)
        {
            if (section.Paragraphs[0] != paragraph)
            {
                for (int i = 0; i < section.Paragraphs.Count - 1; i++)
                {
                    if (section.Paragraphs[i + 1] == paragraph)
                    {
                        prev = section.Paragraphs[i];
                        return true;
                    }
                    if (section.Paragraphs[i + 1] is SectionParagraphs section1)
                    {
                        if (Prev(section1, paragraph, ref prev))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void Delete(IParagraphData paragraph)
    {
        if (paragraph != null)
        {
            bool t = false;
            IParagraphData? prev = null;
            if (paragraph == Selected)
            {
                t = Prev(Data, paragraph, ref prev);
            }

            if (paragraph is ParagraphTitle)
            {
                DeleteTitle();
            }
            else if (paragraph is ParagraphTaskSheet)
            {
                DeleteTaskSheet();
            }
            else if (paragraph is ParagraphListOfReferences)
            {
                DeleteListOfReferences();
            }
            else if (paragraph is ParagraphAppendix)
            {
                DeleteAppendix();
            }
            else
            {
                Data.RemoveParagraph(paragraph);
            }
            if (t)
            {
                Selected = prev;
            }
            SaveHelper.NeedSave = true;
            UpdatingObjectNumbering();
        }
    }

    readonly Timer timer;

    bool autoSave;
    public bool AutoSave
    {
        get => autoSave;
        set
        {
            SetProperty(ref autoSave, value);
            Properties.Settings.Default.AutoSave = autoSave;
            Properties.Settings.Default.Save();
            if (autoSave)
            {
                timer.Change(300000, 300000);
            }
            else
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
    }

    public void AutoSaveFile(object obj)
    {
        if (!string.IsNullOrEmpty(File.SavePath))
        {
            File.Save(Data);
        }
    }

    ViewModelVisibility visibility;
    public ViewModelVisibility VisibilitY { get => visibility; set => SetProperty(ref visibility, value); }

    string? winTitle;
    public string? WinTitle { get => winTitle; set => SetProperty(ref winTitle, value); }

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

                    VisibilitY.TitleMI = Visibility.Collapsed;
                    DeleteTitle();
                    VisibilitY.TaskSheetMI = Visibility.Collapsed;
                    DeleteTaskSheet();
                    Data.Type = DocumentType.DefaultDocument;
                    TextHeader("DefaultDocument");
                    Data.Title.UpdateTitleItems(Data.Type);
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
                    AddListOfReferences();
                    AddAppendix();
                    Data.Type = DocumentType.Coursework;
                    TextHeader("Coursework");
                    Data.Title.UpdateTitleItems(Data.Type);
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

                    Data.Type = DocumentType.LaboratoryWork;
                    TextHeader("LaboratoryWork");
                    Data.Title.UpdateTitleItems(Data.Type);
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

                    Data.Type = DocumentType.PracticeWork;
                    TextHeader("PracticeWork");
                    Data.Title.UpdateTitleItems(Data.Type);
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

                    Data.Type = DocumentType.ControlWork;
                    TextHeader("ControlWork");
                    Data.Title.UpdateTitleItems(Data.Type);
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
                    AddListOfReferences();
                    Data.Type = DocumentType.Referat;
                    TextHeader("Referat");
                    Data.Title.UpdateTitleItems(Data.Type);
                }
            }
            else if (referat)
            {
                SetProperty(ref referat, value);
            }
        }
    }

    bool productionPractice;
    public bool ProductionPractice
    {
        get => productionPractice;
        set
        {
            if (value)
            {
                if (!productionPractice)
                {
                    DocumentTypeFalse();
                    SetProperty(ref productionPractice, value);

                    NoDefaultDocument();
                    AddListOfReferences();
                    AddAppendix();
                    Data.Type = DocumentType.ProductionPractice;
                    TextHeader("ProductionPractice");
                    Data.Title.UpdateTitleItems(Data.Type);
                }
            }
            else if (productionPractice)
            {
                SetProperty(ref productionPractice, value);
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
                    AddListOfReferences();
                    AddAppendix();
                    Data.Type = DocumentType.VKR;
                    TextHeader("VKR");
                    Data.Title.UpdateTitleItems(Data.Type);
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
        ProductionPractice = false;
        VKR = false;
    }

    void NoDefaultDocument()
    {
        AddTitle();
        VisibilitY.TaskSheetMI = Visibility.Collapsed;
        DeleteTaskSheet();
    }

    void HeaderUpdate()
    {
        switch (Data.Type)
        {
            case DocumentType.DefaultDocument:
                defaultDocument = true;
                NotifyPropertyChanged(nameof(DefaultDocument));
                VisibilitY.TitleMI = Visibility.Collapsed;
                VisibilitY.TaskSheetMI = Visibility.Collapsed;
                break;
            case DocumentType.LaboratoryWork:
                laboratoryWork = true;
                NotifyPropertyChanged(nameof(LaboratoryWork));
                VisibilitY.TitleMI = Visibility.Visible;
                VisibilitY.TaskSheetMI = Visibility.Collapsed;
                break;
            case DocumentType.PracticeWork:
                practiceWork = true;
                NotifyPropertyChanged(nameof(PracticeWork));
                VisibilitY.TitleMI = Visibility.Visible;
                VisibilitY.TaskSheetMI = Visibility.Collapsed;
                break;
            case DocumentType.Coursework:
                coursework = true;
                NotifyPropertyChanged(nameof(Coursework));
                VisibilitY.TitleMI = Visibility.Visible;
                VisibilitY.TaskSheetMI = Visibility.Visible;
                break;
            case DocumentType.ControlWork:
                controlWork = true;
                NotifyPropertyChanged(nameof(ControlWork));
                VisibilitY.TitleMI = Visibility.Visible;
                VisibilitY.TaskSheetMI = Visibility.Collapsed;
                break;
            case DocumentType.Referat:
                referat = true;
                NotifyPropertyChanged(nameof(Referat));
                VisibilitY.TitleMI = Visibility.Visible;
                VisibilitY.TaskSheetMI = Visibility.Collapsed;
                break;
            case DocumentType.ProductionPractice:
                productionPractice = true;
                NotifyPropertyChanged(nameof(ProductionPractice));
                VisibilitY.TitleMI = Visibility.Visible;
                VisibilitY.TaskSheetMI = Visibility.Collapsed;
                break;
            case DocumentType.VKR:
                vkr = true;
                NotifyPropertyChanged(nameof(VKR));
                VisibilitY.TitleMI = Visibility.Visible;
                VisibilitY.TaskSheetMI = Visibility.Collapsed;
                break;
        }
        Data.Title.UpdateTitleItems(Data.Type);
        TextHeader(Data.Type.ToString());
        if (Data.Properties.Title)
        {
            AddTitle();
        }
        if (Data.Properties.TaskSheet)
        {
            AddTaskSheet();
        }
        if (Data.Properties.ListOfReferences)
        {
            AddListOfReferences();
        }
        if (Data.Properties.Appendix)
        {
            AddAppendix();
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
                UpdatingObjectNumbering();
                Report report = new();
                await Task.Run(() =>
                    Report.Create(Data, Properties.Settings.Default.ExportPDF, Properties.Settings.Default.ExportHTML));
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
                Data = file.NewFile(Data);
                HeaderUpdateSave();
            });
        }
    }


    ICommand? openFile;
    public ICommand OpenFile
    {
        get
        {
            return openFile ??= new RelayCommand(
            async obj =>
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "wordkiller file (*" + Properties.Settings.Default.Extension + ")|*" + Properties.Settings.Default.Extension + "|All files (*.*)|*.*"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    await OpenAsync(openFileDialog.FileName);
                }
            });
        }
    }

    public async Task OpenAsync(string fileName)
    {
        DocumentTypeFalse();
        Data = await file.OpenFile(fileName);
        HeaderUpdate();
        Data.Title.FacultyItems = new();
        Data.Title.CathedraItems = new();
        Data.Title.ProfessorItems = new();
        Data.Title.UpdateFaculty.Execute(null);
        Data.Title.UpdateCathedra.Execute(null);
        Data.Title.UpdateProfessor.Execute(null);
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
            case DocumentType.DefaultDocument:
                TextHeader("DefaultDocument");
                break;
            case DocumentType.LaboratoryWork:
                TextHeader("LaboratoryWork");
                break;
            case DocumentType.PracticeWork:
                TextHeader("PracticeWork");
                break;
            case DocumentType.Coursework:
                TextHeader("Coursework");
                break;
            case DocumentType.ControlWork:
                TextHeader("ControlWork");
                break;
            case DocumentType.Referat:
                TextHeader("Referat");
                break;
            case DocumentType.ProductionPractice:
                TextHeader("ProductionPractice");
                break;
            case DocumentType.VKR:
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

    void AddTitle()
    {
        VisibilitY.TitleMI = Visibility.Visible;
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
            VisibilitY.NotComplexObjects = Visibility.Collapsed;
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
        VisibilitY.TaskSheetMI = Visibility.Visible;
        Data.Properties.TaskSheet = true;
        if (Data.Paragraphs.Count > 0)
        {
            if (Data.Paragraphs[0] is not ParagraphTitle && Data.Paragraphs[0] is not ParagraphTaskSheet)
            {
                Data.InsertBefore(Data.Paragraphs[0], new ParagraphTaskSheet());
            }
            else if (Data.Paragraphs[0] is ParagraphTitle)
            {
                if (Data.Paragraphs.Count > 1)
                {
                    if (Data.Paragraphs[1] is not ParagraphTaskSheet)
                    {
                        Data.InsertAfter(Data.Paragraphs[0], new ParagraphTaskSheet());
                    }
                }
                else
                {
                    Data.InsertAfter(Data.Paragraphs[0], new ParagraphTaskSheet());
                }
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
            VisibilitY.NotComplexObjects = Visibility.Collapsed;
        }
    }

    ICommand? listOfReferencesMI;
    public ICommand ListOfReferencesMI
    {
        get
        {
            return listOfReferencesMI ??= new RelayCommand(obj =>
            {
                if (Data.Properties.ListOfReferences)
                {
                    AddListOfReferences();
                }
                else
                {
                    DeleteListOfReferences();
                }
            });
        }
    }

    void AddListOfReferences()
    {
        Data.Properties.ListOfReferences = true;
        if (Data.Paragraphs.Count > 0)
        {
            if (Data.Paragraphs.Count > 1 && Data.Paragraphs[^1] is ParagraphAppendix && Data.Paragraphs[^2] is not ParagraphListOfReferences)
            {
                Data.InsertBefore(Data.Paragraphs[^1], new ParagraphListOfReferences());
            }
            else if (Data.Paragraphs[^1] is not ParagraphListOfReferences && Data.Paragraphs[^1] is not ParagraphAppendix)
            {
                Data.InsertAfter(Data.Paragraphs[^1], new ParagraphListOfReferences());
            }
        }
        else
        {
            Data.AddParagraph(new ParagraphListOfReferences());
        }
    }

    void DeleteListOfReferences()
    {
        Data.Properties.ListOfReferences = false;
        if (Data.Paragraphs.Count > 0)
        {
            if (Data.Paragraphs.Count > 1 && Data.Paragraphs[^1] is ParagraphAppendix && Data.Paragraphs[^2] is ParagraphListOfReferences)
            {
                Data.Paragraphs.RemoveAt(Data.Paragraphs.Count - 2);
            }
            else if (Data.Paragraphs[^1] is ParagraphListOfReferences)
            {
                Data.Paragraphs.RemoveAt(Data.Paragraphs.Count - 1);
            }
        }
        if (Data.Paragraphs.Count == 0)
        {
            VisibilitY.NotComplexObjects = Visibility.Collapsed;
        }
    }

    ICommand? appendixMI;
    public ICommand AppendixMI
    {
        get
        {
            return appendixMI ??= new RelayCommand(obj =>
            {
                if (Data.Properties.Appendix)
                {
                    AddAppendix();
                }
                else
                {
                    DeleteAppendix();
                }
            });
        }
    }

    void AddAppendix()
    {
        Data.Properties.Appendix = true;
        if (Data.Paragraphs.Count > 0)
        {
            if (Data.Paragraphs[^1] is not ParagraphAppendix)
            {
                Data.InsertAfter(Data.Paragraphs[^1], new ParagraphAppendix());
            }
        }
        else
        {
            Data.AddParagraph(new ParagraphAppendix());
        }
    }

    void DeleteAppendix()
    {
        Data.Properties.Appendix = false;
        if (Data.Paragraphs.Count > 0)
        {
            if (Data.Paragraphs[^1] is ParagraphAppendix)
            {
                Data.Paragraphs.RemoveAt(Data.Paragraphs.Count - 1);
            }
        }
        if (Data.Paragraphs.Count == 0)
        {
            VisibilitY.NotComplexObjects = Visibility.Collapsed;
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

    ICommand? productionPractice_Click;
    public ICommand ProductionPractice_Click
    {
        get
        {
            return productionPractice_Click ??= new RelayCommand(
            obj =>
            {
                if (!ProductionPractice)
                {
                    ProductionPractice = true;
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

    ICommand? autoList;
    public ICommand AutoList
    {
        get
        {
            return autoList ??= new RelayCommand(
            obj =>
            {
                if (Selected != null)
                {

                    string tt = Selected.Data;
                    string[] lines = tt.Split("\r\n");
                    for (int j = 0; j < lines.Length; j++)
                    {
                        string[] words = lines[j].Split(' ');
                        if (words.Length > 0)
                        {
                            int after = words[0].IndexOf(')');
                            bool before = words[0].Contains('(');
                            int numberSeparators = 0;
                            if (after != -1 && !before)
                            {
                                if (words[0].Length == after + 1)
                                {
                                    int separator = -1;
                                    for (int i = 0; i < words[0].Length; i++)
                                    {
                                        separator = words[0].IndexOf('.', separator + 1);
                                        if (separator != -1)
                                        {
                                            numberSeparators++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    if (numberSeparators > 0)
                                    {
                                        string sep = "";
                                        for (int f = 0; f < numberSeparators; f++)
                                        {
                                            sep += "!";
                                        }
                                        words[0] = sep;
                                    }
                                    else
                                    {
                                        words = words.Skip(1).ToArray();
                                    }
                                }
                                else
                                {
                                    string txt = words[0][(after + 1)..];
                                    int separator = -1;
                                    for (int i = 0; i < words[0].Length; i++)
                                    {
                                        separator = words[0].IndexOf('.', separator + 1);
                                        if (separator != -1)
                                        {
                                            numberSeparators++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    if (numberSeparators > 0)
                                    {
                                        string sep = "";
                                        for (int f = 0; f < numberSeparators; f++)
                                        {
                                            sep += "!";
                                        }
                                        words[0] = sep + " " + txt;
                                    }
                                    else
                                    {
                                        words[0] = txt;
                                    }
                                }
                            }
                        }
                        lines[j] = string.Join(" ", words);
                    }
                    Selected.Data = string.Join("\n", lines);
                }
            });
        }
    }

    bool allowDropRTB;
    public bool AllowDropRTB { get => allowDropRTB; set => SetProperty(ref allowDropRTB, value); }

    public ViewModelDocument()
    {
        timer = new Timer(new TimerCallback(AutoSaveFile));
        AddIndex = -1;
        visibility = new();
        AllowDropRTB = false;
        autoSave = Properties.Settings.Default.AutoSave;
        data = new();
        file = new();
        DefaultDocument = true;
    }
}