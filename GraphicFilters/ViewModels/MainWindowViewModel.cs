using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GraphicFilters.Models;
using GraphicFilters.Commands;
using GraphicFilters.Services;
using GraphicFilters.Views.Dialogs;

namespace GraphicFilters.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ImageModel img;

        public MainWindowViewModel()
        {
            img = new ImageModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapSource SourceImage
        {
            get { return img.SourceImage; }
            set
            {
                img.SourceImage = value;

                OnPropertyChanged(nameof(SourceImage));
            }
        }

        public ICommand OpenImageCommand { get { return new RelayCommand(OpenImage); } }

        public ICommand ThresholdCommand { get { return new RelayCommand(OpenThresholdDialog, CanFilterExecute); } }

        public ICommand GaussianBlurCommand { get { return new RelayCommand(OpenGaussianBlurDialog, CanFilterExecute); } }

        public ICommand ExitCommand { get { return new RelayCommand(Exit); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OpenImage()
        {
            var fileService = new FileService();
            var bitmap = fileService.OpenImage();

            if (bitmap == null)
            {
                return;
            }

            img = new ImageModel(bitmap);

            OnPropertyChanged(nameof(SourceImage));
        }

        private void OpenThresholdDialog()
        {
            var thresholdDialog = new ThresholdDialog()
            {
                DataContext = new ThresholdDialogViewModel(img, OnPropertyChanged),
                Owner = App.Current.MainWindow
            };

            ((ThresholdDialogViewModel)thresholdDialog.DataContext).Close += thresholdDialog.Close;

            thresholdDialog.Show();
        }

        private void OpenGaussianBlurDialog()
        {
            var gaussianBlurDialog = new GaussianBlurDialog
            {
                DataContext = new GaussianBlurDialogViewModel(img, OnPropertyChanged),
                Owner = App.Current.MainWindow
            };

            ((GaussianBlurDialogViewModel)gaussianBlurDialog.DataContext).Close += gaussianBlurDialog.Close;

            gaussianBlurDialog.Show();
        }
        private bool CanFilterExecute()
        {
            if (img.SourceImage != null)
            {
                return true;
            }

            return false;
        }

        private void Exit()
        {
            App.Current.MainWindow.Close();
        }

        private void SetBitmap(Bitmap map)
        {
            img.SetSourceImage(map);

            OnPropertyChanged(nameof(SourceImage));
        }
    }
}
