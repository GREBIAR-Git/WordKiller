using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using WordKiller.Scripts;

namespace WordKiller;

public partial class App : Application
{
    void App_Startup(object sender, StartupEventArgs e)
    {
        if (e.Args.Length > 0 && FileAssociation.IsRunAsAdmin())
        {
            if (e.Args[0] == "FileAssociation")
            {
                FileAssociation.Associate("WordKiller");
                System.Environment.Exit(0);
            }
            else if (e.Args[0] == "RemoveFileAssociation")
            {
                FileAssociation.Remove();
                System.Environment.Exit(0);
            }
        }
        MainWindow mainWindow = new(e.Args);
        SelectCulture(WordKiller.Properties.Settings.Default.Language);
        mainWindow.Show();
    }

    public static void SelectCulture(int culture)
    {
        if (culture == 0)
        {
            SelectCulture("ru");
        }
        else if (culture == 1)
        {
            SelectCulture("en");
        }
        else if (culture == 2)
        {
            SelectCulture("be");
        }
        else if (culture == 3)
        {
            SelectCulture("fr");
        }
        else if (culture == 4)
        {
            SelectCulture("de");
        }
        else if (culture == 5)
        {
            SelectCulture("zh");
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
}

