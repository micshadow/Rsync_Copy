using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Rsync_Copy.Settings;
using Rsync_Copy.Daemon;

namespace Rsync_Copy
{
    public partial class frmMain : Form
    {
        public Dictionary<string, string> _col = new Dictionary<string, string>();
        GlobalSettings gs = Program.gs;

        public System.Diagnostics.Process DaemonProcess;
        public frmMain()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.reload;
            DoChmodOp();



            txtDestination.Text = gs._defSavePath;
            DoDaemonCheck();
            UpdateHosts();

        }

        public void UpdateHosts()
        {
            if (Program.remoteList.Count > 0)
            {
                string[] keys = new string[Program.remoteList.Keys.Count];
                Program.remoteList.Keys.CopyTo(keys, 0);
                for (int i = 0; i < Program.remoteList.Keys.Count; i++)
                {
                    if (!cmbRemoteHosts.Items.Contains(keys[i]))
                        cmbRemoteHosts.Items.Add(keys[i]);

                }
            }

            if (cmbRemoteHosts.Text == "")
            {
                if (cmbRemoteHosts.Items.Count > 0)
                    cmbRemoteHosts.SelectedIndex = cmbRemoteHosts.Items.Count - 1;
            }
            populateList();
        }

        public delegate void UpdateHostsDelegate();

        void DoChmodOp()
        {
            return;
            System.IO.File.WriteAllText("bin\\password", "password");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "bin\\chmod.exe";
            p.StartInfo.Arguments = "-v 600 bin\\password";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the destination folder for " + cmbFolder.Text;
            fbd.ShowNewFolderButton = true;
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                txtDestination.Text = fbd.SelectedPath;
            }
        }

        public delegate void DaemonInvokeDelegate();

        void DoDaemonCheck()
        {
            if (InvokeRequired) Invoke(new DaemonInvokeDelegate(DoDaemonCheck));
            if (Program.hasBound)
            {
                btnDaemon.Text = "S";
                btnDaemon.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                btnDaemon.Text = "NS";
                btnDaemon.ForeColor = System.Drawing.Color.Red;
            }
        }



        void populateList()
        {
            if (cmbRemoteHosts.Items.Count > 0)
            {
                string key = cmbRemoteHosts.Text;
                if (key == "") return;
                for (int i = 0; i < Program.remoteList[key].collection.Length; i++)
                {
                    if (!cmbFolder.Items.Contains(Program.remoteList[key].collection[i].Name))
                    {
                        cmbFolder.Items.Add(Program.remoteList[key].collection[i].Name);
                    }
                }
            }

            if (cmbFolder.Items.Count > 0)
            {
                cmbFolder.SelectedIndex = 0;

            }

        }

        void AddDestinationSuffix()
        {
            if (txtDestination.Text.EndsWith("\\") && (!(txtDestination.Text == "")))
            {
                txtDestination.Text = txtDestination.Text + cmbFolder.Text;
            }
            else if ((!(txtDestination.Text == "")))
            {
                txtDestination.Text = txtDestination.Text + "\\" + cmbFolder.Text;
            }
        }

        public Dictionary<string, string> GetList(string Host, string HostSMBPath)
        {
            Dictionary<string, string> collection = new Dictionary<string, string>();
            string[] files = System.IO.Directory.GetFiles("\\\\" + Host + "\\" +
               HostSMBPath);

            for (int i = 0; i < files.Length; i++)
            {
                collection.Add(System.IO.Path.GetFileNameWithoutExtension(files[i]),
                    System.IO.File.ReadAllText(files[i]));
                cmbFolder.Items.Add(System.IO.Path.GetFileNameWithoutExtension(files[i]));
            }

            return collection;
        }

        string makersynccommandarg(string host, string LocationName, string localDestination, bool strictMode)
        {

            string remoteHostString = host + "::'" + (LocationName) + "' ";
            string options = "-v --archive --progress --port=" + ServerDaemon.DAEMON_PORT.ToString() + " --recursive -R " +
                (strictMode ? "--delete --ignore-errors --force " : "--update ");
            string localEnd = "" + ConvertWinStringToUnix(localDestination + "/");
            string cmd = options + remoteHostString + localEnd;

            return cmd;
        }

        void doRsync(string Arg, bool WaitForExit = false, bool SaveBatScript = false)
        {
            if (WaitForExit)
            { System.Diagnostics.Process.Start("bin\\rsync.exe", Arg).WaitForExit(); }
            else
            {
                System.Diagnostics.Process.Start("bin\\rsync.exe", Arg);
            }
            if (SaveBatScript)
            {
                System.IO.File.WriteAllText(Environment.CurrentDirectory + "\\bin\\" + cmbFolder.Text + ".bat",
                    "rsync.exe " + Arg);
            }
        }

        void RsyncPass(string arg)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/k SET \"RSYNC_PASSWORD=password\" & \"" + arg.Replace("--password-file=password ", "") + "\"";
            p.Start();
        }

        public static string ConvertWinStringToUnix(string input, bool insertQuotes = true)
        {
            if (IsPathCygwinReady(input)) return input;
            string final;
            final = input;
            final = final.Remove(0, 3);
            final = final.Replace("\\", "/");
            final = final.Insert(0, "/cygdrive/" + input[0] + "/");
            if (insertQuotes)
            {
                final = final.Replace("'", "");
                final = final.Insert(0, "'");
                final = final.Insert(final.Length, "'");
            }
            return final;
        }

        static bool IsPathCygwinReady(string input)
        {
            if (input.StartsWith("/") &&
                (!input.Contains(":")) && (!input.Contains("\\")))
                return true;
            return false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cmbFolder.Text == "") return;
            if (cmbRemoteHosts.Text == "") return;
            string path = cmbFolder.Text;
            doRsync(makersynccommandarg(cmbRemoteHosts.Text, path, txtDestination.Text, gs.StrictMode));
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            gs._defSavePath = txtDestination.Text;
            gs.SaveAllSettings();
            Program.StillRun = false;




        }

        string GetPathFromGameCollection(string host, string Game)
        {
            for (int i = 0; i < Program.remoteList[host].collection.Length; i++)
            {
                if (Program.remoteList[host].collection[i].Name == Game)
                {
                    return Program.remoteList[host].collection[i].Path;
                }
            }
            return "";
        }

        public static void KillRsync(bool force = false)
        {
            System.Diagnostics.Process[] p =
           System.Diagnostics.Process.GetProcessesByName("rsync");
            if (p != null)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    if (p[i].Id == ServerDaemon.daemonPID)
                    {
                        p[i].Kill();
                        p[i].WaitForExit();
                        Program.hasBound = false;
                        return;
                    }
                    else if (force)
                    {
                        p[i].Kill();
                        p[i].WaitForExit();

                    }
                }
            }
        }

        private void btnBatch_Click(object sender, EventArgs e)
        {
            BatchSelector bs = new BatchSelector(Program.remoteList, cmbRemoteHosts.Text);
            bs.ShowDialog();
            string[] keys = new string[bs.returnedItems.Keys.Count];
            bs.returnedItems.Keys.CopyTo(keys, 0);
            if (bs.returnedItems.Count > 0)
            {
                for (int i = 0; i < bs.returnedItems.Count; i++)
                {
                    string host = cmbRemoteHosts.Text;
                    string key = keys[i];
                    string path = bs.returnedItems[key];
                    doRsync(makersynccommandarg(
                        host, key, txtDestination.Text, gs.StrictMode));
                }
            }

        }

        private void btnDaemon_Click(object sender, EventArgs e)
        {
            ToggleServer();
            if (Program.dd != null) Program.dd.BroadcastAvailability();
        }

        void StopServer()
        {
            KillRsync();
            System.Threading.Thread.Sleep(100);
            if (ServerDaemon.IsRsyncDaemonRunning())
            {

                if (MessageBox.Show(
                    "The rsync daemon could not start. Would you like to force close all running rsync processes?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Error,
                     MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    KillRsync(true);
                }

            }
            Program.gs.ServerRunning = false;

        }

        void StartServer()
        {
            DaemonProcess = ServerDaemon.LaunchDaemon(Environment.CurrentDirectory + "\\bin\\rsync.exe");
            DaemonProcess.Exited += new EventHandler(DaemonProcess_Exited);
            DaemonProcess.EnableRaisingEvents = true;
            System.Threading.Thread.Sleep(100);
            Program.gs.ServerRunning = true;
            //Program.hasBound = true;
            //Program.dd.BroadcastAvailability();

        }

        void ToggleServer()
        {
            if (!ServerDaemon.IsRsyncDaemonRunning())
            {
                StartServer();
            }
            else
            {

                StopServer();
            }
            DoDaemonCheck();
        }

        void DaemonProcess_Exited(object sender, EventArgs e)
        {
            DoDaemonCheck();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            Program.fo.ShowDialog();
            populateList();
        }

        private void cmbRemoteHosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRemoteHosts.Text == "") return;
            if (Program.remoteList.ContainsKey(cmbRemoteHosts.Text))
            {
                cmbFolder.Items.Clear();

                populateList();
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            //Program.dd.RequestGameBroadcast();
            if (Program.gs.ServerRunning) StartServer();
            DoDaemonCheck();
            Program.dd.RequestGameBroadcast();
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            //AddDestinationSuffix();
        }



    }
}
