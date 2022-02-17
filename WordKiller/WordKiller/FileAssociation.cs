using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows;

namespace WordKiller
{
    public class FileAssociation
    {
        const long SHCNE_ASSOCCHANGED = 0x8000000L;
        const uint SHCNF_IDLIST = 0x0U;
        public static bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void Associate(string description, string icon)
        {

            Registry.ClassesRoot.CreateSubKey(Config.extension).SetValue("", Application.ResourceAssembly.GetName().Name);

            if (Application.ResourceAssembly.GetName().Name != null && Application.ResourceAssembly.GetName().Name.Length > 0)
            {
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(Application.ResourceAssembly.GetName().Name))
                {
                    if (description != null)
                        key.SetValue("", description);

                    if (icon != null)
                        key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));

                    key.CreateSubKey(@"Shell\Open\Command").SetValue("", ToShortPathName(System.Reflection.Assembly.GetExecutingAssembly().Location) + " \"%1\"");
                }
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

        [DllImport("Kernel32.dll")]
        static extern uint GetShortPathName(string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

        static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
    }
}