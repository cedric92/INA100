using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace INA.Model
{
    // serializable due to xml formatter
    [Serializable()]
    class MultiTasking : QueueManagement
    {
        #region Member
        DatabaseManagement _databasemanagement;

        MessageQueue queue = GetStringMessageQueue();
        LogFile _Logfile;
        #endregion

        public MultiTasking(LogFile f, DatabaseManagement db)
        {
            this._Logfile = f;
            _databasemanagement = db;
        }

        #region Methods

        public void startTasks()
        {
            // set the formatter to indicate body contains a string
            queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });

            Task.Factory.StartNew(() =>
                //enumerable range + max degree of parallelism => define how many threads will be created
                 Parallel.ForEach(Enumerable.Range(0, 10), new ParallelOptions { MaxDegreeOfParallelism = 4 }, (i) =>
           {
               // Restart the synchronous receive operation
               process();
           })
                );

        }

        //used by each task
        //handle a single line, after completing the handling, execute next iteration(recursive) and handle next line
        private void process()
        {
            // Create a transaction.
            MessageQueueTransaction trans = new MessageQueueTransaction();

            try
            {
                // Begin the transaction.
                trans.Begin();

                // Receive the message from msmq
                //
                Message msg = queue.Receive(trans);
                String value = (String)msg.Body;

                // Display message information.
                if (_databasemanagement.evaluateMessageLine(value))
                {
                    // Commit the transaction.
                    trans.Commit();
                }
                else
                {
                    // Abort the transaction.
                    trans.Abort();
                }
            }

            catch (MessageQueueException e)
            {
                // Handle nontransactional queues. 
                if (e.MessageQueueErrorCode ==
                    MessageQueueErrorCode.TransactionUsage)
                {
                    Console.WriteLine("Queue is not transactional.");
                }

                // Roll back the transaction.
                trans.Abort();
            }
            // rekursiv
            process();
        }

        #endregion
    }
}
