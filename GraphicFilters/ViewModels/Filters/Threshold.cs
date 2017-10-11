using System.Drawing;

namespace GraphicFilters.ViewModels.Filters
{
    public class Threshold
    {
        private float[,] pixelArray;
        float sum;
        int s, t, x1, x2, x3, y1, y2, y3, count;

        public Bitmap Run(Bitmap img, int pixelsWindow, int percentage)
        {
            var outImg = new Bitmap(img.Width, img.Height);
            pixelArray = new float[img.Width, img.Height];
            s = pixelsWindow;
            t = percentage;
            var bytes = ConvertToBytes(img);

            for (int x = 0; x < img.Width; x++)
            {
                sum = 0;

                for (int y = 0; y < img.Height; y++)
                {
                    sum += img.GetPixel(x, y).GetBrightness();

                    if (x == 0)
                    {
                        pixelArray[x, y] = sum;
                    }
                    else
                    {
                        pixelArray[x, y] = pixelArray[x - 1, y] + sum;
                    }
                }
            }

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    x1 = x - s / 2;
                    x2 = x + s / 2;
                    x3 = x1 - 1;
                    y1 = y - s / 2;
                    y2 = y + s / 2;
                    y3 = y1 - 1;

                    if (x2 >= img.Width)
                    {
                        x2 = img.Width-1;
                    }

                    if (x3 < 0)
                    {
                        x3 = 0;
                    }

                    if (y2 >= img.Height)
                    {
                        y2 = img.Height-1;
                    }

                    if (y3 < 0)
                    {
                        y3 = 0;
                    }

                    count = (x2 - x1) * (y2 - y1);
                    sum = pixelArray[x2, y2] - pixelArray[x2, y3] - pixelArray[x3, y2] + pixelArray[x3, y3];

                    if ((img.GetPixel(x, y).GetBrightness() * count) < (sum * (100 - t) / 100))
                    {
                        outImg.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        outImg.SetPixel(x, y, Color.White);
                    }
                }
            }
            return outImg;
        }

        public byte[] ConvertToBytes(Bitmap img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}
