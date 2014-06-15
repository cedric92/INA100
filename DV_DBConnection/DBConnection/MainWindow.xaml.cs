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

namespace DBConnection
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string conStr = @"Server=WINJ5GTVAPSLQX\SQLEXPRESS;Database=INA;Trusted_Connection=True; Connect Timeout=1;";
        public MainWindow()
        {
            InitializeComponent();
            input.Text = conStr;
            show.Text = conStr;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            conStr = input.Text;

            show.Text = conStr;
            try
            {
                SqlConnection con = new SqlConnection(conStr);
                con.Open();

                MessageBox.Show("Success!");
            }
            catch (Exception)
            {

                MessageBox.Show("Connection String nicht erfolgreich");
            }

            
           
        }
    }
}
