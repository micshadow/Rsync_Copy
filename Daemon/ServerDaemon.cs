using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;

namespace Rsync_Copy.Daemon
{
    public class ServerDaemon
    {
        public const int DAEMON_PORT = 4755;
        //public const int DAEMON_PORT = 873;

        public static int daemonPID;
        public static System.Diagnostics.Process LaunchDaemon(string rsyncBinary, int port = DAEMON_PORT, string configFile = "")
        {
            if (configFile == "")
            {
                if (!System.IO.File.Exists(Environment.CurrentDirectory +
                    "\\bin\\rsyncd.conf"))
                {
                    MakeRequiredFiles(Environment.CurrentDirectory + "\\bin");
                }
            }
            else
            {
                if (!System.IO.File.Exists(configFile))
                {
                    System.IO.File.WriteAllText(
                        configFile, MakeDaemonConfig());
                }
            }

            string arg = "--config=" + frmMain.ConvertWinStringToUnix((configFile == "" ? Environment.CurrentDirectory + "\\bin\\rsyncd.conf'" : "'" + configFile + "'")) + " --daemon --no-detach --port=" + port.ToString();
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = rsyncBinary;

            p.StartInfo.Arguments = arg;
            if (Program.hasBound) { frmMain.KillRsync() ; }
            p.Start();
            daemonPID = p.Id;
            if(!Program.remoteList.ContainsKey(Environment.MachineName))
            {
                Program.remoteList.Add(Environment.MachineName, new RemoteGameCollection(new RemoteGame[0], Environment.MachineName));
            }
            Program.dd.BroadcastAvailability();

            Program.hasBound = true;
            System.Threading.Thread.Sleep(100);
            
            return p;
        }

        public static string MakeDaemonConfig()
        {
            string final = "port = " + DAEMON_PORT.ToString() + "\n\n";
            return final;
        }

        public static void MakeRequiredFiles(string directory)
        {
            System.IO.File.WriteAllText(directory + "\\rsyncd.conf", MakeDaemonConfig());
            System.IO.File.WriteAllText(directory + "\\rsyncd.motd", Environment.MachineName + " RCOPY RSYNC SERVER");
            DoChModPerms();
        }

        public static string MakeNewSectionInConf(string name, string path)
        {
            string final = MakeValidSectionString(name) + "\npath = " +
                frmMain.ConvertWinStringToUnix(path,false) + "\nstrict modes = no\n";
            return final;
        }

        public static void RemoveSectionInConf(string name)
        {
            Rsync_Copy.Settings.IniReader ir = new Settings.IniReader(
                Environment.CurrentDirectory + "\\bin\\rsyncd.conf");
            ir.DeleteSection(name);
            
        }

        //now add the code to add these new sections in, plus find a way to map these safe names to code, maybe by using ''s

        static string MakeValidSectionString(string Input)
        {
            string final = Input.Replace("[", "");
            final = final.Replace("]", "");
            final = final.Insert(0, "[");
            final = final.Insert(final.Length, "]");
            return final;
        }

        public static RemoteGameCollection ReadLocalPathsCollection(string confLocation)
        {
            RemoteGameCollection rgc;
            RemoteGame[] collection = null;

            Rsync_Copy.Settings.IniReader ir = new Settings.IniReader(confLocation);
            System.Collections.ArrayList names = ir.GetSectionNames();

            for (int i = 0; i < names.Count; i++)
            {
                if (names[i] == "") continue;
                if (collection == null) collection = new RemoteGame[0];
                Array.Resize(ref collection, collection.Length + 1);
                collection[i] = new RemoteGame((string)names[i],
                    ir.ReadString((string)names[i], "path"));
            }
            if (collection == null) return null;
            rgc = new RemoteGameCollection(collection, Environment.MachineName);
            return rgc;

        }

        public static void AddNewLocation(string Name, string Path, string confLocation)
        {
            if (!System.IO.File.Exists(confLocation))
            {
                //LogError("The specified config file "\" + confLocation + "\" does not exist. Skipping addition of new section string",
                //ErrorType.Warning);
                System.IO.File.WriteAllText(confLocation, MakeDaemonConfig());
            }
            string final = "";
            string originalString = System.IO.File.ReadAllText(confLocation);
            if (!DoesSectionAlreadyExist("[" + Name + "]",
                confLocation))
            {
                final = originalString + "\n" + MakeNewSectionInConf(Name, Path);
            }
            System.IO.File.WriteAllText(confLocation, final);
        }

        public static bool DoesSectionAlreadyExist(string sectionName, string confLocation)
        {
            string contents = System.IO.File.ReadAllText(confLocation);
            if (contents.Contains(sectionName)) return true;
            return false;
        }

        static void DeleteSection(string section, string confLocation)
        {
            if (!System.IO.File.Exists(confLocation))
            {
                //LogError("The specified config file "\" + confLocation + "\" does not exist. Skipping addition of new section string",
                //ErrorType.Warning);
                System.IO.File.WriteAllText(confLocation, MakeDaemonConfig());
            }

            Rsync_Copy.Settings.IniReader ir = new Settings.IniReader(confLocation);
            ir.DeleteSection(section);

        }

        static void DoChModPerms()
        {
            return;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Environment.CurrentDirectory + "\\bin\\chmod.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;



            p.StartInfo.Arguments = "-v 400 rsyncd.scrt";
            p.Start();
            p.WaitForExit();

            p.StartInfo.Arguments = "-v 400 password";
            p.Start();
            p.WaitForExit();


        }

        public static bool IsRsyncDaemonRunning()
        {
            return IsRsyncDaemonRunning_simple();
            //http://stackoverflow.com/a/570461
            int port = DAEMON_PORT; //<--- This is your value
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {

                    if (tcpi.State == (TcpState.Listen | TcpState.TimeWait | TcpState.Established |
                       TcpState.Closing | TcpState.CloseWait))
                    { return true; }

                }
            }
            return false;
            // At this point, if isAvailable is true, we can proceed accordingly.
        }

        public static bool IsRsyncDaemonRunning_simple()
        {
            try
            {

                System.Net.Sockets.TcpListener c = new System.Net.Sockets.TcpListener(DAEMON_PORT);
                c.Start();
                c.Stop();
                return false;
            }
            catch (Exception)
            {
                return true;

            }
        }
    }
}
