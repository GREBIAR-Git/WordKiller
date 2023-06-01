using System.Windows;
using System.Windows.Controls;

namespace WordKiller.XAMLHelper;

public class StretchingTreeView : TreeView
{
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new StretchingTreeViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is StretchingTreeViewItem;
    }
}
