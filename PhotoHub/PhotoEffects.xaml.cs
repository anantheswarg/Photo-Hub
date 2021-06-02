using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;

namespace PhotoHub
{
    /// <summary>
    /// Class that holds the functionality to morph, save and upload the image
    /// </summary>
    public partial class PhotoEffects : PhoneApplicationPage
    {

        #region Initialization
        /// <summary>
        /// Stores the current stroke for the pencil effect
        /// </summary>
        private Stroke strokeCurrent;

        /// <summary>
        /// Stores the current flip mode in x-direction. Varies from -1 to 1 so as to alternate
        /// </summary>
        private int flipModeX = 1;

        /// <summary>
        /// Stores the current flip mode in y-direction. Varies from -1 to 1 so as to alternate
        /// </summary>
        private int flipModeY = 1;

        /// <summary>
        /// Stores the rotation angle. Can be one of 0, 90, 180, 270, 360.
        /// </summary>
        private double initialAngle = 0d;

        /// <summary>
        /// width of the image while its loaded. This is used to find the portion of image in the container grid
        /// </summary>
        private double imageWidth;

        /// <summary>
        /// height of the image while its loaded. This is used to find the portion of image in the container grid
        /// </summary>
        private double imageHeight;

        /// <summary>
        /// Stores the initial coordinates for the crop
        /// </summary>
        //private Point position1;

        /// <summary>
        /// Stores the final coordinates for the crop
        /// </summary>
        //private Point position2;

        /// <summary>
        /// Set when the pencil mode is turned on 
        /// </summary>
        private bool isPencilModeOn = false;

        /// <summary>
        /// Stores the preview of images at any point of execution
        /// </summary>
        private ObservableCollection<EffectPreview> previewImages = new ObservableCollection<EffectPreview>();

        /// <summary>
        /// Stores the palette colors for pencil features
        /// </summary>
        private ObservableCollection<PalletteColor> palletteColors = new ObservableCollection<PalletteColor>();

        /// <summary>
        /// Stores the images for differing brush sizes
        /// </summary>
        private ObservableCollection<BrushSizeButton> brushSizes = new ObservableCollection<BrushSizeButton>();

        /// <summary>
        /// List of images for tool box
        /// </summary>
        private ObservableCollection<ToolBoxButton> toolBoxImages = new ObservableCollection<ToolBoxButton>();


        /// <summary>
        /// List of Uris for toolbox images
        /// </summary>
        /// 
        private string[] TOOLBOX_IMAGE_URIS = {     "/Static/Icons/greyscale.png",
                                                    "/Static/Icons/pencil.png",
                                                    //"/Static/Icons/crop.png",
                                                    "/Static/Icons/fliphorizontal.png",
                                                    "/Static/Icons/flipvertical.png",
                                                    "/Static/Icons/rotateleft.png",
                                                    "/Static/Icons/rotateright.png",
                                                    "/Static/Icons/brightness.png",
                                                    "/Static/Icons/contrast.png",
                                                    "/Static/Icons/blur.png"                                                               
                                              };
        /// <summary>
        /// 
        /// </summary>
        private string[] BRUSH_SIZE_IMAGE_URIS = {    "/Static/Icons/brush1.png",
                                                            "/Static/Icons/brush2.png",
                                                            "/Static/Icons/brush3.png",
                                                            "/Static/Icons/brush4.png",
                                                            "/Static/Icons/brush5.png"
                                                        };


        private float[] BRIGHTNESS_FACTORS = {    -0.6f,
                                                        -0.4f,
                                                        -0.2f,
                                                        0.2f,
                                                        0.4f,
                                                        0.6f
                                                    };

        private float[] CONTRAST_FACTORS = {
                                                        -0.6f,
                                                        -0.4f,
                                                        -0.2f,
                                                        0.2f,
                                                        0.4f,
                                                        0.6f
                                                  };
        private float[] BLUR_SIGMAS = {
                                                0.5f,
                                                1f,
                                                1.5f,
                                                2.0f
                                            };


        private float[] BRUSH_SIZES = {
                                                        2.0f,
                                                        6.0f,
                                                        10.0f,
                                                        14.0f,
                                                        18.0f
                                                    };

        private string[] BRUSH_COLORS = {   
                                                    "B041FF",
                                                    "008080",
                                                    "FFFF00",
                                                    "FF0000",
                                                    "C0C0C0",
                                                    "3BB9FF",
                                                    "F9966B",
                                                    "E238EC",
                                                    "893BFF",
                                                    "52D017",
                                                    "F87217",
                                                    "FFF8C6",
                                                    "CCFB5D",
                                                    "250517",
                                                    "ADDFFF",
                                                    "FDD017",
                                                    "FFFFFF",
                                                    "98AFC7",
                                                    "7E2217",
                                                    "FAAFBE",
                                                    "000000",
                                                    "8D38C9",
                                                    "4AA02C",
                                                    "827839",
                                                    "1589FF"
                                                          
                                            };



        /// <summary>
        /// Temporary image used for storing the bitmap while brightness, contrast or blur effect is in progress 
        /// </summary>
        private WriteableBitmap temporaryImage;

        /// <summary>
        /// Stores the current state of the image at any point of execution 
        /// </summary>
        private WriteableBitmap currentImage;

        /// <summary>
        /// Temporary image used for storing the preview images 
        /// </summary>
        private WriteableBitmap previewImage;

        /// <summary>
        /// Stores the current effect that is in action
        /// </summary>
        private int currentEffect;

        private double currentStylusWidth = 5;

        /// <summary>
        /// set when any rotation takes place. Used to set the correct flip horizontal or vertical. On every rotation this value is set to negative of it 
        /// </summary>
        private bool isRotated = false;

        /// <summary>
        /// Stores the current stroke color for the pencil effect
        /// </summary>
        Color currentStrokeColor = new Color();

        /// <summary>
        /// Set when the crop functionality is in progress
        /// </summary>
        //private bool isCroppingModeOn;

        /// <summary>
        /// Background thread to apply an effects on the picture 
        /// </summary>
        private BackgroundWorker workerProcessSingleImage;

        /// <summary>
        /// Background thread to apply an effects on the picture 
        /// </summary>
        private BackgroundWorker workerProcessImageEfects;

        /// <summary>
        /// File name for saving the image
        /// </summary>
        private string fileName;

        /// <summary>
        /// Default constructor for the class
        /// </summary>
        public PhotoEffects()
        {
            InitializeComponent();

            // setting the default brush color
            currentStrokeColor = Colors.Black;

            this.fileName = (new Guid()).ToString();

            this.currentImage = Utilities.ScaleImage(App.CapturedImage, (int)Constants.DEFAULT_IMAGE_DIMENSION, (int)Constants.DEFAULT_IMAGE_DIMENSION);

            this.ApplicationBar = (ApplicationBar)this.Resources["DefaultAppBar"];

            // set the input image to the captured/ picked up image from the last page 
            this.ImgInput.Source = currentImage;

            this.imageWidth = this.currentImage.PixelWidth;
            this.imageHeight = this.currentImage.PixelHeight;

            // fills the toolbox with images of effects
            this.PopulateToolBox();

            this.workerProcessSingleImage = new BackgroundWorker();
            this.workerProcessImageEfects = new BackgroundWorker();

            // Register BackroundWorker DoWork event handler
            this.workerProcessSingleImage.DoWork += new DoWorkEventHandler(WorkerProcessSingleImage_DoWork);
            this.workerProcessImageEfects.DoWork += new DoWorkEventHandler(WorkerProcessImageEfects_DoWork);

            // Register BackroundWorker RunWorkerCompleted event handler
            this.workerProcessSingleImage.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerProcessSingleImage_RunWorkerCompleted);
            this.workerProcessImageEfects.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerProcessImageEfects_RunWorkerCompleted);

            // Used for rendering the cropping rectangle on the image.
            //CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        #endregion

        #region Background Thread functionality

        /// <summary>
        /// Proceses the image being displayed in the grid and applies an effect based on the selected effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkerProcessSingleImage_DoWork(object sender, DoWorkEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                this.ShowProgressBar();

                this.LsbPreview.IsEnabled = false;

                switch (this.currentEffect)
                {

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BRIGHTNESS:

                        BrightnessContrastEffect brightnessEffect = new BrightnessContrastEffect();

                        brightnessEffect.BrightnessFactor = this.BRIGHTNESS_FACTORS[LsbPreview.SelectedIndex];

                        var resultPixels = brightnessEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                        this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                        break;

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_CONTRAST:

                        BrightnessContrastEffect contrastEffect = new BrightnessContrastEffect();

                        contrastEffect.ContrastFactor = this.BRIGHTNESS_FACTORS[LsbPreview.SelectedIndex];

                        resultPixels = contrastEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                        this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                        break;

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BLUR:

                        GaussianBlurEffect blurEffect = new GaussianBlurEffect();

                        blurEffect.Sigma = this.BLUR_SIGMAS[LsbPreview.SelectedIndex] + 1;

                        resultPixels = blurEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                        this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                        break;

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_MORE:

                        this.ApplySpecialEffect();

                        break;

                    default:
                        break;
                }

            });
        }

        /// <summary>
        /// Get images from selected album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WorkerProcessImageEfects_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.ShowProgressBar();

                EffectPreview preview = new EffectPreview();

                switch (this.currentEffect)
                {
                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_MORE:

                        this.GenerateSpecialEffectPreviews();

                        break;

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BRIGHTNESS:

                        previewImage = Utilities.ScaleImage(currentImage, 80, 80);

                        this.previewImages.Clear();

                        foreach (float factor in this.BRIGHTNESS_FACTORS)
                        {
                            BrightnessContrastEffect brightnessEffect = new BrightnessContrastEffect();

                            brightnessEffect.BrightnessFactor = factor;

                            preview = new EffectPreview();

                            var resultPixels = brightnessEffect.Process(previewImage.Pixels, previewImage.PixelWidth, previewImage.PixelHeight);

                            preview.ImageSource = resultPixels.ToWritableBitmap(previewImage.PixelWidth, previewImage.PixelHeight);

                            previewImages.Add(preview);
                        }

                        break;

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_CONTRAST:

                        previewImage = Utilities.ScaleImage(currentImage, 80, 80);

                        this.previewImages.Clear();

                        foreach (float factor in this.CONTRAST_FACTORS)
                        {

                            BrightnessContrastEffect contrastEffect = new BrightnessContrastEffect();

                            contrastEffect.ContrastFactor = factor;

                            preview = new EffectPreview();

                            var resultPixels = contrastEffect.Process(previewImage.Pixels, previewImage.PixelWidth, previewImage.PixelHeight);

                            preview.ImageSource = resultPixels.ToWritableBitmap(previewImage.PixelWidth, previewImage.PixelHeight);

                            previewImages.Add(preview);
                        }

                        break;

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BLUR:

                        previewImage = Utilities.ScaleImage(currentImage, 80, 80);


                        this.previewImages.Clear();

                        foreach (float factor in this.BLUR_SIGMAS)
                        {

                            GaussianBlurEffect blurEffect = new GaussianBlurEffect();

                            blurEffect.Sigma = factor + 1;

                            preview = new EffectPreview();

                            var resultPixels = blurEffect.Process(previewImage.Pixels, previewImage.PixelWidth, previewImage.PixelHeight);

                            preview.ImageSource = resultPixels.ToWritableBitmap(previewImage.PixelWidth, previewImage.PixelHeight);

                            previewImages.Add(preview);
                        }

                        break;



                    default:
                        break;
                }
            });
        }

        /// <summary>
        /// Executes when BackroundWorker_DoWork completed its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WorkerProcessSingleImage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Enable all the controls
            Dispatcher.BeginInvoke(() =>
            {

                switch (this.currentEffect)
                {
                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_MORE:

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BRIGHTNESS:
                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_CONTRAST:

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BLUR:

                        this.ImgInput.Source = this.temporaryImage;
                        break;
                    default:

                        this.ImgInput.Source = this.currentImage;
                        break;
                }

                this.LsbPreview.IsEnabled = true;

                this.HideProgressBar();

            });
        }

        /// <summary>
        /// Executes when BackroundWorker_DoWork completed its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WorkerProcessImageEfects_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Enable all the controls
            Dispatcher.BeginInvoke(() =>
            {
                switch (this.currentEffect)
                {
                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_MORE:

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BRIGHTNESS:

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_CONTRAST:

                    case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BLUR:

                        this.LsbPreview.ItemsSource = this.previewImages;

                        this.LsbPreview.Visibility = Visibility.Visible;
                        this.LsbPreview.IsEnabled = true;
                        this.BorderPreview.Visibility = Visibility.Visible;


                        break;

                    default:
                        break;
                }

                this.ImgInput.Source = this.currentImage;

                this.HideProgressBar();

            });
        }

        #endregion

        #region Toolbox logic

        /// <summary>
        /// Event handler for click on any of the tool box item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LsbToolBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Parameter checking
            if (e.Equals(null) || e.AddedItems.Count == 0 || this.LsbToolBox.SelectedIndex == -1)
            {
                return;
            }


            // Check for the pencil mode and set  
            if (this.isPencilModeOn)
            {
                this.currentStrokeColor = Utilities.HexToColor(this.BRUSH_COLORS[this.LsbToolBox.SelectedIndex]);
                return;
            }

            // based on the tag the type of effect selected is identified
            switch ((e.AddedItems[0] as ToolBoxButton).Tag)
            {
                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_PENCIL:

                    this.TurnPencilModeOn();

                    break;

                //case (int)Constants.TOOLBOX_EFFECTS.EFFECT_CROP:

                //    this.isCropEnabled = true;

                //    this.InkSignature.Visibility = Visibility.Collapsed;

                //    this.currentEffect = (int)Constants.TOOLBOX_EFFECTS.EFFECT_CROP;

                //    this.ChangeAppBarforEffects();

                //    break;

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_FLIP_HORIZONTAL:

                    if (!this.isRotated)
                    {
                        this.FlipHorizontal();
                    }
                    else
                    {
                        this.FlipVertical();
                    }

                    break;

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_FLIP_VERTICAL:

                    if (!this.isRotated)
                    {
                        this.FlipVertical();
                    }
                    else
                    {
                        this.FlipHorizontal();
                    }

                    break;

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_ROTATE_LEFT:

                    this.InitialRotationFrame.Value = initialAngle;
                    this.FinalRotationFrame.Value = initialAngle - 90d;

                    initialAngle -= 90d;

                    this.Rotate.Begin();

                    Point point = new Point(-1, -1);

                    this.GrdImageMorph.RenderTransformOrigin = point;

                    isRotated = !isRotated;

                    break;

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_ROTATE_RIGHT:

                    this.InitialRotationFrame.Value = initialAngle;
                    this.FinalRotationFrame.Value = initialAngle + 90d;

                    initialAngle += 90d;

                    this.Rotate.Begin();

                    Point point1 = new Point(-1, -1);

                    this.GrdImageMorph.RenderTransformOrigin = point1;

                    isRotated = !isRotated;

                    break;

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BRIGHTNESS:

                    this.ShowProgressBar();

                    this.ChangeAppBarforEffects();

                    // first load the current image into the temporary image for processing purpose

                    temporaryImage = currentImage;

                    this.currentEffect = (int)Constants.TOOLBOX_EFFECTS.EFFECT_BRIGHTNESS;

                    this.workerProcessImageEfects.RunWorkerAsync();

                    break;

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_CONTRAST:

                    this.ShowProgressBar();

                    this.ChangeAppBarforEffects();

                    temporaryImage = currentImage;

                    this.currentEffect = (int)Constants.TOOLBOX_EFFECTS.EFFECT_CONTRAST;

                    this.workerProcessImageEfects.RunWorkerAsync();

                    break;

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BLUR:

                    this.ShowProgressBar();

                    this.ChangeAppBarforEffects();

                    temporaryImage = currentImage;

                    this.currentEffect = (int)Constants.TOOLBOX_EFFECTS.EFFECT_BLUR;

                    // Starts background worker to apply the effect
                    this.workerProcessImageEfects.RunWorkerAsync();

                    break;

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_MORE:

                    this.ShowProgressBar();

                    this.ChangeAppBarforEffects();

                    temporaryImage = currentImage;

                    this.currentEffect = (int)Constants.TOOLBOX_EFFECTS.EFFECT_MORE;

                    // Starts background worker to apply the effect
                    this.workerProcessImageEfects.RunWorkerAsync();

                    break;


                default:
                    break;
            }

        }


        /// <summary>
        /// Event handler for click of any of the preview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LsbPreview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // parameter checking
            if (e.Equals(null) || e.AddedItems.Count == 0 || this.LsbPreview.SelectedIndex == -1)
            {
                return;
            }

            // check for pencil mode and set the current stylus width accordingly
            if (this.isPencilModeOn)
            {
                this.currentStylusWidth = this.BRUSH_SIZES[this.LsbPreview.SelectedIndex];
                return;
            }

            // based on the tag the type of effect selected is identified
            switch (this.currentEffect)
            {
                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_MORE:

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BRIGHTNESS:

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_CONTRAST:

                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_BLUR:

                    this.ProgressBar.Visibility = Visibility.Visible;

                    this.ImgInput.Opacity = 0.4;

                    this.workerProcessSingleImage.RunWorkerAsync();

                    break;

                default:
                    break;
            }

        }

        #endregion

        #region stroke logic

        

        /// <summary>
        /// Event handler for mouse move on the ink presenter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkSignature_MouseMove(object sender, MouseEventArgs e)
        {
            if (strokeCurrent != null)
            {
                Point mousePosition = e.GetPosition(this.GrdImageMorph);

                // check to see if the image is rotated and accordingly check if the mouse position falls in the image region
                if (!isRotated)
                {
                    double actualStartX = (Constants.DEFAULT_IMAGE_DIMENSION - (double)this.imageWidth) / (double)2.0;
                    double actualStartY = (Constants.DEFAULT_IMAGE_DIMENSION - (double)this.imageHeight) / (double)2.0;

                    if ((mousePosition.X >= actualStartX && mousePosition.X <= Constants.DEFAULT_IMAGE_DIMENSION - actualStartX - 10) && (mousePosition.Y >= actualStartY && mousePosition.Y <= Constants.DEFAULT_IMAGE_DIMENSION - actualStartY - 10))
                    {
                        strokeCurrent.StylusPoints.Add(Utilities.GetStylusPoint(e.GetPosition(this.InkSignature)));
                    }

                }
                else
                {
                    double actualStartX = (Constants.DEFAULT_IMAGE_DIMENSION - (double)this.imageWidth) / (double)2;
                    double actualStartY = (Constants.DEFAULT_IMAGE_DIMENSION - (double)this.imageHeight) / (double)2;

                    if ((mousePosition.X >= actualStartY && mousePosition.X <= Constants.DEFAULT_IMAGE_DIMENSION - actualStartY - 10) && (mousePosition.Y >= actualStartX && mousePosition.Y <= Constants.DEFAULT_IMAGE_DIMENSION - actualStartX - 10))
                    {
                        strokeCurrent.StylusPoints.Add(Utilities.GetStylusPoint(e.GetPosition(this.InkSignature)));
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for button click on ink presenter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkSignature_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InkSignature.CaptureMouse();

            strokeCurrent = new Stroke();

            strokeCurrent.DrawingAttributes.Color = currentStrokeColor;
            strokeCurrent.DrawingAttributes.Width = currentStylusWidth;
            strokeCurrent.DrawingAttributes.Height = currentStylusWidth;

            Point mousePosition = e.GetPosition(this.GrdImageMorph);

            // check to see if the image is rotated and accordingly check if the mouse position falls in the image region

            if (!isRotated)
            {
                double actualStartX = (Constants.DEFAULT_IMAGE_DIMENSION - (double)this.imageWidth) / (double)2;
                double actualStartY = (Constants.DEFAULT_IMAGE_DIMENSION - (double)this.imageHeight) / (double)2;

                if ((mousePosition.X >= actualStartX && mousePosition.X <= Constants.DEFAULT_IMAGE_DIMENSION - actualStartX - 10) && (mousePosition.Y >= actualStartY && mousePosition.Y <= Constants.DEFAULT_IMAGE_DIMENSION - actualStartY - 10))
                {
                    strokeCurrent.StylusPoints.Add(Utilities.GetStylusPoint(e.GetPosition(this.InkSignature)));

                    InkSignature.Strokes.Add(strokeCurrent);

                }

            }
            else
            {
                double actualStartX = (Constants.DEFAULT_IMAGE_DIMENSION - (double)this.imageWidth) / (double)2;
                double actualStartY = (Constants.DEFAULT_IMAGE_DIMENSION - (double)this.imageHeight) / (double)2;


                if ((mousePosition.X >= actualStartY && mousePosition.X <= Constants.DEFAULT_IMAGE_DIMENSION - actualStartY - 10) && (mousePosition.Y >= actualStartX && mousePosition.Y <= Constants.DEFAULT_IMAGE_DIMENSION - actualStartX - 10))
                {
                    strokeCurrent.StylusPoints.Add(Utilities.GetStylusPoint(e.GetPosition(this.InkSignature)));

                    InkSignature.Strokes.Add(strokeCurrent);
                }
            }
        }

        /// <summary>
        /// Event handler for stroke complete on the ink presenter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkSignature_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WriteableBitmap testBitmap = new WriteableBitmap((int)this.GrdImage.ActualWidth, (int)this.GrdImage.ActualHeight);

            testBitmap.Render(this.GrdImage, null);

            testBitmap.Invalidate();

            this.ImgInput.Source = testBitmap;

            Point point = new Point(-1, -1);

            this.GrdImageMorph.RenderTransformOrigin = point;

            InkSignature.Strokes.Clear();
            strokeCurrent = null;
        }

        #endregion

        #region application logic

        /// <summary>
        /// Flips the current image horizontally
        /// </summary>
        private void FlipHorizontal()
        {
            // set the values needed for flip animation
            this.InitialXFrame.Value = flipModeX;
            this.FinalXFrame.Value = -flipModeX;

            // begin the flip animation
            this.ImageFlipHorizontal.Begin();

            // change the mode for subsequent flip
            this.flipModeX = -flipModeX;

            // store the current image 
            this.GridToCurrentImage();

            // set the render transform point to top left corner
            Point point = new Point(-1, -1);

            this.GrdImageMorph.RenderTransformOrigin = point;

        }

        /// <summary>
        /// Flips the current image vertically
        /// </summary>
        private void FlipVertical()
        {
            // set the values needed for flip animation
            this.InitialYFrame.Value = flipModeY;
            this.FinalYFrame.Value = -flipModeY;

            // begin the flip animation
            this.ImageFlipVertical.Begin();

            // change the mode for subsequent flip
            this.flipModeY = -this.flipModeY;

            // set the render transform point to top left corner
            Point point = new Point(-1, -1);

            this.GrdImageMorph.RenderTransformOrigin = point;

            // store the current image 
            this.GridToCurrentImage();
        }

        /// <summary>
        /// Changes the application bar for the page to the one containing done and cancel buttons
        /// </summary>
        private void ChangeAppBarforEffects()
        {
            this.ApplicationBar = (ApplicationBar)App.Current.Resources["EffectAppBar"];

            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += new EventHandler(BarButtonDone_Click);
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).Click += new EventHandler(BarButtonCancel_Click);

        }

        /// <summary>
        /// Converts the currently displayed grid image to the current image object
        /// </summary>
        private void GridToCurrentImage()
        {
            WriteableBitmap bitmap = new WriteableBitmap((int)this.GrdImage.ActualWidth, (int)this.GrdImage.ActualHeight);

            // grab the image being displayed into the object
            bitmap.Render(this.GrdImage, null);

            bitmap.Invalidate();

            this.InkSignature.Visibility = Visibility.Collapsed;

            this.ImgInput.Source = bitmap;

            this.currentImage = bitmap;
        }

        /// <summary>
        /// Executed once an animation on the image is complete on the image. This is done to deselect the tool box button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Animation_Completed(object sender, EventArgs e)
        {
            this.LsbToolBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Changes the tool box to colors list and sets the previews list to the brush sizes
        /// </summary>
        private void TurnPencilModeOn()
        {
            this.InkSignature.Strokes.Clear();

            this.InkSignature.Visibility = Visibility.Visible;

            this.palletteColors.Clear();

            foreach (string color in this.BRUSH_COLORS)
            {
                PalletteColor palletteColor = new PalletteColor();

                palletteColor.Color = "#" + color;

                palletteColors.Add(palletteColor);

            }

            this.LsbToolBox.ItemsSource = this.palletteColors;

            this.PopulateBrushSizes();

            this.LsbPreview.ItemsSource = this.brushSizes;

            this.currentEffect = (int)Constants.TOOLBOX_EFFECTS.EFFECT_PENCIL;

            this.ChangeAppBarforEffects();

            this.LsbPreview.Visibility = Visibility.Visible;
            this.BorderPreview.Visibility = Visibility.Visible;

            this.isPencilModeOn = true;

        }

        /// <summary>
        /// Fills the tool box with the toobox buttons
        /// </summary>
        private void PopulateToolBox()
        {
            int effectIndex = 0;

            this.toolBoxImages.Clear();

            foreach (string uri in this.TOOLBOX_IMAGE_URIS)
            {
                ToolBoxButton button = new ToolBoxButton();

                button.ImageSource = uri;
                button.Tag = effectIndex;

                this.toolBoxImages.Add(button);

                effectIndex++;
            }

            // set the item source of the tool box images
            this.LsbToolBox.ItemsSource = this.toolBoxImages;
        }

        /// <summary>
        /// Fills the brush sizes collection with appropriate images
        /// </summary>
        private void PopulateBrushSizes()
        {
            this.brushSizes.Clear();

            foreach (string uri in this.BRUSH_SIZE_IMAGE_URIS)
            {
                BrushSizeButton brush = new BrushSizeButton();

                brush.ImageSource = uri;

                this.brushSizes.Add(brush);
            }
        }

        /// <summary>
        /// Applies one of the special effects on the image being displayed based on the selected preview
        /// </summary>
        private void ApplySpecialEffect()
        {
            switch (this.LsbPreview.SelectedIndex)
            {
                case (int)Constants.SPECIAL_EFFECTS.EFFECT_SEPIA:

                    SepiaEffect sepiaEffect = new SepiaEffect();

                    var resultPixels = sepiaEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    break;

                case (int)Constants.SPECIAL_EFFECTS.EFFECT_VIGNETTE:

                    VignetteEffect vignetteEffect = new VignetteEffect();

                    resultPixels = vignetteEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    break;

                case (int)Constants.SPECIAL_EFFECTS.EFFECT_BLACKWHITE:

                    BlackWhiteEffect blackWhiteEffect = new BlackWhiteEffect();

                    resultPixels = blackWhiteEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    break;

                case (int)Constants.SPECIAL_EFFECTS.EFFECT_TILTSHIFT:

                    TiltShiftEffect tiltShiftEffect = new TiltShiftEffect();

                    resultPixels = tiltShiftEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    break;

                case (int)Constants.SPECIAL_EFFECTS.EFFECT_POLAROID:

                    PolaroidEffect polaroidEffect = new PolaroidEffect();

                    resultPixels = polaroidEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    break;

                case (int)Constants.SPECIAL_EFFECTS.EFFECT_TINT:

                    TintEffect tintEffect = new TintEffect();

                    resultPixels = tintEffect.Process(this.currentImage.Pixels, this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    this.temporaryImage = resultPixels.ToWritableBitmap(this.currentImage.PixelWidth, this.currentImage.PixelHeight);

                    break;

                default:
                    break;

            }

        }

        /// <summary>
        /// Fill the preview images collection with the resized preview images after applying the effects
        /// </summary>
        private void GenerateSpecialEffectPreviews()
        {
            previewImage = Utilities.ScaleImage(currentImage, 80, 80);

            this.previewImages.Clear();

            // sepia effect
            SepiaEffect sepiaEffect = new SepiaEffect();

            EffectPreview preview1 = new EffectPreview();


            var resultPixels = sepiaEffect.Process(this.previewImage.Pixels, this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            preview1.ImageSource = resultPixels.ToWritableBitmap(this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            previewImages.Add(preview1);

            // vignette effect
            VignetteEffect vignetteEffect = new VignetteEffect();

            resultPixels = vignetteEffect.Process(this.previewImage.Pixels, this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            EffectPreview preview2 = new EffectPreview();

            preview2.ImageSource = resultPixels.ToWritableBitmap(this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            previewImages.Add(preview2);

            //// blackwhite effect 

            BlackWhiteEffect blackWhiteEffect = new BlackWhiteEffect();

            resultPixels = blackWhiteEffect.Process(this.previewImage.Pixels, this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            EffectPreview preview3 = new EffectPreview();

            preview3.ImageSource = resultPixels.ToWritableBitmap(this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            previewImages.Add(preview3);

            // tiltshift effect 

            TiltShiftEffect tiltShiftEffect = new TiltShiftEffect();

            resultPixels = tiltShiftEffect.Process(this.previewImage.Pixels, this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            EffectPreview preview4 = new EffectPreview();

            preview4.ImageSource = resultPixels.ToWritableBitmap(this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            previewImages.Add(preview4);

            // tint effect 

            PolaroidEffect ploaroidEffect = new PolaroidEffect();

            resultPixels = ploaroidEffect.Process(this.previewImage.Pixels, this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            EffectPreview preview5 = new EffectPreview();


            preview5.ImageSource = resultPixels.ToWritableBitmap(this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            previewImages.Add(preview5);

            //// blackwhite effect 

            TintEffect tintEfect = new TintEffect();

            resultPixels = tintEfect.Process(this.previewImage.Pixels, this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            EffectPreview preview6 = new EffectPreview();

            preview6.ImageSource = resultPixels.ToWritableBitmap(this.previewImage.PixelWidth, this.previewImage.PixelHeight);

            previewImages.Add(preview6);

        }

        /// <summary>
        /// Shows the progress bar and sets the image opacity. Disables the tool box and previews.
        /// </summary>
        private void ShowProgressBar()
        {
            this.LsbToolBox.IsEnabled = false;

            this.LsbPreview.IsEnabled = false;

            this.ProgressBar.Visibility = Visibility.Visible;

            this.ImgInput.Opacity = 0.4;
        }

        /// <summary>
        /// Hides the progress bar and sets the image opacity.
        /// 
        private void HideProgressBar()
        {
            this.ProgressBar.Visibility = Visibility.Collapsed;

            this.ImgInput.Opacity = 1;
        }

        #endregion

        #region Application Bar icon button event handlers

        /// <summary>
        /// Application bar icon button accepted selected event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplicationBarButtonSave_Click(object sender, EventArgs e)
        {
            // Create filename for JPEG in isolated storage
            // Create virtual store and file stream. Check for duplicate tempJPEG files
            var myStore = IsolatedStorageFile.GetUserStoreForApplication();

            if (myStore.FileExists(fileName + ".jpg"))
            {
                myStore.DeleteFile(fileName + ".jpg");
            }

            IsolatedStorageFileStream myFileStream = myStore.CreateFile(this.fileName + ".jpg");

            // Encode the WriteableBitmap into JPEG stream and place into isolated storage
            Extensions.SaveJpeg(this.currentImage, myFileStream, this.currentImage.PixelWidth, this.currentImage.PixelHeight, 0, 85);
            myFileStream.Close();

            // Create a new file stream.
            myFileStream = myStore.OpenFile(fileName + ".jpg", FileMode.Open, FileAccess.Read);

            using(MediaLibrary library = new MediaLibrary())
            {
                //Add the JPEG file to the photos library on the device
                //MediaLibrary library = new MediaLibrary();
                library.SavePicture(fileName + ".jpg", myFileStream);
                myFileStream.Close();

                
            }

            MessageBox.Show(Messages.PictureSavedMessage, Messages.PictureSavedMessageTitle, MessageBoxButton.OK);
        }

        /// <summary>
        /// Application bar icon button Facebook click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplicationBarBtnFacebook_Click(object sender, EventArgs e)
        {
            FBPhotoPost photo = new FBPhotoPost();

            //this.currentImage.

            //App.FacebookImage = imageUpload;
            //App.FacebookImage = photo.GetBitmapImageFromUIElement(this.canvas);
            App.FacebookImage = photo.GetBitmapImageFromUIElement(this.GrdImageMorph);


            //App.CurrentImage = this.currentImage;

            if (App.FacebookImage != null)
            {
                NavigationService.Navigate(new Uri("/FacebookAuthenticate.xaml", UriKind.Relative));
            }
            
        }

        /// <summary>
        /// Application bar icon button Flickr click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApplicationBarButtonFlickr_Click(object sender, EventArgs e)
        {

            FBPhotoPost photo = new FBPhotoPost();
            //App.FlickrImage = photo.GetBitmapImageFromUIElement(this.canvas);
            App.FlickrImage = photo.GetBitmapImageFromUIElement(this.GrdImageMorph);

            if (App.FlickrImage != null)
            {
                App.IsFromEffectsPage = true;

                NavigationService.Navigate(new Uri("/FlickrAuthenticate.xaml", UriKind.Relative));
            }
            

        }

        /// <summary>
        /// Ok button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BarButtonDone_Click(object sender, EventArgs e)
        {
            this.currentImage = this.temporaryImage;

            this.ApplicationBar = (ApplicationBar)Resources["DefaultAppBar"];

            this.LsbPreview.Visibility = Visibility.Collapsed;
            this.BorderPreview.Visibility = Visibility.Collapsed;

            switch (this.currentEffect)
            {
                case (int)Constants.TOOLBOX_EFFECTS.EFFECT_PENCIL:


                    this.GridToCurrentImage();
                    
                    this.isPencilModeOn = false;

                    this.PopulateToolBox();
                    break;
                //case (int)Constants.TOOLBOX_EFFECTS.EFFECT_CROP:

                //    //this.GridToCurrentImage();

                //    WriteableBitmap testBitmap = new WriteableBitmap((int)this.ImgInput.ActualWidth, (int)this.ImgInput.ActualHeight);

                //    testBitmap.Render(this.ImgInput, null);

                //    testBitmap.Invalidate();

                //    this.InkSignature.Visibility = Visibility.Collapsed;

                //    //this.ImgInput.Source = testBitmap;

                //    this.currentImage = testBitmap;

                //    this.Crop();
                //    this.isCropEnabled = false;
                //    break;

                default:
                    break;
            }

            this.LsbToolBox.SelectedIndex = -1;
            this.LsbToolBox.IsEnabled = true;

        }

        protected void BarButtonCancel_Click(object sender, EventArgs e)
        {
            this.ImgInput.Source = this.currentImage;

            this.ApplicationBar = (ApplicationBar)Resources["DefaultAppBar"];

            this.LsbPreview.Visibility = Visibility.Collapsed;
            this.BorderPreview.Visibility = Visibility.Collapsed;


            if (this.currentEffect == (int)Constants.TOOLBOX_EFFECTS.EFFECT_PENCIL)
            {
                this.InkSignature.Visibility = Visibility.Collapsed;
                this.isPencilModeOn = false;

                this.PopulateToolBox();

            }

            this.LsbToolBox.SelectedIndex = -1;
            this.LsbToolBox.IsEnabled = true;
        }

        #endregion

    }
}
