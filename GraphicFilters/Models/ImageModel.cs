using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace GraphicFilters.Models
{
    public class ImageModel
    {
        public ImageModel()
        {
        }

        public ImageModel(Bitmap bitmap)
        {
            this.ImgBitmap = bitmap;

            SetSourceImage(bitmap);
        }

        public BitmapSource SourceImage { get; set; }

        public Bitmap ImgBitmap { get; set; }

        public void SetSourceImage(Bitmap bitmap)
        {
            SourceImage = Imaging.CreateBitmapSourceFromHBitmap(
               bitmap.GetHbitmap(),
               IntPtr.Zero,
               Int32Rect.Empty,
               BitmapSizeOptions.FromEmptyOptions());
        }
    }
}