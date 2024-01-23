using CsvHelper.Configuration;
using CsvHelper;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using winform = System.Windows.Forms;
using CsvHelper.Configuration.Attributes;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using System.Xml.Serialization;
using ImportData.Data;
using ImportData.Models;
namespace ImportData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Models.ImportDataContext db = new Models.ImportDataContext();

        public MainWindow()
        {
            InitializeComponent();
            listYear.Add("2017");
            listYear.Add("2018");
            listYear.Add("2019");
            listYear.Add("2020");
            listYear.Add("2021");
            ListYear(listYear);

        }

        List<string> listYear = new List<string>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new winform.OpenFileDialog()
            {
                Title = "Select a file",
                Filter = "CSV Files|*.csv"
            };
            if(openFile.ShowDialog() == winform.DialogResult.OK)
            {
                txtName.Text = openFile.FileName;
                if(txtName.Text != null)
                {
                    cbYear.IsEnabled = true;
                }
            }
        }

        private List<Data.Student> LoadAndConvertData(string fileName, string Year)
        {
            try
            {
                var students = new List<Data.Student>();

                using (var reader = new StreamReader(fileName))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                }))
                {
                    // Read records from CSV
                    var records = csv.GetRecords<Data.Excel>().ToList();

                    // Filter records by Year
                    var recordsByYear = records.Where(x => x.Year.Equals(Year)).ToList();

                    // Convert the order of columns if needed
                    foreach (var record in recordsByYear)
                    {
                        var convertedStudent = new Data.Student
                        {
                            // Adjust the order of properties as needed
                            StudentId = record.SBD,
                            Mathematics = record.Toan,
                            Literature = record.Van,
                            Physics = record.Ly,
                            Biology = record.Sinh,
                            English = record.Ngoaingu,
                            Year = record.Year,
                            Chemistry = record.Hoa,
                            History = record.Lichsu,
                            Geography = record.Dialy,
                            CivicEducation = record.GDCD,
                            Province = record.MaTinh
                        };

                        students.Add(convertedStudent);
                    }
                }

                return students;
            }
            catch (Exception ex)
            {
                winform.MessageBox.Show(ex.Message, "Error", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Error);
                return new List<Data.Student>(); // Return an empty list in case of an error
            }
        }

        private void LoadData(string fileName, string Year)
        {
            var recordsByYear = LoadAndConvertData(fileName, Year);
            dgData.ItemsSource = recordsByYear;
            if(dgData.Items.Count > 0)
            {
                btnImport.IsEnabled = true;
                btnClear.IsEnabled = true;
            }
            else
            {
                btnImport.IsEnabled = false;
                btnClear.IsEnabled = false;
            }
        }


        private void ListYear(List<string> list)
        {
            //assign list to combobox
            cbYear.ItemsSource = list;
        }


        //trigger when change data in combobox
        private void cbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = cbYear.SelectedItem.ToString();
            LoadData(txtName.Text, text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int year = int.Parse(cbYear.SelectedItem.ToString());
            var listYear = db.SchoolYears.GroupBy(p=>p.ExamYear).ToList();
            foreach(var item in listYear)
            {
                if(item.Key == year)
                {
                    winform.MessageBox.Show("Data is already exist", "Error", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Error);
                    return;
                }
            }
            try
            {
                //import data from datagrid to database
                var recordsByYear = LoadAndConvertData(txtName.Text, cbYear.SelectedItem.ToString());

                //add province to schoolyear table
                var provinceCode = recordsByYear.GroupBy(p => p.Province).Select(p => p.Key).ToList();
                foreach (var pc in provinceCode)
                {
                    SchoolYear schoolYear = new SchoolYear
                    {
                        Name = pc,
                        Status = "Active",
                        ExamYear = year
                    };
                    db.SchoolYears.Add(schoolYear);
                    db.SaveChanges();
                }

                //add student to student table
                foreach (var record in recordsByYear)
                {
                    var student = new Models.Student
                    {
                        StudentCode = record.StudentId,
                        Status = "Active",
                        SchoolYearId = db.SchoolYears.Where(p => p.Name == record.Province).Select(p => p.Id).FirstOrDefault()
                    };
                    db.Students.Add(student);
                    db.SaveChanges();

                    //add score to score table
                    var subject = db.Subjects.ToList();
                    foreach (var sub in subject)
                    {
                        var prCore = record.GetType().GetProperty(sub.Name);
                        if (prCore != null)
                        {
                            var value = prCore.GetValue(record, null);
                            if (value != null)
                            {
                                var score = new Models.Score
                                {
                                    StudentId = student.Id,
                                    SubjectId = sub.Id,
                                    Score1 = value.ToString() == "" ? 0 : double.Parse(value.ToString())
                                };
                                db.Scores.Add(score);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                winform.MessageBox.Show("Import data successfully", "Success", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                winform.MessageBox.Show(ex.Message, "Error", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Error);
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int year = int.Parse(cbYear.SelectedItem.ToString());
            bool isExist = false;
            var listYear = db.SchoolYears.GroupBy(p => p.ExamYear).ToList();
            foreach (var item in listYear)
            {
                if (item.Key == year)
                {
                    isExist = true;
                }
            } 
            if(isExist == false)
            {
                winform.MessageBox.Show("Data is not exist", "Error", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Error);
                return;
            }
            else
            {
                ClearData(year);
                winform.MessageBox.Show("Clear data successfully", "Success", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Information);
            }
        }
        private void ClearData(int year)
        {
            try
            {
                var yearToRemove = db.SchoolYears
                .Where(s => s.ExamYear == year)
                .FirstOrDefault();

                var listStudentInYear = db.Students
                    .Where(s => s.SchoolYearId == yearToRemove.Id)
                    .ToList();
                foreach (var item in listStudentInYear)
                {
                    // Remove scores
                    var listScore = db.Scores
                        .Where(s => s.StudentId == item.Id)
                        .ToList();
                    db.Scores.RemoveRange(listScore);

                    // Remove students
                    var studentsToRemove = db.Students
                   .Where(s => s.Id == item.Id)
                   .ToList();
                    db.Students.RemoveRange(studentsToRemove);
                }

                // Remove school years
                var schoolYearsToRemove = db.SchoolYears
                    .Where(s => s.ExamYear == year)
                    .ToList();
                db.SchoolYears.RemoveRange(schoolYearsToRemove);

                db.SaveChanges();
            }
            catch(Exception ex)
            {
                winform.MessageBox.Show(ex.Message, "Error", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Error);
            }
        }
    }
}