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

namespace BB_8
{
    ///     Description:            Klasse für die Callmanager Analyse, zeigt Telefonate vom Callmanager an
    ///     Namespace:              BB_8
    ///     Class:                  Call_Report
    ///     Author:                 Tim Christoph Lid                    
    ///     Notes:                  Genutzt nur von Callmanager_Report für die Filter-Anzeige
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       16.06.2017      Erstellt
    ///     Tim Christoph Lid       18.06.2017      Kommentare und Header hinzugefügt
    ///     Tim Chriostph Lid       23.06.2017      Einrichtung der Button für die Date-Verwaltung
    ///     Tim Christoph Lid       23.06.2017      Umstellen, dass die Verbindung des Callmanagers Global ist, um von anderen Klassen ebenfalls zugreifen zu können
    ///     Tim Christoph Lid       02.07.2017      Erweitern der execute()-Methode, falls die ssh-Session verworfen wird, dass eine neue Aufgebaut wird (noch zu testen)
    ///     
    public partial class Call_Report : Window
    {
        BeeBeeAte beeBeeAte;
        List<Filter> FilterList = new List<Filter>();
        public System.Windows.Window Form = Application.Current.MainWindow;
        private System.ComponentModel.BackgroundWorker ConnectingToCallmanagerWorker = new BackgroundWorker();

        /// <summary>
        /// Klasse für Callmanager Analyse, wird direkt geschlossen, nur das Grid wird übernommen und dem Mainwindow hinzugefügt
        /// </summary>
        public Call_Report(BeeBeeAte beeBeeAte)
        {
            this.beeBeeAte = beeBeeAte;
            InitializeComponent();
            this.Close();
        }

        /// <summary>
        /// Gibt ein Datumsformat zurück, welches der Callmanager versteht (yyyy-MM-dd)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public String FormatDateForCallmanager(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Gibt alle ausgewählten Anzeigedaten wieder aus der ListView
        /// </summary>
        /// <returns></returns>
        private String GetSelections()
        {
            String displayItems = "";
            foreach (var item in Network_Callmanager_Report_Filter_Display.Items)
            {
                displayItems += item;
                if (item != Network_Callmanager_Report_Filter_Display.Items.GetItemAt(Network_Callmanager_Report_Filter_Display.Items.Count - 1))
                {
                    displayItems += ",";
                }
            }
            return displayItems;
        }

        /// <summary>
        /// Gibt den aktuellen SQL befehl für den Callmanager wieder
        /// </summary>
        /// <returns></returns>
        private String GetSQLStatement()
        {
            String Filter = "run sql car select " + GetSelections() + " from car:tbl_billing_data where datetimestamporigination between ";
            Filter += "'" + FormatDateForCallmanager(Network_Callmanager_Report_Filter_DateBegin.SelectedDate.Value) + " 00:00:00' and ";
            Filter += "'" + FormatDateForCallmanager(Network_Callmanager_Report_Filter_DateEnd.SelectedDate.Value) + " 23:59:59' and ";
            foreach (Filter filterStringArray in FilterList)
            {
                string[] filterArray = GetField0Translated(filterStringArray).Split(',');
                foreach (String filter in filterArray)
                {
                    if (filter == filterArray[0])
                    {
                        Filter += " ( ";
                    }
                    switch (filterStringArray.GetComboField1().Text)
                    {
                        case "Beginnt":
                            Filter += filter + " LIKE '" + filterStringArray.GetTextField0().Text + "%' ";
                            break;
                        case "Enthält":
                            Filter += filter + " LIKE '%" + filterStringArray.GetTextField0().Text + "%' ";
                            break;
                        case "Endet":
                            Filter += filter + " LIKE '%" + filterStringArray.GetTextField0().Text + "' ";
                            break;
                        case "Genau":
                            Filter += filter + " LIKE '" + filterStringArray.GetTextField0().Text + "' ";
                            break;
                        default:
                            Filter += "LIKE '%' ";
                            break;
                    }
                    if (filter != filterArray[filterArray.Length - 1])
                    {
                        Filter += " OR ";
                    }
                    else
                    {
                        Filter += " ) ";
                    }
                }
                if (filterStringArray != FilterList[FilterList.Count - 1])
                {
                    Filter += " and ";
                }
            }
            Filter += "ORDER BY datetimestamporigination";
            return Filter;
        }

        /// <summary>
        /// Gibt einen String in der TextBox aus, mit anschließender neuer Zeile für neue Einträge
        /// </summary
        /// <param name="output"></param>
        public void OutputLine(String output)
        {
            Dispatcher.BeginInvoke(new Action(() => Network_Callmanager_Report_Output.AppendText(output + System.Environment.NewLine)));
            Dispatcher.BeginInvoke(new Action(() => Network_Callmanager_Report_Output.ScrollToEnd()));
        }

        /// <summary>
        /// Gibt einen String in der Textbox aus, ohne anschließer neuer Zeile
        /// </summary>
        /// <param name="output"></param>
        public void Output(String output)
        {
            Dispatcher.BeginInvoke(new Action(() => Network_Callmanager_Report_Output.AppendText(output)));
            Dispatcher.BeginInvoke(new Action(() => Network_Callmanager_Report_Output.ScrollToEnd()));
        }

        /// <summary>
        /// Neuen Filter initialisieren
        /// </summary>
        public void CreateNewFilter()
        {
            if (FilterList.Count < 4)
            {
                FilterList.Add(new Filter());
                FilterList[FilterList.Count - 1].SetMargin(FilterList[FilterList.Count - 1].GetComboField0(), new Thickness(305, 36 + (FilterList.Count - 1) * 28, 0, 0));
                FilterList[FilterList.Count - 1].SetMargin(FilterList[FilterList.Count - 1].GetComboField1(), new Thickness(434, 36 + (FilterList.Count - 1) * 28, 0, 0));
                FilterList[FilterList.Count - 1].SetMargin(FilterList[FilterList.Count - 1].GetTextField0(), new Thickness(559, 36 + (FilterList.Count - 1) * 28, 0, 0));
                Network_Callmanager_Report_Filter_Grid.Children.Add(FilterList[FilterList.Count - 1].GetComboField0());
                Network_Callmanager_Report_Filter_Grid.Children.Add(FilterList[FilterList.Count - 1].GetComboField1());
                Network_Callmanager_Report_Filter_Grid.Children.Add(FilterList[FilterList.Count - 1].GetTextField0());
                AddItemsToField();
            }
        }

        /// <summary>
        /// Entfernt den untersten Filter aus der Übersicht
        /// </summary>
        public void DeleteLastFilter()
        {
            if (FilterList.Count > 1)
            {
                Network_Callmanager_Report_Filter_Grid.Children.Remove(FilterList[FilterList.Count - 1].GetComboField0());
                Network_Callmanager_Report_Filter_Grid.Children.Remove(FilterList[FilterList.Count - 1].GetComboField1());
                Network_Callmanager_Report_Filter_Grid.Children.Remove(FilterList[FilterList.Count - 1].GetTextField0());
                FilterList.RemoveAt(FilterList.Count - 1);
            }
        }

        /// <summary>
        /// Fügt den ComboBoxen die Items hinzu
        /// </summary>
        public void AddItemsToField()
        {
            FilterList[FilterList.Count - 1].AddItemField0("Called Number");
            FilterList[FilterList.Count - 1].AddItemField0("Caller Number");
            FilterList[FilterList.Count - 1].AddItemField0("Caller/ed Number");
            FilterList[FilterList.Count - 1].AddItemField0("Finally Called");
            FilterList[FilterList.Count - 1].AddItemField1("Beginnt");
            FilterList[FilterList.Count - 1].AddItemField1("Enthält");
            FilterList[FilterList.Count - 1].AddItemField1("Endet");
            FilterList[FilterList.Count - 1].AddItemField1("Genau");
        }

        /// <summary>
        /// Button zum hinzufügen eines neuen Filters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_Add_Click(object sender, RoutedEventArgs e)
        {
            CreateNewFilter();
            Network_Callmanager_Report_Filter_Search.Margin = new Thickness(684, 36 + (FilterList.Count - 1) * 28, 0, 0);
            Network_Callmanager_Report_Filter_Add.Margin = new Thickness(764, 36 + (FilterList.Count - 1) * 28, 0, 0);
            Network_Callmanager_Report_Filter_Delete.Margin = new Thickness(792, 36 + (FilterList.Count - 1) * 28, 0, 0);
            if (FilterList.Count > 3)
            {
                Network_Callmanager_Report_Filter_Add.IsEnabled = false;
            }
            Network_Callmanager_Report_Filter_Delete.IsEnabled = true;
        }

        /// <summary>
        /// Button zum entfernen des untersten Filters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteLastFilter();
            Network_Callmanager_Report_Filter_Search.Margin = new Thickness(684, 36 + (FilterList.Count - 1) * 28, 0, 0);
            Network_Callmanager_Report_Filter_Add.Margin = new Thickness(764, 36 + (FilterList.Count - 1) * 28, 0, 0);
            Network_Callmanager_Report_Filter_Delete.Margin = new Thickness(792, 36 + (FilterList.Count - 1) * 28, 0, 0);
            if (FilterList.Count < 2)
            {
                Network_Callmanager_Report_Filter_Delete.IsEnabled = false;
            }
            Network_Callmanager_Report_Filter_Add.IsEnabled = true;
        }

        /// <summary>
        /// Button zum ausführen des SQL Statemens auf dem Callmanager, mit entsprechendem Filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_Search_Click(object sender, RoutedEventArgs e)
        {
            if (beeBeeAte.callmanagerConnection.debug_SQL) OutputLine(GetSQLStatement());
            if (!beeBeeAte.callmanagerConnection.debug_SQL) Execute(GetSQLStatement());
        }

        /// <summary>
        /// Fügt das Commando "command" auf dem Callmanager aus
        /// </summary>
        /// <param name="command"></param>
        private void Execute(String command)
        {
            OutputLine(beeBeeAte.callmanagerConnection.execute(command));
            if (!beeBeeAte.callmanagerConnection.conectedToCallmanager)
            {
                ConnectingToCallmanagerWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Übersetzt die Wörter für den Callmanager verständlichen ID
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string GetField0Translated(Filter filter)
        {
            switch (filter.GetComboField0().Text)
            {
                case "Called Number":
                    return "originalcalledpartynumber";
                case "Caller Number":
                    return "callingpartynumber";
                case "Caller/ed Number":
                    return "callingpartynumber,originalcalledpartynumber";
                case "Finally Called":
                    return "finalcalledpartyunicodeloginuser";
                default:
                    return "originalcalledpartynumber";
            }
        }

        /// <summary>
        /// Fügt dem ListView "Available" die Doppelklick funktion hinzu, und verschiebt das Objekt zur ListView "Display"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_Available_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Network_Callmanager_Report_Filter_Available.HasItems & Network_Callmanager_Report_Filter_Available.SelectedIndex != -1)
            {
                Network_Callmanager_Report_Filter_Display.Items.Add(Network_Callmanager_Report_Filter_Available.SelectedItem);
                Network_Callmanager_Report_Filter_Available.Items.Remove(Network_Callmanager_Report_Filter_Available.SelectedItem);
            }

        }

        /// <summary>
        /// Fügt dem ListView "Display" die Doppelklick funktion hinzu, und verschiebt das Objekt zur ListView "Available"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_Display_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Network_Callmanager_Report_Filter_Display.HasItems & Network_Callmanager_Report_Filter_Display.SelectedIndex != -1)
            {
                Network_Callmanager_Report_Filter_Available.Items.Add(Network_Callmanager_Report_Filter_Display.SelectedItem);
                Network_Callmanager_Report_Filter_Display.Items.Remove(Network_Callmanager_Report_Filter_Display.SelectedItem);
            }
        }

        /// <summary>
        /// Button für das hinzufügen des Ausgewählten Objektes von der ListView "Display" zur ListView "Available"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_Add_Display_Click(object sender, RoutedEventArgs e)
        {
            if (Network_Callmanager_Report_Filter_Available.HasItems & Network_Callmanager_Report_Filter_Available.SelectedIndex != -1)
            {
                Network_Callmanager_Report_Filter_Display.Items.Add(Network_Callmanager_Report_Filter_Available.SelectedItem);
                Network_Callmanager_Report_Filter_Available.Items.Remove(Network_Callmanager_Report_Filter_Available.SelectedItem);
            }

        }

        /// <summary>
        /// Button für das hinzufügen des Ausgewählten Objektes von der ListView "Available" zur ListView "Display"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_Delete_Display_Click_1(object sender, RoutedEventArgs e)
        {
            if (Network_Callmanager_Report_Filter_Display.HasItems & Network_Callmanager_Report_Filter_Display.SelectedIndex != -1)
            {
                Network_Callmanager_Report_Filter_Available.Items.Add(Network_Callmanager_Report_Filter_Display.SelectedItem);
                Network_Callmanager_Report_Filter_Display.Items.Remove(Network_Callmanager_Report_Filter_Display.SelectedItem);
            }
        }

        /// <summary>
        /// Initialisiert die Klasse Call_Report, stellt Datum auf Heute, für den ListViews die Elemente hinzu etc.
        /// </summary>
        public void InitCallReport()
        {
            Network_Callmanager_Report_Grid.Visibility = Visibility.Visible;
            Network_Callmanager_Report_Filter_DateEnd.SelectedDate = DateTime.Today;
            Network_Callmanager_Report_Filter_DateBegin.SelectedDate = DateTime.Today;

            List<String> Available = new List<string>
            {
                "finalcalledpartynumber",
                "finalcalledpartyunicodeloginuserid"
            };
            foreach (String item in Available)
            {
                Network_Callmanager_Report_Filter_Available.Items.Add(item);
            }

            List<String> Display = new List<string>
            {
                "datetimestamporigination",
                "callingpartynumber",
                "originalcalledpartynumber",
                "duration"
            };
            foreach (String item in Display)
            {
                Network_Callmanager_Report_Filter_Display.Items.Add(item);
            }

            CreateNewFilter();

            ConnectingToCallmanagerWorker.DoWork += ConnectingToCallmanager;
            ConnectingToCallmanagerWorker.RunWorkerAsync();

            Network_Callmanager_Report_Output.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        /// <summary>
        /// Nur für die Ausgabe, dass die Verbindung zum Callmanager nun aufgebaut wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectingToCallmanager(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.Output("Connecting to Callmanager " + Properties.Settings.Default.CallmanagerIPAddress + ".");
            while (!beeBeeAte.callmanagerConnection.conectedToCallmanager)
            {
                this.Output(".");
                System.Threading.Thread.Sleep(500);
            }
            this.OutputLine(" Connected");
        }

        /// <summary>
        /// Löscht den Output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Output_Clear_Click(object sender, RoutedEventArgs e)
        {
            Network_Callmanager_Report_Output.Clear();
        }

        /// <summary>
        /// Gibt die Möglichkeit, den Output in einer Datei zu speichern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Output_Save_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            Nullable<bool> result = saveFileDialog1.ShowDialog();
            if (result == true)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    UnicodeEncoding uniEncoding = new UnicodeEncoding();
                    byte[] messageBytes = uniEncoding.GetBytes(Network_Callmanager_Report_Output.Text);
                    myStream.Write(messageBytes, 0, messageBytes.Length);
                    myStream.Close();
                }
            }
        }

        /// <summary>
        /// Button für das zurück gehen Datum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_DateBack_Click(object sender, RoutedEventArgs e)
        {
            Network_Callmanager_Report_Filter_DateBegin.SelectedDate = Network_Callmanager_Report_Filter_DateBegin.SelectedDate.Value.AddDays(-1);
            Network_Callmanager_Report_Filter_DateEnd.SelectedDate = Network_Callmanager_Report_Filter_DateEnd.SelectedDate.Value.AddDays(-1);
        }

        /// <summary>
        /// Button für das vorwärts im Datum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_DateForward_Click(object sender, RoutedEventArgs e)
        {
            Network_Callmanager_Report_Filter_DateBegin.SelectedDate = Network_Callmanager_Report_Filter_DateBegin.SelectedDate.Value.AddDays(1);
            Network_Callmanager_Report_Filter_DateEnd.SelectedDate = Network_Callmanager_Report_Filter_DateEnd.SelectedDate.Value.AddDays(1);
        }

        /// <summary>
        /// Button für das setzen des Datums auf heute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_DateToday_Click(object sender, RoutedEventArgs e)
        {
            Network_Callmanager_Report_Filter_DateEnd.SelectedDate = DateTime.Today;
            Network_Callmanager_Report_Filter_DateBegin.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// Button für das setzen des Datums auf den letzen Monat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Network_Callmanager_Report_Filter_DateLastMonth_Click(object sender, RoutedEventArgs e)
        {
            Network_Callmanager_Report_Filter_DateBegin.SelectedDate = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, 1);
            Network_Callmanager_Report_Filter_DateEnd.SelectedDate = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month));
        }

    }
}
