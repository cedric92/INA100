using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;

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
           // List<string> list = new List<string>();


            string fileName = Path.GetFileNameWithoutExtension(filePath);
            _Logfile.writeToFile("### Started to import file " + fileName+"###\n");
            
            try
            {
                int count = 0;

                 // Create new stopwatch   
                Stopwatch stopwatch = new Stopwatch();

                // Begin timing
                stopwatch.Start();

                using (var fileStream = File.OpenText(filePath))
                {
                   // list.Add((new KeyValuePair<string, string>(fileName, "Header")).ToString());

                    schreibeInQueue((new KeyValuePair<string, string>(fileName, "Header")).ToString());
                    //SendStringMessageToQueue((new KeyValuePair<string, string>(fileName, "Header").ToString()));

                    // save whole transactions, KeyValuePair simplifies access to key and value (acc-no + sum)
                    List<KeyValuePair<string, string>> transactionBlock = new List<KeyValuePair<string, string>>();

                    do
                    {
                        var line = fileStream.ReadLine();
                        if (!line.Contains('#'))
                        {
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
                                        _Logfile.writeToFile("ERROR: " + fileName + ", " + tline.ToString() + Environment.NewLine);
                                    }
                                    transactionBlock.Clear();
                                }
                                else
                                {
                                    foreach (var t in transactionBlock)
                                    {
                                        count++;
                                      //  SendStringMessageToQueue(t.ToString());

                                        schreibeInQueue(t.ToString());


                                        // send string to lst
                                     //  list.Add(t.ToString());
                                        /*
                                        Task.Factory.StartNew(() =>
                                        {
                                        
                                        });
                                         */
                                      //System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(SendStringMessageToQueue), t);

                                    }
                                    // clear transactionBlock
                                    transactionBlock.Clear();
                                }
                            }

                        }

                    } while (!fileStream.EndOfStream);

                   // list.Add((new KeyValuePair<string, string>(fileName, "Footer " + count)).ToString());
                   //SendStringMessageToQueue((new KeyValuePair<string, string>(fileName, "Footer " + count)).ToString());
                    schreibeInQueue((new KeyValuePair<string, string>(fileName, "Footer " + count)).ToString());
                   //SendStringMessageToQueueListe(list);

                 // Stop timing
                 stopwatch.Stop();

                    // Write result
                  _Logfile.writeToFile("Time elapsed: " + stopwatch.Elapsed);
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

        private void schickmalindieQueue(object t)
        {
           Console.WriteLine("Schickmalindiequeue!!!");
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

        static async void schreibeInQueue(string m)
        {
            await SendStringMessageToQueue(m);
        }

        #endregion
    }


}
