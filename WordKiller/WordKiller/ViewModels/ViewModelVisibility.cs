using System.Windows;

namespace WordKiller.ViewModels;

public class ViewModelVisibility : ViewModelBase
{
    Visibility appendixPanel;

    Visibility autoList;
    Visibility ff;

    Visibility imagePanel;

    Visibility listOfReferencesPanel;

    Visibility notComplexObjects;

    Visibility rtbPanel;

    Visibility tablePanel;

    Visibility taskSheetMI;

    Visibility taskSheetPanel;

    Visibility titleMI;

    Visibility titlePanel;

    Visibility unselectInfo;

    public ViewModelVisibility()
    {
        FF = Visibility.Collapsed;
        autoList = Visibility.Collapsed;
        notComplexObjects = Visibility.Collapsed;
        titlePanel = Visibility.Collapsed;
        taskSheetPanel = Visibility.Collapsed;
        titleMI = Visibility.Collapsed;
        taskSheetMI = Visibility.Collapsed;
        listOfReferencesPanel = Visibility.Collapsed;
        appendixPanel = Visibility.Collapsed;
    }

    public Visibility FF
    {
        get => ff;
        set => SetProperty(ref ff, value);
    }

    public Visibility TitleMI
    {
        get => titleMI;
        set => SetProperty(ref titleMI, value);
    }

    public Visibility TaskSheetMI
    {
        get => taskSheetMI;
        set => SetProperty(ref taskSheetMI, value);
    }

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

    public Visibility UnselectInfo
    {
        get => unselectInfo;
        set => SetProperty(ref unselectInfo, value);
    }

    public Visibility TitlePanel
    {
        get => titlePanel;
        set => SetProperty(ref titlePanel, value);
    }

    public Visibility TaskSheetPanel
    {
        get => taskSheetPanel;
        set => SetProperty(ref taskSheetPanel, value);
    }

    public Visibility ListOfReferencesPanel
    {
        get => listOfReferencesPanel;
        set => SetProperty(ref listOfReferencesPanel, value);
    }

    public Visibility AppendixPanel
    {
        get => appendixPanel;
        set => SetProperty(ref appendixPanel, value);
    }

    public Visibility RTBPanel
    {
        get => rtbPanel;
        set => SetProperty(ref rtbPanel, value);
    }

    public Visibility ImagePanel
    {
        get => imagePanel;
        set => SetProperty(ref imagePanel, value);
    }

    public Visibility TablePanel
    {
        get => tablePanel;
        set => SetProperty(ref tablePanel, value);
    }

    public Visibility AutoList
    {
        get => autoList;
        set => SetProperty(ref autoList, value);
    }
}