using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Xml.Linq;
using winform = System.Windows.Forms;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using ImportData.Models;
using System.Diagnostics;

namespace ImportData
{
    /// <summary>
    /// Interaction logic for Perfomance.xaml
    /// </summary>
    public partial class Perfomance : Window
    {
        public Perfomance()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFile = new winform.OpenFileDialog()
            {
                Title = "Select a file",
                Filter = "CSV Files|*.csv"
            };
            if (openFile.ShowDialog() == winform.DialogResult.OK)
            {
                txtText.Text = openFile.FileName;
                //dgData.ItemsSource = LoadAndConvertData(openFile.FileName, "2018");
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


        private Queue<Data.Student> studentQueue = new Queue<Data.Student>();
        private string connectionString = "Server=MSI\\EVE;Database=ImportData;User=sa;Password=12345;TrustServerCertificate=True";
        private int batchSize = 1000; // Adjust batch size as needed



        private void ProcessQueue()
        {
            Task.Run(() =>
            {
                List<Data.Student> batch = new List<Data.Student>();



                while (studentQueue.Count > 0)
                {
                    batch.Add(studentQueue.Dequeue());

                    if (batch.Count >= batchSize || studentQueue.Count == 0)
                    {
                        InsertStudents(batch);
                        batch.Clear(); 
                    }
                }
            });
        }

        //private void InsertStudent(Data.Student student)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            using (SqlCommand command = connection.CreateCommand())
        //            {
        //                command.CommandType = CommandType.Text;
        //                command.CommandText = $@"
        //                    INSERT INTO {tableName} (StudentId, Mathematics, Literature, Physics, Biology, English, Year, Chemistry, History, Geography, CivicEducation, Province)
        //                    VALUES (@StudentId, @Mathematics, @Literature, @Physics, @Biology, @English, @Year, @Chemistry, @History, @Geography, @CivicEducation, @Province)
        //                ";

        //                // Set parameters
        //                //StudentId, Mathematics, Literature, Physics, Biology, English, Year, Chemistry, History, Geography, CivicEducation, Province
        //                command.Parameters.AddWithValue("@StudentId", student.StudentId);
        //                command.Parameters.AddWithValue("@Mathematics", student.Mathematics);
        //                command.Parameters.AddWithValue("@Literature", student.Literature);
        //                command.Parameters.AddWithValue("@Physics", student.Physics);
        //                command.Parameters.AddWithValue("@Biology", student.Biology);
        //                command.Parameters.AddWithValue("@English", student.English);
        //                command.Parameters.AddWithValue("@Year", student.Year);
        //                command.Parameters.AddWithValue("@Chemistry", student.Chemistry);
        //                command.Parameters.AddWithValue("@History", student.History);
        //                command.Parameters.AddWithValue("@Geography", student.Geography);
        //                command.Parameters.AddWithValue("@CivicEducation", student.CivicEducation);
        //                command.Parameters.AddWithValue("@Province", student.Province);

        //                // Add parameters for other columns

        //                command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        winform.MessageBox.Show($"Error inserting student: {ex.Message}");
        //    }
        //}

        string year = "2017"; // Set your desired year


        private void InsertStudents(List<Data.Student> students)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Batch insert SchoolYears
                    var schoolYearsToAdd = students
                        .GroupBy(p => p.Province)
                        .Select(p => new SchoolYear
                        {
                            Name = p.Key,
                            Status = "Active",
                            ExamYear = int.Parse(year) // Make sure 'year' is defined in your scope
                        })
                        .ToList();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "INSERT INTO SchoolYear (Name, Status, ExamYear) VALUES (@Name, @Status, @ExamYear)";
                        command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 50));
                        command.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 50));
                        command.Parameters.Add(new SqlParameter("@ExamYear", SqlDbType.NVarChar, 50));

                        foreach (var schoolYear in schoolYearsToAdd)
                        {
                            command.Parameters["@Name"].Value = schoolYear.Name;
                            command.Parameters["@Status"].Value = schoolYear.Status;
                            command.Parameters["@ExamYear"].Value = schoolYear.ExamYear;

                            command.ExecuteNonQuery();
                        }
                    }

                    // Batch insert Students
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "INSERT INTO Student (StudentCode, Status, SchoolYearId) VALUES (@StudentCode, @Status, @SchoolYearId)";
                        command.Parameters.Add(new SqlParameter("@StudentCode", SqlDbType.NVarChar, 50));
                        command.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 50));
                        command.Parameters.Add(new SqlParameter("@SchoolYearId", SqlDbType.Int));

                        foreach (var student in students)
                        {
                            command.Parameters["@StudentCode"].Value = student.StudentId;
                            command.Parameters["@Status"].Value = "Active";
                            command.Parameters["@SchoolYearId"].Value = GetSchoolYearId(connection, student.Province);

                            command.ExecuteNonQuery();
                        }
                    }

                    ImportDataContext db = new ImportDataContext();
                    // Batch insert Scores
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "INSERT INTO Score (StudentId, SubjectId, Score) VALUES (@StudentId, @SubjectId, @Score)";
                        command.Parameters.Add(new SqlParameter("@StudentId", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@SubjectId", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@Score", SqlDbType.Float));

                        foreach (var student in students)
                        {
                            int studentId = GetStudentId(connection, student.StudentId);
                            foreach (var subject in db.Subjects)
                            {
                                var propertyName = subject.Name;
                                var propertyInfo = student.GetType().GetProperty(propertyName);
                                if (propertyInfo != null)
                                {
                                    var value = propertyInfo.GetValue(student, null)?.ToString();
                                    command.Parameters["@StudentId"].Value = studentId;
                                    command.Parameters["@SubjectId"].Value = subject.Id;
                                    command.Parameters["@Score"].Value = string.IsNullOrEmpty(value) ? 0 : double.Parse(value);

                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    winform.MessageBox.Show("Import data successfully", "Success", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                winform.MessageBox.Show(ex.Message, "Error", winform.MessageBoxButtons.OK, winform.MessageBoxIcon.Error);
            }
        }

        private int GetSchoolYearId(SqlConnection connection, string province)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT Id FROM SchoolYear WHERE Name = @Name";
                command.Parameters.AddWithValue("@Name", province);
                return (int)command.ExecuteScalar();
            }
        }

        private int GetStudentId(SqlConnection connection, string studentCode)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT Id FROM Student WHERE StudentCode = @StudentCode";
                command.Parameters.AddWithValue("@StudentCode", studentCode);
                return (int)command.ExecuteScalar();
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string fileName = txtText.Text;

            var students = LoadAndConvertData(fileName, year);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var student in students)
            {
                studentQueue.Enqueue(student);
            }

            ProcessQueue();
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;

            winform.MessageBox.Show($"Data import completed in {elapsedTime.TotalSeconds} seconds.", "Success");
        }
    }
}
