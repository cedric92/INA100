using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Messaging;
using System.Windows;

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
            string queueName = @".\private$\INAqueue";

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
        protected static void SendStringMessageToQueue(string transactions)
        {
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
