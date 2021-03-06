using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;

namespace INA.Model
{
    // serializable due to xml formatter
    [Serializable()]
    class MultiTasking : QueueManagement
    {
        #region Member
        DatabaseManagement _databasemanagement;
        Model _Model;
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

            //Task.Run(() => process());

            /*
            Task.Factory.StartNew(() =>
                //enumerable range + max degree of parallelism => define how many threads will be created
                 Parallel.ForEach(Enumerable.Range(0, 20), new ParallelOptions { MaxDegreeOfParallelism = 12 }, (i) =>
           {
             
               // Restart the synchronous receive operation
               process();
           }) 
                );
         */

            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() =>
               process());
                {
                }

            }

        }

        //used by each task
        //handle a single line, after completing the handling, execute next iteration(recursive) and handle next line
        private void process()
        {
            // Create a transaction.
            MessageQueueTransaction queueTrans = new MessageQueueTransaction();
          
            try
            {
                // Begin the transaction.
                queueTrans.Begin();

                // Receive the message from msmq
                //
                Message msg = queue.Receive(queueTrans);
                String value = (String)msg.Body;


                // Display message information.
               if (_databasemanagement.evaluateMessageLine(value))
                //if (true)
                {
                    // Commit the transaction.
                    //Console.WriteLine(value);
                    queueTrans.Commit();
                }
                else
                {
                    // Abort the transaction.
                    queueTrans.Abort();
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
                queueTrans.Abort();
            }
            // rekursiv
            process();
        }

        #endregion
    }
}
