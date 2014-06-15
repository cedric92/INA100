using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INA.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace INA.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region NotifiyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected internal void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion

        #region Members

        Model.Model _Model;
        LogFile _Logfile;
        ProgressBarControl _ProgressBarControl;

        #endregion

        public ViewModel()
        {
            _Logfile = new LogFile(this);
            _ProgressBarControl = new ProgressBarControl(this);
            _Model = new Model.Model(_Logfile, _ProgressBarControl);
            
        }

        #region Getter/Setter

        public string _tbInfo
        {
            get { return _Model._tbInfo; }
            set
            {
                _Model._tbInfo = value;
                OnPropertyChanged("_tbInfo");
            }
        }

        public ObservableCollection<string> _listViewInfo
        {
            get { return _Model._listViewInfo; }
            set
            {
                _Model._listViewInfo = value;
                OnPropertyChanged("_listViewInfo");
            }
        }

        public ObservableCollection<string> _loadedFiles
        {
            get { return _Model._loadedFiles; }
            set
            {
                _Model._loadedFiles = value;
                OnPropertyChanged("_loadedFiles");
            }
        }

        public string setloadedFilesWithAbsolutePath
        {
            set { _Model.setloadedFilesWithAbsolutePath = value; }
        }

        public double ProgressStatus
        {
            get { return _Model._progressStatus; }
            set
            {
                _Model._progressStatus = value;
                OnPropertyChanged("ProgressStatus");
            }
        }
        #endregion

        #region Methods

        public void splitFiles()
        {
            _Model.splitFiles();

        }
        public void startTasks()
        {
            _Model.startTasks();
        }
        public bool compareFilePath(string s)
        {
            return _Model.compareFilePath(s);
        }
        public void clearFilePath(int index)
        {
            _Model.clearFilePath(index);
            OnPropertyChanged("_loadedFiles");
        }
        public void clearGuI()
        {
            this.ProgressStatus = 0;

            this._tbInfo = "";
        }

        // clear msmq
        public void clearMSMQ()
        {
            _Model.clearMSMQ();
        }

        // count loaded files f logfile
        public int countLoadedFiles()
        {
            return _Model.countLoadedFiles();
        }

        // implementing timer for testing purposes
        public void startTimer()
        {
            _Model.startTimer();
        }

        // stop timer and return elapsed time
        public string stopTimer()
        {
            return _Model.stopTimer();
        }

        public bool testDBConnection()
        {
           return _Model.testDBConnection();
        }
        #endregion
    }
}
