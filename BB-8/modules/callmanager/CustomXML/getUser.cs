using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BB_8
{
    class getUser
    {
        ///     Description:            Klasse für XML verbindung zum Callmanager, gibt die IP-Adresse einer angegebenen Rufnummer zurück
        ///     Namespace:              BB_8
        ///     Class:                  getPhoneIP
        ///     Author:                 Tim Christoph Lid                    
        ///     Date-Init:              25.06.2017
        ///     Date-LastChange         25.08.2017
        ///     Notes:                  Ggf. anpassen, dass nicht nur nach DirectoryNumber gefilter wird,
        ///                             Ebenfalls sollten die Weiteren return Werte hinzugefügt werden, Name, DirectoryNumber etc.
        ///                             Verbindung zum Callmanager auslagern?
        ///     Revision History:
        ///     Name:                   Date:           Description:
        ///   
        ///     Tim Christoph Lid       24.06.2017      Erstellt
        ///     Tim Christoph Lid       24.06.2017      Funktioniert, liefert die IP-Adresse wie gewünscht zurück
        ///

        String userID = "%";
        String lastName = "%";
        String firstName = "%";

        public getUser()
        {

        }

        public void setUserID(String userID)
        {
            this.userID = userID;
        }
        public void setlastName(String lastName)
        {
            this.lastName = lastName;
        }
        public void setfirstName(String firstName)
        {
            this.firstName = firstName;
        }

        public XmlDocument getUserResponse()
        {
            XmlDocument xmlDocument = new XmlDocument();
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format(@"https://194.76.158.16:8443/axl/"));
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(BB_8.Properties.Settings.Default.CallmanagerUsername, BB_8.Properties.Settings.Default.CallmanagerPassword);
            request.ContentType = "text/xml; charset=utf-8";
            request.Accept = "text/xml";
            request.Headers.Add("SOAPAction: CUCM:DB ver=11.0");
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            Assembly _assembly;
            StreamReader _textStreamReader;
            _assembly = Assembly.GetExecutingAssembly();
            _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("BB_8.CustomXML.getUser.xml"));
            String soap = _textStreamReader.ReadToEnd().ToString();
            soap = soap.Replace("BB8_UserId", userID);
            soap = soap.Replace("BB8_lastName", lastName);
            soap = soap.Replace("BB8_firstname", firstName);
            byte[] bytes = Encoding.UTF8.GetBytes(soap);
            request.ContentLength = bytes.Length;
            using (
                Stream putStream = request.GetRequestStream())
            {
                putStream.Write(bytes, 0, bytes.Length);
            }
            StreamReader reader;
            HttpWebResponse response;
            String responseString;
            using (response = (HttpWebResponse)request.GetResponse())
            using (reader = new StreamReader(response.GetResponseStream()))
            {
                responseString = reader.ReadToEnd();
            }
            xmlDocument.LoadXml(responseString);
            return xmlDocument;
        }

        public List<User> getUsers()
        {
            XmlDocument xmlDocument = getUserResponse();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("ns", "http://www.cisco.com/AXL/API/11.0");
            List<User> users = new List<User>();
            XmlNodeList elemList = xmlDocument.GetElementsByTagName("user");
            for (int i = 0; i < elemList.Count; i++)
            {
                ///Console.WriteLine(elemList[i].InnerXml);
                try
                {
                    users.Add(new User(elemList[i].SelectSingleNode("firstName", nsmgr).InnerText, elemList[i].SelectSingleNode("lastName", nsmgr).InnerText, elemList[i].SelectSingleNode("userid", nsmgr).InnerText));
                }
                catch (Exception)
                {

                }
            }
            return users;
        }

        public User getSingleUser(String directoryNumber)
        {
            this.userID = directoryNumber;
            if (getUsers().Count > 0)
            {
                return getUsers()[0];
            }
            return new User("NN", "NN", "directoryNumber");
        }

    }
}
