using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace WindowsFormsApp1
{
    public partial class SQLForm : UserControl
    {
        private Database m_db = null;
        public SQLForm()
        {
            InitializeComponent();
            typeof(DataGridView).InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               sqlDatagridView,
               new object[] { true }
            );
        }

        public void Init(Database db)
        {
            m_db = db;
        }

        private void sqlExecuteButton_Click(object sender, EventArgs e )
        {
            string query = sqlQuery.Text;
            errorText.Enabled = false;
            sqlQuery.Enabled = false;
            sqlDatagridView.DataSource = null;
            m_db.ExecuteQueryToDataSet(query, OnQueryFinisehd);
        }
        private void OnQueryFinisehd(DataSet ds, string error)
        {
            this.sqlDatagridView.Invoke((MethodInvoker)delegate {
                if (ds.Tables.Count > 0)
                {
                    sqlDatagridView.DataSource = ds;
                    sqlDatagridView.DataMember = "Files";
                    sqlDatagridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                    sqlDatagridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    /*
                    foreach (DataGridViewColumn column in sqlDatagridView.Columns)
                    {
                        if (column.Index == sqlDatagridView.Columns.Count-1) //Last column will fill extra space
                        {
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                        else //Any other column will be sized based on the max content size
                        {
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                    }
                    */
                }
            });
            this.errorText.Invoke((MethodInvoker)delegate {
                errorText.Text = error;
                errorText.Enabled = true;
            });
            this.sqlQuery.Invoke((MethodInvoker)delegate {
                sqlQuery.Enabled = true;
            });
            this.button1.Invoke((MethodInvoker)delegate {
                button1.Enabled = true;
            });
        }

        private void UpdateStatus(string msg)
        {
            this.errorText.Invoke((MethodInvoker)delegate {
                errorText.Text = msg;
            });
        }

        private void UpdateProgressBar(int a, int b)
        {
            this.progressBar1.Invoke((MethodInvoker)delegate {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = b;
                progressBar1.Value = a;
            });
        }

        private void ExecutDuplicateFindQuery(string checksum, int curIdx, int maxIndex, DataSet finalDataset, string finalError)
        {
            UpdateStatus($"Processing duplicate {curIdx+1}/{maxIndex}");
            UpdateProgressBar(curIdx, maxIndex);
            string query = $"SELECT name, path, size, extension, epoch FROM Files WHERE checksum = '{checksum}' ORDER BY path, name ASC";
            m_db.ExecuteQueryToDataSet(query, (ds2, error2) =>
            {
                finalError = error2;
                lock(finalDataset)
                {
                    finalDataset.Merge(ds2.Tables[0]);
                }
                curIdx++;
                if (curIdx == maxIndex - 1)
                {
                    UpdateStatus($"Displaying results");
                    //need something better ....
                    Thread.Sleep(1000);
                    OnQueryFinisehd(finalDataset, finalError); 
                }
            });
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ExecuteFindDuplicates();
        }

        private void ExecuteFindDuplicates___()
        {
            button1.Enabled = false;
            string query = "SELECT checksum FROM FILES";
            DataSet finalDataset = new DataSet();
            string finalError = "";
            m_db.ExecuteQueryToDataSet(query, (ds, error) =>
            {
                finalError = error;
                int count = ds.Tables[0].Rows.Count;
                List<string> checksums = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    checksums.Add( ds.Tables[0].Rows[i].Field<string>("checksum") );
                }
                List<string> uniques = checksums.Distinct().ToList();
                for(int i=0; i<uniques.Count; i++)
                {
                    checksums.Remove(uniques[i]);
                }
                checksums = checksums.Distinct().ToList();
                count = checksums.Count;
                for (int i = 0; i < count; i++)
                {
                    string s = checksums[i];
                    ExecutDuplicateFindQuery(s, i, count, finalDataset, finalError);
                }

            });
        }

        private void ExecuteFindDuplicates()
        {
            button1.Enabled = false;
            string query = "SELECT COUNT(checksum) as count, checksum, size FROM FILES GROUP BY checksum, size HAVING count(checksum) > 1 AND checksum != '0' ORDER BY size DESC";
            DataSet finalDataset = new DataSet();
            string finalError = "";
            m_db.ExecuteQueryToDataSet(query, (ds, error) =>
            {
                finalError = error;
                int count = ds.Tables[0].Rows.Count;
                List<string> checksums = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    string s = ds.Tables[0].Rows[i].Field<string>("checksum");
                    //checksums.Add( ds.Tables[0].Rows[i].Field<string>("checksum") );
                    ExecutDuplicateFindQuery(s, i, count, finalDataset, finalError);
                }
                //ExecutDuplicateFindQuery(checksums, 0, finalDataset, finalError);
            });
        }
    }
}
