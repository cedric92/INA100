using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WriteToDB
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int rounds;
        public MainWindow()
        {
            InitializeComponent();
        }
        Stopwatch w1 = new Stopwatch();
        Stopwatch w2 = new Stopwatch();
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Task t = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("+++Start task timer +++");
                w1.Start();
                writetoDB(5000);
                w1.Stop();
                Console.WriteLine("+++Stop task timer1"+w1.Elapsed);
            });

            Task t2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("+++Start task timer +++");
                w2.Start();
                writetoDB(5000);
                w2.Stop();
                Console.WriteLine("+++Stop task timer2" + w2.Elapsed);
            });
        }

        private void writetoDB(int rounds)
        {
            this.rounds = rounds;
            string conString = @"Server=CEDRIC\SQLEXPRESS;Database=dv projekt;Trusted_Connection=True; Connect Timeout=1;";

            SqlConnection messageConnection = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = messageConnection;

            messageConnection.Open();
            for (int i = 0; i < rounds; i++)
            {
                cmd.CommandText = "INSERT INTO AccMgmt (Account, Amount, Fileid) VALUES ("+i+","+i+", 'testchica' )";
                cmd.ExecuteNonQuery();
            }

            messageConnection.Close();
        }
    }
}
