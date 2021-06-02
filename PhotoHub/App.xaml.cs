using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using FlickrNet;

namespace PhotoHub
{
    public partial class App : Application
    {
        /// <summary>
        /// Facebook AccessToken
        /// </summary>
        private static string strAccessToken;

        public static string AccessToken
        {
            get { return strAccessToken; }
            set { strAccessToken = value; }
        }

        private static BitmapImage facebookImage;
        public static BitmapImage FacebookImage
        {
            get { return facebookImage; }
            set { facebookImage = value;}
        }

        private static BitmapImage flickrImage;
        public static BitmapImage FlickrImage
        {
            get { return flickrImage; }
            set { flickrImage = value; }
        }

        //public static BitmapImage FlickrImage = new BitmapImage();

        public Flickr Flickr { get; set; }

        public static bool IsFromFlickrPhotoUploadPage { get; set; }

        public static bool IsFromEffectsPage { get; set; }

        public UploadDataViewModel UploadViewModel { get; set; }


        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        //Global variables for the WriteableBitmap objects used throughout the application.
        private static WriteableBitmap capturedImage;
        public static WriteableBitmap CapturedImage
        {
            get { return capturedImage; }
            set { capturedImage = value; }
        }

        private static WriteableBitmap currentImage;
        public static WriteableBitmap CurrentImage
        {
            get { return currentImage; }
            set { currentImage = value; }
        }

        private static WriteableBitmap croppedImage;
        public static WriteableBitmap CroppedImage
        {
            get { return croppedImage; }
            set { croppedImage = value; }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;


            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            Flickr = new Flickr("2ac97679b03fb70d726127e7cb558c33", "dd30d5839d0c5e97", SettingsHelper.AuthenticationToken);
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        protected void Application_Launching(object sender, LaunchingEventArgs e)
        {
            UploadViewModel = new UploadDataViewModel();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        protected void Application_Activated(object sender, ActivatedEventArgs e)
        {
            UploadViewModel = SettingsHelper.UploadDataViewModel;
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        protected void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            SettingsHelper.UploadDataViewModel = UploadViewModel;
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        protected void Application_Closing(object sender, ClosingEventArgs e)
        {
            
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        
    }
}