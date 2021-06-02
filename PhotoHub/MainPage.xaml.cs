using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;

namespace PhotoHub
{

    /// <summary>
    /// Main page class, used  to capture/ choose the photo, Display device picture library
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        #region Declarations

        /// <summary>
        /// Photos of a selected album
        /// </summary>
        private ObservableCollection<Photo> photos;

        /// <summary>
        /// To fetch images from device picture library
        /// </summary>
        private BackgroundWorker backgroundWorker;

        /// <summary>
        /// Thread to get selected image
        /// </summary>
        private BackgroundWorker getSelectedImageBackgroundWorker;

        /// <summary>
        /// Selected Album name
        /// </summary>
        private string albumName;

        /// <summary>
        /// Gets the status of device connected to PC
        /// </summary>
        public bool IsConnectedToPC { get; set; }

        /// <summary>
        /// Photo tag - Album name Photo name and date
        /// </summary>
        public string photoTag { get; set; }

        /// <summary>
        /// To choose the picture from gallery
        /// </summary>
        private PhotoChooserTask photoChooserTask;

        /// <summary>
        /// To capture a picture
        /// </summary>
        private CameraCaptureTask cameraCapturetask;

        /// <summary>
        /// To get all the albums and pictures
        /// </summary>
        private MediaLibrary library;

        /// <summary>
        /// Device picture albums
        /// </summary>
        private PictureAlbumCollection pictureAlbums;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// To get the token, if the user is opening the app from Share picker/ Photo Extras
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.library = new MediaLibrary();
            MediaPlayer.Queue.ToString();

            // Gets a dictionary of query string keys and values
            IDictionary<string, string> queryStrings = this.NavigationContext.QueryString;

            // Check if the "token" key is present in the query string
            if (queryStrings.ContainsKey("token")) // This code is for Photo Extras feature
            {
                // This code retrieves the picture using the token passed to the application
                Picture picture = this.library.GetPictureFromToken(queryStrings["token"]);

                // Creates WriteableBitmap object and adds to the Image control Source property
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(picture.GetImage());
                App.CapturedImage = Utilities.ScaleImage(new WriteableBitmap(bitmap), Constants.DEFAULT_IMAGE_DIMENSION, Constants.DEFAULT_IMAGE_DIMENSION);

                // Navigate the user to photo effects page
                NavigationService.Navigate(new Uri("/PhotoEffects.xaml", UriKind.Relative));
            }
            else if (queryStrings.ContainsKey("FileId")) // Check if the "FileId" key is present in the query string. This code is for Share picker extensibility feature
            {
                // Retrieve the picture using the FileID passed to the application
                Picture picture = this.library.GetPictureFromToken(queryStrings["FileId"]);

                // Create a WriteableBitmap object and add it to the Image control Source property
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(picture.GetImage());
                App.CapturedImage = Utilities.ScaleImage(new WriteableBitmap(bitmap), Constants.DEFAULT_IMAGE_DIMENSION, Constants.DEFAULT_IMAGE_DIMENSION);

                // Navigate the user to photo effects page
                NavigationService.Navigate(new Uri("/PhotoEffects.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// To initialize required variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Object initialization
            this.photos = new ObservableCollection<Photo>();
            this.backgroundWorker = new BackgroundWorker();
            this.getSelectedImageBackgroundWorker = new BackgroundWorker();

            // Create new event handler for the photo capture using camera
            this.cameraCapturetask = new CameraCaptureTask();
            this.cameraCapturetask.Completed += new EventHandler<PhotoResult>(CameraCaptureTask_Completed);

            // Create new event handler  for the photo selected from gallery
            this.photoChooserTask = new PhotoChooserTask();
            this.photoChooserTask.Completed += new EventHandler<PhotoResult>(PhotoChooserTask_Completed);

            // Register BackroundWorker DoWork event handler
            this.backgroundWorker.DoWork += new DoWorkEventHandler(BackroundWorker_DoWork);

            // Register BackroundWorker RunWorkerCompleted event handler
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackroundWorker_RunWorkerCompleted);

            // Register BackroundWorker DoWork event handler
            this.getSelectedImageBackgroundWorker.DoWork += new DoWorkEventHandler(getSelectedImageBackgroundWorker_DoWork);

            // Register BackroundWorker RunWorkerCompleted event handler
            this.getSelectedImageBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(getSelectedImageBackgroundWorker_RunWorkerCompleted);

            // Get picture albums
            this.pictureAlbums = this.library.RootPictureAlbum.Albums;
        }

        #region Event Handlers

        /// <summary>
        /// Pivot item loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pivotItemEventArgs"></param>
        protected void PvtPictures_LoadedPivotItem(object sender, PivotItemEventArgs pivotItemEventArgs)
        {
            // If the selected items are not zero and not null
            if (pivotItemEventArgs != null && (!pivotItemEventArgs.Item.Equals(null)))
            {
                // Get the selected album name
                this.albumName = ((pivotItemEventArgs.Item) as PivotItem).Header.ToString();

                // Clear previous images in the observable collection
                this.photos.Clear();

                // Disable page controls
                this.ProgressBar.Visibility = Visibility.Visible;
                this.PvtPictures.IsEnabled = false;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                this.GrdContent.Opacity = Constants.HALF_OPACITY;
                this.BtnCamera.IsEnabled = false;
                this.BtnGallery.IsEnabled = false;

                // Starts background worker process to get the images from selected library
                this.backgroundWorker.RunWorkerAsync(albumName);
            }
        }

        /// <summary>
        /// Gets the selected image and navigate to Photo Effects page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PhotoChooserTask_Completed(object sender, PhotoResult photoResult)
        {
            try
            {
                // If the Photo selection/ capture result does not contain any error
                if (photoResult != null && photoResult.TaskResult == TaskResult.OK && photoResult.ChosenPhoto != null)
                {
                    // Show progress bar
                    this.ProgressBar.Visibility = Visibility.Visible;

                    // Update the layout
                    /* Reference: http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c6269a1c-7a21-439a-b2af-5c9216036471 */
                    EventHandler layoutUpdate = null;
                    layoutUpdate = (s, a) =>
                    {
                        LayoutUpdated -= layoutUpdate;
                        NavigationService.Navigate(new Uri("/PhotoEffects.xaml", UriKind.Relative));
                    };
                    LayoutUpdated += layoutUpdate;

                    // Take JPEG stream and decode into a WriteableBitmap object
                    App.CapturedImage = Utilities.ScaleImage(PictureDecoder.DecodeJpeg(photoResult.ChosenPhoto), Constants.DEFAULT_IMAGE_DIMENSION, Constants.DEFAULT_IMAGE_DIMENSION);

                }

                // Disable the progress bar
                this.GrdContent.Opacity = Constants.FULL_OPACITY;
                this.ProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (IOException exception)
            {
                Debug.WriteLine(exception.Message);
                MessageBox.Show(Messages.SomeErrorMessage, Messages.ErrorTitle, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// To save image to library and redirect the user to PhotoEffects page with selected image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="photoResult"></param>
        protected void CameraCaptureTask_Completed(object sender, PhotoResult photoResult)
        {
            try
            {
                // If the Photo selection/ capture result does not contain any error
                if (photoResult != null && photoResult.TaskResult == TaskResult.OK && photoResult.ChosenPhoto != null)
                {
                    EventHandler updatelayout = null;
                    updatelayout = (s, a) =>
                    {
                        LayoutUpdated -= updatelayout;
                        NavigationService.Navigate(new Uri("/PhotoEffects.xaml", UriKind.Relative));
                    };
                    LayoutUpdated += updatelayout;

                    // Show progress bar
                    this.ProgressBar.Visibility = Visibility.Visible;

                    WriteableBitmap bitmap = PictureDecoder.DecodeJpeg(photoResult.ChosenPhoto);

                    // Save captured image gallery
                    this.SaveImageToGallery(bitmap);

                    // Take JPEG stream and decode into a WriteableBitmap object
                    App.CapturedImage = Utilities.ScaleImage(bitmap, Constants.DEFAULT_IMAGE_DIMENSION, Constants.DEFAULT_IMAGE_DIMENSION);
                }

                // Disable the progress bar
                this.GrdContent.Opacity = Constants.FULL_OPACITY;
                this.ProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (IOException exception)
            {
                Debug.WriteLine(exception.Message);
                MessageBox.Show(Messages.SomeErrorMessage, Messages.ErrorTitle, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Photo click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Photo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null && e.AddedItems.Count > 0 && sender != null)
            {
                this.ProgressBar.Visibility = Visibility.Visible;
                this.GrdContent.Opacity = Constants.HALF_OPACITY;

                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

                this.photoTag = (e.AddedItems[0] as Photo).Tag;

                // Start the thread
                this.getSelectedImageBackgroundWorker.RunWorkerAsync();

                // Make selected index as -1
                (sender as ListBox).SelectedIndex = -1;

                // Navigate to photo effects page
                NavigationService.Navigate(new Uri("/PhotoEffects.xaml", UriKind.Relative));

            }
        }

        /// <summary>
        /// Camera button click event handler to open the device camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCamera_Click(object sender, RoutedEventArgs e)
        {
            // Show progress bar
            this.GrdContent.Opacity = Constants.HALF_OPACITY;
            this.ProgressBar.Visibility = Visibility.Visible;

            // Show device camera by opening camera capture task
            this.cameraCapturetask.Show();
        }

        /// <summary>
        /// Gallery button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnGallery_Click(object sender, RoutedEventArgs e)
        {
            // Show progress bar
            this.GrdContent.Opacity = Constants.HALF_OPACITY;
            this.ProgressBar.Visibility = Visibility.Visible;

            // Open gallery using Photo choose task
            this.photoChooserTask.Show();
        }

        /// <summary>
        /// Navigates to help page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplicationBarButtonHelp_Click(object sender, EventArgs e)
        {
            // Navigate to help page
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Displaying pictures in wrap panel from album
        /// </summary>
        /// <param name="selectedAlbumName">Album name</param>
        private void GetPictures(object selectedAlbumName)
        {
            // Loop through albums and get images for selected album
            foreach (PictureAlbum album in this.pictureAlbums)
            {
                if (album.Name.Equals(selectedAlbumName.ToString()))
                {
                    // If the album is not empty
                    if (album.Pictures.Count != 0)
                    {
                        // Loop through the images and display each image in the corresponding panel
                        PictureCollection pictures = album.Pictures;

                        foreach (Picture picture in pictures.OrderByDescending(i => i.Date))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.SetSource(picture.GetImage());

                            Photo photo = new Photo();
                            photo.Image = bitmap;
                            photo.Tag = picture.Album.Name + "?" + picture.Name + "?" + picture.Date;

                            this.photos.Add(photo);

                            if (this.photos.Count == Constants.MAXIMUM_PICTURES)
                                break;
                        }
                    }
                    break; // Once you are done with fetching images from required album, no need to check other albums
                }
            }
        }

        /// <summary>
        /// Saving image to device gallery
        /// </summary>
        /// <param name="bitmap"></param>
        private void SaveImageToGallery(WriteableBitmap bitmap)
        {
            string fileName = Guid.NewGuid().ToString();
            // Create filename for JPEG in isolated storage
            // Create virtual store and file stream. Check for duplicate tempJPEG files
            var userStore = IsolatedStorageFile.GetUserStoreForApplication();

            if (userStore.FileExists(fileName + ".jpg"))
            {
                userStore.DeleteFile(fileName + ".jpg");
            }

            IsolatedStorageFileStream fileStream = userStore.CreateFile(fileName + ".jpg");

            // Encode the WriteableBitmap into JPEG stream and place into isolated storage
            Extensions.SaveJpeg(bitmap, fileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 85);
            fileStream.Close();

            // Create a new file stream.
            fileStream = userStore.OpenFile(fileName + ".jpg", FileMode.Open, FileAccess.Read);

            //Add the JPEG file to the photos library on the device
            Picture pic = this.library.SavePicture(fileName + ".jpg", fileStream);
            fileStream.Close();
        }

        /// <summary>
        /// Get image from device gallery by tag (album name and photo name)
        /// </summary>
        /// <param name="tag"></param>
        private void GetSelectedImage(String tag)
        {
            // Get the photo Album name, photo name and photo date into an array
            String[] photoTag = tag.Split('?');

            // Split into Album name, image name, and saved date
            string selectedAlbumName = photoTag[0];
            string photoName = photoTag[1];
            string photoDate = photoTag[2].ToString();

            // For each album
            foreach (PictureAlbum album in this.library.RootPictureAlbum.Albums)
            {
                if (album.Name.Equals(selectedAlbumName))
                {
                    // For each image in album
                    foreach (Picture picture in album.Pictures)
                    {
                        // If the picture name and date matches, load the image stream
                        if (picture.Name.Equals(photoName) && picture.Date.ToString() == photoDate)
                        {
                            // Construct a writable bitmap image and save
                            Image selectedImage = new Image();
                            selectedImage.Height = picture.Height;
                            selectedImage.Width = picture.Width;

                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.SetSource(picture.GetImage());
                            selectedImage.Source = bitmapImage;

                            WriteableBitmap writableBitmap = new WriteableBitmap((int)selectedImage.Width, (int)selectedImage.Height);
                            writableBitmap.Render(selectedImage, null);
                            writableBitmap.Invalidate();

                            App.CapturedImage = writableBitmap;

                            return; // Once you got the image, no need to loop through further images
                        }
                    } // End of for each picture in album

                }
            } // End of for each album in picture library
        }

        #endregion

        #region Threading

        /// <summary>
        /// Get images from selected album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get pictures (Using dispatcher to access/ create UI elements)
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    // Get the pictures
                    this.GetPictures(this.albumName);
                }
                catch (InvalidOperationException exception)
                {
                    this.IsConnectedToPC = true;
                    Debug.WriteLine(exception.Message);
                    MessageBox.Show(Messages.DisconnectDeviceMessage, Messages.DisconnectDeviceMessageTitle, MessageBoxButton.OK);
                }
                catch (IOException exception)
                {
                    Debug.WriteLine(exception.Message);
                    MessageBox.Show(Messages.SomeErrorMessage, Messages.ErrorTitle, MessageBoxButton.OK);
                }
                catch (ArgumentNullException exception)
                {
                    Debug.WriteLine(exception.Message);
                    MessageBox.Show(Messages.SomeErrorMessage, Messages.ErrorTitle, MessageBoxButton.OK);
                }
            });
        }

        /// <summary>
        /// Executes when BackroundWorker_DoWork completed its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            #region Binding images to list

            // Bind images to list box
            if (this.albumName.Equals(Constants.CAMERA_ROLL_ALBUM_NAME))
            {
                if (this.photos.Count == 0) // If No photos exists in album
                {
                    // Show No Photos yet message
                    this.TxtCameraPhotosNothing.Visibility = Visibility.Visible;

                    if (this.IsConnectedToPC)
                    {
                        this.TxtCameraPhotosNothing.Text = Messages.DisconnectDeviceMessage;
                    }
                    else
                    {
                        this.TxtCameraPhotosNothing.Text = Messages.NothingHereCameraRoll;
                    }
                }
                else
                {
                    this.TxtCameraPhotosNothing.Visibility = Visibility.Collapsed;

                    // Bind images to list
                    this.LsbCameraPhotos.ItemsSource = this.photos;
                }

            }
            else if (this.albumName.Equals(Constants.SAMPLE_PICTURES_ALBUM_NAME))
            {
                if (this.photos.Count == 0) // If No photos exists in album
                {
                    // Show No Photos yet message
                    this.TxtFavoritesNothing.Visibility = Visibility.Visible;

                    if (this.IsConnectedToPC)
                    {
                        this.TxtFavoritesNothing.Text = Messages.DisconnectDeviceMessage;
                    }
                    else
                    {
                        this.TxtFavoritesNothing.Text = Messages.NothingHereSamplePictures;
                    }
                }
                else
                {
                    this.TxtFavoritesNothing.Visibility = Visibility.Collapsed;

                    // Bind images to list
                    this.LsbSamplePictures.ItemsSource = this.photos;
                }

            }
            else if (this.albumName.Equals(Constants.SAVED_PICTURES_ALBUM_NAME))
            {
                if (this.photos.Count == 0) // If No photos exists in album
                {
                    // Show No Photos yet message
                    this.TxtSavedPicturesNothing.Visibility = Visibility.Visible;

                    if (this.IsConnectedToPC)
                    {
                        this.TxtSavedPicturesNothing.Text = Messages.DisconnectDeviceMessage;
                    }
                    else
                    {
                        this.TxtSavedPicturesNothing.Text = Messages.NothingHereSavedPictures;
                    }
                }
                else
                {
                    this.TxtSavedPicturesNothing.Visibility = Visibility.Collapsed;

                    // Bind images to list
                    this.LsbSavedPicturesPhotos.ItemsSource = this.photos;
                }

            }

            #endregion

            // Enable all the controls
            Dispatcher.BeginInvoke(() =>
            {
                // Disable progress bar
                this.ProgressBar.Visibility = Visibility.Collapsed;

                // Enable buttons
                this.BtnCamera.IsEnabled = true;
                this.BtnGallery.IsEnabled = true;

                // Enable application bar buttons, pivot and content grid
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                this.PvtPictures.IsEnabled = true;
                this.GrdContent.Opacity = 1;

            });
        }

        /// <summary>
        /// Get images from selected album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void getSelectedImageBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                // Get the image based on tag name
                this.GetSelectedImage(this.photoTag);
            });
        }

        /// <summary>
        /// Executes when BackroundWorker_DoWork completed its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void getSelectedImageBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Enable all the controls
            Dispatcher.BeginInvoke(() =>
            {
                // Disable progress bar
                this.ProgressBar.Visibility = Visibility.Collapsed;

                // Enable buttons
                this.BtnCamera.IsEnabled = true;
                this.BtnGallery.IsEnabled = true;

                // Enable application bar buttons, pivot and content grid
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                this.PvtPictures.IsEnabled = true;
                this.GrdContent.Opacity = 1;

            });
        }
        #endregion
    }
}