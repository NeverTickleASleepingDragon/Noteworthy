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
        public Brush ColorOfNote { get; set; }
    }   
    public partial class MainWindow : Window
    {

        OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=G:\Semester 4\Windows programming\Package draft 2\Noteworthy\Noteworthy\Noteworthy.accdb");
        OleDbDataReader reader = null;
        OleDbCommand cmd;
        TextBlock welcomeText;
        Button noteToChange;
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
            Bind();      
            this.DataContext = this;
        }

        private void Bind()
        {           
            con.Open();
            cmd = new OleDbCommand("Select * FROM MyNotes", con);


            reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                welcomeText = new TextBlock();
                welcomeText.Text = "Noteworthy";
                welcomeText.FontSize = 40;
                welcomeText.FontFamily = new FontFamily("Segoe Condensed");
                welcomeText.RenderTransform = new TranslateTransform { X = 570, Y = 250 };
                welcomeText.Foreground = new SolidColorBrush(Colors.LightGray);
                myNotes.Visibility = Visibility.Hidden;
                myGrid.Children.Add(welcomeText);
            }
            else
            {
                myGrid.Children.Remove(welcomeText);
                myGrid.Children.Clear();
                string nColor;
                BrushConverter bc = new BrushConverter();
                Brush brush;
                Components.Clear();
                if(reader.HasRows)
                {
                    while (reader.Read())
                    {
                        nColor = reader.GetString(reader.GetOrdinal("Color"));
                        brush = (Brush)bc.ConvertFrom(nColor);
                        brush.Freeze();
                        
                        Components.Add(new Tile { NameOfNote = reader.GetString(reader.GetOrdinal("NoteName")), ColorOfNote = brush });
                    }
                }
                
                myNotes.ItemsSource = Components;
                myGrid.Children.Add(myNotes);
            }
            reader.Close();
            con.Close();
            ChangeColorMenu.Visibility = Visibility.Hidden;
            DeleteMenu.Visibility = Visibility.Hidden;
            ColorPallete.Visibility = Visibility.Hidden;
            TakeANoteMenu.Header = "Take A Note..";
        }

        private void takeANoteClick(object sender, RoutedEventArgs e)
        {
            if((string)TakeANoteMenu.Header=="Back")
            {
                ChangeColorMenu.Visibility = Visibility.Hidden;
                DeleteMenu.Visibility = Visibility.Hidden;
                ColorPallete.Visibility = Visibility.Hidden;
                TakeANoteMenu.Header = "Take A Note..";
                return;
            }
            TakeANote tNoteDlg = new TakeANote(con);
            this.Visibility = Visibility.Collapsed;
            tNoteDlg.ShowDialog();
            this.Visibility = Visibility.Visible;

            Bind();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
                 
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

            TakeANote tNoteDlg = new TakeANote((bt.Content as TextBlock).Text, path);
            this.Visibility = Visibility.Collapsed;
            tNoteDlg.ShowDialog();
            this.Visibility = Visibility.Visible;
            Bind();
        }
        private void DeleteNote(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;

            con.Open();

            NoteworthyDataSet noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.Connection.ConnectionString = con.ConnectionString;
            string path;
            var res = MessageBox.Show("Are you sure you want to delete this note?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(res==MessageBoxResult.Yes)
            {
                path = myNotesTldAdp.GetPath((noteToChange.Content as TextBlock).Text);
                File.Delete(path);
                myNotesTldAdp.DeleteQuery((noteToChange.Content as TextBlock).Text);
            }
            noteworthyDataSet.AcceptChanges();
            myNotesTldAdp.Update(noteworthyDataSet.MyNotes);
            
            
            con.Close();
            
            Bind();
        }

        private void ColorChange(object sender, RoutedEventArgs e)
        {
            ColorPallete.Visibility = Visibility.Visible;
        }
        private void AdditionalOptions(object sender, MouseButtonEventArgs e)
        {
            ChangeColorMenu.Visibility = Visibility.Visible;
            DeleteMenu.Visibility = Visibility.Visible;
            TakeANoteMenu.Header = "Back";
            noteToChange = (sender as Button);
        }

        private void NoteColorBlue(object sender, RoutedEventArgs e)
        {
            noteToChange.Background = BlueColor.Background;

            con.Open();
            NoteworthyDataSet noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.Connection.ConnectionString = con.ConnectionString;

            myNotesTldAdp.UpdateColor("#FF68B0E2", (noteToChange.Content as TextBlock).Text);
            noteworthyDataSet.AcceptChanges();
            myNotesTldAdp.Update(noteworthyDataSet.MyNotes);
            con.Close();
        }

        private void NoteColorWhite(object sender, RoutedEventArgs e)
        {
            noteToChange.Background = WhiteColor.Background;

            con.Open();
            NoteworthyDataSet noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.Connection.ConnectionString = con.ConnectionString;

            myNotesTldAdp.UpdateColor("#FFFFFFFF", (noteToChange.Content as TextBlock).Text);
            noteworthyDataSet.AcceptChanges();
            myNotesTldAdp.Update(noteworthyDataSet.MyNotes);
            con.Close();
        }

        private void NoteColorRed(object sender, RoutedEventArgs e)
        {
            noteToChange.Background = RedColor.Background;

            con.Open();
            NoteworthyDataSet noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.Connection.ConnectionString = con.ConnectionString;

            myNotesTldAdp.UpdateColor("#FFF07468", (noteToChange.Content as TextBlock).Text);
            noteworthyDataSet.AcceptChanges();
            myNotesTldAdp.Update(noteworthyDataSet.MyNotes);
            con.Close();
        }

        private void NoteColorGreen(object sender, RoutedEventArgs e)
        {
            noteToChange.Background = GreenColor.Background;

            con.Open();
            NoteworthyDataSet noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.Connection.ConnectionString = con.ConnectionString;

            myNotesTldAdp.UpdateColor("#FF93EA86", (noteToChange.Content as TextBlock).Text);
            noteworthyDataSet.AcceptChanges();
            myNotesTldAdp.Update(noteworthyDataSet.MyNotes);
            con.Close();
        }

        private void NoteColorPurple(object sender, RoutedEventArgs e)
        {
            noteToChange.Background = PurpleColor.Background;

            con.Open();
            NoteworthyDataSet noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.Connection.ConnectionString = con.ConnectionString;

            myNotesTldAdp.UpdateColor("#FFCF89EA", (noteToChange.Content as TextBlock).Text);
            noteworthyDataSet.AcceptChanges();
            myNotesTldAdp.Update(noteworthyDataSet.MyNotes);
            con.Close();
        }
    }
}
