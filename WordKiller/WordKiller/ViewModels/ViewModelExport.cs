using System.Windows;
using WordKiller.Scripts;

namespace WordKiller.ViewModels;

public class ViewModelExport : ViewModelBase
{
    bool exportHTML;

    bool exportPDF;

    Visibility visibilityExportHTML;
    Visibility visibilityExportPDF;

    public ViewModelExport()
    {
        if (WkrExport.IsWordInstall())
        {
            ExportPDF = Properties.Settings.Default.ExportPDF;
            ExportHTML = Properties.Settings.Default.ExportHTML;
        }
        else
        {
            VisibilityExportPDF = Visibility.Collapsed;
            VisibilityExportHTML = Visibility.Collapsed;
            ExportPDF = false;
            ExportHTML = false;
        }
    }

    public Visibility VisibilityExportPDF
    {
        get => visibilityExportPDF;
        set => SetProperty(ref visibilityExportPDF, value);
    }

    public Visibility VisibilityExportHTML
    {
        get => visibilityExportHTML;
        set => SetProperty(ref visibilityExportHTML, value);
    }

    public bool ExportPDF
    {
        get => exportPDF;
        set
        {
            SetProperty(ref exportPDF, value);
            Properties.Settings.Default.ExportPDF = exportPDF;
            Properties.Settings.Default.Save();
        }
    }

    public bool ExportHTML
    {
        get => exportHTML;
        set
        {
            SetProperty(ref exportHTML, value);
            Properties.Settings.Default.ExportHTML = exportHTML;
            Properties.Settings.Default.Save();
        }
    }
}