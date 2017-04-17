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
using System.Windows.Shapes;

namespace Noteworthy
{
    /// <summary>
    /// Interaction logic for TakeANote.xaml
    /// </summary>
    public partial class TakeANote : Window
    {
        string path;
        string title;
        TextBox clickTitle;
        TextBox clickNote;
        string note;
        NoteworthyDataSet noteworthyDataSet;
        OleDbConnection con;
        public TakeANote()
        {
            InitializeComponent();
        }
        public TakeANote(OleDbConnection c)
        {
            InitializeComponent();
            con = c;
        }

        private void enterTitle(object sender, MouseButtonEventArgs e)
        {
            MainStack.Children.Remove(TitleBlock);
            MainStack.Children.Remove(NotesBlock);

            clickTitle = new TextBox();
            clickTitle.FontSize = 22;
            clickTitle.MinWidth = 50;
            clickTitle.MinHeight = 30;
            clickTitle.TextWrapping = TextWrapping.WrapWithOverflow;
            clickTitle.VerticalAlignment = VerticalAlignment.Top;
            clickTitle.HorizontalAlignment = HorizontalAlignment.Left;
            clickTitle.AcceptsReturn=true;
            DockPanel.SetDock(clickTitle, Dock.Top);
            
            MainStack.Children.Add(clickTitle);

            clickNote = new TextBox();
            clickNote.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            clickNote.TextWrapping = TextWrapping.Wrap;
            clickNote.AcceptsReturn = true;
            clickNote.Text = "And a note here..";

            MainStack.Children.Add(clickNote);

            clickNote.GotFocus += RemoveText;

        }
        public void RemoveText(object sender, EventArgs e)
        {
             clickNote.Text = String.Empty;
        }
        private void saveAsClick(object sender, RoutedEventArgs e)
        {
            title = clickTitle.Text;
            note = clickNote.Text;
            Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
            save.FileName = title;
            save.DefaultExt = ".txt";
            save.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = save.ShowDialog();

            if (result == true)
            {
                path = save.FileName;
            }
            con.Open();
            noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.InsertQuery(title, path);
            noteworthyDataSet.AcceptChanges();
            myNotesTldAdp.Update(noteworthyDataSet);
            con.Close();
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                StreamWriter sr = new StreamWriter(path);
                sr.Write(clickNote.Text);
                sr.Close();
            }
            catch(Exception)
            {
                saveAsClick(sender, e);
            }
        }
    }
}
