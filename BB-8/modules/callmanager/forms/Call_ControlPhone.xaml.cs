using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
    ///     Description:            Klasse für die Telefonsteuerung
    ///     Namespace:              BB_8
    ///     Class:                  Call_ControlPhone
    ///     Author:                 Tim Christoph Lid                    
    ///     Notes:                  
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       02.07.2017      Erstellt
    ///     Tim Christoph Lid       06.07.2017      Hinzufügen der Buttons des Keypads
    ///     Tim Christoph Lid       09.07.2017      Kommentare
    ///     Tim Christoph Lid       28.07.2017      Screenshot vom Telefon angepasst, jetzt wird er korrekt angezeigt
    /// 
    public partial class Call_ControlPhone : Window
    {
        BeeBeeAte beeBeeAte;
        public Call_ControlPhone(BeeBeeAte beeBeeAte)
        {
            InitializeComponent();
            TestTextbox.Text = Properties.Settings.Default.CallmanagerIPAddress;
            this.beeBeeAte = beeBeeAte;
            this.Close();
        }

        /// <summary>
        /// Verwaltet alle Buttons auf dem Telefon, die verschiedenen Functionen werden nach drücken des Buttons ausgeführt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_ControlPhone_Phone_ButtonField(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Name.ToString();
            switch (content)
            {
                case "Network_Callmanager_ControlPhone_Phone_Button_1":
                    PressKeyPad(1);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_2":
                    PressKeyPad(2);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_3":
                    PressKeyPad(3);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_4":
                    PressKeyPad(4);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_5":
                    PressKeyPad(5);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_6":
                    PressKeyPad(6);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_7":
                    PressKeyPad(7);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_8":
                    PressKeyPad(8);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_9":
                    PressKeyPad(9);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_10":
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_11":
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_12":
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_Soft_1":
                    PressSoftKey(1);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_Soft_2":
                    PressSoftKey(2);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_Soft_3":
                    PressSoftKey(3);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_Soft_4":
                    PressSoftKey(4);
                    break;
                case "Network_Callmanager_ControlPhone_Phone_Button_Soft_5":
                    PressSoftKey(5);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Setzen des Overlays, wenn die Maus über dem Button ist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_ControlPhone_Phone_Button_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Button).Opacity = 0.2;
        }

        /// <summary>
        /// Press Keypad on Cisco Phone
        /// </summary>
        /// <param name="item"></param>
        public void PressKeyPad(int item)
        {
            Execute("<CiscoIPPhoneExecute><ExecuteItem URL = 'Key:KeyPad" + item + "'/></CiscoIPPhoneExecute>");
        }

        /// <summary>
        /// Press Softkey on Cisco Phone
        /// </summary>
        /// <param name="item"></param>
        public void PressSoftKey(int item)
        {
            Execute("<CiscoIPPhoneExecute><ExecuteItem URL = 'Key:Soft" + item + "'/></CiscoIPPhoneExecute>");
        }

        /// <summary>
        /// Press Line on Cisco Phone
        /// </summary>
        /// <param name="item"></param>
        public void PressLine(int item)
        {
            Execute("<CiscoIPPhoneExecute><ExecuteItem URL = 'Key:Line" + item + "'/></CiscoIPPhoneExecute>");
        }

        /// <summary>
        /// Schreibt den Text aus der Textbox auf das Telefno
        /// </summary>
        /// <param name="text"></param>
        public void WriteTexttoPhone(String text)
        {
            Execute("<CiscoIPPhoneText><Prompt>Message</Prompt><Text>" + text + "</Text></CiscoIPPhoneText>");
        }

        /// <summary>
        /// Führt die verschiedenen Befehle auf dem Telefon aus (in xml Format) / per POST WebRequest
        /// </summary>
        /// <param name="xml"></param>
        public void Execute(String xml)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + Network_Callmanager_ControlPhone_IPPHone.Text + "/CGI/Execute");
            webRequest.Method = "POST";
            webRequest.Credentials = new NetworkCredential(BB_8.Properties.Settings.Default.IPPhoneUsername, BB_8.Properties.Settings.Default.IPPhonePassword);
            byte[] byteArray = Encoding.UTF8.GetBytes(@"XML=" + xml);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            StreamReader reader;
            HttpWebResponse response;
            String responseString;
            using (response = (HttpWebResponse)webRequest.GetResponse())
            using (reader = new StreamReader(response.GetResponseStream()))
            {
                responseString = reader.ReadToEnd();
            }
            TestTextbox.Text = responseString;
            ShowScreenshot(Network_Callmanager_ControlPhone_IPPHone.Text);
        }

        /// <summary>
        /// Button für die Telefonnachricht
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_ControlPhone_TextToSend_Button_Click(object sender, RoutedEventArgs e)
        {
            WriteTexttoPhone(Network_Callmanager_ControlPhone_TextToSend.Text);
        }

        /// <summary>
        /// Zeigt den Screenshot an
        /// </summary>
        /// <param name="ipAdress"></param>
        private void ShowScreenshot(String ipAdress)
        {
            string authHdr = "Authorization: Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("" + Properties.Settings.Default.IPPhoneUsername + ":" + Properties.Settings.Default.IPPhonePassword)) + "\r\n";
            Network_Callmanager_ControlPhone_Screen.Navigate("http://" + ipAdress + "/CGI/Screenshot", null, null, authHdr);
            TestTextbox.Text = "http://" + ipAdress + "/CGI/Screenshot";
        }

        /// <summary>
        /// Ändern das Layout des Telefons, für die verschiedenen größen der unterschiedlichen Telefontypen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_ControlPhone_Screen_LoadCompleted_1(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (((int)(this.Network_Callmanager_ControlPhone_Screen.Document as dynamic).body.scrollHeight + 20) > 100)
            {
                String Zoom = "0,68";
                mshtml.IHTMLDocument2 doc = Network_Callmanager_ControlPhone_Screen.Document as mshtml.IHTMLDocument2;
                doc.parentWindow.execScript("document.body.style.zoom=" + Zoom.ToString().Replace(",", ".") + ";");
                doc.parentWindow.execScript("document.body.style.overflow = 'hidden'");
            }
            Network_Callmanager_ControlPhone_Screen.Visibility = Visibility.Visible;
        }
    }
}
