using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rsync_Copy
{
    public partial class frmOptions : Form
    {
        Rsync_Copy.Settings.GlobalSettings _gs;
        Dictionary<string, RemoteGameCollection> _remoteGameList;

        public frmOptions()
        {
            InitializeComponent();

        }

        public frmOptions(Rsync_Copy.Settings.GlobalSettings gs, Dictionary<string, RemoteGameCollection> remoteGameList)
        {
            InitializeComponent();
            _gs = gs;
            _remoteGameList = remoteGameList;
            chbStrictMode.Checked = gs.StrictMode;
            FillList();


        }

        void FillList()
        {
            if (_remoteGameList != null)
            {
                if (_remoteGameList.Count < 1) return;
                try
                {
                    for (int i = 0; i < _remoteGameList[Environment.MachineName].collection.Length; i++)
                    {
                        string text = _remoteGameList[Environment.MachineName].collection[i].Name;
                        if (lstGames.Items.Contains(text)) continue;
                        lstGames.Items.Add(text);
                    }
                }
                catch
                {
                    MessageBox.Show("There was an error populating the game collection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name, path;
            frmEditor.GetDirectory(out name, out path);
            if (name == "") return;
            if (!System.IO.Directory.Exists(path)) return;
            if (frmEditor.ContainsInvalidCharacters(path))
            {
                MessageBox.Show("The path '" + path + "' contains invalid characters. Please check the path and try again"
                    , "Invalid path characters", MessageBoxButtons.OK, MessageBoxIcon.Error,
                     MessageBoxDefaultButton.Button1);
                return;
            }
            if (name == "") return;
            lstGames.Items.Add(name);
            Program.remoteList[Environment.MachineName].AddItem(
                new RemoteGame(name, path));
            Rsync_Copy.Daemon.ServerDaemon.AddNewLocation(name, path,
                   Environment.CurrentDirectory + "\\bin\\rsyncd.conf");
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstGames.SelectedItems.Count > 0)
            {
                string exisName;

                exisName = lstGames.SelectedItems[0].ToString();

                string name, path;
                frmEditor.GetDirectory(out name, out path, exisName);
                if (name == "") return;
                if (!System.IO.Directory.Exists(path)) return;
                if (frmEditor.ContainsInvalidCharacters(path))
                {
                    MessageBox.Show("The path '" + path + "' contains invalid characters. Please check the path and try again"
                        , "Invalid path characters", MessageBoxButtons.OK, MessageBoxIcon.Error,
                         MessageBoxDefaultButton.Button1);
                    return;
                }
                lstGames.Items[lstGames.SelectedIndex] = name;
                if (Program.remoteList[Environment.MachineName].ContainsKey(exisName))
                {
                    Program.remoteList[Environment.MachineName].EditItem(exisName, name, path);
                }
                Rsync_Copy.Daemon.ServerDaemon.RemoveSectionInConf(name);
                Rsync_Copy.Daemon.ServerDaemon.AddNewLocation(name, path,
                    Environment.CurrentDirectory + "\\bin\\rsyncd.conf");


            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstGames.SelectedItems.Count > 0)
            {
                string exisName;

                exisName = lstGames.SelectedItems[0].ToString();

                if (lstGames.SelectedItem.ToString() == "") { lstGames.Items.RemoveAt(lstGames.SelectedIndex); return; }

                if (Program.remoteList[Environment.MachineName].ContainsKey(exisName))
                {
                    Program.remoteList[Environment.MachineName].RemoveItem(exisName);
                }
                Rsync_Copy.Daemon.ServerDaemon.RemoveSectionInConf(exisName);
                lstGames.Items.RemoveAt(lstGames.SelectedIndex);


            }
        }

        private void frmOptions_Shown(object sender, EventArgs e)
        {
            FillList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            this.Close();
            _gs.StrictMode = chbStrictMode.Checked;
            Program.dd.BroadcastAvailability();

        }

        private void lstGames_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnEdit_Click(this, new EventArgs());
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string txt = "This will completely delete all settings, games and configurations, as well as restart the program. Are you sure?";
            if (System.Windows.Forms.MessageBox.Show(txt, "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                == System.Windows.Forms.DialogResult.Yes)
            {
                List<string> deffiles = new List<string>(new string[] { "cygiconv-2.dll", "cygwin1.dll", "rsync.exe", "cygintl-8.dll", "chmod.exe" });
                string[] files = System.IO.Directory.GetFiles(Environment.CurrentDirectory + "\\bin", "*");
                foreach (string s in files)
                {
                    string fname = System.IO.Path.GetFileName(s);
                    if(!deffiles.Contains(fname))
                    {
                        System.IO.File.Delete(s);
                    }
                }
            }
            Application.Restart();
        }


    }
}
