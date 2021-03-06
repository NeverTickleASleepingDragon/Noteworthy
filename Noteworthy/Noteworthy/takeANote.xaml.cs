﻿using System;
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
        public TakeANote(string t, string p)
        {
            InitializeComponent();
            path = p;
            title = t;

            if (File.Exists(path))
            {
                note = File.ReadAllText(path);
            }

            clickTitle = new TextBox();
            clickTitle.FontSize = 35;
            clickTitle.MinWidth = 50;
            clickTitle.MinHeight = 50;
            clickTitle.Padding = new System.Windows.Thickness(5, 5,5,5);
            clickTitle.BorderThickness = new Thickness(0);
            clickTitle.TextWrapping = TextWrapping.WrapWithOverflow;
            clickTitle.VerticalAlignment = VerticalAlignment.Top;
            clickTitle.HorizontalAlignment = HorizontalAlignment.Left;
            clickTitle.AcceptsReturn = true;
            DockPanel.SetDock(clickTitle, Dock.Top);

            

            clickNote = new TextBox();
            clickNote.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            clickNote.TextWrapping = TextWrapping.Wrap;
            
            clickNote.AcceptsReturn = true;
            clickNote.Padding = new Thickness(2, 10, 2, 2);
            clickNote.BorderThickness = new Thickness(0);
           

            MainStack.Children.Remove(TitleBlock);
            MainStack.Children.Remove(NotesBlock);
            MainStack.Children.Add(clickTitle);
            MainStack.Children.Add(clickNote);
            
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
            clickTitle.FontSize = 35;
            clickTitle.MinWidth = 50;
            clickTitle.MinHeight = 30;
            clickTitle.TextWrapping = TextWrapping.WrapWithOverflow;
            clickTitle.Padding = new System.Windows.Thickness(5, 5, 5, 5);
            clickTitle.VerticalAlignment = VerticalAlignment.Top;
            clickTitle.HorizontalAlignment = HorizontalAlignment.Left;
            clickTitle.AcceptsReturn = true;
            clickTitle.BorderThickness = new Thickness(0);
            DockPanel.SetDock(clickTitle, Dock.Top);
            

            MainStack.Children.Add(clickTitle);

            clickNote = new TextBox();
            clickNote.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            clickNote.TextWrapping = TextWrapping.Wrap;
            clickNote.AcceptsReturn = true;
            clickNote.Padding = new Thickness(2, 10, 2, 2);
            clickNote.BorderThickness = new Thickness(0);
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
                System.IO.File.WriteAllText(path, note);
            }
            con.Open();
            noteworthyDataSet = new NoteworthyDataSet();
            NoteworthyDataSetTableAdapters.MyNotesTableAdapter myNotesTldAdp = new NoteworthyDataSetTableAdapters.MyNotesTableAdapter();
            myNotesTldAdp.Connection.ConnectionString = con.ConnectionString;
            myNotesTldAdp.InsertQuery(title, path,"#FFFFFFFF");

            noteworthyDataSet.AcceptChanges();
            myNotesTldAdp.Update(noteworthyDataSet.MyNotes);
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
            catch (Exception)
            {
                saveAsClick(sender, e);
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
