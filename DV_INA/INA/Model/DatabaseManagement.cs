using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace INA.Model
{
    class DatabaseManagement : QueueManagement
    {
        #region Members
     
        LogFile _LogFile;
        ProgressBarControl _ProgressBarControl;

        string conString = @"Server=JANINE-NETBOOK\SQLEXPRESS;Database=INA;Trusted_Connection=True;Max Pool Size=200;Connect Timeout=1";
       // string conString = @"Server=CEDRIC\SQLEXPRESS;Database=dv projekt;Trusted_Connection=True; Connect Timeout=1;";
        // string conString = @"Server=WINJ5GTVAPSLQX\SQLEXPRESS;Database=INA;Trusted_Connection=True;";

        #endregion

        #region Constructor

        public DatabaseManagement(LogFile f, ProgressBarControl pbc)
        {
            // start new connection with constring
            
            this._ProgressBarControl = pbc;
            this._LogFile = f;
        }

        #endregion

        #region Methods

        //returns an array with 3 entries
        //each array represents a single message from the file
        // array[0] => file-id
        // array[1] => Header, Footer or Accountno
        // array[2] => sum or Amount 

        public string[] splitString(string value)
        {
            // split received string from queue
            string tmp = value;
            tmp = tmp.Remove(0, 1);
            tmp = tmp.Remove(tmp.Length - 1, 1);
            int ind = tmp.IndexOf(',');
            tmp = tmp.Remove(ind, 1);
            string[] s = tmp.Split(' ');

            return s;
        }

        //evaluate each message
        //header => back to queue//do nothing
        //footer => check if all message are in database, otherwise send it back to queue
        //message => write to database
        public bool evaluateMessageLine(string value)
        {
            bool success = true;
            //string value => single message from file

            string[] record = splitString(value);

            // record[0] => file-id
            // record[1] => Header, Footer or Accountno
            // record[2] => sum or Amount 

            switch (record[1])
            {
                //message is a header
                case "Header": // do nothing
                    break;
                //message is a footer
                case "Footer":
                    success = evaluateFooter(record);
                    break;
                //message is no footer/header
                default:
                    success = evaluateMessage(record);
                    break;
            }
            return success;

        }
        private bool evaluateFooter(string[] record)
        {
            SqlTransaction trans;
            // connect to database
                try
                {
                    using (SqlConnection footerConnection = new SqlConnection(conString))
                    {
                        footerConnection.Open();


                        // open connection
                        // SqlConnection footerConnection = new SqlConnection(conString);
                        // footerConnection.OpenAsync();

                        //add transaction to connection for 2phase commit
                        trans = footerConnection.BeginTransaction();

                        //define command 
                        SqlCommand command = footerConnection.CreateCommand();
                        command.Connection = footerConnection;

                        //add transaction to command for 2phase commit
                        command.Transaction = trans;

                        // begin transaction

                        //define query: check how many messages has already been inserted into the db with the specific file id (which is unique)
                        command.CommandText = "select count(*) from AccMgmt where Fileid = '" + record[0] + "'";

                        int i = (int)command.ExecuteScalar();

                        trans.Commit();

                        //  footerConnection.Close();

                        // compare the number of already inserted messages to the total numbers of inserts that must be done (= file number in footer)
                        if (i != Convert.ToInt32(record[2]))
                        {

                            return false;
                        }
                        else
                        {
                            //everything worked => return true
                            //all messages in database
                            this._LogFile.writeToFile("Complete file " + record[0] + " with " + record[2] + " messages successfully inserted!\n");
                            this._LogFile.reportCompleted();
                            return true;
                        }
                    }
                }
                catch (SqlException se)
                {
                    Console.WriteLine("SQL Exception: " + se.Message);
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            
        }

        //returns true if the given array has been successfully inserted into the db
        //otherwise it returns false
        private bool evaluateMessage(string[] record)
        {
            SqlTransaction trans = null;
           // SqlConnection messageConnection = new SqlConnection(conString);
           SqlCommand command = null;

            // connect to database
                try
                {
                    using (SqlConnection messageConnection = new SqlConnection(conString))
                    {
                        if (messageConnection.State == 0)
                            messageConnection.Open();
                     
                        /*
                        var connectionTask = messageConnection.OpenAsync();
                        // other code goes here
                        Task.WaitAll(connectionTask); //make sure the task is completed
                        if (connectionTask.IsFaulted) // in case of failure
                        {
                            throw new Exception("Connection failure", connectionTask.Exception);
                        }
                        // rest of the code
                        */

                        // open connection
                        // messageConnection.OpenAsync();

                        //add transaction to connection for 2phase commit
                        trans = messageConnection.BeginTransaction();

                        command = messageConnection.CreateCommand();
                        command.Connection = messageConnection;

                        //add transaction to command for 2phase commit
                        command.Transaction = trans;

                        // begin transaction
                        command.CommandText = "INSERT INTO AccMgmt (Account, Amount, Fileid) VALUES (" + record[1] + "," + record[2] + ", '" + record[0] + "' )";
                        command.ExecuteNonQuery();

                        trans.Commit();

                      // messageConnection.Close();
                     

                        //everything ok: return true
                        return true;
                    }
                }
                catch (SqlException sqle)
                {
                    string test = "";

                    foreach (var item in record)
                    {
                        test += " " + item;
                    }
                    Console.WriteLine(test);
                    trans.Rollback();
                    Console.WriteLine(sqle.Message);
                    return false;
                }
                catch (Exception e)
                {
                    string test = "";

                    foreach (var item in record)
                    {
                        test += " " + item;
                    }
                    Console.WriteLine(test);
                   // trans.Rollback();
                    Console.WriteLine(e.Message);
                    return false;
                }
        }
        
        //test if the current connection string is valid
        public bool testDBConnection()
        {
            SqlConnection con = new SqlConnection(conString);

            try
            {
                //test, if connection string works
                con.Open();
                con.Close();
                return true;
            }
            catch (Exception)
            {
               return false;
            }
        }

        #endregion
    }
}
