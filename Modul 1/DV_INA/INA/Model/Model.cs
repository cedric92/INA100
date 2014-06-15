using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace INA.Model
{
    class Model
    {
        #region Members

        FileSplit _FileSplit;
        //QueueManagement _QueueManagement;
        private List<string> loadedFilesWithAbsolutePath = new List<string>();
       //to delete
        private ObservableCollection<string> loadedFiles = new ObservableCollection<string>();

        #endregion     
        
        public Model()
        {
            _FileSplit = new FileSplit();
        }

        #region Getter/Setter
        internal FileSplit FileSplit
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public string setloadedFilesWithAbsolutePath
        {
            set { this.loadedFilesWithAbsolutePath.Add(value); }
        }

        public ObservableCollection<string> _loadedFiles
        {
            get { return loadedFiles; }
            set { loadedFiles = value; }
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
    }
}
