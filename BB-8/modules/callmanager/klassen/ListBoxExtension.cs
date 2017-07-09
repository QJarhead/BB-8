using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Listbox
{
    ///     Description:            Klasse die erweiterung der Listbox
    ///     Namespace:              BB_8
    ///     Class:                  ListBoxExtension
    ///     Author:                 Tim Christoph Lid                    
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       09.06.2017      Erstellt
    ///     Tim Christpoh Lid       09.07.2017      Kommentare
    ///  
    public static class ListBoxExtension
    {
        /// <summary>
        /// Löscht die Listbox
        /// </summary>
        /// <param name="listBox"></param>
        public static void Clear(this System.Windows.Controls.ListBox listBox)
        {
            listBox.Items.Clear();
        }

        /// <summary>
        /// Fügt der Listbox ein Item hinzu, welches noch nicht vorhanden ist
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="item"></param>
        public static void AddUnique(this System.Windows.Controls.ListBox listBox, String item)
        {
            if (listBox.Items.IndexOf(item) == -1 && item != "")
            {
                listBox.Items.Add(item);
            }
        }

        /// <summary>
        /// Entfernt ein Item, wenn es vorhanden sein sollte
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="item"></param>
        public  static void Remove(this System.Windows.Controls.ListBox listBox, Object item)
        {
            if (listBox.Items.IndexOf(item) != -1)
            {
                listBox.Items.Remove(item);
            }
        }
    }
}
