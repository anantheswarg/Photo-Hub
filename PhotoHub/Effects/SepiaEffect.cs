#region Header
//
//   Project:           PicFx - Windows Phone 7 image effects app.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using System.Windows.Media.Imaging;

namespace PhotoHub
{
   /// <summary>
   /// Sepia conversion effect.
   /// </summary>
   public class SepiaEffect : IEffect
   {
      readonly TintEffect tintFx;
      readonly BrightnessContrastEffect contrastFx;

      public string Name { get { return "Sepia"; } }

      /// <summary>
      /// The contrast factor. 
      /// Should be in the range [-1, 1]. 
      /// Default is 0.05
      /// </summary>
      public float ContrastFactor 
      {
         get { return contrastFx.ContrastFactor; }
         set { contrastFx.ContrastFactor = value; }
      }

      public SepiaEffect()
      {
         tintFx = TintEffect.Sepia;
         contrastFx = new BrightnessContrastEffect { ContrastFactor = 0.05f };
      }

      /// <summary>
      /// Processes a bitmap and returns a new processed WriteabelBitmap.
      /// </summary>
      /// <param name="input">The input bitmap.</param>
      /// <returns>The result of WriteabelBitmap processing.</returns>
      public WriteableBitmap Process(WriteableBitmap input)
      {
          if (input == null)
          {
              return null;
          }
   
         // Prepare some variables
         var width = input.PixelWidth;
         var height = input.PixelHeight;
         return Process(input.Pixels, width, height).ToWritableBitmap(width, height);
      }

      /// <summary>
      /// Processes an ARGB32 integer bitmap and returns the new processed bitmap data.
      /// </summary>
      /// <param name="inputPixels">The input bitmap as integer array.</param>
      /// <param name="width">The width of the bitmap.</param>
      /// <param name="height">The height of the bitmap.</param>
      /// <returns>The result of the processing.</returns>
      public int[] Process(int[] inputPixels, int width, int height)
      {
          if (inputPixels == null)
          {
              return null;
          }

         var resultPixels = tintFx.Process(inputPixels, width, height);
         resultPixels = contrastFx.Process(resultPixels, width, height);
         return resultPixels;
      }
   }
}
