using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using winform = System.Windows.Forms;
using static FileDialog.ReadFileExcel;
using System.Collections.ObjectModel;
namespace FileDialog
{
    /// <summary>
    /// Interaction logic for ReadFileExcel.xaml
    /// </summary>
    public partial class ReadFileExcel : Window
    {
        public ReadFileExcel()
        {
            InitializeComponent();
            Students = new ObservableCollection<StudentData>();

        }
        public ObservableCollection<StudentData> Students { get; set; }
        public class StudentData
        {
            public int student_id { get; set; }
            public string province { get; set; }
            public double? mathematics { get; set; }
            public string literature { get; set; }
            public string physics { get; set; }
            public string chemistry { get; set; }
            public string biology { get; set; }
            public string combined_natural_sciences { get; set; }
            public string history { get; set; }
            public string geography { get; set; }
            public string civic_education { get; set; }
            public string combined_social_sciences { get; set; }
            public double? english { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //open file dialog
            winform.OpenFileDialog openFile = new winform.OpenFileDialog();
            openFile.Title = "Select a Excel File";
            openFile.InitialDirectory = "C:\\Users\\thain\\Documents\\Zalo Received Files";
            openFile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm;*.csv";
            openFile.ShowDialog();
            //get file path
            string filePath = openFile.FileName;
            //check file path
            if (filePath != "")
            {
                //check file extension
                if (Path.GetExtension(filePath) == ".csv")
                {
                    //ReadFileCSV(filePath);
                    LoadMathAvergate(filePath);
                }
                else
                {
                    System.Windows.MessageBox.Show("File extension is not supported");
                }
            }
        }
        
        //fucntion to read file csv
        private void ReadFileCSV(string filePath)
        {
            try
            {
                var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Encoding = Encoding.UTF8, // Our file uses UTF-8 encoding.
                    Delimiter = "," // The delimiter is a comma.
                };

                using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<StudentData>();
                        Students.Clear();
                        foreach (var record in records)
                        {
                            Students.Add(record);
                        }
                    }
                }
            }
            catch(IOException e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private void LoadMathAvergate(string filePath)
        {
            //read file csv and calculate math avergate for each province
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,      // Nếu có dòng header
                    IgnoreBlankLines = true,     // Bỏ qua các dòng trắng
                    Delimiter = ","
                }))
                {
                    var records = csv.GetRecords<StudentData>().ToList();
                    var averageScores = records.GroupBy(s => s.province)
                                              .Select(group => new
                                              {
                                                 Id = GetIdProvince(group.First().student_id),
                                                  Province = group.Key,
                                                  AvagerMath = group.Average(s => s.mathematics),
                                                  AvagerEnglish = group.Average(s => s.english)
                                              })
                                              .OrderByDescending(item => item.AvagerMath)
                                              .ToList();


                    datagird.ItemsSource = averageScores;
                }
            }
            catch (IOException e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }

        }
        //create function cut string 
        private string GetIdProvince(int id)
        {
            string S_Id = id.ToString();
            //cut first 2 character
            return S_Id.Substring(0, 2);
        }
    }
}
