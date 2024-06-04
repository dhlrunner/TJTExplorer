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
            this.lstTJTFiles = new System.Windows.Forms.ListView();
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileOffset = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBox1 = new System.Windows.Forms.TextBox();
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
            this.lstTJTFiles.FullRowSelect = true;
            this.lstTJTFiles.HideSelection = false;
            this.lstTJTFiles.Location = new System.Drawing.Point(12, 82);
            this.lstTJTFiles.Name = "lstTJTFiles";
            this.lstTJTFiles.Size = new System.Drawing.Size(802, 379);
            this.lstTJTFiles.TabIndex = 0;
            this.lstTJTFiles.UseCompatibleStateImageBehavior = false;
            this.lstTJTFiles.View = System.Windows.Forms.View.Details;
            this.lstTJTFiles.DoubleClick += new System.EventHandler(this.lstTJTFiles_DoubleClick);
            // 
            // colFileName
            // 
            this.colFileName.Text = "Filename";
            this.colFileName.Width = 318;
            // 
            // colFileSize
            // 
            this.colFileSize.Text = "Size (Bytes)";
            this.colFileSize.Width = 132;
            // 
            // colFileOffset
            // 
            this.colFileOffset.Text = "Offset";
            this.colFileOffset.Width = 195;
            // 
            // colIndex
            // 
            this.colIndex.Text = "Index";
            this.colIndex.Width = 50;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 56);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(218, 20);
            this.textBox1.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 473);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lstTJTFiles);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstTJTFiles;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colFileSize;
        private System.Windows.Forms.ColumnHeader colIndex;
        private System.Windows.Forms.ColumnHeader colFileOffset;
        private System.Windows.Forms.TextBox textBox1;
    }
}

