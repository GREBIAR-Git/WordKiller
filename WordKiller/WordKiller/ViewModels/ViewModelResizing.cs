using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels;

public class ViewModelResizing : ViewModelBase
{
    GridLength contentPanelSize;

    ICommand? resizingTreeView;
    GridLength treeViewSize;

    public ViewModelResizing()
    {
        TreeViewSize = new(Properties.Settings.Default.TreeViewSize, GridUnitType.Star);
        ContentPanelSize = new(100 - Properties.Settings.Default.TreeViewSize, GridUnitType.Star);
    }

    public GridLength TreeViewSize
    {
        get => treeViewSize;
        set => SetProperty(ref treeViewSize, value);
    }

    public GridLength ContentPanelSize
    {
        get => contentPanelSize;
        set => SetProperty(ref contentPanelSize, value);
    }

    public ICommand ResizingTreeView
    {
        get
        {
            return resizingTreeView ??= new RelayCommand(obj =>
            {
                Properties.Settings.Default.TreeViewSize =
                    treeViewSize.Value * 100 / (treeViewSize.Value + contentPanelSize.Value);
                Properties.Settings.Default.Save();
            });
        }
    }
}