using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GraphicFilters.Filters
{
    public class Threshold
    {
        private Bitmap imgBitmap;
        private int windowSize;
        private int percentage;
        private float[,] pixelArray;

        public Threshold(Bitmap bitmap, int windowSize, int percent)
        {
            imgBitmap = bitmap;
            this.windowSize = windowSize;
            percentage = percent;
        }

        public Bitmap Run()
        {
            float sum;
            int x1, x2, x3, y1, y2, y3, count;

            Bitmap outBitmap = imgBitmap.Clone(new Rectangle(0, 0, imgBitmap.Width, imgBitmap.Height), imgBitmap.PixelFormat);

            BitmapData outBitmapData = outBitmap.LockBits(new Rectangle(0, 0, imgBitmap.Width, imgBitmap.Height), ImageLockMode.ReadWrite, imgBitmap.PixelFormat);
            BitmapData bitmapData = imgBitmap.LockBits(new Rectangle(0, 0, imgBitmap.Width, imgBitmap.Height), ImageLockMode.ReadWrite, imgBitmap.PixelFormat);

            pixelArray = new float[imgBitmap.Width, imgBitmap.Height];
            int bytesPerPixel = Bitmap.GetPixelFormatSize(imgBitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * imgBitmap.Height;
            byte[] pixels = new byte[byteCount];
            byte[] outPixels = new byte[byteCount];

            IntPtr ptrFirstPixel = bitmapData.Scan0;
            IntPtr ptrFirstOutPixel = outBitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            Marshal.Copy(ptrFirstOutPixel, outPixels, 0, outPixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            Color cl;

            for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
            {
                sum = 0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    int currentLine = y * bitmapData.Stride;

                    if (bytesPerPixel == 1)
                    {
                        cl = Color.FromArgb(pixels[currentLine + x], pixels[currentLine + x], pixels[currentLine + x]);
                    }
                    else
                    {
                        cl = Color.FromArgb(pixels[currentLine + x + 2], pixels[currentLine + x + 1], pixels[currentLine + x]);
                    }
                    sum += cl.GetBrightness();

                    if (x == 0)
                    {
                        pixelArray[x / bytesPerPixel, y] = sum;
                    }
                    else
                    {
                        pixelArray[x / bytesPerPixel, y] = pixelArray[(x / bytesPerPixel) - 1, y] + sum;
                    }
                }
            }

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bitmapData.Stride;

                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    x1 = (x / bytesPerPixel) - (windowSize / 2);
                    x2 = (x / bytesPerPixel) + (windowSize / 2);
                    x3 = x1 - 1;
                    y1 = y - (windowSize / 2);
                    y2 = y + (windowSize / 2);
                    y3 = y1 - 1;

                    if (x2 >= imgBitmap.Width)
                    {
                        x2 = imgBitmap.Width - 1;
                    }

                    if (x3 < 0)
                    {
                        x3 = 0;
                    }

                    if (y2 >= imgBitmap.Height)
                    {
                        y2 = imgBitmap.Height - 1;
                    }

                    if (y3 < 0)
                    {
                        y3 = 0;
                    }

                    count = (x2 - x1) * (y2 - y1);
                    sum = pixelArray[x2, y2] - pixelArray[x2, y3] - pixelArray[x3, y2] + pixelArray[x3, y3];

                    if (bytesPerPixel == 1)
                    {
                        if ((Color.FromArgb(pixels[currentLine + x], pixels[currentLine + x], pixels[currentLine + x]).GetBrightness() * count) < (sum * (100 - percentage) / 100))
                        {
                            outPixels[currentLine + x] = 0;
                        }
                        else
                        {
                            outPixels[currentLine + x] = 255;
                        }
                    }
                    else
                    {
                        if ((Color.FromArgb(pixels[currentLine + x + 2], pixels[currentLine + x + 1], pixels[currentLine + x]).GetBrightness() * count) < (sum * (100 - percentage) / 100))
                        {
                            outPixels[currentLine + x] = 0;
                            outPixels[currentLine + x + 1] = 0;
                            outPixels[currentLine + x + 2] = 0;
                        }
                        else
                        {
                            outPixels[currentLine + x] = 255;
                            outPixels[currentLine + x + 1] = 255;
                            outPixels[currentLine + x + 2] = 255;
                        }
                    }
                }
            }

            Marshal.Copy(outPixels, 0, ptrFirstOutPixel, pixels.Length);
            outBitmap.UnlockBits(outBitmapData);
            imgBitmap.UnlockBits(bitmapData);

            return outBitmap;
        }
    }
}

