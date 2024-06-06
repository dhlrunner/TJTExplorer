namespace TJTExplorer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lstTJTFiles = new System.Windows.Forms.ListView();
            this.colIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileOffset = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.createNewTJTFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openExistTJTFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToTJTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddFiles = new System.Windows.Forms.Button();
            this.btnAddFolder = new System.Windows.Forms.Button();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTJTFileCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pbarTJTSize = new System.Windows.Forms.ProgressBar();
            this.lblSize = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstTJTFiles
            // 
            this.lstTJTFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTJTFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIndex,
            this.colFileName,
            this.colFileSize,
            this.colFileOffset});
            this.lstTJTFiles.ContextMenuStrip = this.contextMenuStrip1;
            this.lstTJTFiles.FullRowSelect = true;
            this.lstTJTFiles.HideSelection = false;
            this.lstTJTFiles.Location = new System.Drawing.Point(12, 85);
            this.lstTJTFiles.Name = "lstTJTFiles";
            this.lstTJTFiles.Size = new System.Drawing.Size(811, 525);
            this.lstTJTFiles.TabIndex = 0;
            this.lstTJTFiles.UseCompatibleStateImageBehavior = false;
            this.lstTJTFiles.View = System.Windows.Forms.View.Details;
            this.lstTJTFiles.SelectedIndexChanged += new System.EventHandler(this.lstTJTFiles_SelectedIndexChanged);
            this.lstTJTFiles.DoubleClick += new System.EventHandler(this.lstTJTFiles_DoubleClick);
            // 
            // colIndex
            // 
            this.colIndex.Text = "Index";
            this.colIndex.Width = 50;
            // 
            // colFileName
            // 
            this.colFileName.Text = "Filename";
            this.colFileName.Width = 322;
            // 
            // colFileSize
            // 
            this.colFileSize.Text = "Size (Bytes)";
            this.colFileSize.Width = 143;
            // 
            // colFileOffset
            // 
            this.colFileOffset.Text = "Offset";
            this.colFileOffset.Width = 135;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripFile});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(835, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripFile
            // 
            this.toolStripFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewTJTFileToolStripMenuItem,
            this.openExistTJTFileToolStripMenuItem,
            this.exportToTJTToolStripMenuItem,
            this.extractAllToolStripMenuItem});
            this.toolStripFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripFile.Image")));
            this.toolStripFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripFile.Name = "toolStripFile";
            this.toolStripFile.Size = new System.Drawing.Size(38, 22);
            this.toolStripFile.Text = "File";
            this.toolStripFile.ToolTipText = "File";
            // 
            // createNewTJTFileToolStripMenuItem
            // 
            this.createNewTJTFileToolStripMenuItem.Name = "createNewTJTFileToolStripMenuItem";
            this.createNewTJTFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.createNewTJTFileToolStripMenuItem.Text = "Create new TJT file";
            this.createNewTJTFileToolStripMenuItem.Click += new System.EventHandler(this.createNewTJTFileToolStripMenuItem_Click);
            // 
            // openExistTJTFileToolStripMenuItem
            // 
            this.openExistTJTFileToolStripMenuItem.Name = "openExistTJTFileToolStripMenuItem";
            this.openExistTJTFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openExistTJTFileToolStripMenuItem.Text = "Open exist TJT file";
            this.openExistTJTFileToolStripMenuItem.Click += new System.EventHandler(this.openExistTJTFileToolStripMenuItem_Click);
            // 
            // exportToTJTToolStripMenuItem
            // 
            this.exportToTJTToolStripMenuItem.Enabled = false;
            this.exportToTJTToolStripMenuItem.Name = "exportToTJTToolStripMenuItem";
            this.exportToTJTToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportToTJTToolStripMenuItem.Text = "Export to TJT";
            this.exportToTJTToolStripMenuItem.Click += new System.EventHandler(this.exportToTJTToolStripMenuItem_Click);
            // 
            // extractAllToolStripMenuItem
            // 
            this.extractAllToolStripMenuItem.Enabled = false;
            this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            this.extractAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extractAllToolStripMenuItem.Text = "Extract All";
            this.extractAllToolStripMenuItem.Click += new System.EventHandler(this.extractAllToolStripMenuItem_Click);
            // 
            // btnAddFiles
            // 
            this.btnAddFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFiles.Enabled = false;
            this.btnAddFiles.Location = new System.Drawing.Point(660, 53);
            this.btnAddFiles.Name = "btnAddFiles";
            this.btnAddFiles.Size = new System.Drawing.Size(77, 23);
            this.btnAddFiles.TabIndex = 3;
            this.btnAddFiles.Text = "파일 추가";
            this.btnAddFiles.UseVisualStyleBackColor = true;
            this.btnAddFiles.Click += new System.EventHandler(this.btnAddFiles_Click);
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFolder.Enabled = false;
            this.btnAddFolder.Location = new System.Drawing.Point(743, 53);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(80, 23);
            this.btnAddFolder.TabIndex = 4;
            this.btnAddFolder.Text = "폴더 추가";
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.CustomFormat = "yyyy/MM/dd";
            this.dtpStartDate.Enabled = false;
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDate.Location = new System.Drawing.Point(13, 56);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(113, 20);
            this.dtpStartDate.TabIndex = 5;
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.CustomFormat = "yyyy/MM/dd";
            this.dtpEndDate.Enabled = false;
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndDate.Location = new System.Drawing.Point(156, 56);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(113, 20);
            this.dtpEndDate.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(132, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "~";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "컨텐츠 버전";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(279, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "총 파일 개수";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTJTFileCount
            // 
            this.lblTJTFileCount.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblTJTFileCount.Location = new System.Drawing.Point(279, 57);
            this.lblTJTFileCount.Name = "lblTJTFileCount";
            this.lblTJTFileCount.Size = new System.Drawing.Size(120, 19);
            this.lblTJTFileCount.TabIndex = 7;
            this.lblTJTFileCount.Text = "-";
            this.lblTJTFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(408, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "사용 용량";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbarTJTSize
            // 
            this.pbarTJTSize.Location = new System.Drawing.Point(411, 57);
            this.pbarTJTSize.Name = "pbarTJTSize";
            this.pbarTJTSize.Size = new System.Drawing.Size(135, 10);
            this.pbarTJTSize.TabIndex = 8;
            // 
            // lblSize
            // 
            this.lblSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSize.Location = new System.Drawing.Point(410, 67);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(136, 10);
            this.lblSize.TabIndex = 9;
            this.lblSize.Text = "0/0";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 619);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(835, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(31, 17);
            this.toolStripStatusLabel1.Text = "준비";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportSelectedToolStripMenuItem,
            this.removeSelectedToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(164, 48);
            // 
            // exportSelectedToolStripMenuItem
            // 
            this.exportSelectedToolStripMenuItem.Name = "exportSelectedToolStripMenuItem";
            this.exportSelectedToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exportSelectedToolStripMenuItem.Text = "Export selected";
            // 
            // removeSelectedToolStripMenuItem
            // 
            this.removeSelectedToolStripMenuItem.Name = "removeSelectedToolStripMenuItem";
            this.removeSelectedToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.removeSelectedToolStripMenuItem.Text = "Remove selected";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 641);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.pbarTJTSize);
            this.Controls.Add(this.lblTJTFileCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.btnAddFolder);
            this.Controls.Add(this.btnAddFiles);
            this.Controls.Add(this.lstTJTFiles);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "TJT Explorer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstTJTFiles;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colFileSize;
        private System.Windows.Forms.ColumnHeader colIndex;
        private System.Windows.Forms.ColumnHeader colFileOffset;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripFile;
        private System.Windows.Forms.ToolStripMenuItem createNewTJTFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openExistTJTFileToolStripMenuItem;
        private System.Windows.Forms.Button btnAddFiles;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem exportToTJTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblTJTFileCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar pbarTJTSize;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedToolStripMenuItem;
    }
}

