using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using WordKiller.Properties;
using WordKiller.Scripts;
using WordKiller.ViewModels;

namespace WordKiller;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        Association(e.Args);
        UIHelper.SelectCulture(Settings.Default.Language);

        string[] args = e.Args;
        //args = new string[] { Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\1.wkr" };

        MainWindow mainWindow = new(await OpenFile(args));
        mainWindow.Show();
    }

    static async Task<ViewModelDocument> OpenFile(IReadOnlyList<string> args)
    {
        ViewModelDocument document = new();
        if (args.Count > 0)
        {
            if (args[0].EndsWith(Settings.Default.Extension) && File.Exists(args[0]))
            {
                await document.OpenAsync(args[0]);
            }
            else
            {
                UIHelper.ShowError("1");
            }
        }

        return document;
    }

    static void Association(IReadOnlyList<string> args)
    {
        if (args.Count > 0 && FileAssociation.IsRunAsAdmin())
        {
            if (args[0] == "FileAssociation")
            {
                FileAssociation.Associate("WordKiller");
                Environment.Exit(0);
            }
            else if (args[0] == "RemoveFileAssociation")
            {
                FileAssociation.Remove();
                Environment.Exit(0);
            }
        }
    }
}