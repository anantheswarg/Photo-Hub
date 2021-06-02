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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using FlickrNet;
using System.IO;
using System.Windows.Media.Imaging;
using PhotoHub;

namespace PhotoHub
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FlickrUpload : PhoneApplicationPage
    {
        /// <summary>
        /// 
        /// </summary>
        public FlickrUpload()
        {
            InitializeComponent();

            this.ImgToUpload.Source = App.FlickrImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.IsFromFlickrPhotoUploadPage = true;

            if (DataContext == null)
                DataContext = (Application.Current as App).UploadViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplicationBarUploadIconButton_Click(object sender, EventArgs e)
        {
            //ApplicationBarUploadIconButton.IsEnabled = false;

            App app = Application.Current as App;

            Flickr f = app.Flickr;
            UploadDataViewModel data = app.UploadViewModel;

            //Stream stream = ImageHelper.LoadImage(ImageHelper.ImageDirectory, SettingsHelper.ImageFilename);

            WriteableBitmap bmp = new WriteableBitmap((int)this.ImgToUpload.RenderSize.Width, (int)this.ImgToUpload.RenderSize.Height);
            bmp.Render(this.ImgToUpload, null);
            bmp.Invalidate();

            // Get an Image Stream
            MemoryStream stream = new MemoryStream();

            // write the image into the stream
            bmp.SaveJpeg(stream, (int)this.ImgToUpload.RenderSize.Width, (int)this.ImgToUpload.RenderSize.Height, 0, 100);

            // reset the stream pointer to the beginning
            stream.Seek(0, 0);

            if (stream == null)
            {
                MessageBox.Show("No image found. Please go back and choose an image to upload.");
                return;
            }

            ContentGrid.Visibility = Visibility.Collapsed;
            UploadProgressBar.Visibility = Visibility.Visible;

            f.UploadPictureAsync(stream, "image.jpg", data.Title, data.Description, data.Tags, true, true, true, ContentType.Photo, SafetyLevel.None, HiddenFromSearch.None, r =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    UploadProgressBar.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Upload complete!");

                    stream.Dispose();
                    if (NavigationService.CanGoBack) NavigationService.GoBack();
                });
            });

            stream.Dispose();
        }

        /// <summary>
        /// To move the cursor to description text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TxtTitle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                this.TxtDescription.Focus();
            }
        }

        /// <summary>
        /// To move the cursor to Tags text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TxtDescription_KeyUp(object sender, KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                this.TxtTags.Focus();
            }
        }

        /// <summary>
        /// To hide the keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TxtTags_KeyUp(object sender, KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                this.BtnNothing.Focus();
            }
        }
    }
}