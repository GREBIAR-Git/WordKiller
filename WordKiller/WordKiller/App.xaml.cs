using System.Globalization;
using System.Linq;
using System.Threading;
using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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

        //Copy all MergedDictionarys into a auxiliar list.
        var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

        //Search for the specified culture.     
        string requestedCulture = string.Format("Dictionary/StringResources.{0}.xaml", culture);
        var resourceDictionary = dictionaryList.
            FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

        if (resourceDictionary == null)
        {
            //If not found, select our default language.             
            requestedCulture = "StringResources.xaml";
            resourceDictionary = dictionaryList.
                FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
        }

        //If we have the requested resource, remove it from the list and place at the end.     
        //Then this language will be our string table to use.      
        if (resourceDictionary != null)
        {
            Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        //Inform the threads of the new culture.     
        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

    }
}

