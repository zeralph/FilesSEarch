using Force.Crc32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class Database
    {
		private readonly string sCurrentDBFileVersion = "5";
		SqlCeEngine m_engine;
		private SqlCeConnection m_connection;
		private string m_settingsFilename;
		private bool m_bStop = false;
		private string m_lastError = "";
		//private DataSet m_dataSet = null;
		private string m_lastQuery = "";
		public Database()
        {
			//m_dataSet = new DataSet();
			string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			appDataFolder = Path.Combine(appDataFolder, "SOPaste");
			if (!Directory.Exists(appDataFolder))
			{
				Directory.CreateDirectory(appDataFolder);
			}

			m_settingsFilename = Path.Combine(appDataFolder, "settings.sdf");
			string connectionString = $@"Data Source={m_settingsFilename};Persist Security Info=False;Max Database Size=512";
			bool bDeleteDB = false;
			bool isNewDb = false;
			bool bUpdateDb = false;
			if (!File.Exists(m_settingsFilename))
			{
				isNewDb = true;
			}
			else
			{
				m_connection = new SqlCeConnection(connectionString);
				m_connection.Open();
				if (GetDBFileVersion() != sCurrentDBFileVersion)
				{
					bUpdateDb = true;
				}
			}
			if(bDeleteDB)
			{
				//dialog alert
				//Dialo
				File.Delete(m_settingsFilename);
			}
			if(bUpdateDb)
			{
				UpdateDb();
			}
			if(isNewDb)
			{
				using (m_engine = new SqlCeEngine(connectionString))
				{
					m_engine.CreateDatabase();
				}
			}
			if(m_connection ==null || m_connection.State != ConnectionState.Open)
			{
				m_connection = new SqlCeConnection(connectionString);
				m_connection.Open();
			}
			if (isNewDb)
			{
				CreateDB();
			}
			ReadConfiguration();
		}

		private void UpdateDb()
		{
			int dbVersion = Convert.ToInt32(GetDBFileVersion());
			while(dbVersion < Convert.ToInt32(sCurrentDBFileVersion))
			{
				dbVersion++;
				if(dbVersion == 5)
				{
					SqlCeCommand command = new SqlCeCommand(@"CREATE INDEX idx_checksum ON Files (checksum)", m_connection);
					command.ExecuteNonQuery();
					SqlCeCommand cmdUpdate = new SqlCeCommand($@"UPDATE Settings SET version = @version", m_connection);
					cmdUpdate.Parameters.Add("@version", System.Data.SqlDbType.NVarChar).Value = dbVersion.ToString();
					cmdUpdate.ExecuteNonQuery();
				}
			}
		}

		private void CreateDB()
		{
			SqlCeCommand command = new SqlCeCommand(@"CREATE TABLE Settings (id INT IDENTITY(1,1) PRIMARY KEY, version nvarchar(4000))", m_connection);
			command.ExecuteNonQuery();

			using (SqlCeCommand cmd = new SqlCeCommand($@"INSERT INTO Settings (version) VALUES (@version)", m_connection))
			{
				cmd.Parameters.Add("@version", System.Data.SqlDbType.NVarChar).Value = sCurrentDBFileVersion;
				cmd.ExecuteNonQuery();
			}

			command = new SqlCeCommand(@"CREATE TABLE Files (Id int IDENTITY(1,1) PRIMARY KEY, name nvarchar(4000), path nvarchar(4000), size BIGINT, checksum nvarchar(4000), extension nvarchar(4000), epoch DATETIME)", m_connection);
			command.ExecuteNonQuery();
		}

		public void AddFiles(List<string> files, Action<string> UpdateAction, AsyncCallback cb)
		{
			m_bStop = false;
			Action<List<string>, Action<string>> addFilesAction = new Action<List<string>, Action<string>>(AddFilesInternal);
			addFilesAction.BeginInvoke(files, UpdateAction, cb, null);
		}

		public void ComputeChecksum(Action<string> UpdateAction, AsyncCallback cb)
		{
			Action<Action<string>> computeChecksumACtion = new Action<Action<string>>(CalculateMd5ForAllFiles);
			m_bStop = false;
			computeChecksumACtion.BeginInvoke(UpdateAction, cb, null);
		}

		public void ExecuteQueryToDataSet(string query, Action<DataSet, string> OnQueryFinishded)
		{
			Action<string, Action<DataSet, string>> ActionExecuteQueryToDataSet = new Action<string, Action<DataSet, string>>(ExecuteQueryToDataSetInternal);
			ActionExecuteQueryToDataSet.BeginInvoke(query, OnQueryFinishded, null, null);
		}

		private void ExecuteQueryToDataSetInternal(string query, Action<DataSet, string> OnQueryFinishded)
		{
			DataSet ds = new DataSet();
			try
			{
				//m_dataSet.Clear();
				m_lastError = "ERR_OK";
				m_lastQuery = query;
				using (SqlCeCommand cmd = new SqlCeCommand(m_lastQuery, m_connection))
				{
					SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter(cmd);
					//dataAdapter.Fill(m_dataSet, "Files");
					dataAdapter.Fill(ds, "Files");
				}
				m_lastError = "OK";
			}
			catch (Exception e)
			{
				m_lastError = e.Message.ToString();
			}
			finally
			{
				//OnQueryFinishded.Invoke(m_dataSet, m_lastError);
				OnQueryFinishded.Invoke(ds, m_lastError);
			}
		}

		private void AddDuplicateToDataset(List<string> checksums, ref DataSet ds)
		{
			for(int i=0; i<checksums.Count; i++)
			{
				using (SqlCeCommand cmd2 = new SqlCeCommand($@"SELECT name, path, size FROM files WHERE checksum = @checksum", m_connection))
				{
					cmd2.Parameters.Add("@checksum", System.Data.SqlDbType.NVarChar).Value = checksums[i];
					SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter(cmd2);
					dataAdapter.Fill(ds, "Files");
				}
			}
		}

		public void Stop()
		{
			m_bStop = true;
		}

		private void ComputeDiffsWithDB(List<string> files, ref List<string> filesInDB, ref List<string> filesNotInDB, Action<string> UpdateAction)
		{
			using (SqlCeCommand cmd = new SqlCeCommand($@"SELECT name, path FROM Files", m_connection))
			{
				SqlCeDataReader reader = cmd.ExecuteReader();
				filesInDB.Clear();
				while (reader.Read())
				{
					string p = Convert.ToString(reader["path"]);
					string n = Convert.ToString(reader["name"]);
					p = Path.Combine(p, n);
					filesInDB.Add(p);
				}
				filesNotInDB = files.Except(filesInDB).ToList();
			}
		}

		private void AddFilesInternal(List<string> files, Action<string> UpdateAction)
		{
			List<string> filesInDB = new List<string>();
			List<string> filesNotInDB = new List<string>();

			ComputeDiffsWithDB(files, ref filesInDB, ref filesNotInDB, UpdateAction);

			SqlCeTransaction transaction = m_connection.BeginTransaction();
			int c = filesNotInDB.Count;
			for (int i = 0; i < c; i++)
			{
				if(m_bStop)
				{
					break;
				}
				FileInfo fi = new FileInfo(filesNotInDB[i]);
				string fileName = fi.Name;
				string filePath = fi.DirectoryName;
				UpdateAction.Invoke($"Adding file {i}/{c} {filesNotInDB[i]} ");
				string fileExtension = fi.Extension;
				string fileChecksum = "";
				DateTime fileEpoch = fi.CreationTime;
				long fileSize = fi.Length;
				SqlCeCommand cmd = new SqlCeCommand($@"INSERT INTO Files (name, path, size, checksum, extension, epoch) VALUES (@name, @path, @size, @checksum, @extension, @epoch)", m_connection);
				cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar).Value = fileName;
				cmd.Parameters.Add("@path", System.Data.SqlDbType.NVarChar).Value = filePath;
				cmd.Parameters.Add("@size", System.Data.SqlDbType.BigInt).Value = fileSize;
				cmd.Parameters.Add("@checksum", System.Data.SqlDbType.NVarChar).Value = fileChecksum;
				cmd.Parameters.Add("@extension", System.Data.SqlDbType.NVarChar).Value = fileExtension;
				cmd.Parameters.Add("@epoch", System.Data.SqlDbType.DateTime).Value = fileEpoch;
				cmd.ExecuteNonQuery();
			}
			transaction.Commit();
		}

		private void CalculateMd5ForAllFiles(Action<string> UpdateAction)
		{
			int nbResults = 0;
			int i = 0;
			using (SqlCeCommand cmd = new SqlCeCommand($@"SELECT COUNT(Id) FROM Files WHERE checksum = '' OR checksum IS NULL", m_connection))
			{
				SqlCeDataReader reader = cmd.ExecuteReader();
				reader.Read();
				nbResults = reader.GetFieldValue<int>(0);
			}
			SqlCeTransaction transaction = m_connection.BeginTransaction();
			using (SqlCeCommand cmd = new SqlCeCommand($@"SELECT Id, name, path, size, checksum FROM Files WHERE checksum = '' OR checksum IS NULL", m_connection))
			{
				SqlCeDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					if(m_bStop)
					{
						return;
					}
					i++;
					int  Id = Convert.ToInt32(reader["Id"]);
					long s = Convert.ToInt64(reader["size"]);
					string p = Convert.ToString(reader["path"]);
					string n = Convert.ToString(reader["name"]);
					p = Path.Combine(p, n);
					UpdateAction.Invoke($"Calculate CRC {i}/{nbResults} for {p} ({FileExplorer.GetFormattedSize(s)})");
					string fileChecksum = CalculateMD5(p, s);
					SqlCeCommand cmdUpdate = new SqlCeCommand($@"UPDATE Files SET checksum = @checksum WHERE Id = @Id", m_connection);
					cmdUpdate.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = Id;
					cmdUpdate.Parameters.Add("@checksum", System.Data.SqlDbType.NVarChar).Value = fileChecksum;
					cmdUpdate.ExecuteNonQuery();
				}
				transaction.Commit();
			}
		}

		private string CalculateMD5(string filename, long size)
		{
			if(size == 0 )
			{
				return "0";
			}
			long bufferSize = Math.Min( 1024 * 1024 * 512, size);
			using (var md5 = MD5.Create())
			{
				try
				{
					using (var stream = new BufferedStream(File.OpenRead(filename), (int)bufferSize))
					{

						//SHA256Managed sha = new SHA256Managed();
						byte[] checksum = md5.ComputeHash(stream);
						return BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
					}
				}
				catch (Exception e)
				{
					return e.ToString();
				}
			}
		}

		private bool IsInDatabase(string fileName, string filePath, bool bVerifyChecksum)
		{
			using(SqlCeCommand cmd = new SqlCeCommand($@"SELECT Id, checksum FROM Files WHERE name = @name AND path = @path", m_connection))
			{
				cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar).Value = fileName;
				cmd.Parameters.Add("@path", System.Data.SqlDbType.NVarChar).Value = filePath;
				SqlCeDataReader reader = cmd.ExecuteReader();
				if (reader.Read())
				{
					//if(bVerifyChecksum)
					int id = Convert.ToInt32(reader["Id"]);
					return true;
				}
			}
			return false;
		}

		private void ReadConfiguration()
		{
			
		}
		private string GetDBFileVersion()
		{
			SqlCeCommand command = new SqlCeCommand(@"SELECT * FROM Settings WHERE id=1", m_connection);
			SqlCeDataReader reader = command.ExecuteReader();
			reader.Read();
			try
			{
				if (reader["version"] != null)
				{
					return (string)reader["version"];
				}
			}
			catch(Exception e)
			{
				return "";
			}
			return "";
		}

	}
}
