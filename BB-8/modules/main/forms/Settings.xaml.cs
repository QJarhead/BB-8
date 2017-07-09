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
using System.Windows.Shapes;

namespace BB_8
{
    ///     Description:            Die Benutzereinstellungen
    ///     Namespace:              BB_8
    ///     Class:                  Settings
    ///     Author:                 Tim Christoph Lid                    
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       08.07.2017      Erstellt
    ///     Tim Christoph Lid       08.07.2017      Kommentare und Header hinzugefügt
    ///     
    public partial class Settings : Window
    {
        /// <summary>
        /// Erstellt die Klasse
        /// </summary>
        public Settings()
        {
            ///this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            ReadProperties();
        }

        /// <summary>
        /// Ließt die Einstellungen aus, und übergibt Sie in die Form
        /// </summary>
        private void ReadProperties()
        {
            Properties_Callmanager_IPAddress.Text = Properties.Settings.Default.CallmanagerIPAddress;
            Properties_Callmanager_Username.Text = Properties.Settings.Default.CallmanagerUsername;
            Properties_Callmanager_Password.Password = Properties.Settings.Default.CallmanagerPassword;
        }

        /// <summary>
        /// Schreibt aus der Form die Einstellungen in die lokalen Benutzerdateien
        /// </summary>
        private void WriteProperties()
        {
            Properties.Settings.Default.CallmanagerIPAddress = Properties_Callmanager_IPAddress.Text;
            Properties.Settings.Default.CallmanagerUsername = Properties_Callmanager_Username.Text;
            Properties.Settings.Default.CallmanagerPassword = Properties_Callmanager_Password.Password;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Setzt alle Grids auf Visible
        /// </summary>
        private void SetAllVisible()
        {
            Properties_Callmanager_Grid.Visibility = Visibility.Hidden;
            Properties_IPPhones_Grid.Visibility = Visibility.Hidden;
            Properties_Layout_Grid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Callmanager Treeview-Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Properties_Callmanager_TreeView_Selected(object sender, RoutedEventArgs e)
        {
            SetAllVisible();
            Properties_Callmanager_Grid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// IPPhone TreeView-Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Properties_IPPhone_TreeView_Selected(object sender, RoutedEventArgs e)
        {
            SetAllVisible();
            Properties_IPPhones_Grid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Layout TreeView-Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Properties_Layout_TreeView_Selected(object sender, RoutedEventArgs e)
        {
            SetAllVisible();
            Properties_Layout_Grid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Button für die Speicherung der Einstellungen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Properties_Submit_Click(object sender, RoutedEventArgs e)
        {
            WriteProperties();
        }
    }
}
