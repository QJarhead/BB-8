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
using Custom_Listbox;
using Renci.SshNet;
using System.IO;
using BB_8.Properties;

namespace BB_8
{
    ///     Description:            Interaktionslogik für MainWindow.xaml
    ///     Namespace:              BB_8
    ///     Class:                  MainWindow
    ///     Author:                 Tim Christoph Lid                    
    ///     Notes:                  Genutzt nur von Callmanager_Report für die Filter-Anzeige
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       14.06.2017      Erstellt
    ///     Tim Christoph Lid       18.06.2017      Kommentare und Header hinzugefügt
    ///     Tim Christoph Lid       23.06.2017      Umstellen, dass die Verbindung des Callmanagers Global ist, um von anderen Klassen ebenfalls zugreifen zu können
    ///     
    public partial class BeeBeeAte : Window
    {
        List<Filter> FilterList = new List<Filter>();
        Call_Analysis callAnalysis;
        Call_Report callReport;
        Call_DeviceMgmt callDeviceMgmt;
        Call_ControlPhone callControlPhone;
        public CallmanagerConnection callmanagerConnection;
        public bool connectedToCallmanager = false;
        public Settings settings;
        public System.Windows.Window Form = Application.Current.MainWindow;

        public BeeBeeAte()
        {
            InitializeComponent();
            Network_Callmanager_DeviceMgmt_Treeview(null, null);
        }

        private void Network_Callmanager_Analysis_Treeview(object sender, MouseButtonEventArgs e)
        {
            if (!connectedToCallmanager)
            {
                InitCallmanagerConnection();
            }
            if (callAnalysis == null)
            {
                callAnalysis = new Call_Analysis(this);
                callAnalysis.BB_8_CallAnalysis_InitGrid.Children.RemoveAt(0);
                BB_8_MainGrid.Children.Add(callAnalysis.Network_Callmanager_Analysis_Grid);
                callAnalysis.Network_Callmanager_Analysis_Grid.Margin = new Thickness(210, 30, 10, 10);
                callAnalysis.Network_Callmanager_Analysis_Output_Clear.VerticalAlignment = VerticalAlignment.Bottom;
                callAnalysis.Network_Callmanager_Analysis_Output_Save.VerticalAlignment = VerticalAlignment.Bottom;
                callAnalysis.InitCallAnalysis();
            }
            SetAllVisible();
            callAnalysis.Network_Callmanager_Analysis_Grid.Visibility = Visibility.Visible;
        }

        private void Network_Callmanager_Report_Treeview(object sender, MouseButtonEventArgs e)
        {
            if (!connectedToCallmanager)
            {
                InitCallmanagerConnection();
            }
            if (callReport == null)
            {
                callReport = new Call_Report(this);
                callReport.BB_8_CallReport_InitGrid.Children.RemoveAt(0);
                BB_8_MainGrid.Children.Add(callReport.Network_Callmanager_Report_Grid);
                callReport.Network_Callmanager_Report_Grid.Margin = new Thickness(210, 30, 10, 10);
                callReport.Network_Callmanager_Report_Output_Clear.VerticalAlignment = VerticalAlignment.Bottom;
                callReport.Network_Callmanager_Report_Output_Save.VerticalAlignment = VerticalAlignment.Bottom;
                callReport.InitCallReport();
            }
            SetAllVisible();
            callReport.Network_Callmanager_Report_Grid.Visibility = Visibility.Visible;
        }

        private void InitCallmanagerConnection()
        {
            callmanagerConnection = new CallmanagerConnection();
        }

        private void Network_Callmanager_DeviceMgmt_Treeview(object sender, MouseButtonEventArgs e)
        {
            if (callDeviceMgmt == null)
            {
                callDeviceMgmt = new Call_DeviceMgmt(this);
                callDeviceMgmt.BB_8_CallDeviceMgmt_InitGrid.Children.RemoveAt(0);
                BB_8_MainGrid.Children.Add(callDeviceMgmt.Network_Callmanager_DeviceMgmt_Grid);
                callDeviceMgmt.Network_Callmanager_DeviceMgmt_Grid.Margin = new Thickness(210, 30, 10, 10);
            }
            SetAllVisible();
            callDeviceMgmt.Network_Callmanager_DeviceMgmt_Grid.Visibility = Visibility.Visible;
        }

        private void SetAllVisible()
        {
            if (callReport != null)
            {
                callReport.Network_Callmanager_Report_Grid.Visibility = Visibility.Hidden;
            }
            if (callAnalysis != null)
            {
                callAnalysis.Network_Callmanager_Analysis_Grid.Visibility = Visibility.Hidden;
            }
            if (callDeviceMgmt != null)
            {
                callDeviceMgmt.Network_Callmanager_DeviceMgmt_Grid.Visibility = Visibility.Hidden;
            }
            if (callControlPhone != null)
            {
                callControlPhone.Network_Callmanager_ControlPhone_Grid.Visibility = Visibility.Hidden;
            }
        }

        private void Network_Callmanager_ControlPhone_Treeview(object sender, MouseButtonEventArgs e)
        {
            if (callControlPhone == null)
            {
                callControlPhone = new Call_ControlPhone(this);
                callControlPhone.BB_8_CallControlPhone_InitGrid.Children.RemoveAt(0);
                BB_8_MainGrid.Children.Add(callControlPhone.Network_Callmanager_ControlPhone_Grid);
                callControlPhone.Network_Callmanager_ControlPhone_Grid.Margin = new Thickness(210, 30, 10, 10);
            }
            SetAllVisible();
            callControlPhone.Network_Callmanager_ControlPhone_Grid.Visibility = Visibility.Visible;
        }

        private void MainMenu_Tools_Properties_Click(object sender, RoutedEventArgs e)
        {
            settings = new Settings();
            settings.Show();
        }

        internal void ShowSettings()
        {
            settings = new Settings();
            settings.Show();
        }
    }
}
