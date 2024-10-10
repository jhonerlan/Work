using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;

namespace Labeling
{
    public partial class MainWindow : Window
    {
        private Bitmap originalBitmap;
        private Bitmap processedImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                string file_name = ofd.FileName;
                BitmapImage originalImage = new BitmapImage(new Uri(file_name));
                img_display.Source = originalImage;

                // Convert BitmapImage to Bitmap
                originalBitmap = BitmapImageToBitmap(originalImage);
                txtObjectCount.Text = "Objects Count: ";
            }
        }

        private Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(stream);
                Bitmap bitmap = new Bitmap(stream);
                return new Bitmap(bitmap);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void BtnLabelWithoutAForge_Click(object sender, RoutedEventArgs e)
        {
            if (img_display.Source == null)
            {
                MessageBox.Show("Por favor, carga una imagen primero.");
                return;
            }

            Bitmap bitmap;
            using (var stream = new System.IO.MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img_display.Source));
                encoder.Save(stream);
                bitmap = new Bitmap(stream);
            }

            Bitmap grayScaleBitmap = ConvertToGrayScale(bitmap);

            int threshold = 128;
            Bitmap binaryBitmap = ApplyThreshold(grayScaleBitmap, threshold);
            int objectCount = LabelObjects(binaryBitmap);

            img_display.Source = BitmapToImageSource(binaryBitmap);
            txtObjectCount.Text = "Número de Objetos: " + objectCount;
        }

        private Bitmap ApplyThreshold(Bitmap original, int threshold)
        {
            Bitmap grayScaleImage = ConvertToGrayScale(original);

            Bitmap result = new Bitmap(grayScaleImage.Width, grayScaleImage.Height);

            for (int y = 0; y < grayScaleImage.Height; y++)
            {
                for (int x = 0; x < grayScaleImage.Width; x++)
                {
                    Color color = grayScaleImage.GetPixel(x, y);
                    int grayValue = color.R;

                    if (grayValue < threshold)
                        result.SetPixel(x, y, Color.Black);
                    else
                        result.SetPixel(x, y, Color.White);
                }
            }

            return result;
        }

        private Bitmap ConvertToGrayScale(Bitmap original)
        {
            Bitmap grayScaleImage = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color color = original.GetPixel(x, y);
                    int grayValue = (int)(0.2989 * color.R + 0.5870 * color.G + 0.1140 * color.B);
                    grayScaleImage.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            return grayScaleImage;
        }

        private int LabelObjects(Bitmap binaryImage)
        {
            int label = 0;
            int[,] labels = new int[binaryImage.Width, binaryImage.Height];
            List<Rectangle> objectRectangles = new List<Rectangle>();

            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    if (binaryImage.GetPixel(x, y).ToArgb() == Color.Black.ToArgb() && labels[x, y] == 0)
                    {
                        label++;
                        Rectangle rect = FloodFill(binaryImage, labels, x, y, label);
                        objectRectangles.Add(rect);
                    }
                }
            }

            // Dibujar rectángulos
            using (Graphics g = Graphics.FromImage(binaryImage))
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    foreach (var rect in objectRectangles)
                    {
                        g.DrawRectangle(pen, rect);
                    }
                }
            }

            return label;
        }

        private Rectangle FloodFill(Bitmap image, int[,] labels, int x, int y, int label)
        {
            int minX = x, maxX = x, minY = y, maxY = y;

            Queue<System.Drawing.Point> queue = new Queue<System.Drawing.Point>();
            queue.Enqueue(new System.Drawing.Point(x, y));

            while (queue.Count > 0)
            {
                System.Drawing.Point pt = queue.Dequeue();
                int px = pt.X;
                int py = pt.Y;

                if (px < 0 || px >= image.Width || py < 0 || py >= image.Height)
                    continue;

                if (image.GetPixel(px, py).ToArgb() == Color.Black.ToArgb() && labels[px, py] == 0)
                {
                    labels[px, py] = label;

                    if (px < minX) minX = px;
                    if (px > maxX) maxX = px;
                    if (py < minY) minY = py;
                    if (py > maxY) maxY = py;

                    queue.Enqueue(new System.Drawing.Point(px - 1, py));
                    queue.Enqueue(new System.Drawing.Point(px + 1, py));
                    queue.Enqueue(new System.Drawing.Point(px, py - 1));
                    queue.Enqueue(new System.Drawing.Point(px, py + 1));
                }
            }

            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }


        private void BtnLabelWithAForge_Click(object sender, RoutedEventArgs e)
        {
            if (originalBitmap != null)
            {
                Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                Bitmap grayImage = filter.Apply(originalBitmap);
                Threshold thresholdFilter = new Threshold(128);
                thresholdFilter.ApplyInPlace(grayImage);
                GaussianBlur blurFilter = new GaussianBlur();
                blurFilter.ApplyInPlace(grayImage);
                SobelEdgeDetector sobelFilter = new SobelEdgeDetector();
                grayImage = sobelFilter.Apply(grayImage);
                // Detección de objetos
                BlobCounter blobCounter = new BlobCounter();
                blobCounter.ProcessImage(grayImage);
                Blob[] blobs = blobCounter.GetObjectsInformation();

                // Dibujar rectángulos 
                Graphics g = Graphics.FromImage(originalBitmap);
                Pen pen = new Pen(Color.Red, 2);

                foreach (Blob blob in blobs)
                {
                    Rectangle rect = blob.Rectangle;
                    g.DrawRectangle(pen, rect);
                }

                // Actualizar imagen procesada 
                processedImage = originalBitmap.Clone() as Bitmap;
                img_display.Source = BitmapToBitmapImage(processedImage);
                txtObjectCount.Text = $"Número de Objetos: {blobs.Length}";
            }
        }

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memory;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
