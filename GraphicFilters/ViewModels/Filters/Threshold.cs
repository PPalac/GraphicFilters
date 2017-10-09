using System.Drawing;

namespace GraphicFilters.ViewModels.Filters
{
    public class Threshold
    {
        private int[,] pixelArray;
        int sum;
        int s, t, x1, x2, y1, y2, count;

        public Bitmap Run(Bitmap img, int pixelsWindow, int percentage)
        {
            var outImg = new Bitmap(img.Width, img.Height);
            pixelArray = new int[img.Width, img.Height];
            s = pixelsWindow;
            t = percentage;

            for (int x = 0; x < img.Width; x++)
            {
                sum = 0;

                for (int y = 0; y < img.Height; y++)
                {
                    sum += img.GetPixel(x, y).ToArgb();

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
                    y1 = y - s / 2;
                    y2 = y + s / 2;
                    if (x1 <= 0)
                    {
                        x1 = 1;
                    }
                    if (y1 <= 0)
                    {
                        y1 = 1;
                    }
                    if (x1 >= img.Width)
                    {
                        x1 = img.Width - 1;
                    }
                    if (y1 > img.Height)
                    {
                        y1 = img.Height - 1;
                    }
                    if (x2 >= img.Width)
                    {
                        x2 = img.Width - 1;
                    }
                    if (y2 >= img.Height)
                    {
                        y2 = img.Height - 1;
                    }
                    count = (x2 - x1) * (y2 - y1);
                    sum = pixelArray[x2, y2] - pixelArray[x2, y1 - 1] - pixelArray[x1 - 1, y2] + pixelArray[x1 - 1, y1 - 1];
                    if ((img.GetPixel(x, y).ToArgb() * count) < (sum * (100 - t) / 100))
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
    }
}
