using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TJTExplorer.Class;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TJTExplorer
{
    public partial class Form1 : Form
    {
        private TJTar TJTFile;
        public Form1()
        {
            InitializeComponent();
        }

        #region Custom methods

        private void RefreshFileList()
        {
            lstTJTFiles.Items.Clear();

            foreach (TJTarFile file in TJTFile.Files)
            {

                var item = new ListViewItem(new string[] {
                    file.Index.ToString(),
                    file.Name,
                    file.Size.ToString(),
                    file.StartOffset.ToString("X8")
                });
                item.Tag = file;

                if (file.IsNew)
                {
                    item.UseItemStyleForSubItems = false;
                    item.BackColor = Color.LimeGreen;
                }

                lstTJTFiles.Items.Add(item);
               
                
            }
            lblTJTFileCount.Text = $"{TJTFile.Files.Count}개";
            lblSize.Text = $"{TJTFile.GetTotalFileSize()}/{uint.MaxValue}";
            pbarTJTSize.Maximum = 100;
            int v = (int)Math.Round((double)TJTFile.GetTotalFileSize() / (double)uint.MaxValue * 100.0, 0);
            pbarTJTSize.Value = v > 100 ? 100 : v;
            dtpStartDate.Value = TJTFile.ContentStartDate;
            dtpEndDate.Value = TJTFile.ContentEndDate;
        }
        private void LoadTJT(string filePath)
        {
            TJTFile = new TJTar();
            TJTFile.LoadFromFile(filePath);

            RefreshFileList();

            btnAddFiles.Enabled = true;
            btnAddFolder.Enabled = true;
            exportToTJTToolStripMenuItem.Enabled = true;
            extractAllToolStripMenuItem.Enabled = true;
            dtpEndDate.Enabled = true;
            dtpStartDate.Enabled = true;

            this.Text = $"TJT Explorer - [{Path.GetFileName(filePath)}({TJTFile.Version})]";
        }

        

        private string GetRelativePath(string fullPath, string folder)
        {
            Uri pathUri = new Uri(fullPath);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        #endregion

        #region Form event handling
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void lstTJTFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lstTJTFiles.SelectedItems.Count > 0)
            {
                TJTarFile file = (TJTarFile)lstTJTFiles.SelectedItems[0].Tag;
                
                if (!file.IsNew) 
                {
                    if (!Directory.Exists("./temp"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "./temp");
                    }

                    if (!Directory.Exists("./temp/" + Path.GetDirectoryName(file.Name)))
                    {
                        Directory.CreateDirectory("./temp/" + Path.GetDirectoryName(file.Name));
                    }

                    Task extTask = TJTFile.ExtractFileAsync(file, "./temp/" + file.Name);

                    extTask.GetAwaiter().OnCompleted(() => {
                        try
                        {
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.FileName = "temp\\" + file.Name;
                            psi.UseShellExecute = true;
                            psi.WorkingDirectory = Application.StartupPath;
                            Process.Start(psi);
                        }
                        catch (Win32Exception)
                        {
                            MessageBox.Show($"\"{file.Name}\"\n연결된 어플리케이션이 없어 직접 파일을 열 수 없습니다.", "파일 열기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }


                        Console.WriteLine("Extract task end");
                    });

                    Console.WriteLine("Extract task started");
                }

               
            }

        }

        private void openExistTJTFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TJMedia Tar file|*.TJT|All file|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Open existing TJT file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadTJT(openFileDialog.FileName);
                 //_ = this.TJTFileForRead.BuildAsync("test.tjt");
            }
        }
        private void lstTJTFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            
            //folderBrowserDialog.Description = "";
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Task.Factory.StartNew(() =>
                {
                    this.Invoke((Action)(() =>
                    {

                        toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
                        toolStripStatusLabel1.Text = "파일/폴더 정보 가져오는 중";
                        string folder = folderBrowserDialog.SelectedPath;
                        string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
                        toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
                        toolStripStatusLabel1.Text = "추가 중..";
                        (string, string)[] fileNamePair = new (string, string)[files.Length];

                        toolStripProgressBar1.Maximum = files.Length;
                        toolStripProgressBar1.Value = 0;
                        for (int i = 0; i < files.Length; i++)
                        {
                            fileNamePair[i] = (GetRelativePath(files[i], folder), files[i]);
                            toolStripProgressBar1.Value = i;
                        }
                        toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
                        toolStripStatusLabel1.Text = "추가 중..";
                        try
                        {
                            //Application.DoEvents();
                            Task t = TJTFile.AddFilesAsync(fileNamePair);
                            t.GetAwaiter().OnCompleted(() =>
                            {
                                RefreshFileList();
                                toolStripStatusLabel1.Text = $"{files.Length}개 파일 추가됨";
                                toolStripProgressBar1.Value = 0;
                                toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
                            });
                            t.ContinueWith((E) => 
                            {
                                
                            });
                        }
                        catch (TJTarFileSizeOverException)
                        {
                            MessageBox.Show("전체 크기가 4GB를 초과하여 더 이상 파일을 추가할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                       
                    }));
                    
                });
                
                //t.Start();
                
            }
           
        }

        private void createNewTJTFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TJTFile = new TJTar();
            RefreshFileList();

            btnAddFiles.Enabled = true;
            btnAddFolder.Enabled = true;
            exportToTJTToolStripMenuItem.Enabled = true;
            dtpEndDate.Enabled = true;
            dtpStartDate.Enabled = true;

            this.Text = $"TJT Explorer - [New file({TJTFile.Version})]";

            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "TJMedia Tar file|*.TJT|All file|*.*";
            //saveFileDialog.DefaultExt = ".TJT";
            //saveFileDialog.Title = "Create new TJT file";
            //if( saveFileDialog.ShowDialog() == DialogResult.OK)
            //{

            //}
        }
        private async void btnAddFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All file|*.*";
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Select a file(s) to add";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                

                await Task.Factory.StartNew(() => 
                {
                    this.Invoke((Action)(() => 
                    {
                        btnAddFiles.Enabled = false;
                        btnAddFolder.Enabled = false;
                        openExistTJTFileToolStripMenuItem.Enabled = false;
                        createNewTJTFileToolStripMenuItem.Enabled = false;
                        exportToTJTToolStripMenuItem.Enabled = false;
                        extractAllToolStripMenuItem.Enabled = false;

                        var p = new Progress<int>((cnt) =>
                        {
                            this.Invoke((Action)(() =>
                            {
                                toolStripProgressBar1.Value = cnt + 1;
                                toolStripStatusLabel1.Text = $"추가 중... ({cnt + 1}/{toolStripProgressBar1.Maximum})";
                                //Application.DoEvents();
                            }));
                        });

                        toolStripProgressBar1.Maximum = openFileDialog.FileNames.Length;
                        

                        (string, string)[] fileNamePair = new (string, string)[openFileDialog.FileNames.Length];

                        toolStripStatusLabel1.Text = "파일 정보 분석 중..";
                        //여기 왜그런지 모르겠는데 엄청 느림
                        for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                        {
                            string fn = openFileDialog.FileNames[i];
                            string n = fn.Substring(fn.LastIndexOf('\\')+1); //파일명만 떼오기
                            fileNamePair[i] = (n, fn);
                            toolStripProgressBar1.Value = i + 1;
                            toolStripStatusLabel1.Text = $"파일 정보 분석 중.. ({i + 1}/{toolStripProgressBar1.Maximum})";
                            Application.DoEvents(); //Task.Factory 쓰고 있는데 왜 이거 안해주면 UI 스레드가 차단되는지 모르겠음
                        }
                        
                        
                        
                        Task t = TJTFile.AddFilesAsync(fileNamePair, p);
                        t.GetAwaiter().OnCompleted(() =>
                        {
                            RefreshFileList();
                            toolStripStatusLabel1.Text = $"{openFileDialog.FileNames.Length}개 파일 추가됨";
                            btnAddFiles.Enabled = true;
                            btnAddFolder.Enabled = true;
                            openExistTJTFileToolStripMenuItem.Enabled = true;
                            createNewTJTFileToolStripMenuItem.Enabled = true;
                            exportToTJTToolStripMenuItem.Enabled = true;
                            extractAllToolStripMenuItem.Enabled = true;
                        });
                        t.ContinueWith(ex =>
                        {
                            if (ex.Exception.InnerExceptions[0] is TJTarFileSizeOverException)
                            {
                                MessageBox.Show("전체 크기가 4GB를 초과하여 더 이상 파일을 추가할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }, TaskContinuationOptions.OnlyOnFaulted);
                        //t.Start();

                        
                    }));

                   
                });
                
            }
        }

        private void exportToTJTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TJMedia Tar file|*.TJT";
            saveFileDialog.Title = "Select TJT Output name";
            saveFileDialog.DefaultExt = ".TJT";

            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                toolStripProgressBar1.Maximum = TJTFile.Files.Count;

                var p = new Progress<int>((cnt) => 
                {
                    this.Invoke((Action)(() => 
                    {
                        toolStripProgressBar1.Value = cnt+1;
                        toolStripStatusLabel1.Text = $"TJT 빌드 중 ({cnt+1}/{toolStripProgressBar1.Maximum})";

                    }));
                });

                Task.Factory.StartNew(() =>
                {
                    this.Invoke((Action)(() =>
                    {
                        toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
                        toolStripStatusLabel1.Text = "TJT 빌드 중";
                        exportToTJTToolStripMenuItem.Enabled = false;
                        btnAddFiles.Enabled = false;
                        btnAddFolder.Enabled = false;
                        TJTFile.SetContentDate(dtpStartDate.Value, dtpEndDate.Value);
                        TJTFile.BuildAsync(saveFileDialog.FileName, p).GetAwaiter().OnCompleted(() =>
                        {
                            toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
                            toolStripStatusLabel1.Text = $"TJT 빌드 완료. {saveFileDialog.FileName}";
                            exportToTJTToolStripMenuItem.Enabled = true;
                            btnAddFiles.Enabled = true;
                            btnAddFolder.Enabled = true;
                        });
                    }));
                });
            }
           
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstTJTFiles.SelectedItems.Count > 0)
            {
                uint count = 0;
                //TJTarFile[] file = new TJTarFile[lstTJTFiles.SelectedItems.Count];
                //추출할 파일이 1개 이상인지 확인
                for (int i = 0; i < lstTJTFiles.SelectedItems.Count; i++)
                {
                    TJTarFile f = (TJTarFile)lstTJTFiles.SelectedItems[i].Tag;
                    if (!f.IsNew) count++;
                    //file[i] = f;
                }

                if (count == 0)
                {
                    MessageBox.Show("추출 가능한 파일이 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {

                    Task.Factory.StartNew(() => {
                        string rootDir = folderDialog.SelectedPath;
                        this.Invoke((Action)(() =>
                        {
                            toolStripStatusLabel1.Text = "선택한 파일 추출 중...";
                            toolStripProgressBar1.Maximum = (int)count;
                            toolStripProgressBar1.Value = 0;
                            foreach (ListViewItem item in lstTJTFiles.SelectedItems)
                            {
                                TJTarFile f = (TJTarFile)item.Tag;
                                if (!f.IsNew)
                                {

                                    if (!Directory.Exists(rootDir + "\\" + Path.GetDirectoryName(f.Name)))
                                    {
                                        Directory.CreateDirectory(rootDir + "\\" + Path.GetDirectoryName(f.Name));
                                    }

                                    TJTFile.ExtractFile(f, rootDir + "\\" + f.Name);
                                    toolStripProgressBar1.Value++;
                                    toolStripStatusLabel1.Text = $"선택한 파일 추출 중... ({toolStripProgressBar1.Value}/{toolStripProgressBar1.Maximum})";
                                    Application.DoEvents();
                                }
                            }
                            toolStripStatusLabel1.Text = $"선택된 {count}개 파일을 \"{rootDir}\" 폴더에 추출했습니다.";
                        }));


                    });
                }


            }
        }
        #endregion


    }

    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this System.Windows.Forms.ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }

}
