using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace INA.Model
{

    class FileSplit : QueueManagement
    {

        #region Members
        LogFile _Logfile;
        MultiTasking _MultiTasking;
        ProgressBarControl _ProgressBarControl;

        private int numberOfFiles = 0;
        #endregion

        #region Constructor

        public FileSplit(LogFile f, MultiTasking m, ProgressBarControl pbc)
        {
            this._Logfile = f;
            this._MultiTasking = m;
            this._ProgressBarControl = pbc;
        }

        #endregion

        #region Getter/setter

        #endregion

        #region Methods

        // split file into lines
        public void splitFile(List<string> loadedFilePaths)
        {
            numberOfFiles = loadedFilePaths.Count();

            /*Info für Parallel:
             * http://stackoverflow.com/questions/5009181/parallel-foreach-vs-task-factory-startnew
             * */
            var loopResult =
            Task.Factory.StartNew(() =>
            Parallel.ForEach<string>(loadedFilePaths, path => readFile(path))
            );
        }

        // import and check files
        public void readFile(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            _Logfile.writeToFile("###Started to import file " + fileName+"###\n");

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line = "";

                    //count lines
                    int count = 0;

                    // save whole transactions, KeyValuePair simplifies access to key and value (acc-no + sum)
                    List<KeyValuePair<string, string>> transactionBlock = new List<KeyValuePair<string, string>>();

                    // send header to queue
                    startMessageQueue((new KeyValuePair<string, string>(fileName, "Header")).ToString());

                    // read file
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Contains('#'))
                        {
                            // add line to transactionBlock
                            transactionBlock.Add((new KeyValuePair<string, string>(fileName, line)));
                        }
                        else
                        {
                            if (transactionBlock.Count > 0)
                            {
                                // check transactionBlock
                                if (!checkTextLines(transactionBlock, filePath))
                                {
                                    foreach (var tline in transactionBlock)
                                    {
                                        _Logfile.writeToFile("ERROR: " + fileName + ", "
                                            + tline.ToString() + Environment.NewLine);
                                    }
                                    transactionBlock.Clear();
                                }
                                else
                                {
                                    foreach (var t in transactionBlock)
                                    {
                                        count++;
                                        // send string to queue
                                        startMessageQueue(t.ToString());
                                    }
                                    // clear transactionBlock
                                    transactionBlock.Clear();
                                }
                            }
                        }
                    }

                    // send footer to queue, add count
                    startMessageQueue((new KeyValuePair<string, string>(fileName, "Footer " + count)).ToString());


                    _Logfile.writeToFile("### File "+fileName+" successfully imported ###\n");


                    _ProgressBarControl.setProgressStatus(1);
                    count = 0;
                }

            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //check if the entries in the list are valid. 
        // fileName is used to show errormessages
        private bool checkTextLines(List<KeyValuePair<string, string>> transactionBlock, string filePath)
        {
            int sum = 0;
            int value = 0;

            bool check = true;

            foreach (var line in transactionBlock)
            {
                // split line.Value: acc no, amount
                string[] transaction = line.Value.Split(' ');
                //try to parse to an int
                if (int.TryParse(transaction[1], out value) && check)
                {
                    sum += value;
                }
                else
                {
                    check = false;
                }

            }
            //check if the transaction block is balanced (sum = 0)
            if (sum != 0)
            {
                check = false;
            }

            return check;
        }

        public void startTasks()
        {
            _MultiTasking.startTasks();
        }

        #endregion
    }


}
