using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Input;
using GraphicFilters.Models;
using GraphicFilters.ViewModels.Commands;
using GraphicFilters.ViewModels.Filters;
using GraphicFilters.ViewModels.Services;

namespace GraphicFilters.ViewModels
{
    public class GaussianBlurDialogViewModel : INotifyPropertyChanged
    {
        public Action Close;

        private DataTable kernel;
        private int kernelSize;
        private ImageModel img;
        private Bitmap originalBitmap;
        private Action<string> MainWindowPropertyChanged;

        public GaussianBlurDialogViewModel(ImageModel img, Action<string> mainWindowPropChanged)
        {
            kernel = new DataTable();
            kernel.TableName = "Kernel";
            kernelSize = 3;
            this.img = img;
            originalBitmap = new Bitmap(img.ImgBitmap);
            MainWindowPropertyChanged = mainWindowPropChanged;


            for (int i = 0; i < 3; i++)
            {
                kernel.Columns.Add();
            }

            for (int i = 0; i < 3; i++)
            {
                kernel.Rows.Add(new object[] { "1", "1", "1" });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DataTable Kernel
        {
            get { return kernel; }
            set
            {
                kernel = value;
                OnPropertyChanged(nameof(Kernel));
            }
        }

        public int KernelSize
        {
            get { return kernelSize; }
            set
            {
                if (value <= 3)
                {
                    kernelSize = 3;
                }
                else
                {
                    if (value % 2 == 0)
                    {
                        kernelSize = value - 1;
                    }
                    else
                    {
                        kernelSize = value;
                    }
                }

                OnKernelSizeChanged();
            }
        }

        public ICommand ApplyChangesCommand { get { return new RelayCommand(RunBlur); } }

        public ICommand DiscardChangesCommand { get { return new RelayCommand(DiscardChanges); } }

        public ICommand SaveCommand { get { return new RelayCommand(Save); } }

        public ICommand CloseCommand { get { return new RelayCommand(CloseDialog); } }

        public ICommand LoadKernelCommand { get { return new RelayCommand(LoadKernel); } }

        public ICommand SaveKernelCommand { get { return new RelayCommand(SaveKernel); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RunBlur()
        {
            var kernelArr = GetKernel();

            img.ImgBitmap = new Bitmap(originalBitmap);

            var blur = new GaussianBlur(img, kernelSize, kernelArr);
            blur.WorkFinished += Blur_WorkFinished;
            blur.Run();
        }

        private void Blur_WorkFinished(object sender, EventArgs e)
        {
            MainWindowPropertyChanged.Invoke("SourceImage");
        }

        private void OnKernelSizeChanged()
        {
            object[] rowData = new object[kernelSize];
            kernel = new DataTable();

            for (int i = 0; i < kernelSize; i++)
            {
                rowData[i] = "1";
                kernel.Columns.Add();
            }

            for (int i = 0; i < kernelSize; i++)
            {
                kernel.Rows.Add(rowData);
            }

            OnPropertyChanged("Kernel");
        }

        private void DiscardChanges()
        {
            img.ImgBitmap = originalBitmap;
            img.SetSourceImage(img.ImgBitmap);
            MainWindowPropertyChanged.Invoke("SourceImage");
        }

        private void Save()
        {
            originalBitmap = img.ImgBitmap;
            Close.Invoke();
        }

        private void CloseDialog()
        {
            Close.Invoke();
        }

        private float[] GetKernel()
        {
            var columns = kernel.Columns.Count;
            var rows = kernel.Rows.Count;
            int index = 0;

            float[] kernelArr = new float[columns * rows];

            for (int i = 0; i < columns; i++)
            {
                foreach (var element in kernel.Rows[i].ItemArray)
                {
                    kernelArr[index] = float.Parse(element.ToString());
                    index++;
                }
            }

            return kernelArr;
        }

        private void SaveKernel()
        {
            var fileService = new FileService();

            fileService.SaveKernel(kernel);
        }

        private void LoadKernel()
        {
            var fileService = new FileService();

            var newKernel = fileService.LoadKernel();

            if (newKernel == null)
            {
                return;
            }

            kernel = newKernel;

            OnPropertyChanged("Kernel");
        }
    }
}
