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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Database m_db;
        private FileExplorer m_fileExplorer;
        private List<string> m_paths = new List<string>() { @"D:\", @"E:\", @"F:\", @"G:\", @"I:\" };
        private bool m_bStop = false;
        public Form1()
        {
            InitializeComponent();
            m_db = new Database();
            m_fileExplorer = new FileExplorer();
            sqlForm.Init(m_db);
            typeof(DataGridView).InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               dataGridView1,
               new object[] { true }
            );

            //tsst
            //dataGridView1.
        }

        private void OnUpdate(string s)
        {
            this.file.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                this.file.Text = s;
            });
        }

        private void fIleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_paths != null && m_paths.Count > 1)
            {
                m_bStop = false;
                m_fileExplorer.ScanPaths(m_paths, OnUpdate, OnFilesGathered);
            }
        }

        private void OnFilesAnalyzed(IAsyncResult result)
        {    
            if (!m_bStop)
            {
                OnUpdate("DONE file analysis");
            }
            else
            {
                OnUpdate("Stopped");
            }
        }
        private void OnFilesGathered(IAsyncResult result)
        {
            if (!m_bStop)
            {
                m_db.AddFiles(m_fileExplorer.GetFilesList(), OnUpdate, OnFilesAnalyzed);
            }
            else
            {
                OnUpdate("Stopped");
            }
        }

        private void OnChecksumsComputed(IAsyncResult result)
        {
            if (!m_bStop)
            {
                OnUpdate("DONE CRC calculation");
            }
            else
            {
                OnUpdate("Stopped");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //m_db.ExecuteQueryToDataSet("SELECT count(*) FROM Files", OnQueryCount);
            m_db.ExecuteQueryToDataSet("SELECT * FROM Files", OnQueryFinisehd);
        }

        private void OnQueryFinisehd(DataSet ds, string error)
        {
            this.dataGridView1.Invoke((MethodInvoker)delegate {
                dataGridView1.DataSource = ds;
                dataGridView1.DataMember = "Files";
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            m_bStop = true;
            m_fileExplorer.Stop();
            m_db.Stop();
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            m_db.ComputeChecksum(OnUpdate, OnChecksumsComputed);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
