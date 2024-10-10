using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


namespace WpfApp1
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapImage originalImage;
        BitmapImage originalImage2;
        BitmapImage resultImage;


        BitmapImage image_dividir;
        BitmapImage image_escalar;
        int dividir = 0;
        int escalar = 0;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                originalImage = new BitmapImage(new Uri(fileName));
                imgDisplay.Source = originalImage;
                
            }
        }

        public static BitmapImage ToBitmapImage(System.Drawing.Image bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage imgOrg)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(imgOrg));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           OpenFileDialog ofd = new OpenFileDialog();
           if (ofd.ShowDialog() == true)
           {
             string fileName = ofd.FileName;
             originalImage2 = new BitmapImage(new Uri(fileName));
             imgDisplay2.Source = originalImage2;
                
           }
            
        }

        private void sldMezcla_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private Bitmap MergeImages(Bitmap bitmap1, Bitmap bitmap2, double percentage)
        {
            Bitmap result = new Bitmap(bitmap1.Width, bitmap1.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bitmap1, new System.Drawing.Rectangle(0, 0, result.Width, result.Height));
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                    {
                       new float[] {1, 0, 0, 0, 0},
                       new float[] {0, 1, 0, 0, 0},
                       new float[] {0, 0, 1, 0, 0},
                       new float[] {0, 0, 0, (float)percentage, 0},
                       new float[] {0, 0, 0, 0, 1}
                    });
                    attributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(bitmap2, new System.Drawing.Rectangle(0, 0, result.Width, result.Height), 0, 0, bitmap2.Width, bitmap2.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return result;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (originalImage != null && originalImage2 != null)
            {
                double Value = sldMezcla.Value / 100.0;
                Bitmap bitmap1 = BitmapImage2Bitmap(originalImage);
                Bitmap bitmap2 = BitmapImage2Bitmap(originalImage2);
                Bitmap resultBitmap = MergeImages(bitmap1, bitmap2, Value);

                resultImage = ToBitmapImage(resultBitmap);

                imgDisplay3.Source = resultImage;
            }
        }

        BitmapImage ScaleImage(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);

            Bitmap bitmapResult = new Bitmap(bitmapAux.Width * 2, bitmapAux.Height * 2);

            for (int x = 0; x < bitmapAux.Width; x++)
            {
                for (int y = 0; y < bitmapAux.Height; y++)
                {
                    System.Drawing.Color color = bitmapAux.GetPixel(x, y);

                    bitmapResult.SetPixel(2 * x, 2 * y, color);
                    bitmapResult.SetPixel(2 * x + 1, 2 * y, color);
                    bitmapResult.SetPixel(2 * x, 2 * y + 1, color);
                    bitmapResult.SetPixel(2 * x + 1, 2 * y + 1, color);
                }
            }

            return ToBitmapImage(bitmapResult);
        }

        BitmapImage HalveImageSize(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);

            int newWidth = bitmapAux.Width / 2;
            int newHeight = bitmapAux.Height / 2;

            Bitmap bitmapResult = new Bitmap(newWidth, newHeight);

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    System.Drawing.Color color = bitmapAux.GetPixel(2 * x, 2 * y);

                    bitmapResult.SetPixel(x, y, color);
                }
            }
            return ToBitmapImage(bitmapResult);
        }

        BitmapImage InvertVertical(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);

            Bitmap bitmapResult = new Bitmap(bitmapAux.Width, bitmapAux.Height);

            for (int y = 0; y < bitmapAux.Height; y++)
            {
                for (int x = 0; x < bitmapAux.Width; x++)
                {
                    System.Drawing.Color color = bitmapAux.GetPixel(x, bitmapAux.Height - y - 1);

                    bitmapResult.SetPixel(x, y, color);
                }
            }
            return ToBitmapImage(bitmapResult);
        }

        BitmapImage InvertHorizontal(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);

            Bitmap bitmapResult = new Bitmap(bitmapAux.Width, bitmapAux.Height);

            for (int y = 0; y < bitmapAux.Height; y++)
            {
                for (int x = 0; x < bitmapAux.Width; x++)
                {
                    System.Drawing.Color color = bitmapAux.GetPixel(bitmapAux.Width - x - 1, y);

                    bitmapResult.SetPixel(x, y, color);
                }
            }

            return ToBitmapImage(bitmapResult);
        }



        private void btnDuplicarTamanio_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bitmapaux = BitmapImage2Bitmap((BitmapImage)(imgDisplay3.Source));
            Bitmap bitmapresult = new Bitmap(bitmapaux.Width * 2, bitmapaux.Height * 2);
            imgDisplay3.Width = imgDisplay3.Width * 2;
            imgDisplay3.Height = imgDisplay3.Height * 2;

            int cRed, cGreen, cBlue;
            for (int f = 0; f < bitmapaux.Width; f++)
            {
                for (int c = 0; c < bitmapaux.Height; c++)
                {
                    System.Drawing.Color p = bitmapaux.GetPixel(f, c);
                    cRed = p.R;
                    cGreen = p.G;
                    cBlue = p.B;
                    bitmapresult.SetPixel(f * 2, c * 2, System.Drawing.Color.FromArgb(cRed, cGreen, cBlue));
                    bitmapresult.SetPixel(f * 2 + 1, c * 2, System.Drawing.Color.FromArgb(cRed, cGreen, cBlue));
                    bitmapresult.SetPixel(f * 2, c * 2 + 1, System.Drawing.Color.FromArgb(cRed, cGreen, cBlue));
                    bitmapresult.SetPixel(f * 2 + 1, c * 2 + 1, System.Drawing.Color.FromArgb(cRed, cGreen, cBlue));
                }
            }

            resultImage = ToBitmapImage(bitmapresult);

            imgDisplay3.Source = resultImage;
        }


        private void btnDividir_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bitmapaux = BitmapImage2Bitmap((BitmapImage)(imgDisplay3.Source));
            Bitmap bitmapresult = new Bitmap(bitmapaux.Width / 2, bitmapaux.Height / 2);
            imgDisplay3.Width = imgDisplay3.Width / 2;
            imgDisplay3.Height = imgDisplay3.Height / 2;

            int cRed, cGreen, cBlue;
            for (int f = 0; f < bitmapresult.Width; f++)
            {
                for (int c = 0; c < bitmapresult.Height; c++)
                {
                    System.Drawing.Color p1 = bitmapaux.GetPixel(f * 2, c * 2);
                    System.Drawing.Color p2 = bitmapaux.GetPixel(f * 2 + 1, c * 2);
                    System.Drawing.Color p3 = bitmapaux.GetPixel(f * 2, c * 2 + 1);
                    System.Drawing.Color p4 = bitmapaux.GetPixel(f * 2 + 1, c * 2 + 1);
                    cRed = (p1.R + p2.R + p3.R + p4.R) / 4;
                    cGreen = (p1.G + p2.G + p3.G + p4.G) / 4;
                    cBlue = (p1.B + p2.B + p3.B + p4.B) / 4;
                    bitmapresult.SetPixel(f, c, System.Drawing.Color.FromArgb(cRed, cGreen, cBlue));
                }
            }

            resultImage = ToBitmapImage(bitmapresult);

            imgDisplay3.Source = resultImage;
        }


        bool isInvertedVertically = false;
        private void btnInvertirVertical_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                if (isInvertedVertically)
                {
                    imgDisplay3.Source = resultImage;
                    isInvertedVertically = false;
                }
                else
                {
                    BitmapImage invertedImage = InvertVertical(resultImage);
                    imgDisplay3.Source = invertedImage;
                    isInvertedVertically = true;
                }
            }
        }

        private void btnInvertirHorizontal_Click(object sender, RoutedEventArgs e)
        {

            if (originalImage != null)
            {
                BitmapImage invertedImage = InvertHorizontal(resultImage);
                imgDisplay3.Source = invertedImage;
                resultImage = invertedImage; 
            }
        }

        BitmapImage Dividir2(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);
            Bitmap bitmapResult = new Bitmap(bitmapAux.Width * dividir, bitmapAux.Height * dividir);
            System.Drawing.Color p;

            for (int f = 0; f < bitmapAux.Width; f++)
            {
                for (int c = 0; c < bitmapAux.Height; c++)
                {
                    p = bitmapAux.GetPixel(f, c);
                    bitmapResult.SetPixel(f, c, System.Drawing.Color.FromArgb(p.R, p.G, p.B));
                }
            }

            return ToBitmapImage(bitmapResult);
        }

        BitmapImage Escalar2(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);
            Bitmap bitmapResult = new Bitmap(bitmapAux.Width * escalar, bitmapAux.Height * escalar);
            System.Drawing.Color p;

            for (int f = 0; f < bitmapAux.Width; f++)
            {
                for (int c = 0; c < bitmapAux.Height; c++)
                {
                    p = bitmapAux.GetPixel(f, c);


                    bitmapResult.SetPixel(f * escalar, c * escalar, System.Drawing.Color.FromArgb(p.R, p.G, p.B));
                    bitmapResult.SetPixel(f * escalar + 1, c * escalar, System.Drawing.Color.FromArgb(p.R, p.G, p.B));
                    bitmapResult.SetPixel(f * escalar, c * escalar + 1, System.Drawing.Color.FromArgb(p.R, p.G, p.B));
                    bitmapResult.SetPixel(f * escalar + 1, c * escalar + 1, System.Drawing.Color.FromArgb(p.R, p.G, p.B));
                }
            }

            return ToBitmapImage(bitmapResult);
        }

        
    }


}
