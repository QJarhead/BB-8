using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using BB_8.WebApiNetTerrain;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace BB_8
{
    ///     Description:            Klasse für die Verwaltung der API zu netTerrain
    ///     Namespace:              BB_8
    ///     Class:                  netTerrain_Api
    ///     Author:                 Tim Christoph Lid
    ///     Notes:                  
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       08.08.2017      Erstellt
    ///  
    public class netTerrain_Api
    {
        private INetTerrainWebApi api;
        SqlConnection sqlConnection;

        private static ChannelFactory<WebApiNetTerrain.INetTerrainWebApi> CreateConnectionFactory(string url, string username, string password)
        {
            var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.EnableUnsecuredResponse = true;
            security.AllowInsecureTransport = true;
            var customBinding = new CustomBinding(security,
            new TextMessageEncodingBindingElement(),
            new HttpTransportBindingElement());
            EndpointAddress endpointAddress = new EndpointAddress(new Uri(url));
            ChannelFactory<WebApiNetTerrain.INetTerrainWebApi> factory =
            new ChannelFactory<WebApiNetTerrain.INetTerrainWebApi>(customBinding,
            endpointAddress);
            factory.Credentials.UserName.UserName = username;
            factory.Credentials.UserName.Password = password;
            return factory;
        }

        public void setupWebApi()
        {
            ChannelFactory<WebApiNetTerrain.INetTerrainWebApi> factory = CreateConnectionFactory(@"http://10.1.1.18/vis/WebApi/NetTerrainWebApi.svc","" + Properties.Settings.Default.netTerrainUsername + "|AD_H4fU7d", "" + Properties.Settings.Default.netTerrainPassword + "");
            api = factory.CreateChannel();
        }

        public long getCategoryIdByName(String Name)
        {
            return api.CatalogNodeTypeGetByName(Name).CategoryId;
        }

        private String getCategorieNameById(long categoryId)
        {
            using (SqlDataReader reader = executeSqlCommand("SELECT Name From dbo.NodeCategories WHERE id like " + categoryId))
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return "null";
        }

        public void addPropertyByTypeId(long id, String defaulValue)
        {
            try
            {
                long catalogLinkTypePropertyId = api.CatalogNodeTypePropertyAdd(id, "Topology_View", defaulValue, true, false, true);
            }
            catch (Exception)
            {

            }
        }

        public long getNodePropertyIdByName(long id, String Name)
        {
            Dictionary<long, string> list = api.CatalogGetNodeTypeProperties(id);
            foreach (KeyValuePair<long, string> entry in list)
            {
                if (entry.Value == "Topology_View")
                {
                    return entry.Key;
                }
            }
            return 0000000;
        }

        public void addOverrideImage(String Name, long OverrideId, FileStream stream)
        {
            try
            {
                api.CatalogNodeOverrideUploadImage(new FileUploadAttributes1()
                {
                    FileName = @"" + Name + "",
                    FileByteStream = stream,
                    ObjectId = OverrideId
                });
            }
            catch (Exception ex)
            {

            }

        }

        public NodeOverride getNodeOverrideByTypeId(long id, String Name)
        {

            try
            {
                long[] nodeOverrideIds = api.CatalogNodeOverridesGetByTypeId(id);

                for (int i = 0; i < nodeOverrideIds.Length; i++)
                {
                    if(getNodeOverrideById(nodeOverrideIds[0]).ListValue == Name)
                    {
                        return getNodeOverrideById(nodeOverrideIds[0]);
                    }
                }
            }

            catch (FaultException<FaultInfo> info)
            {
                Console.WriteLine(string.Format("Error: {0}", info.Detail.Details));
            }

            return new NodeOverride();
        }

        public NodeOverride getNodeOverrideByIdAndName(String Name, long id)
        {
            using (SqlDataReader reader = executeSqlCommand("SELECT * FROM NodeProperties where name like 'Topology_View' and NodeTypeId like " + id))
            {
                while (reader.Read())
                {
                    MessageBox.Show(reader.GetDecimal(0).ToString(), "");
                    return getNodeOverrideById(Convert.ToInt64(reader.GetDecimal(0)));
                }
            }
            return new NodeOverride();
        }

        public void updateOverrideId(NodeOverride nodeOverride)
        {
            api.CatalogNodeOverrideUpdate(nodeOverride);
        }

        public NodeOverride getNodeOverrideById(long id)
        {
            return api.CatalogNodeOverrideGet(id);
        }

        internal long addNodeOverride(long id, bool isOverride, OverrideRules overrideRule, string Value, string imageName)
        {
            try
            {
                return api.CatalogNodeOverrideAdd(id, isOverride, overrideRule, Value, imageName);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public void updateNodeType(NodeType nodeType)
        {
            api.CatalogNodeTypeUpdate(nodeType);
        }

        public NodeType getNodeTypeByName(String Name)
        {
            return api.CatalogNodeTypeGetByName( Name);
        }

        public NodeCategory getNodeCategoryByNodeName(String Name)
        {
            return api.CatalogNodeCategoryGetByName(Name);
        }

        public long getNodeCatgetoryIdByNodeName(String Name)
        {
            return getNodeCategoryByNodeName(Name).Id;
        }

        public String getNodeCategoryNameByNodeName(String Name)
        {
            return getNodeCategoryByNodeName(Name).Name;
        }

        public SqlDataReader executeSqlCommand(String sqlCommand)
        {
            sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString =
            "Data Source=" + Properties.Settings.Default.netTerrainDatabaseIPAddress + ";" +
            "Initial Catalog=" + Properties.Settings.Default.netTerrainDatabaseName + ";" +
            "User id=" + Properties.Settings.Default.netTerrainDatabaseUser + ";" +
            "Password=" + Properties.Settings.Default.netTerrainDatabasePassword + ";";
            sqlConnection.Open();
            using (SqlCommand command = new SqlCommand(sqlCommand, sqlConnection))
            {
                return command.ExecuteReader();
            }
        }
    }
}
