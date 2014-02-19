using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KataBankOCR
{
    public partial class RunOCR : Form
    {
        private BackgroundWorker _worker = new BackgroundWorker();

        public RunOCR()
        {
            InitializeComponent();
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_Completed;
            _worker.WorkerReportsProgress = true;
            _worker.ProgressChanged += Worker_ProgressChanged;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            pbProgress.Maximum = 100;
            rtbResult.Text = "";
            _worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var lines = File.ReadAllLines(string.Format("{0}{1}", baseDir, Properties.Settings.Default.OCRTestFilePath));
            var p = new OCRProcessor(_worker);
            p.ProcessOCRFile(lines);
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            pbProgress.Value = pbProgress.Maximum;
            rtbResult.Text += Environment.NewLine + Environment.NewLine;
            rtbResult.Text += "Processing complete." + Environment.NewLine;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
            rtbResult.Text += e.UserState.ToString() + Environment.NewLine;
        }
    }
}
