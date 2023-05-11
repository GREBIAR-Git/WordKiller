using System.Windows;

namespace WordKiller.ViewModels;

public class ViewModelVisibility : ViewModelBase
{
    Visibility titleMI;
    public Visibility TitleMI { get => titleMI; set => SetProperty(ref titleMI, value); }

    Visibility taskSheetMI;
    public Visibility TaskSheetMI { get => taskSheetMI; set => SetProperty(ref taskSheetMI, value); }

    Visibility notComplexObjects;
    public Visibility NotComplexObjects
    {
        get => notComplexObjects;
        set
        {
            if (SetProperty(ref notComplexObjects, value))
            {
                if (NotComplexObjects == Visibility.Collapsed)
                {
                    UnselectInfo = Visibility.Visible;
                }
            }
        }
    }

    Visibility unselectInfo;
    public Visibility UnselectInfo
    {
        get => unselectInfo;
        set
        {
            SetProperty(ref unselectInfo, value);
        }
    }

    Visibility titlePanel;
    public Visibility TitlePanel
    {
        get => titlePanel;
        set
        {
            SetProperty(ref titlePanel, value);
        }
    }

    Visibility taskSheetPanel;
    public Visibility TaskSheetPanel
    {
        get => taskSheetPanel;
        set
        {
            SetProperty(ref taskSheetPanel, value);
        }
    }

    Visibility listOfReferencesPanel;
    public Visibility ListOfReferencesPanel
    {
        get => listOfReferencesPanel;
        set
        {
            SetProperty(ref listOfReferencesPanel, value);
        }
    }

    Visibility appendixPanel;
    public Visibility AppendixPanel
    {
        get => appendixPanel;
        set
        {
            SetProperty(ref appendixPanel, value);
        }
    }

    Visibility rtbPanel;
    public Visibility RTBPanel
    {
        get => rtbPanel;
        set
        {
            SetProperty(ref rtbPanel, value);
        }
    }

    Visibility imagePanel;
    public Visibility ImagePanel
    {
        get => imagePanel;
        set
        {
            SetProperty(ref imagePanel, value);
        }
    }

    Visibility tablePanel;
    public Visibility TablePanel
    {
        get => tablePanel;
        set
        {
            SetProperty(ref tablePanel, value);
        }
    }

    Visibility autoList;
    public Visibility AutoList
    {
        get => autoList;
        set
        {
            SetProperty(ref autoList, value);
        }
    }

    public ViewModelVisibility()
    {
        autoList = Visibility.Collapsed;
        notComplexObjects = Visibility.Collapsed;
        titlePanel = Visibility.Collapsed;
        taskSheetPanel = Visibility.Collapsed;
        titleMI = Visibility.Collapsed;
        taskSheetMI = Visibility.Collapsed;
        listOfReferencesPanel = Visibility.Collapsed;
        appendixPanel = Visibility.Collapsed;
    }
}
