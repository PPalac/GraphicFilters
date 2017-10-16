using System.Data;
using System.Drawing;
using Microsoft.Win32;

namespace GraphicFilters.ViewModels.Services
{
    public class FileService
    {
        private Bitmap sourceImage;

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

            sourceImage = new Bitmap(filepicker.FileName);
            return sourceImage;
        }

        public void SaveKernel(DataTable kernel)
        {
            var fileDialog = new SaveFileDialog()
            {
                Filter = "Graphic Filters Kernel (*.gf)| *.gf",
                AddExtension = true,
                DefaultExt = "gf"
            };

            fileDialog.ShowDialog();

            if (string.IsNullOrEmpty(fileDialog.FileName))
            {
                return;
            }

            kernel.WriteXmlSchema(fileDialog.FileName + "s");
            kernel.WriteXml(fileDialog.FileName, XmlWriteMode.IgnoreSchema);
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
            kernel.Reset();

            kernel.ReadXmlSchema(filePicker.FileName + "s");
            kernel.ReadXml(filePicker.FileName);

            return kernel;
        }
    }
}
