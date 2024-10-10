using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
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
namespace FalsoColor
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        BitmapImage IMG1;
        BitmapImage IMG1Result;

        BitmapImage IMG2;
        BitmapImage IMG2Result;

        public MainWindow()
        {
            InitializeComponent();
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

        private void btnIMG1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                IMG1 = new BitmapImage(new Uri(filename));
                Displey1.Source = IMG1;
            }
        }

        private void btnIMG2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                IMG2 = new BitmapImage(new Uri(filename));
                Displey2.Source = IMG2;
            }

        }

            BitmapImage ToGrayScale(BitmapImage img)
            {

                Bitmap bitmapAux = BitmapImage2Bitmap(img);
                Bitmap bitmapResult = new Bitmap(bitmapAux.Width, bitmapAux.Height);
                System.Drawing.Color p;
                for (int f = 0; f < bitmapAux.Width; f++)
                {
                    for (int c = 0; c < bitmapAux.Height; c++)
                    {
                        p = bitmapAux.GetPixel(f, c);
                        double cGray = (p.R * 0.3) + (p.G * 0.59) + (p.B * 0.11);

                        if (cGray < 64)
                        {
                            float percentage = (float)cGray / 64.0f;
                            int red = (int)(255 * percentage);
                            int green = (int)(180 * percentage);
                            int blue = (int)(128 * percentage);
                            bitmapResult.SetPixel(f, c, System.Drawing.Color.FromArgb(255, red, green, blue));
                        }

                        else if (cGray < 128)
                        {
                            float percentage = (float)cGray / 128.0f;
                            int red = (int)(66 * percentage);
                            int green = (int)(214 * percentage);
                            int blue = (int)(164 * percentage);
                            bitmapResult.SetPixel(f, c, System.Drawing.Color.FromArgb(255, red, green, blue));
                        }

                        else if (cGray < 192)
                        {
                            float percentage = (float)cGray / 192.0f;
                            int red = (int)(89 * percentage);
                            int green = (int)(173 * percentage);
                            int blue = (int)(246 * percentage);
                            bitmapResult.SetPixel(f, c, System.Drawing.Color.FromArgb(255, red, green, blue));

                        }
                        else
                        {
                            float percentage = (float)cGray / 256.0f;
                            int red = (int)(199 * percentage);
                            int green = (int)(128 * percentage);
                            int blue = (int)(232 * percentage);
                            bitmapResult.SetPixel(f, c, System.Drawing.Color.FromArgb(255, red, green, blue));

                        }   
                    }
                }
                return ToBitmapImage(bitmapResult);
            }

        BitmapImage ToGrayScaleWithIntensity(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);
            Bitmap bitmapResult = new Bitmap(bitmapAux.Width, bitmapAux.Height);
            System.Drawing.Color p;
            for (int f = 0; f < bitmapAux.Width; f++)
            {
                for (int c = 0; c < bitmapAux.Height; c++)
                {
                    p = bitmapAux.GetPixel(f, c);
                    double cGray = (p.R * 0.3) + (p.G * 0.59) + (p.B * 0.11);

                    int red = 0;
                    int green = 0;
                    int blue = 0;

                    if (cGray < 64)
                    {
                        int intensity = (int)((cGray / 64) * 255);

                        red = 255 - intensity / 2;
                        green = 180 - intensity / 2;
                        blue = 128 - intensity / 2;
                    }

                    else if (cGray < 128)
                    {
                        int intensity = (int)(((cGray - 64) / 64) * 255);

                        red = 255 - intensity / 2;
                        green = 180 + intensity / 2;
                        blue = 128 - intensity / 2;
                    }
                    else if (cGray < 192)
                    {
                        int intensity = (int)(((cGray - 128) / 64) * 255);

                        red = 66 - intensity / 2;
                        green = 214 - intensity / 2;
                        blue = 164 + intensity / 2;
                    }
                    else
                    {
                        int intensity = (int)(((cGray - 192) / 64) * 255);

                        red = 89 + intensity / 2;
                        green = 173 - intensity / 2;
                        blue = 246 + intensity / 2;
                    }

                    red = Math.Max(0, Math.Min(255, red));
                    green = Math.Max(0, Math.Min(255, green));
                    blue = Math.Max(0, Math.Min(255, blue));

                    bitmapResult.SetPixel(f, c, System.Drawing.Color.FromArgb(red, green, blue));
                }
            }
            return ToBitmapImage(bitmapResult);
        }


        BitmapImage GrayScale(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);
            Bitmap bitmapResult = new Bitmap(bitmapAux.Width, bitmapAux.Height);
            System.Drawing.Color p;

            int grayColor;
            for (int f = 0; f < bitmapAux.Width; f++)
            {
                for (int c = 0; c < bitmapAux.Height; c++)
                {
                    p = bitmapAux.GetPixel(f, c);
                    grayColor = (int)(p.R * 0.3 + p.G * 0.59 + p.B * 0.11);
                    bitmapResult.SetPixel(f, c, System.Drawing.Color.FromArgb(grayColor, grayColor, grayColor));
                }
            }

            return ToBitmapImage(bitmapResult);
        }

        BitmapImage FakeColor(BitmapImage img)
        {
            Bitmap bitmapAux = BitmapImage2Bitmap(img);
            Bitmap bitmapResult = new Bitmap(bitmapAux.Width, bitmapAux.Height);
            System.Drawing.Color p;
            System.Drawing.Color highlightColor;

            for (int f = 0; f < bitmapAux.Width; f++)
            {
                for (int c = 0; c < bitmapAux.Height; c++)
                {
                    p = bitmapAux.GetPixel(f, c);
                    float r = p.R / 255.0f;
                    float g = p.G / 255.0f;
                    float b = p.B / 255.0f;

                    if (p.R > 5)
                    {
                        if (p.R <= 40)
                        {
                            // Cambio a Cian
                            highlightColor = System.Drawing.Color.FromArgb(0, 255, 255);
                        }
                        else if (p.R <= 80)
                        {
                            // Cambio a Magenta
                            highlightColor = System.Drawing.Color.FromArgb(255, 0, 255);
                        }
                        else if (p.R <= 120)
                        {
                            // Cambio a Amarillo Claro
                            highlightColor = System.Drawing.Color.FromArgb(255, 255, 153);
                        }
                        else if (p.R <= 160)
                        {
                            // Cambio a Turquesa
                            highlightColor = System.Drawing.Color.FromArgb(64, 224, 208);
                        }
                        else if (p.R <= 200)
                        {
                            // Cambio a Naranja Claro
                            highlightColor = System.Drawing.Color.FromArgb(255, 165, 0);
                        }
                        else
                        {
                            // Cambio a Violeta
                            highlightColor = System.Drawing.Color.FromArgb(238, 130, 238);
                        }

                        float opacity = 0.5f; // Puedes ajustar la opacidad aquí si deseas
                        r = (1 - opacity) * r + opacity * (highlightColor.R / 255.0f);
                        g = (1 - opacity) * g + opacity * (highlightColor.G / 255.0f);
                        b = (1 - opacity) * b + opacity * (highlightColor.B / 255.0f);

                        bitmapResult.SetPixel(f, c, System.Drawing.Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255)));
                    }
                    else
                    {
                        bitmapResult.SetPixel(f, c, p);
                    }
                }
            }

            return ToBitmapImage(bitmapResult);
        }

        private void btnIMG1falseC_Click(object sender, RoutedEventArgs e)
        {
            IMG1Result = FakeColor(IMG1);
            Displey1Result.Source = IMG1Result;
        }

        private void btnIMG2FalseC_Click(object sender, RoutedEventArgs e)
        {
            IMG2Result = ToGrayScaleWithIntensity(IMG2);
            Displey2Result.Source = IMG2Result;
        }
    }
}
