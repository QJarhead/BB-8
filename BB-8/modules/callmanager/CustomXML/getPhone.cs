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
    class getPhone
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

        String directoryNumber = "";

        public getPhone()
        {

        }

        public void setDirectoryNumber(String directoryNumber)
        {
            this.directoryNumber = directoryNumber;
        }

        public XmlDocument getPhoneResponse()
        {
            XmlDocument xmlDocument = new XmlDocument();
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format(@"https://cucm01.voice.flexx.local:8443/realtimeservice2/services/RISService70?wsdl/"));
            request.Method = "POST";
            request.Credentials = new NetworkCredential(BB_8.Properties.Settings.Default.CallmanagerUsername, BB_8.Properties.Settings.Default.CallmanagerPassword);
            request.ContentType = "text/xml; charset=utf-8";
            request.Accept = "text/xml";
            request.Headers.Add("SOAPAction: CUCM:DB ver=11.0");
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            Assembly _assembly;
            StreamReader _textStreamReader;
            _assembly = Assembly.GetExecutingAssembly();
            _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("BB_8.CustomXML.getPhone.xml"));
            String soap = _textStreamReader.ReadToEnd().ToString();
            soap = soap.Replace("BB8_DirectoryNumber", directoryNumber);

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

        public List<Phone> getPhones()
        {
            XmlDocument xmlDocument = getPhoneResponse();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("ns1", "http://schemas.cisco.com/ast/soap");

            String deviceCountString = xmlDocument.SelectSingleNode("/soapenv:Envelope/soapenv:Body/ns1:selectCmDeviceResponse/ns1:selectCmDeviceReturn/ns1:SelectCmDeviceResult/ns1:TotalDevicesFound", nsmgr).InnerText;


            List<Phone> phones = new List<Phone>();

            XmlNodeList elemList = xmlDocument.GetElementsByTagName("ns1:item");
            for (int i = 0; i < elemList.Count; i++)
            {
                try
                {
                    if ((elemList[i].SelectSingleNode("ns1:Name", nsmgr).InnerText).IndexOf("SEP") == 0)
                    {
                        phones.Add(new Phone(elemList[i].SelectSingleNode("ns1:IPAddress/ns1:item/ns1:IP", nsmgr).InnerText, elemList[i].SelectSingleNode("ns1:Name", nsmgr).InnerText, elemList[i].SelectSingleNode("ns1:LinesStatus/ns1:item/ns1:DirectoryNumber", nsmgr).InnerText, ""));

                    }
                }
                catch (Exception)
                {

                }
            }
            return phones;
        }
        public Phone getSinglePhone(String directoryNumber)

        {
            this.directoryNumber = directoryNumber;
            if (getPhones().Count > 0)
            {
                return getPhones()[0];
            }
            return new Phone("NN", "NN", "NN", "NN");
        }

    }
}
