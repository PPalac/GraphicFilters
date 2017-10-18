using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using GraphicFilters.Models;
using GraphicFilters.Commands;
using GraphicFilters.Filters;

namespace GraphicFilters.ViewModels
{
    public class ThresholdDialogViewModel : INotifyPropertyChanged
    {
        private const byte DEFAULTWINDOWSIZE = 3;

        public Action Close;

        private int percentage, windowSize;
        private ImageModel img;
        private Action<string> MainWindowPropChanged;
        private Bitmap bitmap;
        private bool isFilterExecuting = false;

        public ThresholdDialogViewModel(ImageModel imgModel, Action<string> changed)
        {
            img = imgModel;
            MainWindowPropChanged = changed;
            windowSize = DEFAULTWINDOWSIZE;
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

        public ICommand ApplyChangesCommand { get { return new RelayCommand(RunThreshold, () => !isFilterExecuting); } }

        public ICommand DiscardChangesCommand { get { return new RelayCommand(DiscardChanges); } }

        public ICommand SaveCommand { get { return new RelayCommand(Save); } }

        public ICommand CloseCommand { get { return new RelayCommand(CloseDialog); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RunThreshold()
        {
            isFilterExecuting = true;

            var threshold = new Threshold(img.ImgBitmap, windowSize, percentage);
            bitmap = threshold.Run();

            img.SetSourceImage(bitmap);

            MainWindowPropChanged.Invoke("SourceImage");

            isFilterExecuting = false;
        }

        private void DiscardChanges()
        {
            img.SetSourceImage(img.ImgBitmap);
            MainWindowPropChanged.Invoke("SourceImage");
        }

        private void Save()
        {
            if (bitmap == null)
            {
                bitmap = img.ImgBitmap;
            }
            img.ImgBitmap = bitmap;
            Close.Invoke();
        }

        private void CloseDialog()
        {
            Close.Invoke();
        }
    }
}
