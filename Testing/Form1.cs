using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataAccess.Repositories;
using DataAccess.BulkLoaders;

namespace Testing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonXLS_Click(object sender, EventArgs e)
        {
            progressXLS.Text = "Starting...";
            backgroundWorkerXLS.RunWorkerAsync();
        }

        private void backgroundWorkerXLS_DoWork(object sender, DoWorkEventArgs e)
        {
            //Get the BackgroundWorker object that raised this event.
            System.ComponentModel.BackgroundWorker worker = (System.ComponentModel.BackgroundWorker)sender;
            //Create XLS Loader
            XLSLoader loader = new XLSLoader(@"C:\Projects\ccg.xls",
                XMLRepositoryFactory.Instance, 
                worker, 
                e);
            loader.Load();
        }
        private void backgroundWorkerXLS_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            progressXLS.Text = string.Format("{0}%: {1}", e.ProgressPercentage, e.UserState);
        }

        private void backgroundWorkerXLS_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            progressXLS.Text = "Import completed.";

        }
    }
}
