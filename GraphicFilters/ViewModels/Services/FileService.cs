using System.Data;
using System.Drawing;
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

            if (string.IsNullOrEmpty(filepicker.FileName))
            {
                return null;
            }

            SourceImage = new Bitmap(filepicker.FileName);
            return SourceImage;
        }

        public void SaveKernel(DataTable kernel)
        {
            var fileDialog = new SaveFileDialog();

            fileDialog.ShowDialog();

            if (string.IsNullOrEmpty(fileDialog.FileName))
            {
                return;
            }

            kernel.WriteXmlSchema(fileDialog.FileName + ".gfs");
            kernel.WriteXml(fileDialog.FileName + ".gf", XmlWriteMode.IgnoreSchema);
        }

        public DataTable LoadKernel()
        {
            var filePicker = new OpenFileDialog()
            {
                Filter = "Graphic Filters Kernel (*.gf)| *.gf"
            };

            filePicker.ShowDialog();

            if (string.IsNullOrEmpty(filePicker.FileName))
            {
                return null;
            }

            DataTable kernel = new DataTable();

            kernel.ReadXmlSchema(filePicker.FileName + "s");
            kernel.ReadXml(filePicker.FileName);

            return kernel;
        }
    }
}
