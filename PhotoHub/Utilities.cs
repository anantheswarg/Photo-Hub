using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace PhotoHub
{
    public static class Utilities
    {
        /// <summary>
        /// Converts the point into a StylusPoint object
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static StylusPoint GetStylusPoint(Point point)
        {
            return new StylusPoint(point.X, point.Y);
        }

        /// <summary>
        /// Generates a color object from html string
        /// </summary>
        /// <param name="hexColor">html string without #</param>
        /// <returns>Color object with R,G,B values set</returns>
        public static Color HexToColor(string hexColor)
        {

            if (hexColor == null || hexColor.Length != 6)
            {
                return Colors.Black;
            }

            Color color = new Color();

            color.A = System.Convert.ToByte("FF", 16);

            color.R = System.Convert.ToByte(hexColor.Substring(0, 2), 16);
            color.G = System.Convert.ToByte(hexColor.Substring(2, 2), 16);
            color.B = System.Convert.ToByte(hexColor.Substring(4, 2), 16);

            return color;
        }

        /// <summary>
        /// Scales the image to the specified width and height maintaining the aspect ratio
        /// </summary>
        /// <param name="inputImage">image to be scaled </param>
        /// <param name="width">max width of the scaled image</param>
        /// <param name="height">max height of the scaled image</param>
        /// <returns>scaled image</returns>
        public static WriteableBitmap ScaleImage(WriteableBitmap inputImage, double width, double height)
        {
            if (inputImage == null)
            {
                return null;
            }

            //calculate the scaling factor
            double scaleX = (double)inputImage.PixelWidth / width;
            double scaleY = (double)inputImage.PixelHeight / height;

            double scale = scaleX > scaleY ? scaleX : scaleY;

            // calculate the final width and height using the scaling factor
            int finalHeight = (int)(inputImage.PixelHeight / scale);

            int finalWidth = (int)(inputImage.PixelWidth / scale);

            return WriteableBitmapExtensions.Resize(inputImage, finalWidth, finalHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
        }
    }
}
