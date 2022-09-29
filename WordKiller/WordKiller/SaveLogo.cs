using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace WordKiller
{
    class SaveLogo
    {
        readonly DispatcherTimer saveTimer;
        Image logo;

        public SaveLogo(Image logo)
        {
            saveTimer = new();
            saveTimer.Tick += new EventHandler(Hide);
            saveTimer.Interval = new TimeSpan(0, 0, 2);
            saveTimer.Start();
            this.logo = logo;
        }

        public void Show()
        {
            logo.Visibility = Visibility.Visible;
            saveTimer.Stop();
            saveTimer.Start();
        }

        void Hide(object source, EventArgs e)
        {
            logo.Visibility = Visibility.Collapsed;
            saveTimer.Stop();
        }
    }
}
