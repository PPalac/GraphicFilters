using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GraphicFilters.Models;
using GraphicFilters.ViewModels.Commands;
using GraphicFilters.ViewModels.Filters;
using GraphicFilters.ViewModels.Services;
using GraphicFilters.ViewModels.Services.Interfaces;
using GraphicFilters.Views.Dialogs;

namespace GraphicFilters.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        IFileService fileService;
        private ImageModel img;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            fileService = new FileService();
            img = new ImageModel();
        }

        public BitmapSource SourceImage
        {
            get { return img.SourceImage; }
            set
            {
                img.SourceImage = value;
                OnPropertyChanged("SourceImage");
            }
        }

        public ICommand OpenImageCommand { get { return new RelayCommand(OpenImage); } }

        public ICommand ThresholdCommand { get { return new RelayCommand(OpenThresholdDialog, CanThresholdExecute); } }

        public ICommand GaussianBlurCommand { get { return new RelayCommand(OpenGaussianBlurDialog); } }

        public ICommand ExitCommand { get { return new RelayCommand(Exit); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OpenImage()
        {
            var bitmap = fileService.OpenImage();
            if (bitmap == null)
            {
                return;
            }

            img = new ImageModel(bitmap);
            OnPropertyChanged("SourceImage");
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
        private bool CanThresholdExecute()
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
            OnPropertyChanged("SourceImage");
        }
    }
}
