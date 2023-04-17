using System.Windows;
using System.Windows.Input;
using WordKiller.Commands;

namespace WordKiller.ViewModels;

public class ViewModelResizing : ViewModelBase
{
    GridLength treeViewSize;
    public GridLength TreeViewSize
    {
        get => treeViewSize;
        set => SetProperty(ref treeViewSize, value);
    }

    GridLength contentPanelSize;
    public GridLength ContentPanelSize
    {
        get => contentPanelSize;
        set => SetProperty(ref contentPanelSize, value);
    }

    ICommand? resizingTreeView;
    public ICommand ResizingTreeView
    {
        get
        {
            return resizingTreeView ??= new RelayCommand(obj =>
            {
                Properties.Settings.Default.TreeViewSize = treeViewSize.Value * 100 / (treeViewSize.Value + contentPanelSize.Value);
                Properties.Settings.Default.Save();
            });
        }
    }

    public ViewModelResizing()
    {
        TreeViewSize = new GridLength(Properties.Settings.Default.TreeViewSize, GridUnitType.Star);
        ContentPanelSize = new GridLength(100 - Properties.Settings.Default.TreeViewSize, GridUnitType.Star);
    }
}