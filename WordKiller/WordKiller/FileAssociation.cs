using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows;

namespace WordKiller;

public class FileAssociation
{
    const long SHCNE_ASSOCCHANGED = 0x8000000L;
    const uint SHCNF_IDLIST = 0x0U;
    public static bool IsRunAsAdmin()
    {
        WindowsIdentity id = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new(id);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void Associate(string description)
    {
        string appName = Application.ResourceAssembly.GetName().Name;
        Registry.ClassesRoot.CreateSubKey(Config.extension).SetValue("", appName);

        if (Application.ResourceAssembly.GetName().Name != null && appName.Length > 0)
        {
            using RegistryKey key = Registry.ClassesRoot.CreateSubKey(appName);
            if (description != null)
                key.SetValue("", description);

            key.CreateSubKey(@"Shell\Open\Command").SetValue("", ToShortPathName(Environment.ProcessPath) + " \"%1\"");
        }
        SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
    }

    public static void Remove()
    {
        Registry.ClassesRoot.DeleteSubKeyTree(Config.extension);
        Registry.ClassesRoot.DeleteSubKeyTree(Application.ResourceAssembly.GetName().Name);
    }

    [DllImport("shell32.dll", SetLastError = true)]
    static extern void SHChangeNotify(long wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
    static extern uint GetShortPathName(string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

    static string ToShortPathName(string longName)
    {
        StringBuilder s = new(1000);
        uint iSize = (uint)s.Capacity;
        _ = GetShortPathName(longName, s, iSize);
        return s.ToString();
    }
}
