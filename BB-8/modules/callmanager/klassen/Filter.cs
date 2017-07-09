using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

///     Description:            Klasse für "Filter", jeder Filter ist eine eigenen Instanz dieser Klasse
///     Namespace:              BB_8
///     Class:                  Filter
///     Author:                 Tim Christoph Lid                    
///     Date-Init:              16.06.2017
///     Date-LastChange         18.08.2017
///     Notes:                  Genutzt nur von Callmanager_Report für die Filter-Anzeige
///     Revision History:
///     Name:                   Date:           Description:
///   
///     Tim Christoph Lid       16.06.2017      Erstellt
///     Tim Christoph Lid       18.08.2017      Änder der Standardauswahl für einen Filter von Caller Number zu Caller/ed Number
///     Tim Christoph Lid       18.06.2017      Kommentare und Header hinzugefügt
///     Tim Christoph Lid       18.06.2017      Anpassen der Größe der Button
///     
namespace BB_8
{
    class Filter
    {
        /// <summary>
        /// Variablen initalisieren
        /// </summary>
        ComboBox comboField0 = new ComboBox();
        ComboBox comboField1 = new ComboBox();
        TextBox textField0 = new TextBox();

        /// <summary>
        /// Gibt die Combobox wieder (Caller,Called etc.)
        /// </summary>
        /// <returns>comboField0</returns>
        public ComboBox GetComboField0()
        {
            return comboField0;
        }

        /// <summary>
        /// Gibt die Combobox wieder (Enthält, Beginnt etc.)
        /// </summary>
        /// <returns>comboField1</returns>
        public ComboBox GetComboField1()
        {
            return comboField1;
        }

        /// <summary>
        /// Gibt die TextBox für die eingebgen Filter wieder
        /// </summary>
        /// <returns>textField0</returns>
        public TextBox GetTextField0()
        {
            return textField0;
        }

        /// <summary>
        /// Für dem ItemField0 ein neues Item hinzu
        /// </summary>
        /// <param name="newItem"></param>
        public void AddItemField0(String newItem)
        {
            comboField0.Items.Add(newItem);
            comboField0.SelectedIndex = 2;
        }

        /// <summary>
        /// Für dem ItemField1 ein neues Item hinzu
        /// </summary>
        /// <param name="newItem"></param>
        public void AddItemField1(String newItem)
        {
            comboField1.Items.Add(newItem);
            comboField1.SelectedIndex = 1;
        }

        /// <summary>
        /// Setz die Layout-Parameter für die Comboboxen
        /// </summary>
        public void SetMargin(ComboBox comboBox, Thickness thickness)
        {
            comboBox.Margin = thickness;
            comboBox.Height = 23;
            comboBox.Width = 123;
            comboBox.VerticalAlignment = VerticalAlignment.Top;
            comboBox.VerticalContentAlignment = VerticalAlignment.Top;
            comboBox.HorizontalAlignment = HorizontalAlignment.Left;
            comboBox.HorizontalContentAlignment = HorizontalAlignment.Left;
        }

        /// <summary>
        /// Setz die Layout-Parameter für die TextBox
        /// </summary>
        internal void SetMargin(TextBox textBox, Thickness thickness)
        {
            textBox.Margin = thickness;
            textBox.Height = 23;
            textBox.Width = 120;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.VerticalContentAlignment = VerticalAlignment.Top;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.HorizontalContentAlignment = HorizontalAlignment.Left;
        }

    }
}
