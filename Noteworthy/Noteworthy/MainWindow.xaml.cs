using System;
using System.IO;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class Tile
    {
        public string NameOfNote { get; set; }
    }
    
    public partial class MainWindow : Window
    {

        OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=G:\Semester 4\Windows programming\Package draft 2\Noteworthy\Noteworthy\Noteworthy.accdb");
        OleDbDataReader reader = null;
        TextBlock welcomeText;

        private ObservableCollection<Object> components;
        public ObservableCollection<Object> Components
        {
            get
            {
                if (components == null)
                    components = new ObservableCollection<Object>();
                return components;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            con.Open();
            OleDbCommand cmd = new OleDbCommand("Select * FROM MyNotes", con);

            reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    Components.Add(new Tile { NameOfNote = reader.GetString(reader.GetOrdinal("NoteName")) });
                }
            }
            con.Close();

            //Components.Add(new Tile { NameOfNote = "Hola!" });
        }

        private void Bind()
        {
            con.Open();
            OleDbCommand cmd = new OleDbCommand("Select * FROM MyNotes", con);

            reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                welcomeText = new TextBlock();
                welcomeText.Text = "Click to Add Note";
                myNotes.Visibility = Visibility.Hidden;
                myGrid.Children.Add(welcomeText);
            }
            else
            {
                myGrid.Children.Remove(welcomeText);
                myNotes.ItemsSource = Components;
                //myNotes.ItemsSource = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter().GetData();
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

        private void OnNoteClick(object sender, RoutedEventArgs e)
        {


            Button bt = sender as Button;
            
            con.Open();

            NoteworthyDataSet noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.Connection.ConnectionString = con.ConnectionString;
            string path = myNotesTldAdp.GetPath((bt.Content as TextBlock).Text);
            con.Close();
            MessageBox.Show(path);
            TakeANote tNoteDlg = new TakeANote((bt.Content as TextBlock).Text, path);
            this.Visibility = Visibility.Collapsed;
            tNoteDlg.ShowDialog();
            this.Visibility = Visibility.Visible;
            Bind();
        }
    }
}
