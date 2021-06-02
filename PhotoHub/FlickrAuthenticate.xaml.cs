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
using PhotoHub;

namespace PhotoHub
{
    /// <summary>
    /// Authenticate user using Flickr API to upload the picture to his Flickr account
    /// </summary>
    public partial class FlickrAuthenticate : PhoneApplicationPage
    {
        /// <summary>
        /// Flickr Authentication Result
        /// </summary>
        string flickrAuthenticationResult;

        /// <summary>
        /// if this.flickrAuthenticationResult has been authenticated
        /// </summary>
        bool IsAuthenticateResultChecked = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public FlickrAuthenticate()
        {
            InitializeComponent();

            if (App.IsFromEffectsPage)
            {
                App.IsFromEffectsPage = false;
            }
        }

        /// <summary>
        /// Page loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(SettingsHelper.AuthenticationToken) && NavigationContext.QueryString.Count == 0)
            {
                // Going to skip this page, so don't load the web browser.
                this.ProgressBar.Visibility = Visibility.Collapsed;
                this.WebBrowser.Opacity = 1;

                return;
            }

            App app = Application.Current as App;
            app.Flickr.AuthGetFrobAsync(result =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (result.HasError)
                    {
                        this.TxtStatus.Text = result.Error.Message;
                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.GoBack();
                        }
                    }
                    else
                    {
                        this.flickrAuthenticationResult = result.Result;
                        this.WebBrowser.Navigate(new Uri(app.Flickr.AuthCalcUrlAsync(this.flickrAuthenticationResult, FlickrNet.AuthLevel.Write)));
                    }
                });
            });
        }

        /// <summary>
        /// On Navigated to event handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.WebBrowser.Opacity = 0.5;
            this.ProgressBar.Visibility = Visibility.Visible;

            // If auth token is not empty
            if (!String.IsNullOrEmpty(SettingsHelper.AuthenticationToken) && NavigationContext.QueryString.Count == 0)
            {
                if (NavigationService.CanGoBack)
                {
                    if (App.IsFromFlickrPhotoUploadPage) // If photo is uploaded, go back
                    {
                        NavigationService.GoBack();
                        App.FlickrImage = null;
                        App.IsFromFlickrPhotoUploadPage = false; // Change photo uploaded Boolean to false
                    }
                    else
                    {
                            this.VerifyAuthentication();
                    }
                }
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Application bar button continue click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ApplicationBarContinueIconButton_Click(object sender, EventArgs e)
        {
            this.VerifyAuthentication();
        }

        private void VerifyAuthentication()
        {
            // If already this.checking the return
            if (this.IsAuthenticateResultChecked) return;

            // If not authenticate then check to see if frob has been approved.
            if (String.IsNullOrEmpty(SettingsHelper.AuthenticationToken))
            {
                this.IsAuthenticateResultChecked = true;
                App app = Application.Current as App;

                app.Flickr.AuthGetTokenAsync(this.flickrAuthenticationResult, result =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (result.HasError)
                        {
                            if (result.ErrorCode == 1)
                            {
                                MessageBox.Show("You must authenticate before you can upload. Please completed the web page authentication above.");
                            }
                            else
                            {
                                MessageBox.Show(result.Error.Message);
                            }
                        }
                        else
                        {
                            SettingsHelper.AuthenticationToken = result.Result.Token;

                            NavigationService.Navigate(new Uri("/FlickrUpload.xaml", UriKind.Relative));
                        }

                        this.IsAuthenticateResultChecked = false;

                    });
                });

                return;
            }

            NavigationService.Navigate(new Uri("/FlickrUpload.xaml", UriKind.Relative));
        }

        protected void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            this.WebBrowser.Opacity = 1;
            this.ProgressBar.Visibility = Visibility.Collapsed;

            if(this.WebBrowser.Source.AbsoluteUri.Contains("http://www.flickr.com/services/auth/?api_key="))
            {
                this.TxtStatus.Text = Messages.FlickrAuthorizeText;
            }
            if (WebBrowser.Source.AbsoluteUri.Equals("http://www.flickr.com/services/auth/"))
            {
                this.VerifyAuthentication();
            }
        }

        protected void WebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            this.WebBrowser.Opacity = 0.5;
            this.ProgressBar.Visibility = Visibility.Visible;

        }
    }
}