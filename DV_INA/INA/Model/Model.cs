using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace INA.Model
{
    // ---------- MODEL ---------------------------------------
    class Model
    {
        #region Members
        FileSplit _FileSplit;
        QueueManagement _QueueManagement;
        DatabaseManagement _databasemanagement;

        Stopwatch stopwatch;

        private List<string> loadedFilesWithAbsolutePath = new List<string>();

        private ObservableCollection<string> loadedFiles = new ObservableCollection<string>();
        private ObservableCollection<string> listViewInfo = new ObservableCollection<string>();

        private double progressStatus;

        private string tbInfo="";

        #endregion

        public Model(LogFile _Logfile, ProgressBarControl pbc)
        {
            _databasemanagement = new DatabaseManagement(_Logfile, pbc);
            MultiTasking _MultiTasking = new MultiTasking(_Logfile, _databasemanagement);
            _QueueManagement = new QueueManagement();
            _FileSplit = new FileSplit(_Logfile, _MultiTasking, pbc);
            
            
        }
        #region Getter/Setter

        public string _tbInfo
        {
            get { return this.tbInfo; }
            set { this.tbInfo = value; }
        }
        public double _progressStatus
        {
            get { return this.progressStatus; }
            set { this.progressStatus = value; }
        }
        public ObservableCollection<string> _listViewInfo
        {
            get { return listViewInfo; }
            set { listViewInfo = value; }
        }
        public string addListViewInfo
        {
            set { 

                this._listViewInfo.Add(value); 
            }
        }
        public ObservableCollection<string> _loadedFiles
        {
            get { return loadedFiles; }
            set { loadedFiles = value; }
        }

        public string setloadedFilesWithAbsolutePath
        {
            set { this.loadedFilesWithAbsolutePath.Add(value); }
        }

        #endregion

        #region Methods

        public void splitFiles()
        {
            _FileSplit.splitFile(loadedFilesWithAbsolutePath);
        }
        //check if the given parameter path already exists in the file path list
        public bool compareFilePath(string path)
        {
            for (int i = 0; i < loadedFilesWithAbsolutePath.Count; i++)
            {
                if (loadedFilesWithAbsolutePath.ElementAt(i).Equals(path))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        public void startTasks()
        {
            _FileSplit.startTasks();
        }

        public void clearFilePath(int index)
        {
            string s1 = loadedFiles.ElementAt(index);
            loadedFiles.Remove(s1);

            string s2 = loadedFilesWithAbsolutePath.ElementAt(index);
            loadedFilesWithAbsolutePath.Remove(s2);
        }

        public void clearMSMQ()
        {
            _QueueManagement.clearMSMQ();
        }

        public int countLoadedFiles()
        {
            return loadedFilesWithAbsolutePath.Count();
        }

        public void startTimer()
        {
            // create new stopwatch
            stopwatch = new Stopwatch();
            // start watch
            stopwatch.Start();
        }

        public string stopTimer()
        {
            stopwatch.Stop();
            return "Time elapsed: " + stopwatch.Elapsed.ToString();
        }
        public bool testDBConnection()
        {
           return _databasemanagement.testDBConnection();
        }

    }
}
