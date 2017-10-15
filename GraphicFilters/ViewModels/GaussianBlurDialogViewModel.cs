﻿using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Input;
using GraphicFilters.Models;
using GraphicFilters.ViewModels.Commands;
using GraphicFilters.ViewModels.Filters;

namespace GraphicFilters.ViewModels
{
    public class GaussianBlurDialogViewModel : INotifyPropertyChanged
    {
        private DataTable kernel;
        private int kernelSize;
        private ImageModel img;
        private Bitmap originalBitmap;

        private Action<string> MainWindowPropertyChanged;
        public Action Close;


        public GaussianBlurDialogViewModel(ImageModel img, Action<string> MainWindowPropChanged)
        {
            kernel = new DataTable();
            kernelSize = 3;
            this.img = img;
            originalBitmap = img.ImgBitmap;
            MainWindowPropertyChanged = MainWindowPropChanged;

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

        public ICommand CancelCommand { get { return new RelayCommand(CloseDialog); } }

        public ICommand SaveCommand { get { return new RelayCommand(Save); } }

        public ICommand CloseCommand { get { return new RelayCommand(CloseDialog); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RunBlur()
        {
            var kernelArr = GetKernel();
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
            kernel = new DataTable();

            for (int i = 0; i < kernelSize; i++)
            {
                kernel.Columns.Add();
            }

            for (int i = 0; i < kernelSize; i++)
            {
                kernel.Rows.Add();
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
                foreach (var element in (kernel.Rows[i].ItemArray))
                {
                    kernelArr[index] = float.Parse(element.ToString());
                    index++;
                }
            }

            return kernelArr;
        }
    }
}
