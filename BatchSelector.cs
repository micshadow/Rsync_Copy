using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rsync_Copy
{
    public partial class BatchSelector : Form
    {
        Dictionary<string, RemoteGameCollection> _col;
     
        public string _hostKeyName;

        public Dictionary<string, string> returnedItems = new Dictionary<string, string>();
        public BatchSelector(Dictionary<string, RemoteGameCollection> Games, string hostKeyName)
        {
            InitializeComponent();
            _hostKeyName = hostKeyName;
            _col = Games;

           

            for (int i = 0; i < _col[hostKeyName].collection.Length; i++)
            {
                clbGames.Items.Add(_col[hostKeyName].collection[i].Name);
            }

            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbGames.CheckedItems.Count; i++)
            {
                string gameKey = clbGames.SelectedItems[i].ToString();
                RemoteGame game = _col[_hostKeyName].GetItem(gameKey);
                returnedItems.Add(game.Name, game.Path);

            }
            this.Close();
        }
    }
}
