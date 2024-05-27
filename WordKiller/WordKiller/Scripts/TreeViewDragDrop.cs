using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.ParagraphData.Paragraphs;
using WordKiller.ViewModels;

namespace WordKiller.Scripts;

internal static class TreeViewDragDrop
{
    static IParagraphData? target;

    public static void MouseMove(MouseEventArgs e, ViewModelDocument document, TreeView treeView)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            IParagraphData? drag = document.Selected;
            if (drag is not null && drag is not ParagraphTitle && drag is not ParagraphTaskSheet &&
                drag is not ParagraphListOfReferences && drag is not ParagraphAppendix)
            {
                DragDropEffects finalDropEffect =
                    DragDrop.DoDragDrop(treeView, new DragDropInfo(drag), DragDropEffects.Move);
                if (finalDropEffect == DragDropEffects.Move && target != null)
                {
                    document.DragDrop(drag, target);
                    target = null;
                }
            }
        }
    }

    public static void DragOver(DragEventArgs e)
    {
        TreeViewItem TargetItem = UIHelper.GetNearestContainer(e.OriginalSource as UIElement);
        DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
        if (dragDropInfo == null || TargetItem.Header == dragDropInfo.ParagraphData ||
            dragDropInfo.ParagraphData is ParagraphTitle || dragDropInfo.ParagraphData is ParagraphTaskSheet ||
            dragDropInfo.ParagraphData is ParagraphListOfReferences ||
            dragDropInfo.ParagraphData is ParagraphAppendix ||
            TargetItem.Header is ParagraphTitle || TargetItem.Header is ParagraphTaskSheet ||
            TargetItem.Header is ParagraphListOfReferences || TargetItem.Header is ParagraphAppendix)
        {
            e.Effects = DragDropEffects.None;
        }
        else
        {
            e.Effects = DragDropEffects.Move;
        }

        e.Handled = true;
    }

    public static void Drop(DragEventArgs e)
    {
        try
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            TreeViewItem TargetItem = UIHelper.GetNearestContainer(e.OriginalSource as UIElement);

            DragDropInfo dragDropInfo = (DragDropInfo)e.Data.GetData(typeof(DragDropInfo));
            if (TargetItem != null && dragDropInfo != null)
            {
                target = (IParagraphData)TargetItem.Header;
                e.Effects = DragDropEffects.Move;
            }
        }
        catch (Exception)
        {
        }
    }
}