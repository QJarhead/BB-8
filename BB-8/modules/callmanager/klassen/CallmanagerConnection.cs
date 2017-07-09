using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB_8
{
    ///     Description:            Klasse für die Callmanager Analyse, zeigt Telefonate vom Callmanager an
    ///     Namespace:              BB_8
    ///     Class:                  Call_Analysis
    ///     Author:                 Tim Christoph Lid                    
    ///     Notes:                  Erstelle Callmanager Reports
    ///     Revision History:
    ///     Name:                   Date:           Description:
    ///   
    ///     Tim Christoph Lid       23.06.2017      Erstellt
    ///     Tim Chriopph Lid        23.06.2017      Auslagern der CallmanagerConnection in eine eigene Klasse
    ///     Tim Christoph Lid       23.06.2017      Umstellen, dass die Verbindung des Callmanagers Global ist, um von anderen Klassen ebenfalls zugreifen zu können
    ///     Tim Christoph Lid       02.07.2017      Erweitern der execute()-Methode, für den Fall das die SSH-Session verworfen wurde, dadurch wir connectedToCallmanager auf false gesetzt, die Klassen erkennen dies und bauen die Session neu auf
    ///     Tim Christoph Lid       07.07.2017      Enternen der CallmanagerConnectionsStrings für IP und Credentials, diese werden nun aus den Settings gelesen
    ///     
    public class CallmanagerConnection
    {
        public bool debug_SQL = false;
        public bool conectedToCallmanager = false;
        public bool connectionFailed = false;
        private System.ComponentModel.BackgroundWorker ConnectToCallmanager = new BackgroundWorker();
        private System.ComponentModel.BackgroundWorker ConnectingToCallmanager = new BackgroundWorker();
        SshClient client;
        public ShellStream sshStream;

        public CallmanagerConnection()
        {
            ConnectToCallmanager.DoWork += connectToCallmanager;
            ConnectToCallmanager.RunWorkerCompleted += connectedToCallmanager;
            if(!debug_SQL) ConnectToCallmanager.RunWorkerAsync();

        }

        /// <summary>
        /// Gibt aus, dass die Verbindung zum Callmanager aufgebaut wurde konnte, und der Backgroundworkerprozess beendet ist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectedToCallmanager(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!connectionFailed)
            {
                conectedToCallmanager = true;
            }
        }

        /// <summary>
        /// Baut mit Hilfe von Backgroundworkern die Verbindung zum Callmanager Asynchron auf, damit die GUI bereits genutzt werden kann
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectToCallmanager(object sender, DoWorkEventArgs e)
        {
            var connectionInfo = new ConnectionInfo(Properties.Settings.Default.CallmanagerIPAddress, Properties.Settings.Default.CallmanagerUsername,
                      new PasswordAuthenticationMethod(Properties.Settings.Default.CallmanagerUsername, Properties.Settings.Default.CallmanagerPassword),
                      new PrivateKeyAuthenticationMethod("rsa.key"));
            client = new SshClient(connectionInfo);

            try
            {
                client.Connect();
                sshStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
                var output = sshStream.Expect("admin:");
            }
            catch (Exception)
            {
                connectionFailed = true;
            }

        }

        public String execute(String command)
        {
            try
            {
                sshStream.WriteLine(command);
            }
            catch (Exception)
            {
                conectedToCallmanager = false;
                ///connectToCallmanager();
            }
            var output = sshStream.Expect("admin:");
            return output.ToString();
        }
    }
}
