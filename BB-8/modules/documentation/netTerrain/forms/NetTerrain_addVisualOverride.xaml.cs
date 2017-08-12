using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using BB_8.WebApiNetTerrain;
using System.ServiceModel.Channels;
using System.IO;

namespace BB_8
{
    /// <summary>
    /// Interaktionslogik für NetTerrain_addVisualOverride.xaml
    /// </summary>
    public partial class NetTerrain_addVisualOverride : Window
    {
        uploadImage uploadImage;
        FileStream stream;
        public netTerrain_Api api;

        public NetTerrain_addVisualOverride()
        {
            api = new netTerrain_Api();
            api.setupWebApi();
            InitializeComponent();
        }

        void uploadImage_MyEvent(object sender, EventArgs e)
        {
            MessageBox.Show("Handle");
        }

        private void Documentation_NetTerrain_addVisualOverride_Upload_Click(object sender, RoutedEventArgs e)
        {
            uploadImage = new uploadImage();
            uploadImage.Closed += new EventHandler(loginWnd_Closed);
            uploadImage.ShowDialog();
        }

        private void loginWnd_Closed(object sender, EventArgs e)
        {
            if (!(((uploadImage)sender).Documentation_NetTerrain_uploadImage_Source.Text == ""))
            {
                stream = new FileStream(@"" + ((uploadImage)sender).Documentation_NetTerrain_uploadImage_Source.Text + "", FileMode.Open, FileAccess.Read);
                Documentation_NetTerrain_addVisualOverride_Image.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Documentation_NetTerrain_addVisualOverride_Devices.Text == "*All Selected*")
            {
                for (int i = 1; i < Documentation_NetTerrain_addVisualOverride_Devices.Items.Count; i++)
                {

                    NodeType nodeType = api.getNodeTypeByName(Documentation_NetTerrain_addVisualOverride_Devices.Items.GetItemAt(i).ToString());
                    try
                    {
                        FileStream stream = new FileStream(@"" + uploadImage.Documentation_NetTerrain_uploadImage_Source.Text, FileMode.Open, FileAccess.Read);
                        api.addPropertyByTypeId(nodeType.Id, "$zz_get_TopologyType([diagramid])");
                        long newOverrideId = api.addNodeOverride(api.getNodePropertyIdByName(nodeType.Id, "Topology_View"), true, OverrideRules.Equals, "Logical View", "000000.png");
                        newOverrideId = api.getNodeOverrideByTypeId(nodeType.Id, "Logical View").Id;
                        api.addOverrideImage(uploadImage.Documentation_NetTerrain_uploadImage_Destination.Text, newOverrideId, stream);
                        NodeOverride nodeOverride = api.getNodeOverrideById(newOverrideId);
                        nodeOverride.ImageFileName = uploadImage.Documentation_NetTerrain_uploadImage_Destination.Text;
                        api.updateOverrideId(nodeOverride);
                    }
                    catch (Exception info)
                    {
                        MessageBox.Show(string.Format("Error" + info.Message, info.Message), "");
                    }
                }
            }
            else
            {
                NodeType nodeType = api.getNodeTypeByName(Documentation_NetTerrain_addVisualOverride_Devices.Text);

                try
                {
                    FileStream stream = new FileStream(@"" + uploadImage.Documentation_NetTerrain_uploadImage_Source.Text, FileMode.Open, FileAccess.Read);
                    api.addPropertyByTypeId(nodeType.Id, "$zz_get_TopologyType([diagramid])");
                    long newOverrideId = api.addNodeOverride(api.getNodePropertyIdByName(nodeType.Id, "Topology_View"), true, OverrideRules.Equals, "Logical View", "000000.png");
                    newOverrideId = api.getNodeOverrideByTypeId(nodeType.Id, "Logical View").Id;
                    api.addOverrideImage(uploadImage.Documentation_NetTerrain_uploadImage_Destination.Text, newOverrideId, stream);
                    NodeOverride nodeOverride = api.getNodeOverrideById(newOverrideId);
                    nodeOverride.ImageFileName = uploadImage.Documentation_NetTerrain_uploadImage_Destination.Text;
                    api.updateOverrideId(nodeOverride);
                }
                catch (FaultException<FaultInfo> info)
                {
                    Console.WriteLine(string.Format(
                    "Cannot create node override: {0}", info.Detail.Details));
                }

            }

        }

        private void Documentation_NetTerrain_addVisualOverride_Value_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
