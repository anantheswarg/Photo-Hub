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
using System.Windows.Media.Imaging;
using System.IO;
using System.Globalization;
using System.Windows.Navigation;

namespace PhotoHub
{
    public partial class FacebookAuthenticate : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FacebookAuthenticate()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Web browser log in completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e != null)
            {
                this.WebBrowser.Opacity = 1;
                this.ProgressBar.Visibility = Visibility.Collapsed;

                if (this.WebBrowser.Source.AbsoluteUri.Contains("http://www.facebook.com/connect/login_success.html?error"))
                {
                    NavigationService.GoBack();
                    return;
                }

                // Get the navigated URL
                string strLoweredAddress = e.Uri.OriginalString.ToLower(CultureInfo.InvariantCulture);

                // Check the URL with login success string
                if (strLoweredAddress.StartsWith("http://www.facebook.com/connect/login_success.html?code=", StringComparison.OrdinalIgnoreCase))
                {
                    txtStatus.Text = Messages.FacebookRetrieveAccessTokenText;
                    WebBrowser.Navigate(FBUris.GetTokenLoadUri(e.Uri.OriginalString.Substring(56)));
                    return;
                }

                string strTest = WebBrowser.SaveToString();

                if (strTest.Contains("access_token"))
                {
                    // Get access key from the URL
                    int nPos = strTest.IndexOf("access_token", StringComparison.OrdinalIgnoreCase);

                    string strPart = strTest.Substring(nPos + 13);
                    nPos = strPart.IndexOf("</PRE>", StringComparison.OrdinalIgnoreCase);
                    strPart = strPart.Substring(0, nPos);

                    // Remove expires if found
                    nPos = strPart.IndexOf("&amp;expires=", StringComparison.OrdinalIgnoreCase);

                    if (nPos != -1)
                    {
                        strPart = strPart.Substring(0, nPos);
                    }

                    // Save the access token
                    App.AccessToken = strPart;

                    // Automatically leave the page after login success
                    txtStatus.Text = Messages.FacebookAddTagText;

                    this.WebBrowser.Visibility = Visibility.Collapsed;
                    this.PhotoTagPanel.Visibility = Visibility.Visible;

                    ApplicationBar.IsVisible = true;
                    this.PageTitle.Text = Messages.FacebookShare;

                    return;
                }
            }
        }

        /// <summary>
        /// Page loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Navigate to Facebook login page url
            WebBrowser.Navigate(FBUris.GetLoginUri());

            this.ImgProfile.Source = App.FacebookImage;
        }

        protected void ApplicationBarIconButtonUpload_Click(object sender, EventArgs e)
        {
            // Disable the application bar
            ApplicationBar.IsVisible = false;

            this.ProgressBar.Visibility = Visibility.Visible;
            this.ContentPanel.Opacity = 0.2;

            try
            {
                // Create a FBPhotoPost instance
                FBPhotoPost m_fbPost = new FBPhotoPost();

                // Event handler for uploading the photo to Facebook
                m_fbPost.PostStatus += new FBPhotoPost.PostStatusEventHandler(m_fbPost_PostStatus);

                // Generate a new unique boundary string
                m_fbPost.ResetBoundaryString();

                // Set the access token
                m_fbPost.AccessToken = App.AccessToken;

                // Set the Caption
                m_fbPost.PhotoCaption = String.IsNullOrEmpty(this.TxtPhotoTag.Text.Trim()) ? "Hi, Photo from PhotoHub - a Windows Phone 7 application" : this.TxtPhotoTag.Text.Trim();

                // Set the Photo to Post - in this case the bitmap created by the UIElement pnlImages
                //                m_fbPost.PhotoToPost = m_fbPost.GetBitmapImageFromUIElement(ImgProfile);
                m_fbPost.PhotoToPost = App.FacebookImage;

                // Post the data
                m_fbPost.PostPhoto();

            }

            catch (WebException eX)
            {
                // Display the result
                MessageBox.Show("Post to wall failed: " + eX.Message);

                // Enable the application bar
                ApplicationBar.IsVisible = true;
            }

        }

        /// <summary>
        /// Status of uploading photo to Facebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_fbPost_PostStatus(object sender, FBPhotoPost.PostStatusEventArgs e)
        {
            // Display the response
            //MessageBox.Show(e.MessageHeader + e.MessageContents);
            if (e.MessageHeader.Equals("Upload Success! Picture ID:"))
            {
                MessageBox.Show(Messages.FacebookUploadSuccessMessage, Messages.FacebookUploadSuccessTitle, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(e.MessageHeader + e.MessageContents);
            }

            this.ProgressBar.Visibility = Visibility.Collapsed;
            this.ContentPanel.Opacity = 1;

            // Navigate to the previous page
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// To display progress bar while navigating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void WebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            this.WebBrowser.Opacity = 0.5;
            this.ProgressBar.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// To hide the key board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TxtPhotoTag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                this.BtnNothing.Focus();
            }

        }
    }
}