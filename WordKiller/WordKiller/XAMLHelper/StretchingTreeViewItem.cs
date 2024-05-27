using System.Windows;
using System.Windows.Controls;

namespace WordKiller.XAMLHelper;

public class StretchingTreeViewItem : TreeViewItem
{
    public StretchingTreeViewItem()
    {
        Loaded += StretchingTreeViewItem_Loaded;
    }

    void StretchingTreeViewItem_Loaded(object sender, RoutedEventArgs e)
    {
        if (VisualChildrenCount > 0)
        {
            if (GetVisualChild(0) is Grid grid && grid.ColumnDefinitions.Count == 3)
            {
                grid.ColumnDefinitions.RemoveAt(2);
                grid.ColumnDefinitions[1].Width = new(1, GridUnitType.Star);
            }
        }
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new StretchingTreeViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is StretchingTreeViewItem;
    }
}