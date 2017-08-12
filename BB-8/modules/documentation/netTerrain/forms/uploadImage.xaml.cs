using BB_8.WebApiNetTerrain;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
    /// <summary>
    /// Interaktionslogik für uploadImage.xaml
    /// </summary>
    public partial class uploadImage : Window
    {
        private INetTerrainWebApi api;

        public uploadImage()
        {
            InitializeComponent();
            setupWebApi();
        }

        private void Documentation_NetTerrain_uploadImage_Upload_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void setupWebApi()
        {
            ChannelFactory<WebApiNetTerrain.INetTerrainWebApi> factory = CreateConnectionFactory(
@"http://10.1.1.18/vis/WebApi/NetTerrainWebApi.svc",
"" + Properties.Settings.Default.netTerrainUsername + "|AD_H4fU7d", // Username postfix to distinguish AD users.
"" + Properties.Settings.Default.netTerrainPassword + "");
            api = factory.CreateChannel();

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

        private void Documentation_NetTerrain_uploadImage_Select_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG files (*.jpeg)|*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Documentation_NetTerrain_uploadImage_Source.Text = openFileDialog.FileName;
                Documentation_NetTerrain_uploadImage_Destination.Text = openFileDialog.SafeFileName;
            }
        }
    }

}