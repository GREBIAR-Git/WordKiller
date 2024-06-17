using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WordKiller.XAMLHelper
{
    public class MainWindowSaveStateBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += Window_Closing;
            LoadSettings();
        }

        bool _initialized = false;

        private void Window_Closing(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            if (Properties.Settings.Default == null) return;

            AssociatedObject.Width = Properties.Settings.Default.Width;
            AssociatedObject.Height = Properties.Settings.Default.Height;
            AssociatedObject.WindowState = Properties.Settings.Default.WindowState;
            _initialized = true;
        }

        private void SaveSettings()
        {
            if (Properties.Settings.Default == null || !_initialized) return;

            if(AssociatedObject.WindowState != WindowState.Minimized)
            {
                Properties.Settings.Default.WindowState = AssociatedObject.WindowState;
                if (AssociatedObject.WindowState != WindowState.Maximized)
                {
                    Properties.Settings.Default.Width = (int)AssociatedObject.Width;
                    Properties.Settings.Default.Height = (int)AssociatedObject.Height;
                }
            }
            Properties.Settings.Default.Save();
        }
    }
}
