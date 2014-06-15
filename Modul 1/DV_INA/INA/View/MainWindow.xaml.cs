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
        string filename="";
        ObservableCollection<string> tmp = new ObservableCollection<string>();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            _ViewModel = new ViewModel.ViewModel();

           this.DataContext = _ViewModel;

        }

        public INA.ViewModel.ViewModel ViewModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }


        private void Button_ClickStart(object sender, RoutedEventArgs e)
        {
            //call method splitFile which splits the chosen file according to the fileNam
            _ViewModel.splitFiles();
        }

        
        private void btBeenden_Click(object sender, RoutedEventArgs e)
        {

            //this.Close();
        }

        private void btOpenFile_Click(object sender, RoutedEventArgs e)
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
                //get absolute file path
                this.filename = dlg.FileName;

                //check if the choosen file is loaded
                if (_ViewModel.compareFilePath(filename))
                {
                    //file already loaded
                    MessageBox.Show("Achtung: Die ausgwählte Datei wurde bereits geladen. Ein weiteres laden ist deshalb nicht moeglich!");
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

        
    }
}
