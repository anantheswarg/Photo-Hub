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
    public class EffectPreview
    {

        /// <summary>
        /// Stores the source of the image for the button
        /// </summary>
        private WriteableBitmap imageSource;

        public WriteableBitmap ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; }
        }

    }
}
