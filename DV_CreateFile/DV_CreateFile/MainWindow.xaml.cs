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
using System.IO;
using System.Xml;


namespace DV_CreateFile
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void btCreate_Click(object sender, RoutedEventArgs e)
        {
            int min = 0;
            int max = 0;

            int no_acc = 0;
            int sum = 0;

            try
            {
                min = Convert.ToInt32(txtMin.Text);
                max = Convert.ToInt32(txtMax.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Bitte geeignete Beträge eingeben.");
                txtMax.Text = "";
                txtMin.Text = "";
            }

            if (min < max)
            {
                string filename = string.Format("{0:yyyy-MM-dd_hh-mm-ss}.txt", DateTime.Now);

                Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.FileName = filename; // Default file name
                saveDialog.DefaultExt = ".txt"; // Default file extension
                saveDialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = saveDialog.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    filename = saveDialog.FileName;
                }

                int lines = Convert.ToInt32(txtLines.Text);

                if (lines < 2)
                {
                    lines = 10;
                    txtLines.Text = "10";
                }

                string extension = Path.GetExtension(saveDialog.FileName);

                if (extension == ".txt")
                {
                    Random rnd_sum = new Random();
                    Random rnd_acc = new Random();
                    Random rnd_entries = new Random();

                    int value = 0;

                    using (StreamWriter sw = File.CreateText(filename))
                    {
                        // hashtag line 1
                        sw.WriteLine("#");

                        // while > 5 generate lines
                        while (lines > 5)
                        {

                            no_acc = rnd_entries.Next(2, 4);
                            sum = 0;

                            for (int j = 0; j < no_acc - 1; j++)
                            {
                                value = rnd_sum.Next(min, max);
                                // write random account
                                sw.Write(rnd_acc.Next(1, 100));
                                // write random amount
                                sw.WriteLine(" " + value.ToString());
                                // add to sum
                                sum += value;
                            }

                            // write random account
                            sw.Write(rnd_acc.Next(1, 100));
                            // write difference
                            sw.WriteLine(" " + (sum * -1).ToString());
                            // write closing #
                            sw.WriteLine("#");

                            lines -= no_acc;
                        }

                        // lines < 6
                        no_acc = lines;
                        sum = 0;

                        for (int j = 0; j < no_acc - 1; j++)
                        {
                            value = rnd_sum.Next(min, max);
                            sw.Write(rnd_acc.Next(1, 100));
                            sw.WriteLine(" " + value.ToString());
                            sum += value;
                        }

                        if (chbWrongamount.IsChecked == false)
                        {
                            sw.Write(rnd_acc.Next(1, 100));
                            sw.WriteLine(" " + (sum * -1).ToString());
                        }
                        else
                        {
                            sw.Write(rnd_acc.Next(1, 100));
                            sw.WriteLine(" 0");
                        }

                        // write closing #
                        sw.WriteLine("#");
                    }
                    MessageBox.Show("Die Datei wurde erfolgreich erstellt.\nPfad: " + filename, "Information");
                }
                else
                {
                    MessageBox.Show("Bitte korrekte Werte eingeben.", "Information");
                }
            }
            /*     else if (extension == ".xml")
                 {
                     // experimental

                     XmlDocument doc = new XmlDocument();
                     XmlNode myRoot, myNode;
                    // XmlAttribute myAttribute;

                     Random rnd_sum = new Random();
                     Random rnd_acc = new Random();

                     int sum = 0;
                     int value = 0;

                     // creare root
                     myRoot = doc.CreateElement("AccountList");
                     doc.AppendChild(myRoot);

                     for (int i = 0; i < lines - 1; i++)
                     {
                         value = rnd_sum.Next(-1000, 1000);
                         sum += value; 
                    
                         myNode = doc.CreateElement("Amount");

                         myNode.InnerText = value.ToString();

                       //  myAttribute = doc.CreateAttribute("accountNo");
                       //  myAttribute.InnerText = rnd_acc.Next(1, 100).ToString();
                       //  myNode.Attributes.Append(myAttribute);

                         myRoot.AppendChild(myNode);  

                     }

                     myNode = doc.CreateElement("Amount");

                     myNode.InnerText = (sum * -1).ToString();

                     // myAttribute = doc.CreateAttribute("accountNo");
                     // myAttribute.InnerText = rnd_acc.Next(1, 100).ToString();
                     // myNode.Attributes.Append(myAttribute);

                     myRoot.AppendChild(myNode);  

                     doc.Save(filename);

                     MessageBox.Show("Die Datei wurde erfolgreich erstellt.\nPfad: " + filename, "Information");

                 }
     */
        }

        private void cbFileformat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
