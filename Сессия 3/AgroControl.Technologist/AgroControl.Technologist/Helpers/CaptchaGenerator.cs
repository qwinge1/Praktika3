using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AgroControl.Technologist.Helpers
{
    public static class CaptchaGenerator
    {
        private static readonly Random rand = new Random();

        public static BitmapImage GenerateImage(string text)
        {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, 200, 80));
                for (int i = 0; i < 5; i++)
                {
                    dc.DrawLine(new Pen(new SolidColorBrush(Color.FromRgb((byte)rand.Next(200, 256),
                        (byte)rand.Next(200, 256), (byte)rand.Next(200, 256))), 2),
                        new Point(rand.Next(200), rand.Next(80)), new Point(rand.Next(200), rand.Next(80)));
                }

                double x = 10;
                foreach (char c in text)
                {
                    var charText = new FormattedText(c.ToString(), System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, new Typeface(new FontFamily("Arial"), FontStyles.Italic, FontWeights.Bold, FontStretches.Normal),
                        28, Brushes.DarkBlue, 1.25);
                    dc.PushTransform(new RotateTransform(rand.Next(-20, 20), x + charText.Width / 2, 40));
                    dc.DrawText(charText, new Point(x, 20));
                    dc.Pop();
                    x += charText.Width * 0.9;
                }

                for (int i = 0; i < 100; i++)
                {
                    dc.DrawRectangle(new SolidColorBrush(Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))),
                        null, new Rect(rand.Next(200), rand.Next(80), 1, 1));
                }
            }

            var renderTarget = new RenderTargetBitmap(200, 80, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(visual);
            var bitmapImage = new BitmapImage();
            using (var stream = new System.IO.MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                encoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static string GenerateRandomText()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Range(0, 6).Select(_ => chars[rand.Next(chars.Length)]).ToArray());
        }
    }
}