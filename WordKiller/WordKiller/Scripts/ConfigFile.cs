using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace WordKiller.Scripts;

static class ConfigFile
{
    public static void Open()
    {
        if (ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).HasFile)
        {
            Process.Start("explorer.exe", Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath));
        }
    }

    public static void Delete()
    {
        if (ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).HasFile)
        {
            FileInfo fi = new(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);
            fi.Delete();
        }
    }

    public static void DeleteAll()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WordKiller";
        if (Directory.Exists(path))
        {
            DirectoryInfo di = new(path);
            di.Delete(true);
        }
    }
}
