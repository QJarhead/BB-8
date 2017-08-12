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
            SetAllVisible();
            Properties_Callmanager_Grid.Visibility = Visibility.Visible;
            ReadProperties();
        }

        /// <summary>
        /// Ließt die Einstellungen aus, und übergibt Sie in die Form
        /// </summary>
        private void ReadProperties()
        {
            Properties_Callmanager_IPAddress.Text = Properties.Settings.Default.CallmanagerIPAddress;
            Properties_Callmanager_UCUsername.Text = Properties.Settings.Default.CallmanagerUCUsername;
            Properties_Callmanager_UCPassword.Password = Properties.Settings.Default.CallmanagerUCPassword;
            Properties_Callmanager_OSUsername.Text = Properties.Settings.Default.CallmanagerOSUsername;
            Properties_Callmanager_OSPassword.Password = Properties.Settings.Default.CallmanagerOSPassword;
            Properties_IPPhone_Username.Text = Properties.Settings.Default.IPPhoneUsername;
            Properties_IPPhone_Password.Password = Properties.Settings.Default.IPPhonePassword;
            Properties_netTerrain_DBSource.Text = Properties.Settings.Default.netTerrainDatabaseIPAddress;
            Properties_netTerrain_DBName.Text = Properties.Settings.Default.netTerrainDatabaseName;
            Properties_netTerrain_DBUsername.Text = Properties.Settings.Default.netTerrainDatabaseUser;
            Properties_netTerrain_DBPassword.Password = Properties.Settings.Default.netTerrainDatabasePassword;
            Properties_netTerrain_Password.Password = Properties.Settings.Default.netTerrainPassword;
            Properties_netTerrain_Username.Text = Properties.Settings.Default.netTerrainUsername;
        }

        /// <summary>
        /// Schreibt aus der Form die Einstellungen in die lokalen Benutzerdateien
        /// </summary>
        private void WriteProperties()
        {
            Properties.Settings.Default.CallmanagerIPAddress = Properties_Callmanager_IPAddress.Text;
            Properties.Settings.Default.CallmanagerUCUsername = Properties_Callmanager_UCUsername.Text;
            Properties.Settings.Default.CallmanagerUCPassword = Properties_Callmanager_UCPassword.Password;
            Properties.Settings.Default.CallmanagerOSUsername = Properties_Callmanager_OSUsername.Text;
            Properties.Settings.Default.CallmanagerOSPassword = Properties_Callmanager_OSPassword.Password;
            Properties.Settings.Default.IPPhoneUsername = Properties_IPPhone_Username.Text;
            Properties.Settings.Default.IPPhonePassword = Properties_IPPhone_Password.Password;
            Properties.Settings.Default.netTerrainDatabaseIPAddress = Properties_netTerrain_DBSource.Text;
            Properties.Settings.Default.netTerrainDatabaseName = Properties_netTerrain_DBName.Text;
            Properties.Settings.Default.netTerrainDatabaseUser = Properties_netTerrain_DBUsername.Text;
            Properties.Settings.Default.netTerrainDatabasePassword = Properties_netTerrain_DBPassword.Password;
            Properties.Settings.Default.netTerrainUsername = Properties_netTerrain_Username.Text;
            Properties.Settings.Default.netTerrainPassword = Properties_netTerrain_Password.Password;
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
            Properties_netTerrain_Grid.Visibility = Visibility.Hidden;
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

        private void Properties_netTerrain_TreeView_Selected(object sender, RoutedEventArgs e)
        {
            SetAllVisible();
            Properties_netTerrain_Grid.Visibility = Visibility.Visible;
        }
    }
}
