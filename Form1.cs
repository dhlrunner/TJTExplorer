using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TJTExplorer.Class;

namespace TJTExplorer
{
    public partial class Form1 : Form
    {
        private TJT TJTfile = new TJT();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            TJTfile.loadFromFile("C:\\tjmedia_web\\202405_UHD.TJT");

            textBox1.Text = TJTfile.contentVersion;

            lstTJTFiles.Items.Clear();

            foreach (TJTFile file in TJTfile.files)
            {
                var item = new ListViewItem(new string[] {
                    file.fileIndex.ToString(),
                    file.fileName,
                    file.fileSize.ToString(),
                    file.fileStartOffset.ToString("X2")
                });
                item.Tag = file;
                lstTJTFiles.Items.Add(item);
            }


            
        }

        private void lstTJTFiles_DoubleClick(object sender, EventArgs e)
        {
            if(lstTJTFiles.SelectedItems.Count > 0)
            {
                TJTFile file = (TJTFile)lstTJTFiles.SelectedItems[0].Tag;
                _ = TJTfile.extractFileAsync(file, "./" + file.fileName);


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = file.fileName;
                psi.UseShellExecute = true;
                psi.WorkingDirectory = Application.StartupPath;
                Process.Start(psi);
            }
            
        }
    }
}
