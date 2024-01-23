using AForge.Video;
using AForge.Video.DirectShow;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Drawing.Imaging;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using System.Windows.Input;
using System.Diagnostics;
using Firebase.Storage;
using System.Windows.Media;
using System;

namespace FileDialog
{
    /// <summary>
    /// Interaction logic for Webcam.xaml
    /// </summary>
    public partial class Webcam : Window
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private DispatcherTimer timer;
        private Bitmap currentFrame;

        public class SnapshotItem
        {
            public string? FileName { get; set; }
            public string? Path { get; set; }
            public BitmapImage? Image { get; set; }
            public DateTime CaptureDate { get; set; }

        }
        public Webcam()
        {
            InitializeComponent();
            Loaded += Webcam_Loaded;
            Closing += Webcam_Closing;
        }

        private void Webcam_Loaded(object sender, RoutedEventArgs e)
        {
            InitializaWebcam();
        }
        private void Webcam_Closing(object sender, System.ComponentModel.CancelEventArgs e) 
        {
            StopvideoSource();
        }

        private void InitializaWebcam()
        {
            //get avaiable video device(webcam)
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            //check at least 1 video device is avaiable
            if (videoDevices.Count > 0 )
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;

                //Start the video source
                videoSource.Start();
            }
            else
            {
                System.Windows.MessageBox.Show("No video device found!");
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var bitmap = eventArgs.Frame;
            currentFrame = (Bitmap)eventArgs.Frame.Clone();

            //update the image with the new frame from the webcam
            Dispatcher.Invoke(() =>
            {
                var bitmapImage = ConvertBitmapToBitmapImage(bitmap);
                camImg.Source = bitmapImage;
            });
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream,System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
         private async void StopvideoSource()
        {
            if(videoSource != null && videoSource.IsRunning) 
            {
                videoSource.SignalToStop();
                await Task.Run(() => videoSource.WaitForStop());
            }
        }

        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            txtPath.Text = dialog.SelectedPath;

            var nowTime = DateTime.Now;
            ScanAndDisplaySnapshots(txtPath.Text, nowTime);
            
        }
        private void ScanAndDisplaySnapshots(string directoryPath, DateTime captureDate)
        {
            try
            {
                if (string.IsNullOrEmpty(txtPath.Text))
                {
                    System.Windows.MessageBox.Show("Plase choose a directory");
                    return;
                }
                else
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        System.Windows.MessageBox.Show("Directory is not exist!");
                        return;
                    }
                    else
                    {
                        string[] sapshotFiles = Directory.GetFiles(directoryPath, "*.png");
                        lsvImge.Items.Clear();
                        foreach (string filePath in sapshotFiles)
                        {
                            string filename = System.IO.Path.GetFileName(filePath);

                            BitmapImage bitmapImage = new BitmapImage();
                            using (FileStream stream = File.OpenRead(filePath))
                            {
                                bitmapImage.BeginInit();
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.StreamSource = stream;
                                bitmapImage.EndInit();
                                bitmapImage.Freeze();
                            }

                            var listViewItem = new System.Windows.Controls.ListViewItem();
                            listViewItem.Content = new SnapshotItem
                            {
                                FileName = filename,
                                Path = filePath,
                                Image = bitmapImage
                            };
                            lsvImge.Items.Add(listViewItem);
                        }
                        lsvImge.Items.SortDescriptions.Add(
                            new System.ComponentModel.SortDescription("CaptureDate", System.ComponentModel.ListSortDirection.Descending));
                    }
                    
                }
            }
            catch(Exception e) 
            {
                System.Windows.MessageBox.Show($"Error scanning directory: {e.Message}");
            }
        }

        private ImageSource GetImage(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (FileStream stream = File.OpenRead(path))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                System.Windows.MessageBox.Show("Please choose a directory!");
                return;
            }
            else
            {
                SnapShot(txtPath.Text);
            }
        }

        private void SnapShot(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    System.Windows.MessageBox.Show("Directory does not exist.");
                    return;
                }
                else
                {
                    var captureDate = DateTime.Now;
                    if(currentFrame != null)
                    {
                        string filename = $"snapshot_{captureDate:yyyyyMMddHHss}.png";
                        string filepath = System.IO.Path.Combine(path, filename);

                        currentFrame.Save(filepath, ImageFormat.Png);
                        System.Windows.MessageBox.Show($"Snapshot saved to {filepath}");

                        ScanAndDisplaySnapshots(path, captureDate);
                    }
                }
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show($"Error capture snapshot: {e.Message}");
            }
        }

        private void Open_Image(object sender, RoutedEventArgs e)
        {
            if (lsvImge.SelectedItem is System.Windows.Controls.ListViewItem selectedItem)
            {
                if(selectedItem.Content is SnapshotItem snapshotItem)
                {
                    string seletedImage = snapshotItem.Path;
                    //if(!string.IsNullOrEmpty(seletedImage))
                    //{
                    //    try
                    //    {
                    //        Process.Start(new ProcessStartInfo
                    //        {
                    //            FileName = seletedImage,
                    //            UseShellExecute = true,
                    //            WorkingDirectory = System.IO.Path.GetDirectoryName(seletedImage)
                    //        });
                    //    }
                    //    catch(Exception ex)
                    //    {
                    //        System.Windows.MessageBox.Show($"Error opening image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    //    }
                    //}
                    lsvImge.Items.Clear();
                    Image image = new Image();
                    image.OpenImage(seletedImage);
                    image.Show();
                    this.Close();
                }
            }
        }

        private void OnDeleteImageClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lsvImge.SelectedItem is System.Windows.Controls.ListViewItem selectedItem)
                {
                    if (selectedItem.Content is SnapshotItem snapshotItem)
                    {
                        string remoteStoragePath = "images" + snapshotItem.FileName;
                        string path = snapshotItem.Path;
                        string absoluteImagePath = GetAbsolutePath(path);
                        if (File.Exists(absoluteImagePath))
                        {
                            File.Delete(absoluteImagePath);

                            System.Windows.MessageBox.Show("Image deleted successfully.");
                            var nowTime = DateTime.Now;
                            ScanAndDisplaySnapshots(txtPath.Text, nowTime);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Image file not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error deleting image: {ex.Message}");
            }
        }
        private string GetAbsolutePath(string packUri)
        {
            return System.IO.Path.Combine(Environment.CurrentDirectory, packUri.TrimStart('/').Replace('/', '\\'));
        }

        private async void Upload_Image(object sender, RoutedEventArgs e)
        {
            if (lsvImge.SelectedItem is System.Windows.Controls.ListViewItem selectedItem)
            {
                if (selectedItem.Content is SnapshotItem snapshotItem)
                {
                    string remoteStoragePath = "images"+snapshotItem.FileName;
                    string path = snapshotItem.Path;
                    await UploadImageToFirebaseStorage(path,remoteStoragePath);
                    System.Windows.MessageBox.Show("Upload Successfull","Success",MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private async Task UploadImageToFirebaseStorage(string localImagePath, string remoteStoragePath)
        {
            try
            {
                byte[] fileBytes;

                // Read the file into a byte array
                using (FileStream fileStream = new FileStream(localImagePath, FileMode.Open, FileAccess.Read))
                {
                    fileBytes = new byte[fileStream.Length];
                    await fileStream.ReadAsync(fileBytes, 0, (int)fileStream.Length);
                }

                // Convert the byte array to a MemoryStream
                using (MemoryStream memoryStream = new MemoryStream(fileBytes))
                {
                    // Upload the MemoryStream to Firebase Storage
                    var task = new FirebaseStorage("uploadimage-76af7.appspot.com")
                        .Child(remoteStoragePath)
                        .PutAsync(memoryStream);
                    await task;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
