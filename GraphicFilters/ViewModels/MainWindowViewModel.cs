using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using GraphicFilters.ViewModels.Commands;
using GraphicFilters.ViewModels.Services;
using GraphicFilters.ViewModels.Services.Interfaces;
using GraphicFilters.Views.Dialogs;

namespace GraphicFilters.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        IFileService fileService;
        BitmapSource sourceImage;
        Bitmap imgBitmap;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            fileService = new FileService();
        }

        public BitmapSource SourceImage
        {
            get { return sourceImage; }
            set
            {
                sourceImage = value;
                OnPropertyChanged("SourceImage");
            }
        }

        public ICommand OpenImageCommand { get { return new RelayCommand(OpenImage, CanExecute); } }

        public ICommand ThresholdCommand { get { return new RelayCommand(OpenThresholdDialog, CanThresholdExecute); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OpenImage()
        {
            imgBitmap = fileService.OpenImage();
            SourceImage = Imaging.CreateBitmapSourceFromHBitmap(
                imgBitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
                );
        }

        private void OpenThresholdDialog()
        {
            var thresholdDialog = new ThresholdDialog()
            {
                DataContext = new ThresholdDialogViewModel(ref sourceImage, ref imgBitmap, OnPropertyChanged),
                Owner = App.Current.MainWindow
            };
            thresholdDialog.Show();
        }
        private bool CanThresholdExecute()
        {
            if (sourceImage != null)
            {
                return true;
            }

            return false;
        }

        private bool CanExecute()
        {
            return true;
        }
    }
}
