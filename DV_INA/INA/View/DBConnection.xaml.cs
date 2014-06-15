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
using System.Windows.Shapes;
using System.Timers;

namespace INA.View
{
    /// <summary>
    /// Interaktionslogik für DBConnection.xaml
    /// </summary>
    public partial class DBConnection : Window
    {
        public DBConnection(string message)
        {
            InitializeComponent();
            this.msgText.Content = message;
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }), null);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            Timer t = new Timer();
            t.Interval = 3000;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();
        }
    }
}
