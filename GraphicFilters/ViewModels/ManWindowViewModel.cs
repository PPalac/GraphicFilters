using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace GraphicFilters.ViewModels
{
    public class ManWindowViewModel : INotifyPropertyChanged
    {
        public ManWindowViewModel()
        {
            SourceImage = new BitmapImage(new Uri("../../large.jpg", UriKind.RelativeOrAbsolute));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage SourceImage { get; set; }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public void Open_Click(object sender, RoutedEventArgs e)
        //{
        //    var filepicker = new OpenFileDialog()
        //    {
        //        Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png | Bitmap (*.bmp) | *.bmp"
        //    };
        //    filepicker.ShowDialog();
        //}
    }
}
