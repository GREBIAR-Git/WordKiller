using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WordKiller.Scripts.ForUI;

public class UIHelper
{
    public static void ShowError(string number)
    {
        MessageBox.Show(FindResourse("Error" + number), FindResourse("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
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

        if (String.IsNullOrEmpty(culture))
            return;

        var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

        string requestedCulture = string.Format("Resources/Dictionary/StringResources.{0}.xaml", culture);
        var resourceDictionary = dictionaryList.
            FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

        if (resourceDictionary == null)
        {
            requestedCulture = "StringResources.xaml";
            resourceDictionary = dictionaryList.
                FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
        }

        if (resourceDictionary != null)
        {
            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

    }

    public static BitmapImage GetImage(string imageUri)
    {
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri("pack://siteoforigin:,,,/" + imageUri, UriKind.RelativeOrAbsolute);
        bitmapImage.EndInit();
        return bitmapImage;
    }

    public static TreeViewItem GetNearestContainer(UIElement element)
    {
        TreeViewItem? container = element as TreeViewItem;
        while ((container == null) && (element != null))
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

    public static void WindowClose()
    {
        Application.Current.Shutdown();
    }
}
