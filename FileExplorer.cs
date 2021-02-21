using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class FileExplorer
    {
        private bool m_bStop = false;
        private List<string> m_files = null;
        
        public FileExplorer()
        {
            m_files = new List<string>();
        }

        public void ScanPaths(List<string> paths, Action<string> OnUpdate, AsyncCallback OnScanFinished)
        {
            m_bStop = false;
            Action<List<string>, Action<string>> getFilesAction = new Action<List<string>, Action<string>>(ScanPathsInternal);
            getFilesAction.BeginInvoke(paths, OnUpdate, OnScanFinished, null);
        }

        private void ScanPathsInternal(List<string> paths, Action<string> UpdateAction)
        {
            m_files.Clear();
            for (int i = 0; i < paths.Count; i++)
            {
               GetAllFilesFromFolderInternal(paths[i], UpdateAction);
            }
        }
        public void Stop()
        {
            m_bStop = true;
        }
        private void GetAllFilesFromFolderInternal(string path, Action<string> UpdateAction)
        {
            if (!m_bStop)
            {
                UpdateAction.Invoke($"Discovering path {path} ");
                try
                {
                    m_files.AddRange(Directory.GetFiles(path));
                    string[] folders = Directory.GetDirectories(path);
                    for (int i = 0; i < folders.Length; i++)
                    {
                        GetAllFilesFromFolderInternal(folders[i], UpdateAction);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public List<string> GetFilesList()
        {
            return m_files;
        }

        public static string GetFormattedSize(long length)
        {
            long B = 0, KB = 1024, MB = KB * 1024, GB = MB * 1024, TB = GB * 1024;
            double size = length;
            string suffix = nameof(B);

            if (length >= TB)
            {
                size = Math.Round((double)length / TB, 2);
                suffix = nameof(TB);
            }
            else if (length >= GB)
            {
                size = Math.Round((double)length / GB, 2);
                suffix = nameof(GB);
            }
            else if (length >= MB)
            {
                size = Math.Round((double)length / MB, 2);
                suffix = nameof(MB);
            }
            else if (length >= KB)
            {
                size = Math.Round((double)length / KB, 2);
                suffix = nameof(KB);
            }

            return $"{size} {suffix}";
        }
    }
}
