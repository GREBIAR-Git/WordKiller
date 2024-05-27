using System.Windows;
using Microsoft.Xaml.Behaviors;
using WordKiller.Scripts;

namespace WordKiller.XAMLHelper;

public class BindableSelectedItemBehavior : Behavior<StretchingTreeView>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject != null)
        {
            AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }
    }

    void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        SelectedItem = e.NewValue;
    }

    #region SelectedItem Property

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior),
            new UIPropertyMetadata(null, OnSelectedItemChanged));

    static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var tv = sender as BindableSelectedItemBehavior;
        if (e.NewValue == null)
        {
            var tvi = UIHelper.FindTviFromObjectRecursive(tv.AssociatedObject, e.OldValue);
            if (tvi != null)
            {
                tvi.IsSelected = false;
            }
        }
        else
        {
            var tvi = UIHelper.FindTviFromObjectRecursive(tv.AssociatedObject, e.NewValue);
            if (tvi != null)
            {
                tvi.IsSelected = true;
            }
        }
    }

    #endregion
}