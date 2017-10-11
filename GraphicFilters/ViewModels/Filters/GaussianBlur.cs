using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicFilters.ViewModels.Filters
{
    class GaussianBlur
    {
        private Bitmap inImage;
        private Bitmap outImage;
        private byte byte1;

        public GaussianBlur(Bitmap img)
        {
            inImage = img;
            outImage = new Bitmap(img.Width, img.Height);
        }

        public Bitmap Blur()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += BlurRed;
        }

        private void BlurRed(object sender, DoWorkEventArgs e)
        {
            byte[,] red = new byte[inImage.Width, inImage.Height];

            for (int x = 0; x < inImage.Width; x++)
            {
                for (int y = 0; y < inImage.Height; y++)
                {
                    red[x,y] = inImage.GetPixel(x, y).R;
                }
            }
        }

        private void BlurGreen()
        {
            byte[,] green = new byte[inImage.Width, inImage.Height];

            for (int x = 0; x < inImage.Width; x++)
            {
                for (int y = 0; y < inImage.Height; y++)
                {
                    green[x, y] = inImage.GetPixel(x, y).G;
                }
            }
        }

        private void BlurBlue()
        {
            byte[,] blue = new byte[inImage.Width, inImage.Height];

            for (int x = 0; x < inImage.Width; x++)
            {
                for (int y = 0; y < inImage.Height; y++)
                {
                    blue[x, y] = inImage.GetPixel(x, y).B;
                }
            }
        }
    }
}
