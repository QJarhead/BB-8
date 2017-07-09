using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB_8
{
    ///     Description:            Klasse für die Anzeige der Telefone in der DataGrid-View
    ///     Namespace:              BB_8
    ///     Class:                  DataGrid_Device_Entry
    ///     Author:                 Tim Christoph Lid                    
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       09.06.2017      Erstellt
    ///     Tim Christpoh Lid       09.07.2017      Kommentare
    ///     
    class DataGrid_Device_Entry
    {
        public String UserID { get; set; }
        public String lastName { get; set; }
        public String firstName { get; set; }
        public String DeviceName { get; set; }
        public String IPAddress { get; set; }

        /// <summary>
        /// gibt die IP-Adresse wieder
        /// </summary>
        /// <returns></returns>
        public String GetIPAdress()
        {
            return IPAddress;
        }

    }
}
