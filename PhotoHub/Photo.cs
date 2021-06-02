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
    public class Photo
    {
        private BitmapImage image = new BitmapImage();
        private String tag;

        public BitmapImage Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }
        public String Tag
        {
            get
            {
                return tag;
            }
            set
            {
                this.tag = value;
            }
        }
    }
}
