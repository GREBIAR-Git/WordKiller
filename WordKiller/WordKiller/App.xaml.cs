using System.Windows;

namespace WordKiller;

public partial class App : Application
{
    void App_Startup(object sender, StartupEventArgs e)
    {
        if (e.Args.Length > 0 && FileAssociation.IsRunAsAdmin())
        {
            if (e.Args[0] == "FileAssociation")
            {
                FileAssociation.Associate("WordKiller", null);
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
}
