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
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Channels;
using BB_8.WebApiNetTerrain;

namespace BB_8
{
    ///     Description:            Klasse für die automatische änderungen von meherer netTerrain Kategorien
    ///     Namespace:              BB_8
    ///     Class:                  Call_Analysis
    ///     Author:                 Tim Christoph Lid
    ///     Notes:                  Kann in netTerain mehrere Kategorien von Devicen tauschen
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       05.08.2017      Erstellt
    ///     Tim Christoph Lid       07.08.2017      Erweitert
    ///     
    public partial class NetTerrain_Categories : Window
    {
        BeeBeeAte beeBeeAte;
        List<Filter> FilterList = new List<Filter>();
        List<DataGrid_netTerrain_Entry> list;
        netTerrain_Api api;
        public System.Windows.Window Form = Application.Current.MainWindow;
        private System.ComponentModel.BackgroundWorker ConnectingToCallmanagerWorker = new BackgroundWorker();


        /// <summary>
        /// Klasse für Callmanager Analyse, wird direkt geschlossen, nur das Grid wird übernommen und dem Mainwindow hinzugefügt
        /// </summary>
        public NetTerrain_Categories(BeeBeeAte beeBeeAte)
        {
            InitializeComponent();
            this.beeBeeAte = beeBeeAte;
            this.Close();
            api = new netTerrain_Api();
            api.setupWebApi();
            SetupCategories();
            SetupVisualOverrides();
        }

        private void SetupVisualOverrides()
        {
            Documentation_NetTerrain_Categories_Filter_VisualOverride.Items.Clear();
            using (SqlDataReader reader = api.executeSqlCommand("SELECT DISTINCT(Image1) From dbo.NodeOverrides"))
            {
                list = new List<DataGrid_netTerrain_Entry>();
                while (reader.Read())
                {
                        Documentation_NetTerrain_Categories_Filter_VisualOverride.Items.Add(reader.GetString(0));
                }
            }
            Documentation_NetTerrain_Categories_Filter_VisualOverride.SelectedIndex = 0;
        }

        private void SetupCategories()
        {
            using (SqlDataReader reader = api.executeSqlCommand("SELECT Name From dbo.NodeCategories WHERE TypeGroup like 7"))
            {
                list = new List<DataGrid_netTerrain_Entry>();
                while (reader.Read())
                {
                    Documentation_NetTerrain_Categories_Filter_Categories.Items.Add(reader.GetString(0));
                }
            }
            Documentation_NetTerrain_Categories_Filter_Categories.SelectedIndex = 0;
        }

        private void Documentation_NetTerrain_Categories_DataGrid_Checkbox_Checked(object sender, RoutedEventArgs e)
        {

            foreach (DataGrid_netTerrain_Entry item in list)
            {
                item.booleanFlag = true;
            }
            Documentation_NetTerrain_Categories_DataGrid.Items.Refresh();
        }

        private void Documentation_NetTerrain_Categories_DataGrid_Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (DataGrid_netTerrain_Entry item in list)
            {
                item.booleanFlag = false;
            }
            Documentation_NetTerrain_Categories_DataGrid.Items.Refresh();
        }

        private void executeSqlFilter(String sqlQuery)
        {
            String sqlCommand = "";
            sqlCommand = "SELECT * FROM dbo.NodeTypes WHERE Name";

            switch (Documentation_NetTerrain_Categories_Filter_State.Text)
            {
                case "Beginnt":
                    sqlCommand += " LIKE '" + Documentation_NetTerrain_Categories_Filter_Text.Text + "%' ";
                    break;
                case "Enthält":
                    sqlCommand += " LIKE '%" + Documentation_NetTerrain_Categories_Filter_Text.Text + "%' ";
                    break;
                case "Endet":
                    sqlCommand += " LIKE '%" + Documentation_NetTerrain_Categories_Filter_Text.Text + "' ";
                    break;
                case "Genau":
                    sqlCommand += " LIKE '" + Documentation_NetTerrain_Categories_Filter_Text.Text + "' ";
                    break;
                default:
                    sqlCommand += "LIKE '%' ";
                    break;
            }
            
            using (SqlDataReader reader = api.executeSqlCommand(sqlCommand))
            {
                list = new List<DataGrid_netTerrain_Entry>();
                while (reader.Read())
                {
                    String Category = "null";
                    try
                    {
                        Category = getCategorieNameById(api.getCategoryIdByName(reader.GetString(1)));
                    }
                    catch (FaultException<FaultInfo> info)
                    {
                        Documentation_NetTerrain_Categories_Filter_Textbox.Text = info.Detail.Details;
                        Console.WriteLine(string.Format(
                        "Cannot get node type: {0}", info.Detail.Details));
                    }

                    list.Add(new DataGrid_netTerrain_Entry() { Id = reader.GetDecimal(0).ToString(), Name = reader.GetString(1), Categorie = Category });
                    Documentation_NetTerrain_Categories_Filter_Textbox.Text = reader.GetString(1);
                }
                Documentation_NetTerrain_Categories_DataGrid.ItemsSource = list;
            }
            Documentation_NetTerrain_Categories_DataGrid.Items.Refresh();
        }

        private String getCategorieNameById(long categoryId)
        {
            using (SqlDataReader reader = api.executeSqlCommand("SELECT Name From dbo.NodeCategories WHERE id like " + categoryId))
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return "null";
        }

        private static ChannelFactory<WebApiNetTerrain.INetTerrainWebApi> CreateConnectionFactory(
 string url, string username, string password)
        {
            // 1. Configure security properties for unprotected access.
            var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.EnableUnsecuredResponse = true;
            security.AllowInsecureTransport = true;
            // 2. Create custom binding element with unprotected security, text message encoding
            // and http transport.
            var customBinding = new CustomBinding(security,
            new TextMessageEncodingBindingElement(),
            new HttpTransportBindingElement());
            EndpointAddress endpointAddress = new EndpointAddress(new Uri(url));
            // 4. Channel factory combines binding options with the service address.
            ChannelFactory<WebApiNetTerrain.INetTerrainWebApi> factory =
            new ChannelFactory<WebApiNetTerrain.INetTerrainWebApi>(customBinding,
            endpointAddress);
            // 5. Add user credentials to the factory.
            // The service accepts native netTerrain or Active Directory users.
            // To distinguish AD users from native ones the AD postfix ‘|AD_H4fU7d’ must be added
            // to the username.
            factory.Credentials.UserName.UserName = username;
            factory.Credentials.UserName.Password = password;
            return factory;
        }

        private void getNetTerrainDevicesByName()
        {
            executeSqlFilter(Documentation_NetTerrain_Categories_Filter_Text.Text);

        }

        private void Documentation_NetTerrain_Categories_Filter_SearchIP_Click(object sender, RoutedEventArgs e)
        {
            getNetTerrainDevicesByName();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (DataGrid_netTerrain_Entry item in list)
            {
                item.booleanFlag = false;
            }
            Documentation_NetTerrain_Categories_DataGrid.Items.Refresh();
        }

        private String getVisualOverrideImageByValue(String value)
        {
            using (SqlDataReader reader = api.executeSqlCommand("SELECT Image1 From dbo.NodeOverrides WHERE Value LIKE '" + value + "'"))
            {
                //list = new List<DataGrid_netTerrain_Entry>();
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return "null";
        }

        private void Documentation_NetTerrain_Categories_Filter_SetCategorie_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataGrid_netTerrain_Entry item in list)
            {
                if (item.booleanFlag)
                {
                    Documentation_NetTerrain_Categories_Filter_Textbox.Text += "\r" + item.Name;
                    NodeType nodeType = api.getNodeTypeByName(item.Name);
                    NodeCategory nodeCategory = api.getNodeCategoryByNodeName(Documentation_NetTerrain_Categories_Filter_Categories.SelectedValue.ToString());
                    nodeType.CategoryId = nodeCategory.Id;
                    api.updateNodeType(nodeType);
                    item.Categorie = nodeCategory.Name;
                }
            }
            Documentation_NetTerrain_Categories_DataGrid.Items.Refresh();
        }

        private void Documentation_NetTerrain_Categories_Filter_SetVisualOverride_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataGrid_netTerrain_Entry item in list)
            {
                if (item.booleanFlag)
                {
                    //Documentation_NetTerrain_Categories_Filter_Textbox.Text += "\r" + item.Name;
                    NodeType nodeType = api.getNodeTypeByName(item.Name);
                    api.addPropertyByTypeId(nodeType.Id, "$zz_get_TopologyType([diagramid])");

                    long newOverrideId = api.addNodeOverride(api.getNodePropertyIdByName(nodeType.Id, "Topology_View"), true, OverrideRules.Equals, "Logical View", "000000.png");
                    newOverrideId = api.getNodeOverrideByTypeId(nodeType.Id, "Logical View").Id;
                    NodeOverride nodeOverride = api.getNodeOverrideById(newOverrideId);
                    nodeOverride.ImageFileName = Documentation_NetTerrain_Categories_Filter_VisualOverride.Text;
                    api.updateOverrideId(nodeOverride);
                    //api.addNodeOverride(api.getNodePropertyIdByName(nodeType.Id,"Topology_View"), true, OverrideRules.Equals, "Logical View", Documentation_NetTerrain_Categories_Filter_VisualOverride.SelectedValue.ToString());
                }
            }
            Documentation_NetTerrain_Categories_DataGrid.Items.Refresh();
        }

        private void Documentation_NetTerrain_Categories_Filter_VisualOverride_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Documentation_NetTerrain_Categories_Filter_VisualOverride.Items.Count > 0)
            {
                var fullFilePath = @"http://10.1.1.18/vis/Images/Nodes/" + Documentation_NetTerrain_Categories_Filter_VisualOverride.SelectedValue.ToString();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                bitmap.EndInit();
                Documentation_NetTerrain_Categories_Filter_VisualOverride_Image.Source = bitmap;
            }
        }

        public void addVisualOverrideClose(object sender, EventArgs e)
        {
            SetupVisualOverrides();
        }

        private void Documentation_NetTerrain_Categories_Filter_addCategorie_Click(object sender, RoutedEventArgs e)
        {
            NetTerrain_addVisualOverride netTerrain_addVisualOverride = new NetTerrain_addVisualOverride();
            netTerrain_addVisualOverride.Closed += new EventHandler(addVisualOverrideClose);
            netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Field.Items.Add("Topology_View");
            netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Devices.Items.Add("*All Selected*");
            netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Override.Items.Add("true");
            netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Rule.Items.Add("=");
            foreach (DataGrid_netTerrain_Entry item in list)
            {
                
                if (item.booleanFlag)
                {
                    netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Devices.Items.Add(item.Name);
                }
            }
            netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Devices.SelectedIndex = 0;
            netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Field.SelectedIndex = 0;
            netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Override.SelectedIndex = 0;
            netTerrain_addVisualOverride.Documentation_NetTerrain_addVisualOverride_Rule.SelectedIndex = 0;
            netTerrain_addVisualOverride.ShowDialog();
        }
    }
}
