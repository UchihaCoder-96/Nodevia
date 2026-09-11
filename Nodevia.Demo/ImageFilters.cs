using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Nodevia.Demo
{
    internal class ImageFilters
    {
        public static Bitmap OpenImage(string path)
        {
            return new Bitmap(path);
        }

        public static unsafe Bitmap? Posterize(Bitmap image, float threshold)
        {
            if (image == null) return null;

            threshold = Math.Clamp(threshold, 0f, 255f);

            Bitmap result = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            BitmapData srcData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            BitmapData dstData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int thresholdInt = (int)threshold;

            byte* src = (byte*)srcData.Scan0;
            byte* dst = (byte*)dstData.Scan0;

            for (int y = 0; y < image.Height; y++)
            {
                byte* srcRow = src + y * srcData.Stride;
                byte* dstRow = dst + y * dstData.Stride;

                for (int x = 0; x < image.Width; x++)
                {
                    int i = x * 4;

                    byte b = srcRow[i + 0];
                    byte g = srcRow[i + 1];
                    byte r = srcRow[i + 2];
                    byte a = srcRow[i + 3];

                    dstRow[i + 0] = b >= thresholdInt ? (byte)255 : (byte)0;
                    dstRow[i + 1] = g >= thresholdInt ? (byte)255 : (byte)0;
                    dstRow[i + 2] = r >= thresholdInt ? (byte)255 : (byte)0;
                    dstRow[i + 3] = a;
                }
            }

            image.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }
    }
}
