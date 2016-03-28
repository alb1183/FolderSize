using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Globalization;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
       //private int sortColumn = -1;
        public Form1()
        {
            InitializeComponent(); 
            listView1.ListViewItemSorter = new Sorter();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double sizeOfDir_total = 0;
            listView1.Items.Clear();
            foreach (string dir in Directory.GetDirectories(textBox1.Text))
            {
                try
                {
                    DirectoryInfo dInfo = new DirectoryInfo(@dir);
                    // set bool parameter to false if you
                    // do not want to include subdirectories.
                    double sizeOfDir = DirectorySize(dInfo, true);
                    sizeOfDir_total += sizeOfDir;

                    String size = "";

                    if (sizeOfDir < 1024)
                    {
                        size = sizeOfDir.ToESString() + " Bytes";
                    }
                    else if (sizeOfDir < 1024 * 1024)
                    {
                        size = Math.Round(sizeOfDir / 1024, 3).ToESString() + " KB";
                    }
                    else if (sizeOfDir < 1024 * 1024 * 1024)
                    {
                        size = Math.Round(sizeOfDir / (1024 * 1024), 3).ToESString() + " MB";
                    }
                    else
                    {
                        size = Math.Round(sizeOfDir / (1024 * 1024 * 1024), 3).ToESString() + " GB";
                    }

                    ListViewItem objListViewItem = new ListViewItem();
                    objListViewItem.Text = dir;
                    objListViewItem.SubItems.Add(size);
                    objListViewItem.SubItems.Add(sizeOfDir.ToESString());
                    listView1.Items.Add(objListViewItem);

                    /*Console.WriteLine("Directory size in Bytes : " +
                    "{0:N0} Bytes", sizeOfDir);
                    Console.WriteLine("Directory size in KB : " +
                    "{0:N2} KB", ((double)sizeOfDir) / 1024);
                    Console.WriteLine("Directory size in MB : " +
                    "{0:N2} MB", ((double)sizeOfDir) / (1024 * 1024));

                    Console.ReadLine();*/
                }
                catch
                {

                }
            }

            label1.Text = "Total: " + Math.Round(((double)sizeOfDir_total) / (1024 * 1024 * 1024), 3) + " GB";
        }
                
        static long DirectorySize(DirectoryInfo dInfo, bool includeSubDir)
        {
            // Enumerate all the files
            long totalSize = dInfo.EnumerateFiles()
                         .Sum(file => file.Length);

            // If Subdirectories are to be included
            if (includeSubDir)
            {
                // Enumerate all sub-directories
                totalSize += dInfo.EnumerateDirectories()
                         .Sum(dir => DirectorySize(dir, true));
            }
            return totalSize;
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            Sorter s = (Sorter)listView1.ListViewItemSorter;
            s.Column = e.Column;

            if (s.Order == System.Windows.Forms.SortOrder.Ascending)
            {
                s.Order = System.Windows.Forms.SortOrder.Descending;
            }
            else
            {
                s.Order = System.Windows.Forms.SortOrder.Ascending;
            }
            listView1.Sort();
        }

        class Sorter : System.Collections.IComparer
        {
            public int Column = 1;
            public System.Windows.Forms.SortOrder Order = SortOrder.Ascending;
            public int Compare(object x, object y) // IComparer Member
            {
                if (!(x is ListViewItem))
                    return (0);
                if (!(y is ListViewItem))
                    return (0);

                ListViewItem l1 = (ListViewItem)x;
                ListViewItem l2 = (ListViewItem)y;

                if (l1.ListView.Columns[Column].Tag == null)
                {
                    l1.ListView.Columns[Column].Tag = "Text";
                }

                if (l1.ListView.Columns[Column].Tag.ToString() == "Numeric")
                {
                    double fl1 = double.Parse(l1.SubItems[Column].Text);
                    double fl2 = double.Parse(l2.SubItems[Column].Text);

                    if (Order == SortOrder.Ascending)
                    {
                        return fl1.CompareTo(fl2);
                    }
                    else
                    {
                        return fl2.CompareTo(fl1);
                    }
                }
                else
                {
                    string str1 = l1.SubItems[Column].Text;
                    string str2 = l2.SubItems[Column].Text;

                    if (Order == SortOrder.Ascending)
                    {
                        return str1.CompareTo(str2);
                    }
                    else
                    {
                        return str2.CompareTo(str1);
                    }
                }
            }
        }
    }

    public static class DoubleExtensions
    {
        public static string ToGBString(this double value)
        {
            return value.ToString(CultureInfo.GetCultureInfo("en-GB"));
        }
        public static string ToESString(this double value)
        {
            return value.ToString(CultureInfo.GetCultureInfo("es-ES"));
        }
    }
}
