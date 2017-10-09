using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using GraphicFilters.ViewModels.Services.Interfaces;
using Microsoft.Win32;

namespace GraphicFilters.ViewModels.Services
{
    public class FileService : IFileService
    {
        private Bitmap SourceImage;
        public Bitmap OpenImage()
        {
            var filepicker = new OpenFileDialog()
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png | Bitmap (*.bmp) | *.bmp"
            };

            filepicker.ShowDialog();
            SourceImage = new Bitmap(filepicker.FileName);

            return SourceImage;

        }
    }
}
