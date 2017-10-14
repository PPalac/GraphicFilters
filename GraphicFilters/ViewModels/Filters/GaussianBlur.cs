using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GraphicFilters.ViewModels.Filters
{
    class GaussianBlur
    {
        private Bitmap Img;
        private Bitmap outImg;
        private BackgroundWorker redWorker;
        private BackgroundWorker greenWorker;
        private BackgroundWorker blueWorker;
        private int kernelSize;
        private byte workersCounter;
        private byte[,] red;
        private byte[,] green;
        private byte[,] blue;
        private byte[,] bluredRedPixels;
        private byte[,] bluredGreenPixels;
        private byte[,] bluredBluePixels;
        private object syncObject = new object();

        public GaussianBlur(Bitmap img, int kernelSize)
        {
            Img = img;
            this.kernelSize = kernelSize;
            red = new byte[Img.Width, Img.Height];
            green = new byte[Img.Width, Img.Height];
            blue = new byte[Img.Width, Img.Height];
            outImg = new Bitmap(Img.Width, Img.Height);

            SetUpBackgroundWorkers();

        }

        public void Run()
        {
            GetColors();
            workersCounter = 3;
            redWorker.RunWorkerAsync(red);
            greenWorker.RunWorkerAsync(green);
            blueWorker.RunWorkerAsync(blue);
        }

        private void SetUpBackgroundWorkers()
        {
            redWorker = new BackgroundWorker();
            greenWorker = new BackgroundWorker();
            blueWorker = new BackgroundWorker();

            redWorker.WorkerReportsProgress = true;
            redWorker.DoWork += Blur;
            redWorker.ProgressChanged += ProgressChanged;
            redWorker.RunWorkerCompleted += RedWorkerFinished;

            greenWorker.WorkerReportsProgress = true;
            greenWorker.DoWork += Blur;
            greenWorker.ProgressChanged += ProgressChanged;
            greenWorker.RunWorkerCompleted += GreenWorkerFinished;

            blueWorker.WorkerReportsProgress = true;
            blueWorker.DoWork += Blur;
            blueWorker.ProgressChanged += ProgressChanged;
            blueWorker.RunWorkerCompleted += BlueWorkerFinished;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RedWorkerFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            bluredRedPixels = (byte[,])e.Result;

            lock (syncObject)
            {
                workersCounter--;
            }

            if (workersCounter == 0)
            {
                ApplyBlur();
            }
        }

        private void GreenWorkerFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            bluredGreenPixels = (byte[,])e.Result;

            lock (syncObject)
            {
                workersCounter--;
            }

            if (workersCounter == 0)
            {
                ApplyBlur();
            }
        }

        private void BlueWorkerFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            bluredBluePixels = (byte[,])e.Result;

            lock (syncObject)
            {
                workersCounter--;
            }

            if (workersCounter == 0)
            {
                ApplyBlur();
            }
        }

        public void GetColors()
        {
            BitmapData bitmapData = Img.LockBits(new Rectangle(0, 0, Img.Width, Img.Height), ImageLockMode.ReadWrite, Img.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(Img.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * Img.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            int currentWidthInPixels;

            for (int y = 0; y < heightInPixels; y++)
            {
                currentWidthInPixels = 0;
                int currentLine = y * bitmapData.Stride;
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    blue[currentWidthInPixels, y] = pixels[currentLine + x];
                    green[currentWidthInPixels, y] = pixels[currentLine + x + 1];
                    red[currentWidthInPixels, y] = pixels[currentLine + x + 2];
                    currentWidthInPixels++;
                }
            }
            Img.UnlockBits(bitmapData);
        }
        private void Blur(object sender, DoWorkEventArgs e)
        {
            byte[,] pixels = (byte[,])e.Argument;
            var width = pixels.GetLength(0);
            var height = pixels.GetLength(1);
            float sum = 0;
            int kernelX = 0;
            int kernelY = 0;
            byte[,] outPixels = new byte[width, height];
            float[] kernelArr = new float[9] { 1f, 2f, 1f, 2f, 4f, 2f, 1f, 2f, 1f };
            int iterator;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    iterator = 0;
                    sum = 0;

                    for (int x1 = x - kernelSize / 2; x1 <= x + kernelSize / 2; x1++)
                    {
                        if (x1 < 0)
                        {
                            kernelX = 0;
                        }
                        else
                        {
                            if (x1 < width)
                            {
                                kernelX = x1;
                            }
                            else
                            {
                                kernelX = width - 1;
                            }
                        }

                        for (int y1 = y - kernelSize / 2; y1 <= y + kernelSize / 2; y1++)
                        {
                            if (y1 < 0)
                            {
                                kernelY = 0;
                            }
                            else
                            {
                                if (y1 < height)
                                {
                                    kernelY = y1;
                                }
                                else
                                {
                                    kernelY = height - 1;
                                }
                            }

                            sum += pixels[kernelX, kernelY] * kernelArr[iterator];
                            iterator++;
                        }
                    }

                    outPixels[x, y] = (byte)(sum / 16);
                }
            }

            e.Result = outPixels;
        }

        public void ApplyBlur()
        {
            BitmapData bitmapData = outImg.LockBits(new Rectangle(0, 0, outImg.Width, outImg.Height), ImageLockMode.ReadWrite, Img.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(Img.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * outImg.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            int currentWidthInPixels;
            int currentLine;

            for (int y = 0; y < heightInPixels; y++)
            {
                currentLine = y * bitmapData.Stride;
                currentWidthInPixels = 0;

                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    pixels[currentLine + x] = bluredBluePixels[currentWidthInPixels, y];
                    pixels[currentLine + x + 1] = bluredGreenPixels[currentWidthInPixels, y];
                    pixels[currentLine + x + 2] = bluredRedPixels[currentWidthInPixels, y];

                    currentWidthInPixels++;
                }
            }

            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            outImg.UnlockBits(bitmapData);
        }
    }
}
