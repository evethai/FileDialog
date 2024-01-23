using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using winform = System.Windows.Forms;
namespace FileDialog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public class FileSystemItem
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public ImageSource Icon { get; set; }
            public bool IsFolder { get; set; }
        }


        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            winform.FolderBrowserDialog dialog = new winform.FolderBrowserDialog();
            dialog.InitialDirectory = "E:\\WPF";
            winform.DialogResult result = dialog.ShowDialog();
            if(result == winform.DialogResult.OK)
            {
                txtPath.Text = dialog.SelectedPath;
                getList(txtPath.Text);
            }
            
        }
        private void getList(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            txtName.Text = directoryInfo.Name;

            FileSystemInfo[] items = directoryInfo.GetFileSystemInfos();
            List<FileSystemItem> fileSystemItemList = new List<FileSystemItem>();

            foreach (FileSystemInfo item in items)
            {
                FileSystemItem fileSystemItem = new FileSystemItem
                {
                    Name = item.Name,
                    Path = item.FullName,
                    Icon = GetIconForItemType((item is DirectoryInfo) ? "Folder" : "File"),
                    IsFolder = item is DirectoryInfo ? true : false

                };

                fileSystemItemList.Add(fileSystemItem);
            }

            lsvList.ItemsSource = fileSystemItemList;
        }
        private BitmapImage GetIconForItemType(string itemType)
        {
            // Example: Use built-in icons for folders and files
            string iconPath = (itemType == "Folder") ? "E:\\WPF\\FileDialog\\icon\\folder.png" : "E:\\WPF\\FileDialog\\icon\\file.png";

            BitmapImage icon = new BitmapImage(new Uri(iconPath));
            return icon;
        }

        private void btnCreate_click(object sender, RoutedEventArgs e)
        {
            string folderPath = txtPath.Text.Trim();
            string folderName = txtName.Text.Trim();

            if(cbName.SelectedIndex == 0)
            {
                if (!string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(folderName))
                {
                    string fullPath = Path.Combine(folderPath, folderName);

                    try
                    {
                        Directory.CreateDirectory(fullPath);
                        winform.MessageBox.Show("Folder created successfully.");
                        getList(txtPath.Text);
                    }
                    catch (Exception ex)
                    {
                        winform.MessageBox.Show($"Error creating folder: {ex.Message}", "Error");
                    }
                }
                else
                {
                    winform.MessageBox.Show("Please enter both folder path and name.");
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(folderName))
                {
                    string fullFilePath = Path.Combine(folderPath, folderName + ".txt"); 
                    try
                    {
                        
                        File.Create(fullFilePath).Close();
                        winform.MessageBox.Show("File created successfully.");
                        getList(txtPath.Text);
                    }
                    catch (Exception ex)
                    {
                        winform.MessageBox.Show($"Error creating file: {ex.Message}", "Error");
                    }
                }
                else
                {
                    winform.MessageBox.Show("Please enter both file path and name.");
                }
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            FileSystemItem selectedFolder = (FileSystemItem)lsvList.SelectedItem;

            if (selectedFolder != null)
            {
                try
                {
                    if (selectedFolder.IsFolder)
                    {
                        Directory.Delete(selectedFolder.Path, true);
                        winform.MessageBox.Show("Folder deleted successfully.");
                        getList(txtPath.Text);
                    }
                    else
                    {
                        File.Delete(selectedFolder.Path);
                        winform.MessageBox.Show("File deleted successfully.");
                        getList(txtPath.Text);
                    }
                }
                catch (Exception ex)
                {
                    winform.MessageBox.Show($"Error deleting folder: {ex.Message}", "Error");
                }
            }
            else
            {
                winform.MessageBox.Show("Please select a folder to delete.", "Error");
            }
        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            FileSystemItem selectedFolder = (FileSystemItem)lsvList.SelectedItem;
            string newFolderName = txtName.Text.Trim();

            if (selectedFolder != null && !string.IsNullOrWhiteSpace(newFolderName))
            {
                string newPath = Path.Combine(Path.GetDirectoryName(selectedFolder.Path), newFolderName);

                try
                {
                    Directory.Move(selectedFolder.Path, newPath);
                    
                    winform.MessageBox.Show("Folder renamed successfully.");
                    getList(txtPath.Text);
                }
                catch (Exception ex)
                {
                    winform.MessageBox.Show($"Error renaming folder: {ex.Message}", "Error");
                }
            }
            else
            {
                winform.MessageBox.Show("Please select a folder and enter a new name to rename.", "Error");
            }
        }

        //write the function to rename the file 
        private void btnRenameFile_Click(object sender, RoutedEventArgs e)
        {
            FileSystemItem selectedFolder = (FileSystemItem)lsvList.SelectedItem;
            string newFolderName = txtName.Text.Trim();

            if (selectedFolder != null && !string.IsNullOrWhiteSpace(newFolderName))
            {
                string newPath = Path.Combine(Path.GetDirectoryName(selectedFolder.Path), newFolderName);

                try
                {
                    if(selectedFolder.IsFolder == false)
                    {
                        File.Move(selectedFolder.Path, newPath);

                        winform.MessageBox.Show("File renamed successfully.");
                        getList(txtPath.Text);
                    }
                    else
                    {
                        Directory.Move(selectedFolder.Path, newPath);

                        winform.MessageBox.Show("Folder renamed successfully.");
                        getList(txtPath.Text);
                    }
                }
                catch (Exception ex)
                {
                    winform.MessageBox.Show($"Error renaming file: {ex.Message}", "Error");
                }
            }
            else
            {
                winform.MessageBox.Show("Please select a file and enter a new name to rename.", "Error");
            }
        }


        private void folderListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenDialog();
        }
        private void OpenDialog()
        {
            FileSystemItem selectedFolder = (FileSystemItem)lsvList.SelectedItem;

            if (selectedFolder != null)
            {
                try
                {
                    if (selectedFolder.IsFolder)
                    {
                        txtPath.Text = selectedFolder.Path;
                        getList(txtPath.Text);
                    }
                    else
                    {
                        if (File.Exists(selectedFolder.Path))
                        {
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(selectedFolder.Path);
                            psi.UseShellExecute = true;
                            System.Diagnostics.Process.Start(psi);
                        }
                        else
                        {
                            winform.MessageBox.Show("File not found or no associated application.", "Error");
                        }
                    }
                }
                catch (Exception ex)
                {
                    winform.MessageBox.Show($"Error opening folder: {ex.Message}", "Error");
                }
            }
        }
        private void btnOpenDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //in function to open Webcam.xaml and close MainWindow.xaml
            Webcam webcam = new Webcam();
            webcam.Show();
            this.Close();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string path = txtPath.Text;
            string parent = Directory.GetParent(path).FullName;
            txtPath.Text = parent;
            getList(txtPath.Text);
        }
    }
}