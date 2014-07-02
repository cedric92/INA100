using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Messaging;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace INA.Model
{
    // serializable due to xml formatter
    [Serializable()]
    class QueueManagement
    {
    #region Members
   
    #endregion

    #region Methods

        public QueueManagement()
        {
           
        }

        // start 
        protected void startMessageQueue(string transactions)
        {
            SendStringMessageToQueue(transactions);
        }

        // create queue + queue name
        protected static MessageQueue GetStringMessageQueue()
        {
            MessageQueue msgQueue = null;
            string queueName = @".\private$\INAqueueT";

            if (!MessageQueue.Exists(queueName))
            {
                msgQueue = MessageQueue.Create(queueName,true);

            }
            else
            { 
                msgQueue = new MessageQueue(queueName);
            }
            return msgQueue;
        }

        // send messages to queue
        public static async Task<string> SendStringMessageToQueue(object transactions)
        {
            await Task.Delay(0);

            MessageQueue msgQueue = GetStringMessageQueue();

            string t = (string)transactions;

                using (var msmqTx = new MessageQueueTransaction())
                {
                    msmqTx.Begin();


                    msgQueue.Send(new Message(t)

                    {
                    }, msmqTx);


                    msmqTx.Commit();

                }

                return "toll";
       

        }

        protected static void SendStringMessageToQueueListe(List<string> transactions)
        {
            // List<int> list = new List<int>();
            MessageQueue msgQueue = GetStringMessageQueue();

            using (var msmqTx = new MessageQueueTransaction())
            {
                msmqTx.Begin();

                foreach (var item in transactions)
                {
                    msgQueue.Send(item, msmqTx);
                }
            
                msmqTx.Commit();

            }
            /*
            MessageQueue msgQueue = GetStringMessageQueue();

            // serialize the message while sending
          // msgQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });

           // a transaction type used for Microsoft Transaction Server (MTS) it will be used when sending or receiving the message
           // Send a message to the queue. 

           if (msgQueue.Transactional == true)
           {
               // Create a transaction.
               MessageQueueTransaction myTransaction = new MessageQueueTransaction();

               // Begin the transaction.
               myTransaction.Begin();

               // Send the message.
               msgQueue.Send(transactions, myTransaction);

               // Commit the transaction.
               myTransaction.Commit();
           }
             * 
             * */
        }

        public void clearMSMQ()
        {
            // clear msmq
            MessageQueue msgQueue = GetStringMessageQueue();
            msgQueue.Purge();
        }

    #endregion
    }
}
