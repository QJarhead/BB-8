using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB_8
{
    ///     Description:            Klasse ein Telefon
    ///     Namespace:              BB_8
    ///     Class:                  Phone
    ///     Author:                 Tim Christoph Lid                    
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       09.06.2017      Erstellt
    ///     Tim Christpoh Lid       09.07.2017      Kommentare
    ///  
    class Phone
    {
        public string ipAadress { get; set; }
        public string name { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }

        /// <summary>
        /// Erstellt ein neues Telefon, mit dazuhgehöriger IP-Adresse, Name, line1 und line 2
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="name"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        public Phone(String ipAddress, String name, String line1, String line2)
        {
            this.ipAadress = ipAddress;
            this.name = name;
            this.line1 = line1;
            this.line2 = line2;
        }

        /// <summary>
        /// Gibt dieses Telefon wieder
        /// </summary>
        /// <returns></returns>
        public Phone getPhone()
        {
            return this;
        }
    }
}
