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
using System.Collections.ObjectModel;

namespace PhotoHub
{
    public static class Constants
    {

        public static readonly string DEFAULT_PICTURE_NAME = "photohub";
        public static readonly string CAMERA_ROLL_ALBUM_NAME = "Camera Roll";
        public static readonly string SAMPLE_PICTURES_ALBUM_NAME = "Sample Pictures";
        public static readonly string SAVED_PICTURES_ALBUM_NAME = "Saved Pictures";

        public static readonly double DEFAULT_IMAGE_DIMENSION = 450.0;

        public enum SPECIAL_EFFECTS
        {
            EFFECT_SEPIA,
            EFFECT_VIGNETTE,
            EFFECT_BLACKWHITE,
            EFFECT_TILTSHIFT,
            EFFECT_POLAROID,
            EFFECT_TINT
        };

        public static readonly string HELP_PAGE = "HelpContent.xml";
        public static readonly string SUPPORT_EMAIL = "Support@EdgeQ.com";

        public static readonly string SUPPORT_SUBJECT = "Need support for Photo Hub WP7";

        public static readonly string FEEDBACK_EMAIL = "Feedback@EdgeQ.com";

        public static readonly string FEEDBACK_SUBJECT = "Feedback on Photo Hub";

        public static readonly string EDGEQ_URL = "http://www.EdgeQ.com";

        public enum TOOLBOX_EFFECTS
        {
            EFFECT_MORE,
            EFFECT_PENCIL,
            //EFFECT_CROP,
            EFFECT_FLIP_HORIZONTAL,
            EFFECT_FLIP_VERTICAL,
            EFFECT_ROTATE_LEFT,
            EFFECT_ROTATE_RIGHT,
            EFFECT_BRIGHTNESS,
            EFFECT_CONTRAST,
            EFFECT_BLUR

        };


        ///// <summary>
        ///// List of tips
        ///// </summary>
        //public static readonly string[] TIPTITLES = {   "Fix it Easy!",
        //                                                "More effects!",
        //                                                "New Looks!",
        //                                                "Share with your friends!",
        //                                                "Share with your friends!",
        //                                                "More effects!",
        //                                                "Fix it Easy!"
        //                                            };
        ///// <summary>
        ///// List of tips
        ///// </summary>
        //public static readonly string[] TIPS = { "Does your photo needs a small change? Use basic editing options to quickly crop, flip, rotate, adjust brightness, contrast and more!",
        //                                         "Use multiple effects to bring sphistication to the most casual snapshot",
        //                                         "See your photos in whole new ways. Add colors and cool borders for striking results",                                                 
        //                                         "Quickly share your photos with your friends on Facebook and Flickr",
        //                                         "Quickly share your photos with your friends on Facebook and Flickr",
        //                                         "Use multiple effects to bring sphistication to the most casual snapshot",
        //                                         "Does your photo needs a small change? Use basic editing options to quickly crop, flip, rotate, adjust brightness, contrast and more!"
        //                                       };

        public static readonly double HALF_OPACITY = 0.4;

        public static readonly double FULL_OPACITY = 1;

        /// <summary>
        /// Maximum no of pictures to display from album
        /// </summary>
        public static readonly int MAXIMUM_PICTURES = 10;
    }
}
