using System.Windows;
using WordKiller.Scripts;
using WordKiller.Scripts.ForUI;

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
        UIHelper.SelectCulture(WordKiller.Properties.Settings.Default.Language);
        MainWindow mainWindow = new(e.Args);
        mainWindow.Show();
    }
}

