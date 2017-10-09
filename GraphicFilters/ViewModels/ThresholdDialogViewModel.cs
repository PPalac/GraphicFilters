using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using GraphicFilters.ViewModels.Commands;
using GraphicFilters.ViewModels.Filters;

namespace GraphicFilters.ViewModels
{
    public class ThresholdDialogViewModel : INotifyPropertyChanged
    {
        private int percentage, windowSize;
        private BitmapSource sourceImage;
        private Bitmap imgBitmap;

        public ThresholdDialogViewModel(ref BitmapSource SourceImage, ref Bitmap ImgBitmap, Action<String>Changed)
        {
            sourceImage = SourceImage;
            imgBitmap = ImgBitmap;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Percentage
        {
            get { return percentage; }
            set
            {
                percentage = value;
                OnPropertyChanged("Percentage");
            }
        }


        public int WindowSize
        {
            get { return windowSize; }
            set
            {
                windowSize = value;
                OnPropertyChanged("WindowSize");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand ApplyChangesCommand { get { return new RelayCommand(Threshold, CanExecute); } } 

        private void Threshold()
        {
            var threshold = new Threshold();
            sourceImage = Imaging.CreateBitmapSourceFromHBitmap(
                threshold.Run(imgBitmap,windowSize,percentage).GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
                );
        }

        private bool CanExecute()
        {
            return true;
        }
    }
}
