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

namespace PhotoHub
{
    /// <summary>
    /// Template class for the toolbox buttons
    /// </summary>
    public class ToolBoxButton
    {
        /// <summary>
        /// Stores the source of the image for the button
        /// </summary>
        private string imageSource;

        public string ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; }
        }

        /// <summary>
        /// Stores the index of the effect associated with the button 
        /// </summary>

        private int tag;


        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }
    }
}
