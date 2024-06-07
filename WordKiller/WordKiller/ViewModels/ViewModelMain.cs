using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.Scripts;
using WordKiller.Scripts.File;
using WordKiller.ViewModels.Settings;

namespace WordKiller.ViewModels;

public class ViewModelMain : ViewModelBase
{
    ICommand? allowDrop;

    ICommand? allowDropImage;

    ICommand? allowDropNotComplexObjects;

    ICommand? disallowDrop;

    ICommand? disallowDropImage;
    ViewModelDocument document;

    ICommand? drop;

    ICommand? dropAppendix;

    ICommand? dropNotComplexObjects;

    ICommand? dropNotComplexObjectsAppendix;

    ICommand? dropTaskSheet1;

    ICommand? dropTaskSheet2;


    ICommand? dropTitle;

    ICommand? exitSettings;

    ViewModelExport export;

    ICommand? hideFF;

    ICommand? keyDownEvent;

    ICommand? openSettings;

    ICommand? quit;

    ViewModelResizing resizing;

    ViewModelSettings settings;

    ICommand? updatePerformed;

    Visibility visibilityDrag;

    Visibility visibilityMainPanel;

    Visibility visibilitySettingsPanel;

    public ViewModelMain()
    {
        Document = new();
        export = new();
        HelpCommands = new();
        NetworkCommans = new();
        settings = new();
        resizing = new();
        VisibilitySettingsPanel = Visibility.Collapsed;
        VisibilityMainPanel = Visibility.Visible;
        VisibilityDrag = Visibility.Collapsed;
    }

    public ViewModelDocument Document
    {
        get => document;
        set => SetProperty(ref document, value);
    }

    public ViewModelSettings Settings
    {
        get => settings;
        set => SetProperty(ref settings, value);
    }

    public ViewModelResizing Resizing
    {
        get => resizing;
        set => SetProperty(ref resizing, value);
    }

    public ViewModelExport Export
    {
        get => export;
        set => SetProperty(ref export, value);
    }

    public NetworkCommans NetworkCommans { get; set; }

    public HelpCommands HelpCommands { get; set; }

    public Visibility VisibilityMainPanel
    {
        get => visibilityMainPanel;
        set => SetProperty(ref visibilityMainPanel, value);
    }

    public Visibility VisibilitySettingsPanel
    {
        get => visibilitySettingsPanel;
        set => SetProperty(ref visibilitySettingsPanel, value);
    }

    public ICommand OpenSettings
    {
        get
        {
            return openSettings ??= new RelayCommand(
                obj =>
                {
                    VisibilityMainPanel = Visibility.Collapsed;
                    VisibilitySettingsPanel = Visibility.Visible;
                });
        }
    }

    public ICommand ExitSettings
    {
        get
        {
            return exitSettings ??= new RelayCommand(obj =>
            {
                VisibilitySettingsPanel = Visibility.Collapsed;
                VisibilityMainPanel = Visibility.Visible;
            });
        }
    }

    public ICommand Quit
    {
        get { return quit ??= new RelayCommand(obj => { UIHelper.WindowClose(); }); }
    }

    public ICommand UpdatePerformed
    {
        get
        {
            return updatePerformed ??= new RelayCommand(
                obj => 
                { 
                    Document.Data.Title.Performed = [.. Settings.Profile.Users];
                });
        }
    }

    public Visibility VisibilityDrag
    {
        get => visibilityDrag;
        set => SetProperty(ref visibilityDrag, value);
    }

    public ICommand AllowDrop
    {
        get
        {
            return allowDrop ??= new RelayCommand(
                obj =>
                {
                    DragEventArgs e = (DragEventArgs)obj;
                    e.Handled = true;
                    e.Effects = DragDropEffects.None;
                    if (EnableDragDrop(e))
                    {
                        VisibilityDrag = Visibility.Visible;
                    }
                });
        }
    }

    public ICommand DisallowDrop
    {
        get
        {
            return disallowDrop ??= new RelayCommand(
                obj =>
                {
                    DragEventArgs e = (DragEventArgs)obj;
                    e.Handled = true;
                    e.Effects = DragDropEffects.None;
                    if (EnableDragDrop(e))
                    {
                        VisibilityDrag = Visibility.Collapsed;
                    }
                });
        }
    }

    public ICommand AllowDropImage
    {
        get
        {
            return allowDropImage ??= new RelayCommand(
                obj =>
                {
                    DragEventArgs e = (DragEventArgs)obj;
                    if (EnableDragDrop(e))
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        VisibilityDrag = Visibility.Visible;
                    }
                });
        }
    }

    public ICommand DisallowDropImage
    {
        get
        {
            return disallowDropImage ??= new RelayCommand(
                obj =>
                {
                    DragEventArgs e = (DragEventArgs)obj;
                    if (EnableDragDrop(e))
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        VisibilityDrag = Visibility.Collapsed;
                    }
                });
        }
    }

    public ICommand Drop
    {
        get
        {
            return drop ??= new RelayCommand(
                obj => { MainDragDrop((DragEventArgs)obj, Document.Selected); });
        }
    }

    public ICommand DropAppendix
    {
        get
        {
            return dropAppendix ??= new RelayCommand(
                obj => { MainDragDrop((DragEventArgs)obj, Document.Data.Appendix.Selected); });
        }
    }

    public ICommand DropTitle
    {
        get
        {
            return dropTitle ??= new RelayCommand(
                obj => { TitleDragDrop((DragEventArgs)obj); });
        }
    }

    public ICommand DropTaskSheet1
    {
        get
        {
            return dropTaskSheet1 ??= new RelayCommand(
                obj => { TaskSheet1DragDrop((DragEventArgs)obj); });
        }
    }

    public ICommand DropTaskSheet2
    {
        get
        {
            return dropTaskSheet2 ??= new RelayCommand(
                obj => { TaskSheet2DragDrop((DragEventArgs)obj); });
        }
    }

    public ICommand DropNotComplexObjects
    {
        get
        {
            return dropNotComplexObjects ??= new RelayCommand(
                obj =>
                {
                    DragEventArgs e = (DragEventArgs)obj;
                    var data = e.Data.GetData(DataFormats.FileDrop);
                    if (data != null)
                    {
                        foreach (string path in data as string[])
                        {
                            if (path.Length > 0)
                            {
                                string nameFile = Path.GetFileNameWithoutExtension(path);
                                try
                                {
                                    Bitmap bitmap = new(path);
                                    Document.ParagraphToTreeView(new ParagraphPicture(nameFile, bitmap),
                                        Document.Selected);
                                }
                                catch
                                {
                                    FileStream file = new(path, FileMode.Open);
                                    StreamReader reader = new(file);
                                    string data1 = reader.ReadToEnd();
                                    Document.ParagraphToTreeView(new ParagraphCode(nameFile, data1), Document.Selected);
                                }

                                SaveHelper.NeedSave = true;
                            }
                        }
                    }

                    VisibilityDrag = Visibility.Collapsed;
                });
        }
    }

    public ICommand DropNotComplexObjectsAppendix
    {
        get
        {
            return dropNotComplexObjectsAppendix ??= new RelayCommand(
                obj =>
                {
                    DragEventArgs e = (DragEventArgs)obj;
                    var data = e.Data.GetData(DataFormats.FileDrop);
                    if (data != null)
                    {
                        foreach (string path in data as string[])
                        {
                            if (path.Length > 0)
                            {
                                string nameFile = Path.GetFileNameWithoutExtension(path);
                                try
                                {
                                    Bitmap bitmap = new(path);
                                    if (Document.Data.Appendix.Selected == null)
                                    {
                                        Document.Data.Appendix.Paragraphs.Add(new ParagraphPicture(nameFile, bitmap));
                                    }
                                    else
                                    {
                                        Document.Data.Appendix.InsertAfter(Document.Data.Appendix.Selected,
                                            new ParagraphPicture(nameFile, bitmap));
                                    }
                                }
                                catch
                                {
                                    FileStream file = new(path, FileMode.Open);
                                    StreamReader reader = new(file);
                                    string data1 = reader.ReadToEnd();
                                    if (Document.Data.Appendix.Selected == null)
                                    {
                                        Document.Data.Appendix.Paragraphs.Add(new ParagraphCode(nameFile, data1));
                                    }
                                    else
                                    {
                                        Document.Data.Appendix.InsertAfter(Document.Data.Appendix.Selected,
                                            new ParagraphCode(nameFile, data1));
                                    }
                                }

                                SaveHelper.NeedSave = true;
                            }
                        }
                    }

                    VisibilityDrag = Visibility.Collapsed;
                });
        }
    }

    public ICommand AllowDropNotComplexObjects
    {
        get
        {
            return allowDropNotComplexObjects ??= new RelayCommand(
                obj =>
                {
                    DragEventArgs e = (DragEventArgs)obj;
                    DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
                    if (dragDropInfo == null)
                    {
                        e.Effects = DragDropEffects.All;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }

                    e.Handled = true;
                });
        }
    }

    public ICommand KeyDownEvent
    {
        get
        {
            return keyDownEvent ??= new RelayCommand(
                obj =>
                {
                    KeyEventArgs e = (KeyEventArgs)obj;
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        if (e.Key == Key.F)
                        {
                            Document.VisibilitY.FF = Visibility.Visible;
                        }
                    }
                });
        }
    }

    public ICommand HideFF
    {
        get
        {
            return hideFF ??= new RelayCommand(
                obj => { Document.VisibilitY.FF = Visibility.Collapsed; });
        }
    }

    public bool EnableDragDrop(DragEventArgs e)
    {
        DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
        if (dragDropInfo == null && e.Data.GetData(DataFormats.FileDrop) != null &&
            (Document.Selected is ParagraphPicture || Document.Selected is ParagraphCode ||
             Document.Selected is ParagraphAppendix || Document.Selected is ParagraphTitle ||
             Document.Selected is ParagraphTaskSheet))
        {
            return true;
        }

        return false;
    }

    void MainDragDrop(DragEventArgs e, IParagraphData? selected)
    {
        if (EnableDragDrop(e))
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string path = (data as string[])[0];
                if (path.Length > 0)
                {
                    string nameFile = Path.GetFileNameWithoutExtension(path);
                    if (selected is ParagraphPicture paragraphPicture)
                    {
                        Bitmap bitmap;
                        try
                        {
                            bitmap = new(path);
                        }
                        catch
                        {
                            return;
                        }

                        paragraphPicture.Bitmap = bitmap;
                        paragraphPicture.UpdateBitmapImage();
                        paragraphPicture.Description = nameFile;
                    }
                    else if (selected is ParagraphCode paragraphCode)
                    {
                        FileStream file = new(path, FileMode.Open);
                        StreamReader reader = new(file);
                        string data1 = reader.ReadToEnd();
                        paragraphCode.Description = nameFile;
                        paragraphCode.Data = data1;
                    }
                }
            }

            VisibilityDrag = Visibility.Collapsed;
        }
    }

    void TitleDragDrop(DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string path = (data as string[])[0];
                if (path.Length > 0)
                {
                    Bitmap bitmap;
                    try
                    {
                        bitmap = new(path);
                    }
                    catch
                    {
                        return;
                    }

                    Document.Data.Title.Picture.Bitmap = bitmap;
                    Document.Data.Title.Picture.UpdateBitmapImage();
                }
            }

            VisibilityDrag = Visibility.Collapsed;
        }
    }

    void TaskSheet1DragDrop(DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string path = (data as string[])[0];
                if (path.Length > 0)
                {
                    Bitmap bitmap;
                    try
                    {
                        bitmap = new(path);
                    }
                    catch
                    {
                        return;
                    }

                    Document.Data.TaskSheet.FirstPicture.Bitmap = bitmap;
                    Document.Data.TaskSheet.FirstPicture.UpdateBitmapImage();
                }
            }

            VisibilityDrag = Visibility.Collapsed;
        }
    }

    void TaskSheet2DragDrop(DragEventArgs e)
    {
        if (EnableDragDrop(e))
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string path = (data as string[])[0];
                if (path.Length > 0)
                {
                    Bitmap bitmap;
                    try
                    {
                        bitmap = new(path);
                    }
                    catch
                    {
                        return;
                    }

                    Document.Data.TaskSheet.SecondPicture.Bitmap = bitmap;
                    Document.Data.TaskSheet.SecondPicture.UpdateBitmapImage();
                }
            }

            VisibilityDrag = Visibility.Collapsed;
        }
    }


    void SecondDrop(DragEventArgs e, ObservableCollection<IParagraphData> section)
    {
        var data = e.Data.GetData(DataFormats.FileDrop);
        if (data != null)
        {
            foreach (string path in data as string[])
            {
                if (path.Length > 0)
                {
                    string nameFile = Path.GetFileNameWithoutExtension(path);
                    try
                    {
                        Bitmap bitmap = new(path);
                        Document.ParagraphToTreeView(new ParagraphPicture(nameFile, bitmap), Document.Selected);
                    }
                    catch
                    {
                        FileStream file = new(path, FileMode.Open);
                        StreamReader reader = new(file);
                        string data1 = reader.ReadToEnd();
                        Document.ParagraphToTreeView(new ParagraphCode(nameFile, data1), Document.Selected);
                    }

                    SaveHelper.NeedSave = true;
                }
            }
        }

        VisibilityDrag = Visibility.Collapsed;
    }
}