using Common.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Common.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Base64 To ImageSource
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static ImageSource Base642ImageSource(this string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                return null;
            }
            byte[] binaryData = Convert.FromBase64String(base64);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            bi.Freeze();
            return bi;
        }


        /// <summary>
        /// 将图片保存为Base64
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        public static string Save2Base64(this BitmapSource imageSource)
        {
            using (MemoryStream ms = new MemoryStream())
            using (Bitmap bitmap = imageSource.ToBitmap())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return Save2Base64(ms);
            }
        }

        /// <summary>
        /// 将图片保存为Base64
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static string Save2Base64(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return Save2Base64(ms);
            }
        }

        /// <summary>
        /// 将流保存为Base64
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <returns></returns>
        public static string Save2Base64(this MemoryStream memoryStream)
        {
            return Convert.ToBase64String(memoryStream.ToArray());
        }



        public static Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            Bitmap bmp = new Bitmap(bitmapSource.PixelWidth,
                bitmapSource.PixelHeight, PixelFormat.Format32bppArgb);
            BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);

            return bmp;
        }


        /// <summary>
        /// bitmap convert to bitmapsource
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static ImageSource ToBitmapSource(this Bitmap bmp)
        {
            try
            {
                IntPtr hBitmap = bmp.GetHbitmap();
                ImageSource newSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                InteropMethods.DeleteObject(hBitmap);
                return newSource;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
