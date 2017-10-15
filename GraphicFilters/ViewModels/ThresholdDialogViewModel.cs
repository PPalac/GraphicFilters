using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using GraphicFilters.Models;
using GraphicFilters.ViewModels.Commands;
using GraphicFilters.ViewModels.Filters;

namespace GraphicFilters.ViewModels
{
    public class ThresholdDialogViewModel : INotifyPropertyChanged
    {
        private int percentage, windowSize;
        private ImageModel img;
        private Action<String> MainWindowPropChanged;
        private Bitmap bitmap;

        public Action Close;

        public ThresholdDialogViewModel(ImageModel imgModel, Action<String>Changed)
        {
            img = imgModel;
            MainWindowPropChanged = Changed;
            windowSize = 3;
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

        public ICommand ApplyChangesCommand { get { return new RelayCommand(RunThreshold); } }

        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }

        public ICommand SaveCommand { get { return new RelayCommand(Save); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RunThreshold()
        {
            var threshold = new Threshold();
            bitmap = threshold.Run(img.ImgBitmap, windowSize, percentage);

            img.SetSourceImage(bitmap);

            MainWindowPropChanged.Invoke("SourceImage");
        }

        private void Cancel()
        {
            img.SetSourceImage(img.ImgBitmap);
            MainWindowPropChanged.Invoke("SourceImage");
            //Close.Invoke();   
        }

        private void Save()
        {
            img.ImgBitmap = bitmap;
            Close.Invoke();
        }
    }
}
