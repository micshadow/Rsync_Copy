using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rsync_Copy
{
    public partial class frmEditor : Form
    {
        public frmEditor()
        {
            InitializeComponent();
        }

        public frmEditor(string selectedPath = "")
        {
            InitializeComponent();
            txtPath.Text = selectedPath;
        }

        public static void GetDirectory(out string Name,out string Path,string OptionalName="")
        {
            frmEditor ed = new frmEditor();
            ed.txtName.Text = OptionalName;
            ed.ShowDialog();

            Name = ed.txtName.Text;
            Path = ed.txtPath.Text;
            ed.Dispose();
            ed = null;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the folder to add";
            if (System.IO.Directory.Exists(txtPath.Text)) fbd.SelectedPath = txtPath.Text;
            fbd.ShowNewFolderButton = true;
            fbd.ShowDialog();
            if (System.IO.Directory.Exists(fbd.SelectedPath)) txtPath.Text = fbd.SelectedPath;
        }
        
        
        public static bool ContainsInvalidCharacters(string input)
        {
            char[] invalidchars = System.IO.Path.GetInvalidPathChars();
            System.Collections.ArrayList al = new System.Collections.ArrayList(invalidchars);

            for (int i = 0; i < input.Length; i++)
            {
                if (al.Contains(input[i])) return true;
            }

            return false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
