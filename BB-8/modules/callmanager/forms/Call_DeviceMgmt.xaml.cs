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
using Renci.SshNet;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using System.Data;

namespace BB_8
{
    ///     Description:            Klasse für die Callmanager Geräte-Verwaltung
    ///     Namespace:              BB_8
    ///     Class:                  Call_DeviceMgmt
    ///     Author:                 Tim Christoph Lid                    
    ///     Notes:                  Genutzt nur von Callmanager Geräte-Verwaltung
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       24.06.2017      Erstellt
    ///     Tim Christoph Lid       25.05.2017      Hinzufügen des Screenhots vom Telefon
    ///     Tim Christoph Lid       26.06.2017      Umgestellt, nun DirectoryNumber; DeviceName und IP-Adresse in der anzeige
    ///     Tim Christoph Lid       09.07.2017      Umbenennen der Methoden (großbuchstaben)
    ///     
    public partial class Call_DeviceMgmt : Window
    {
        BeeBeeAte beeBeeAte;
        List<DataGrid_Device_Entry> list;
        ComboBox filter = new ComboBox();
        string IPAdress = "";

        /// <summary>
        /// Klasse für Callmanager Analyse, wird direkt geschlossen, nur das Grid wird übernommen und dem Mainwindow hinzugefügt
        /// </summary>
        public Call_DeviceMgmt(BeeBeeAte beeBeeAte)
        {
            this.beeBeeAte = beeBeeAte;
            InitializeComponent();
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen_Close.Visibility = Visibility.Hidden;
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen.Visibility = Visibility.Hidden;
            Close();
        }

        /// <summary>
        /// Button zum suchen der Telefone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_DeviceMgmt_Filter_SearchIP_Click(object sender, RoutedEventArgs e)
        {
            SearchPhone();
        }

        /// <summary>
        /// Für die Skalierung der Größe, unterschied zwischen 79XX und 88XX wegen der unterschiedlichen Auflösungen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_DeviceMgmt_Filter_ShowScreen_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

            if (((int)(this.Network_Callmanager_DeviceMgmt_Filter_ShowScreen.Document as dynamic).body.scrollHeight + 20) > 500)
            {
                String Zoom = "0,5";
                mshtml.IHTMLDocument2 doc = Network_Callmanager_DeviceMgmt_Filter_ShowScreen.Document as mshtml.IHTMLDocument2;
                doc.parentWindow.execScript("document.body.style.zoom=" + Zoom.ToString().Replace(",", ".") + ";");
            }
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen.Visibility = Visibility.Visible;
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen_Close.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Zeigt den Screenshot an
        /// </summary>
        /// <param name="ipAdress"></param>
        private void ShowScreenshot(String ipAdress)
        {
            string authHdr = "Authorization: Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("BB-8:mSMHaEfyI6PGYG0XCltD")) + "\r\n";
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen.Navigate("http://" + ipAdress + "/CGI/Screenshot", null, null, authHdr);
        }

        /// <summary>
        /// Button zum beenden des "ShowScreenshots
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_DeviceMgmt_Filter_ShowScreen_Close_Click(object sender, RoutedEventArgs e)
        {
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen.Visibility = Visibility.Hidden;
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen_Close.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Context-Menu "ShowScreenshot"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_DeviceMgmt_Output_Data_ContextMenu_ShowScreenshot_Click(object sender, System.EventArgs e)
        {
            foreach (DataGrid_Device_Entry row in Network_Callmanager_DeviceMgmt_Output_Data.SelectedItems)
            {
                IPAdress = row.IPAddress;
            }
            ShowScreenshot(IPAdress);
        }

        /// <summary>
        /// Enter-Taste in Textfeld
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_DeviceMgmt_Filter_Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SearchPhone();
            }
        }

        /// <summary>
        /// Gibt alle Telefone in der Liste aus
        /// </summary>
        public void SearchPhone()
        {
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen_Close.Visibility = Visibility.Hidden;
            Network_Callmanager_DeviceMgmt_Filter_ShowScreen.Visibility = Visibility.Hidden;
            String prefix = "";
            String suffix = "";
            String regEx = "*";
            if (Network_Callmanager_DeviceMgmt_Filter_Choice.SelectedIndex != 0)
            {
                regEx = "%";
            }

            switch (Network_Callmanager_DeviceMgmt_Filter_State.SelectedIndex)
            {
                case 0:
                    suffix = regEx;
                    prefix = regEx;
                    break;
                case 1:
                    prefix = regEx;
                    break;
                case 2:
                    suffix = regEx;
                    break;
                default:
                    break;
            }

            switch (Network_Callmanager_DeviceMgmt_Filter_Choice.SelectedIndex)
            {
                case 0:
                    SetOutputByPhones(suffix + Network_Callmanager_DeviceMgmt_Filter_Text.Text + prefix);
                    break;
                case 5:
                    SetOutputByUser(suffix + Network_Callmanager_DeviceMgmt_Filter_Text.Text + prefix, "%", "%");
                    break;
                case 2:
                    SetOutputByUser("%", suffix + Network_Callmanager_DeviceMgmt_Filter_Text.Text + prefix, "%");
                    break;
                case 1:
                    SetOutputByUser("%", "%", suffix + Network_Callmanager_DeviceMgmt_Filter_Text.Text + prefix);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Setzt die informationen in die GridView anhand der Telefone
        /// </summary>
        /// <param name="FilterText"></param>
        public void SetOutputByPhones(String FilterText)
        {
            getUser getUser = new getUser();
            getPhone getPhone = new getPhone();
            getPhone.setDirectoryNumber(FilterText);
            List<Phone> phones = getPhone.getPhones();

            list = new List<DataGrid_Device_Entry>();

            foreach (Phone phone in phones)
            {
                String lastName = getUser.getSingleUser(phone.line1.ToString()).LastName;
                String firstName = getUser.getSingleUser(phone.line1.ToString()).FirstName;

                list.Add(new DataGrid_Device_Entry() { UserID = phone.line1, firstName = firstName, lastName = lastName, IPAddress = phone.ipAadress, DeviceName = phone.name });
            }

            Network_Callmanager_DeviceMgmt_Output_Data.ItemsSource = list;

        }

        /// <summary>
        /// Setzt die Informationen in die GridView anhand der User
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        public void SetOutputByUser(String userID, String lastName, String firstName)
        {

            getUser getUser = new getUser();
            getPhone getPhone = new getPhone();
            getUser.setUserID(userID);
            getUser.setfirstName(firstName);
            getUser.setlastName(lastName);
            List<User> users = getUser.getUsers();
            list = new List<DataGrid_Device_Entry>();

            foreach (User user in users)
            {
                String name = getPhone.getSinglePhone(user.UserID.ToString()).name;
                String line1 = getPhone.getSinglePhone(user.UserID.ToString()).line1;
                String ipAdress = getPhone.getSinglePhone(user.UserID.ToString()).ipAadress;

                list.Add(new DataGrid_Device_Entry() { UserID = user.UserID, firstName = user.FirstName, lastName = user.LastName, IPAddress = ipAdress, DeviceName = name });
            }

            Network_Callmanager_DeviceMgmt_Output_Data.ItemsSource = list;
        }

        private void Network_Callmanager_DeviceMgmt_Output_Data_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (DataGrid_Device_Entry row in Network_Callmanager_DeviceMgmt_Output_Data.SelectedItems)
            {
                IPAdress = row.IPAddress;
            }

            ShowScreenshot(IPAdress);
        }
    }
}
