namespace Testing
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
            this.buttonXLS = new System.Windows.Forms.Button();
            this.backgroundWorkerXLS = new System.ComponentModel.BackgroundWorker();
            this.progressXLS = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonXLS
            // 
            this.buttonXLS.Location = new System.Drawing.Point(12, 12);
            this.buttonXLS.Name = "buttonXLS";
            this.buttonXLS.Size = new System.Drawing.Size(106, 31);
            this.buttonXLS.TabIndex = 0;
            this.buttonXLS.Text = "Load XLS";
            this.buttonXLS.UseVisualStyleBackColor = true;
            this.buttonXLS.Click += new System.EventHandler(this.buttonXLS_Click);
            // 
            // backgroundWorkerXLS
            // 
            this.backgroundWorkerXLS.WorkerReportsProgress = true;
            this.backgroundWorkerXLS.WorkerSupportsCancellation = true;
            this.backgroundWorkerXLS.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerXLS_DoWork);
            this.backgroundWorkerXLS.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerXLS_ReportProgress);
            this.backgroundWorkerXLS.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerXLS_Completed);
            // 
            // progressXLS
            // 
            this.progressXLS.Enabled = false;
            this.progressXLS.Location = new System.Drawing.Point(124, 16);
            this.progressXLS.Name = "progressXLS";
            this.progressXLS.Size = new System.Drawing.Size(100, 22);
            this.progressXLS.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 255);
            this.Controls.Add(this.progressXLS);
            this.Controls.Add(this.buttonXLS);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonXLS;
        private System.ComponentModel.BackgroundWorker backgroundWorkerXLS;
        private System.Windows.Forms.TextBox progressXLS;
    }
}

