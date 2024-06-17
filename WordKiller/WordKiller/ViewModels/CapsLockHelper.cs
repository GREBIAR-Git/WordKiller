using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordKiller.ViewModels
{
    internal static class CapsLockHelper
    {
        public delegate void MethodContainer();

        public static event MethodContainer Change;

        static bool capsLockH1;

        public static bool CapsLockH1
        {
            get => capsLockH1;
            set
            {
                capsLockH1 = value;
                Change();
            }
        }

        static bool capsLockH2;

        public static bool CapsLockH2
        {
            get => capsLockH2;
            set
            {
                capsLockH2 = value;
                Change();
            }
        }

        public static string ToCapsLockH1(string value)
        {
            return ToCapsLock(value, CapsLockH1);
        }

        public static string ToCapsLockH2(string value)
        {
            return ToCapsLock(value, CapsLockH2);
        }

        static string ToCapsLock(string value, bool isCapsLock)
        {
            if (isCapsLock)
            {
                value = value.ToUpper();
            }
            return value;
        }
    }
}
