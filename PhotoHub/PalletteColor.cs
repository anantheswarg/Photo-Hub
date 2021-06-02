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
    public class PalletteColor
    {
        /// <summary>
        /// Stores the pallette color
        /// </summary>

        private string color;

        public string Color
        {
            get { return color; }
            set { color = value; }
        }
    }
}
