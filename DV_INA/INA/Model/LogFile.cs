using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace INA.Model
{
    class LogFile
    {
        #region Member
        ViewModel.ViewModel _vm;
        int completedFiles = 0;
        
        #endregion

        #region ctor
        public LogFile(ViewModel.ViewModel vm)
        {
            this._vm = vm;
        }
        #endregion

        #region Methods

        static readonly object _lock = new object();
        static readonly object _lock2 = new object();

        public void writeToFile(string message)
        {
            lock (_lock)
            {
                callVM(message); 
            }

            // lock ensures that one thread does not enter a critical section of code while 
            // another thread is in the critical section. If another thread tries to enter a 
            // locked code, it will wait, block, until the object is released.
            lock (_lock2)
            {
                string path = Path.Combine((Environment.ExpandEnvironmentVariables("%USERPROFILE%") + @"\Desktop"), "Log.txt");
               
                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    File.WriteAllText(path, message + Environment.NewLine);
                }
                else
                {
                    try
                    {
                        File.AppendAllText(path, message + Environment.NewLine);

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error in Logfile: Write To File");
                    }
                }
            }
            
        }
      
        private void callVM(string message)
        {            
              /*  ObservableCollection<string> tmp = _vm._listViewInfo;
                //tmp.Add(message);
                this._vm._listViewInfo = tmp;

                string s = _vm._tbInfo + message;*/
            
                _vm._tbInfo += message;    
        }

        // one footer completed, count it - then check sum
        public void reportCompleted()
        {
            completedFiles++;
            checkCompleted();
        }

        // check if all footers are committed
        private void checkCompleted()
        {
            if (this._vm.countLoadedFiles() == completedFiles)
            {
                writeToFile("Imported all files successfully into the database." + Environment.NewLine);
                writeToFile(_vm.stopTimer());
            }
        }

        #endregion
    }
}
