using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows;

namespace WordKiller;

public class FileAssociation
{
    const int SHCNE_ASSOCCHANGED = 0x8000000;
    const uint SHCNF_IDLIST = 0x0U;
    public static bool IsRunAsAdmin()
    {
        WindowsIdentity id = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new(id);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void Associate(string description)
    {
        string productName = Application.ResourceAssembly.GetName().Name ?? "Wordkiller";
        Registry.ClassesRoot.CreateSubKey(Properties.Settings.Default.Extension).SetValue("", productName);

        if (Application.ResourceAssembly.GetName().Name != null && productName.Length > 0)
        {
            using RegistryKey key = Registry.ClassesRoot.CreateSubKey(productName);
            if (description != null)
                key.SetValue("", description);

            key.CreateSubKey(@"Shell\Open\Command").SetValue("", ToShortPathName(Environment.ProcessPath) + " \"%1\"");
        }
        SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
    }

    public static bool IsAssociated
    {
        get { return (Registry.ClassesRoot.OpenSubKey(Properties.Settings.Default.Extension, false) != null); }
    }

    public static void Remove()
    {
        string productName = Application.ResourceAssembly.GetName().Name ?? "Wordkiller";
        Registry.ClassesRoot.DeleteSubKeyTree(Properties.Settings.Default.Extension);
        Registry.ClassesRoot.DeleteSubKeyTree(productName);
    }

    [DllImport("shell32.dll", SetLastError = true)]
    static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

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
