using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB_8
{
    ///     Description:            Klasse ein User
    ///     Namespace:              BB_8
    ///     Class:                  User
    ///     Author:                 Tim Christoph Lid                    
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       09.06.2017      Erstellt
    ///     Tim Christpoh Lid       09.07.2017      Kommentare
    ///  
    class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserID { get; set; }
        
        /// <summary>
        /// erstellt einen neuen User, mit vorname, name und UserID
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="UserID"></param>
        public User(String FirstName, String LastName, String UserID)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.UserID = UserID;
        }

        /// <summary>
        /// Gibt diesen User wieder
        /// </summary>
        /// <returns></returns>
        public User getUser()
        {
            return this;
        }
    }
}
