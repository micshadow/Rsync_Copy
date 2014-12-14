#undef CATCHCODE
#define CATCHCODE
using System;
using System.Windows.Forms;
using Rsync_Copy.Settings;
using Rsync_Copy.Daemon;

namespace Rsync_Copy
{
    static class Program
    {
        public static System.Collections.Generic.Dictionary<string, RemoteGameCollection> remoteList;
        public static GlobalSettings gs;
        public static frmOptions fo;
        public static Daemon.DaemonDiscovery dd;
        public static bool StillRun, hasBound;
        public static frmMain mainForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

           
#if CATCHCODE 
            try
            {
#endif
                if (IsRSyncAlreadyRunning()) frmMain.KillRsync(true);
            StillRun = true;
            try
            {
                Program.gs = new GlobalSettings(Environment.CurrentDirectory + "\\bin\\settings.ini");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Could not initialise the settings database. Ensure the file \\bin\\settings.ini exists and is accessable\nError: " + ex.Message
                    , "Error in intialisation", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            remoteList = new System.Collections.Generic.Dictionary<string, RemoteGameCollection>();
            AddLocalScriptsToList();
            try
            {
                mainForm = new frmMain();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Could not initialise the main form. Check your settings and try again\nError: " + ex.Message,
                    "Error while loading form", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }

            try
            {
                dd = new Daemon.DaemonDiscovery();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Could not initialise daemon discovery code. Check RCopy is not running, and try again\nError: " + ex.Message,
                    "Could not bind to port", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }


            if (IsRCopyAlreadyRunning()) return;


            if(args!=null && args.Length > 0) HandleCommandLineArgs(args);
               

              
               fo = new frmOptions( gs, remoteList);

             //  ServerDaemon.ReadLocalPathsCollection(@"C:\cygwin\etc\rsyncd.conf");
               dd.InitUDPIfNull();
               //dd.RequestGameBroadcast();
               Application.Run(mainForm);

               dd.Shutdown();
               gs.SaveAllSettings();
               #if CATCHCODE
            }
            catch (Exception ex)
            {
                MessageBox.Show("A critical error has caused Rsync Copy to crash. The message is:\n" +
                    ex.Message + "\nIn method: " + ex.TargetSite.Name, "Error", MessageBoxButtons.OK,
                     MessageBoxIcon.Error);
            } 
#endif
        }

        

        static bool IsRCopyAlreadyRunning()
        {
            System.Diagnostics.Process[] p =
                System.Diagnostics.Process.GetProcessesByName("Rsync_Copy");
            if (p.Length > 0) return true;
            else return false;
        }
        static bool IsRSyncAlreadyRunning()
        {
            System.Diagnostics.Process[] p =
                System.Diagnostics.Process.GetProcessesByName("rsync");
            if (p.Length > 0) return true;
            else return false;
        }

        public static void AddLocalScriptsToList()
        {

            RemoteGameCollection rgc  = ServerDaemon.ReadLocalPathsCollection(
                Environment.CurrentDirectory + "\\bin\\rsyncd.conf");
            if (rgc != null)
            {
                remoteList.Add(Environment.MachineName, rgc);
            }
        }

        public static bool HandleCommandLineArgs(string[] args)
        {
            switch (args[0])
            {

                default:
                    {
                        if(System.IO.Directory.Exists(args[0]))
                        {
                            ServerDaemon.RemoveSectionInConf(
                                new System.IO.DirectoryInfo(args[0]).Name);
                            ServerDaemon.AddNewLocation(new System.IO.DirectoryInfo(args[0]).Name,
                                args[0], Environment.CurrentDirectory + "\\bin\\rsyncd.conf");
                            return true;
                        }
                    }
                    break;
            }
            return true;
        }
    }
}
