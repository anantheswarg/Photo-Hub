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
    public sealed class EffectsData
    {
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
        public static string[] BRUSH_SIZE_IMAGE_URIS = {    "/Static/Icons/brush1.png",
                                                            "/Static/Icons/brush2.png",
                                                            "/Static/Icons/brush3.png",
                                                            "/Static/Icons/brush4.png",
                                                            "/Static/Icons/brush5.png"
                                                        };


        public static float[] BRIGHTNESS_FACTORS = {    -0.6f,
                                                        -0.4f,
                                                        -0.2f,
                                                        0.2f,
                                                        0.4f,
                                                        0.6f
                                                    };

        public static float[] CONTRAST_FACTORS = {
                                                        -0.6f,
                                                        -0.4f,
                                                        -0.2f,
                                                        0.2f,
                                                        0.4f,
                                                        0.6f
                                                  };
        public static float[] BLUR_SIGMAS = {
                                                0.5f,
                                                1f,
                                                1.5f,
                                                2.0f
                                            };


        public static float[] BRUSH_SIZES = {
                                                        2.0f,
                                                        6.0f,
                                                        10.0f,
                                                        14.0f,
                                                        18.0f
                                                    };

        public static string[] BRUSH_COLORS = {   
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
        private ObservableCollection<BrushSizeButton> BRUSH_SIZES = new ObservableCollection<BrushSizeButton>();

        /// <summary>
        /// List of images for tool box
        /// </summary>
        private ObservableCollection<ToolBoxButton> toolBoxImages = new ObservableCollection<ToolBoxButton>();


    }
}
