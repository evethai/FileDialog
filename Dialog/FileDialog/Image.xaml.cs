using System;
using System.Collections.Generic;
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
using static FileDialog.Webcam;

namespace FileDialog
{
    /// <summary>
    /// Interaction logic for Image.xaml
    /// </summary>
    public partial class Image : Window
    {
        public string imagePath;
        public Image()
        {
            InitializeComponent();
           
        }


        //function open image by address    
        public void OpenImage(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            using (FileStream stream = File.OpenRead(path))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            ImageMain.Source = bitmap;
            imagePath = path;
        }

        //write function delete image
        public void DeleteImage()
        {
            File.Delete(imagePath);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ImageMain.Source = null;
            DeleteImage();
            Webcam webcam = new Webcam();
            webcam.Show();
            this.Close();
        }
        private string GetAbsolutePath(string packUri)
        {
            return System.IO.Path.Combine(Environment.CurrentDirectory, packUri.TrimStart('/').Replace('/', '\\'));
        }
    }
}
