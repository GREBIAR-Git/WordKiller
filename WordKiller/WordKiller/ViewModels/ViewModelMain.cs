using System.IO;
using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.Scripts.ForUI;
using WordKiller.ViewModels.Settings;

namespace WordKiller.ViewModels;
public class ViewModelMain : ViewModelBase
{
    ViewModelDocument document;
    public ViewModelDocument Document { get => document; set => SetProperty(ref document, value); }

    ViewModelSettings settings;
    public ViewModelSettings Settings { get => settings; set => SetProperty(ref settings, value); }

    ViewModelResizing resizing;
    public ViewModelResizing Resizing { get => resizing; set => SetProperty(ref resizing, value); }

    public NetworkCommans NetworkCommans { get; set; }

    public HelpCommands HelpCommands { get; set; }

    Visibility visibilityMainPanel;

    public Visibility VisibilityMainPanel { get => visibilityMainPanel; set => SetProperty(ref visibilityMainPanel, value); }

    Visibility visibilitySettingsPanel;

    public Visibility VisibilitySettingsPanel { get => visibilitySettingsPanel; set => SetProperty(ref visibilitySettingsPanel, value); }

    ICommand? openSettings;

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

    ICommand? exitSettings;

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

    ICommand? quit;

    public ICommand Quit
    {
        get
        {
            return quit ??= new RelayCommand(obj =>
            {
                UIHelper.WindowClose();
            });
        }
    }

    ICommand? updatePerformed;

    public ICommand UpdatePerformed
    {
        get
        {
            return updatePerformed ??= new RelayCommand(
            obj =>
            {
                Document.Data.Title.Performed = Settings.Profile.Users;
            });
        }
    }

    Visibility visibilityDrag;
    public Visibility VisibilityDrag { get => visibilityDrag; set => SetProperty(ref visibilityDrag, value); }

    public bool EnableDragDrop(DragEventArgs e)
    {
        DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
        if (dragDropInfo == null && e.Data.GetData(DataFormats.FileDrop) != null && (Document.Selected is ParagraphPicture || Document.Selected is ParagraphCode || Document.Selected is ParagraphAppendix))
        {
            return true;
        }
        return false;
    }

    ICommand? allowDrop;

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

    ICommand? disallowDrop;

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

    ICommand? allowDropImage;
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

    ICommand? disallowDropImage;

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

    ICommand? drop;
    public ICommand Drop
    {
        get
        {
            return drop ??= new RelayCommand(
            obj =>
            {
                MainDragDrop((DragEventArgs)obj, Document.Selected);
            });
        }
    }

    ICommand? dropAppendix;
    public ICommand DropAppendix
    {
        get
        {
            return dropAppendix ??= new RelayCommand(
            obj =>
            {
                MainDragDrop((DragEventArgs)obj, Document.Data.Appendix.Selected);
            });
        }
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
                        System.Drawing.Bitmap bitmap;
                        try
                        {
                            bitmap = new(path);
                        }
                        catch
                        {
                            return;
                        }
                        paragraphPicture.Bitmap = bitmap;
                        Document.MainImage = paragraphPicture.BitmapImage;
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

    ICommand? dropNotComplexObjects;
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
                            System.Drawing.Bitmap bitmap;
                            try
                            {
                                bitmap = new(path);
                                Document.Data.AddParagraph(new ParagraphPicture(nameFile, bitmap));
                            }
                            catch
                            {
                                FileStream file = new(path, FileMode.Open);
                                StreamReader reader = new(file);
                                string data1 = reader.ReadToEnd();
                                Document.Data.AddParagraph(new ParagraphCode(nameFile, data1));
                            }
                        }
                    }
                }
                VisibilityDrag = Visibility.Collapsed;
            });
        }
    }

    ICommand? allowDropNotComplexObjects;
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

    public ViewModelMain()
    {
        Document = new();
        HelpCommands = new();
        NetworkCommans = new();
        settings = new();
        resizing = new();
        VisibilitySettingsPanel = Visibility.Collapsed;
        VisibilityMainPanel = Visibility.Visible;
        VisibilityDrag = Visibility.Collapsed;
    }
}
