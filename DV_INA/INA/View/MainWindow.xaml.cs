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
using INA.ViewModel;
using System.IO;
using System.Collections.ObjectModel;

namespace INA
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    // ---------- VIEW ---------------------------------------
    public partial class MainWindow : Window
    {
        #region Members
        ViewModel.ViewModel _ViewModel;
        string filename = "";
        ObservableCollection<string> tmp = new ObservableCollection<string>();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            _ViewModel = new ViewModel.ViewModel();

            this.DataContext = _ViewModel;

            testDPConnection();

        }
        private void Button_ClickStart(object sender, RoutedEventArgs e)
        {
            progBar.Value = 0;

            //call method splitFile which splits the chosen file according to the fileNam
            _ViewModel.splitFiles();
            // start tasks reading from msmq
            _ViewModel.startTasks();
            // start timer
            _ViewModel.startTimer();

            // de/activate buttons
            btStart.IsEnabled = false;
            btBeenden.IsEnabled = true;           
        }

        private void btBeenden_Click(object sender, RoutedEventArgs e)
        {
            closeINA();
        }

        private void closeINA()
        {
            MessageBoxResult result = MessageBox.Show("Möchten Sie wirklich beenden?", "INA beenden",
            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btOpenFile_Click(object sender, RoutedEventArgs e)
        {
            openFile();
        }

        private void openFile()
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // acticate start button
                btStart.IsEnabled = true;

                //clear progressbar and tbinfo field
                _ViewModel.clearGuI();

                //get absolute file path
                this.filename = dlg.FileName;

                //check if the choosen file is loaded
                if (_ViewModel.compareFilePath(filename))
                {
                    //file already loaded
                    MessageBox.Show("Achtung: Die ausgwählte Datei wurde bereits geladen.");
                }
                else
                {
                    //file not loaded

                    //add absolute file path to list
                    _ViewModel.setloadedFilesWithAbsolutePath = filename;

                    //code for \
                    char c = '\\';
                    //get last position of \ in absolute path
                    int pos = filename.LastIndexOf(c);
                    //cut the path at the last pos of \ => shows only the file name without absolute path
                    string sub = filename.Substring(pos + 1);
                    //set text for loaded files => databinding

                    //_ViewModel.loadedFiles = sub;

                    tmp.Add(sub);
                    _ViewModel._loadedFiles = tmp;
                }
            }
        }


        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if (filesView.SelectedItem != null)
            {
                //clear progress bar and textblock info
                _ViewModel.clearGuI();

                int index = filesView.SelectedIndex;
                _ViewModel.clearFilePath(index);

                if (filesView.Items.Count == 0)
                {
                    btStart.IsEnabled = false;
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            closeINA();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            openFile();
        }

        private void filesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            // get depency to listview filesView
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            // get concerned index
            int index = filesView.ItemContainerGenerator.IndexFromContainer(dep);

            // mvvm
            _ViewModel.clearFilePath(index);
        }

        private void menu_opencompmgmnt_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "compmgmt.msc";
            process.Start();
        }

        private void menu_clearMSMQ_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Möchten Sie die MSMQ wirklich leeren?\nEntfernte Nachrichten werden nicht mehr in die Datenbank übertragen.", "INAqueue leeren",
            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            if (result == MessageBoxResult.Yes)
            {
                _ViewModel.clearMSMQ();
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void progBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void testDPConnection()
        {
            bool dbSucces = _ViewModel.testDBConnection();

            View.DBConnection _conWindow;

            string message = "";

            if (dbSucces)
            {
                message = "DB connection successfull";
                
               
            }
            else
            {
                message = "DB connection failed";

                controlContainer.IsEnabled = false;


                btBeenden.IsEnabled = true;
            }

            _conWindow = new View.DBConnection(message);
            _conWindow.Show();
           
        }
    }
}
