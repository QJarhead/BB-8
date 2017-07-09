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

namespace BB_8
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Filter> FilterList = new List<Filter>();
        List<System.Windows.Controls.ComboBox> searchField0 = new List<System.Windows.Controls.ComboBox>();
        List<System.Windows.Controls.ComboBox> searchField1 = new List<System.Windows.Controls.ComboBox>();
        List<System.Windows.Controls.TextBox> searchField2 = new List<System.Windows.Controls.TextBox>();
        SshClient client;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCustomComponents();
            createNewFilter();

            var connectionInfo = new ConnectionInfo("194.76.158.4", "administrator",
                                 new PasswordAuthenticationMethod("administrator", "pwrs2305"),
                                 new PrivateKeyAuthenticationMethod("rsa.key"));
            client = new SshClient(connectionInfo);
            client.Connect();

            Network_Callmanager_Report_Output.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        public String formatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public void Report_Output(String output)
        {
            Network_Callmanager_Report_Output.AppendText(output + "\n");
            Network_Callmanager_Report_Output.ScrollToEnd();
        }

        public void createNewFilter()
        {
            if(FilterList.Count < 4)
            {
                FilterList.Add(new Filter());
                FilterList[FilterList.Count - 1].setMargin(FilterList[FilterList.Count - 1].getComboField0(), new Thickness(309, 36 + (FilterList.Count - 1) * 28, 0, 0));
                FilterList[FilterList.Count - 1].setMargin(FilterList[FilterList.Count - 1].getComboField1(), new Thickness(434, 36 + (FilterList.Count - 1) * 28, 0, 0));
                FilterList[FilterList.Count - 1].setMargin(FilterList[FilterList.Count - 1].getTextField0(), new Thickness(559, 36 + (FilterList.Count - 1) * 28, 0, 0));
                Network_Callmanager_Report_Filter_Grid.Children.Add(FilterList[FilterList.Count - 1].getComboField0());
                Network_Callmanager_Report_Filter_Grid.Children.Add(FilterList[FilterList.Count - 1].getComboField1());
                Network_Callmanager_Report_Filter_Grid.Children.Add(FilterList[FilterList.Count - 1].getTextField0());
                addItemsToField();
            }
        }

        public void deleteLastFilter()
        {
            if (FilterList.Count > 1)
            {
                Network_Callmanager_Report_Filter_Grid.Children.Remove(FilterList[FilterList.Count - 1].getComboField0());
                Network_Callmanager_Report_Filter_Grid.Children.Remove(FilterList[FilterList.Count - 1].getComboField1());
                Network_Callmanager_Report_Filter_Grid.Children.Remove(FilterList[FilterList.Count - 1].getTextField0());
                FilterList.RemoveAt(FilterList.Count-1);
            }
        }

        /// <summary>
        /// adding Items to the Field
        /// </summary>
        public void addItemsToField()
        {
            FilterList[FilterList.Count - 1].addItemField0("Called Number");
            FilterList[FilterList.Count - 1].addItemField0("Caller Number");
            FilterList[FilterList.Count - 1].addItemField1("Beginnt");
            FilterList[FilterList.Count - 1].addItemField1("Enthält");
            FilterList[FilterList.Count - 1].addItemField1("Endet");
            FilterList[FilterList.Count - 1].addItemField1("Genau");
        }

        private void Network_Callmanager_Report_Filter_Add_Click(object sender, RoutedEventArgs e)
        {
            createNewFilter();
            Network_Callmanager_Report_Filter_Search.Margin = new Thickness(684, 36 + (FilterList.Count-1) * 28, 0, 0);
            Network_Callmanager_Report_Filter_Add.Margin = new Thickness(764, 36 + (FilterList.Count-1) * 28, 0, 0);
            Network_Callmanager_Report_Filter_Delete.Margin = new Thickness(792, 36 + (FilterList.Count-1) * 28, 0, 0);
            if (FilterList.Count > 3)
            {
                Network_Callmanager_Report_Filter_Add.IsEnabled = false;
            }
            Network_Callmanager_Report_Filter_Delete.IsEnabled = true;
        }

        private void Network_Callmanager_Report_Filter_Delete_Click(object sender, RoutedEventArgs e)
        {
            deleteLastFilter();
            Network_Callmanager_Report_Filter_Search.Margin = new Thickness(684, 36 + (FilterList.Count - 1) * 28, 0, 0);
            Network_Callmanager_Report_Filter_Add.Margin = new Thickness(764, 36 + (FilterList.Count-1) * 28, 0, 0);
            Network_Callmanager_Report_Filter_Delete.Margin = new Thickness(792, 36 + (FilterList.Count-1) * 28, 0, 0);
            if(FilterList.Count < 2)
            {
                Network_Callmanager_Report_Filter_Delete.IsEnabled = false;
            }
            Network_Callmanager_Report_Filter_Add.IsEnabled = true;
        }

        private void Network_Callmanager_Report_Filter_Search_Click(object sender, RoutedEventArgs e)
        {
            String commandOutput = client.CreateCommand(getFilter()).Execute();
            ///Report_Output(formatDate(Network_Callmanager_Report_Filter_DateBegin.SelectedDate.Value));
            Report_Output(commandOutput);
        }

        private String getFilter()
        {
            String Filter = "run sql car select datetimestampconnect,duration from car:tbl_billing_data where datetimestamporigination between ";
            Filter += "'" + formatDate(Network_Callmanager_Report_Filter_DateBegin.SelectedDate.Value) + " 00:00:00' and ";
            Filter += "'" + formatDate(Network_Callmanager_Report_Filter_DateEnd.SelectedDate.Value) + " 23:59:59' and ";

            foreach (Filter filter in FilterList)
            {

                ///Filter += filter.getComboField0().Text + " " + filter.getComboField1().Text + " " + filter.getTextField0().Text;
                switch (filter.getComboField1().Text)
                {
                    case "Beginnt":
                        Filter += GetField0Translated(filter) + " LIKE '" + filter.getTextField0().Text + "%' ";
                        break;
                    case "Enthält":
                        Filter += GetField0Translated(filter) + " LIKE '%" + filter.getTextField0().Text + "%' ";
                        break;
                    case "Endet":
                        Filter += GetField0Translated(filter) + " LIKE '%" + filter.getTextField0().Text + "' ";
                        break;
                    case "Genau":
                        Filter += GetField0Translated(filter) + " LIKE '" + filter.getTextField0().Text + "' ";
                        break;
                    default:
                        Filter += "LIKE '%' ";
                        break;
                }

                if (filter != FilterList[FilterList.Count-1])
                {
                    Filter += " & ";
                }
                
            }
            Filter += "ORDER BY datetimestamporigination";
            return Filter;
        }

        private string GetField0Translated(Filter filter)
        {
            switch (filter.getComboField0().Text)
            {
                case "Called Number":
                    return "originalcalledpartynumber";
                case "Caller Number":
                    return "callingpartynumber";
            }
            return "";
        }

        public String getField0(String inputText)
        {
            
            return "called.number";
        }

        public void InitializeCustomComponents()
        {
            Network_Callmanager_Report_Filter_DateEnd.SelectedDate = DateTime.Today;
            Network_Callmanager_Report_Filter_DateBegin.SelectedDate = DateTime.Today;
        }

    }
}
