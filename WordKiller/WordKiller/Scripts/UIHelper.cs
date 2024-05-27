using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WordKiller.Properties;
using WordKiller.XAMLHelper;

namespace WordKiller.Scripts;

internal class UIHelper
{
    public static void ShowError(string number)
    {
        MessageBox.Show(FindResourse("Error" + number), FindResourse("Error"), MessageBoxButton.OK,
            MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
    }

    public static string FindResourse(string key)
    {
        return Application.Current.FindResource(key) as string;
    }

    public static void SelectCulture(int culture)
    {
        string name = string.Empty;
        if (culture == 0)
        {
            name = "ru";
        }
        else if (culture == 1)
        {
            name = "en";
        }
        else if (culture == 2)
        {
            name = "be";
        }
        else if (culture == 3)
        {
            name = "fr";
        }
        else if (culture == 4)
        {
            name = "de";
        }
        else if (culture == 5)
        {
            name = "zh";
        }

        if (Thread.CurrentThread.CurrentCulture.Name != name)
        {
            SelectCulture(name);
        }
    }

    static void SelectCulture(string culture)
    {
        if (string.IsNullOrEmpty(culture))
            return;

        var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

        string requestedCulture = string.Format("Resources/Dictionary/StringResources.{0}.xaml", culture);
        var resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

        if (resourceDictionary == null)
        {
            requestedCulture = "StringResources.xaml";
            resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
        }

        if (resourceDictionary != null)
        {
            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        Thread.CurrentThread.CurrentCulture = new(culture);
        Thread.CurrentThread.CurrentUICulture = new(culture);
    }

    public static BitmapImage GetImage(string imageUri)
    {
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new("pack://siteoforigin:,,,/" + imageUri, UriKind.RelativeOrAbsolute);
        bitmapImage.EndInit();
        return bitmapImage;
    }

    public static TreeViewItem GetNearestContainer(UIElement element)
    {
        TreeViewItem? container = element as TreeViewItem;
        while (container == null && element != null)
        {
            element = VisualTreeHelper.GetParent(element) as UIElement;
            container = element as TreeViewItem;
        }

        return container;
    }


    public static T FindChild<T>(DependencyObject parent, string childName)
        where T : DependencyObject
    {
        if (parent == null) return null;

        T foundChild = null;

        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is not T)
            {
                foundChild = FindChild<T>(child, childName);

                if (foundChild != null) break;
            }
            else if (!string.IsNullOrEmpty(childName))
            {
                if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                {
                    foundChild = (T)child;
                    break;
                }
            }
            else
            {
                foundChild = (T)child;
                break;
            }
        }

        return foundChild;
    }

    public static TreeViewItem? FindTviFromObjectRecursive(ItemsControl ic, object o)
    {
        //Search for the object model in first level children (recursively)
        if (ic.ItemContainerGenerator.ContainerFromItem(o) is TreeViewItem tvi) return tvi;
        //Loop through user object models
        foreach (object i in ic.Items)
        {
            //Get the TreeViewItem associated with the iterated object model
            TreeViewItem tvi2 = ic.ItemContainerGenerator.ContainerFromItem(i) as TreeViewItem;
            tvi = FindTviFromObjectRecursive(tvi2, o);
            if (tvi != null) return tvi;
        }

        return null;
    }

    public static TextPointer FindPointerAtTextOffset(TextPointer from, int offset, bool seekStart)
    {
        if (from == null)
            return null;

        TextPointer current = from;
        TextPointer end = from.DocumentEnd;
        int charsToGo = offset;

        while (current.CompareTo(end) != 0)
        {
            if (current.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text &&
                current.Parent is Run currentRun)
            {
                var remainingLengthInRun = current.GetOffsetToPosition(currentRun.ContentEnd);
                if (charsToGo < remainingLengthInRun ||
                    (charsToGo == remainingLengthInRun && !seekStart))
                    return current.GetPositionAtOffset(charsToGo);
                charsToGo -= remainingLengthInRun;
                current = currentRun.ElementEnd;
            }
            else
            {
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        if (charsToGo == 0 && !seekStart)
            return end;
        return null;
    }

    public static void SelectedWord(string s, int yy1, RTBox richTextBox)
    {
        int startposition = richTextBox.Text.IndexOf(s, yy1);
        int endposition = startposition + s.Length;


        var flowDocument = richTextBox.Document;
        TextPointer start = FindPointerAtTextOffset(
            flowDocument.ContentStart, startposition, true);
        if (start == null)
        {
            return;
        }

        TextPointer end = FindPointerAtTextOffset(
            start, endposition - startposition, false);
        if (end == null)
        {
            return;
        }

        richTextBox.Selection.Select(start, end);
    }

    public static void UnselectTreeViewItem(TreeView treeView)
    {
        if (treeView.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem item)
        {
            item.IsSelected = true;
            item.IsSelected = false;
        }
    }

    public static void TableValidation(TextCompositionEventArgs e, TextBox textBox, bool max = false)
    {
        Regex regex = new("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
        if (max)
        {
            if (!e.Handled)
            {
                textBox.SelectedText = e.Text;
                string text = textBox.Text;
                int beginningNumber = 0;
                foreach (char number in text)
                {
                    if (number == '0')
                    {
                        beginningNumber++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (beginningNumber > 0)
                {
                    text = text[beginningNumber..];
                    e.Handled = true;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    int count = int.Parse(text);
                    if (count > Settings.Default.MaxRowAndColumn)
                    {
                        count = Settings.Default.MaxRowAndColumn;
                        text = count.ToString();
                        e.Handled = true;
                    }
                }

                if (e.Handled)
                {
                    textBox.Text = text;
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }
    }

    public static void WindowClose()
    {
        Application.Current.Shutdown();
    }
}