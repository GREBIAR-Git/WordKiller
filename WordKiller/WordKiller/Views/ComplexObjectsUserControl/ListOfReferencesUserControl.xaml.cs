using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordKiller.Views.ComplexObjectsUserControl
{
    /// <summary>
    /// Interaction logic for ListOfReferencesUserControl.xaml
    /// </summary>
    public partial class ListOfReferencesUserControl : UserControl
    {
        public ListOfReferencesUserControl()
        {
            InitializeComponent();
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            books.Items.Refresh();
            electronicResources.Items.Refresh();
        }
    }
}
