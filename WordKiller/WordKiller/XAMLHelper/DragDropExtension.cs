using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WordKiller.XAMLHelper;

public static class DragDropExtension
{
    #region ScrollOnDragDropProperty

    public static readonly DependencyProperty ScrollOnDragDropProperty =
        DependencyProperty.RegisterAttached("ScrollOnDragDrop",
            typeof(bool),
            typeof(DragDropExtension),
            new(false, HandleScrollOnDragDropChanged));

    public static bool GetScrollOnDragDrop(DependencyObject element)
    {
        if (element == null)
        {
            throw new ArgumentNullException("element");
        }

        return (bool)element.GetValue(ScrollOnDragDropProperty);
    }

    public static void SetScrollOnDragDrop(DependencyObject element, bool value)
    {
        if (element == null)
        {
            throw new ArgumentNullException("element");
        }

        element.SetValue(ScrollOnDragDropProperty, value);
    }

    static void HandleScrollOnDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement container = d as FrameworkElement;
        if (d == null)
        {
            Debug.Fail("Invalid type!");
            return;
        }

        Unsubscribe(container);

        if (true.Equals(e.NewValue))
        {
            Subscribe(container);
        }
    }

    static void Subscribe(FrameworkElement container)
    {
        container.PreviewDragOver += OnContainerPreviewDragOver;
    }

    static void OnContainerPreviewDragOver(object sender, DragEventArgs e)
    {
        if (sender is not FrameworkElement container)
        {
            return;
        }

        ScrollViewer scrollViewer = GetFirstVisualChild<ScrollViewer>(container);

        if (scrollViewer == null)
        {
            return;
        }

        const double tolerance = 10;
        double verticalPos = e.GetPosition(container).Y;
        const double offset = 35;

        if (verticalPos < tolerance)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
        }

        else if (verticalPos > container.ActualHeight - tolerance)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
        }
    }

    static void Unsubscribe(FrameworkElement container)
    {
        container.PreviewDragOver -= OnContainerPreviewDragOver;
    }

    public static T GetFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }

                T childItem = GetFirstVisualChild<T>(child);

                if (childItem != null)
                {
                    return childItem;
                }
            }
        }

        return null;
    }

    #endregion
}