using System;
using System.IO;
using System.Data.OleDb;
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

namespace Noteworthy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=G:\Semester 4\Windows programming\Package\Noteworthy\Noteworthy\Noteworthy.accdb");
        OleDbDataReader reader = null;
        TextBlock welcomeText;
        public MainWindow()
        {
            InitializeComponent();
            

        }

        private void Bind()
        {
            con.Open();
            OleDbCommand cmd = new OleDbCommand("Select * FROM MyNotes", con);
            reader = cmd.ExecuteReader();
            if(!reader.HasRows)
            {
                welcomeText = new TextBlock();
                welcomeText.Text = "Click to Add Note";
                myNotes.Visibility = Visibility.Hidden;
                myGrid.Children.Add(welcomeText);
            }
            else
            {
                myGrid.Children.Remove(welcomeText);
                myNotes.ItemsSource = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter().GetData();
            }
            con.Close();
        }

        private void takeANoteClick(object sender, RoutedEventArgs e)
        {
            TakeANote tNoteDlg = new TakeANote(con);
            this.Visibility = Visibility.Collapsed;
            tNoteDlg.ShowDialog();
            this.Visibility = Visibility.Visible;
            Bind();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            Bind();
        }



    }
}
