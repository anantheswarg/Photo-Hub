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

using System.Windows.Media.Imaging;

namespace PhotoHub
{
   /// <summary>
   /// Interface of a bitmap effect.
   /// </summary>
   public interface IEffect
   {
      /// <summary>
      /// The human readable name of the effect.
      /// </summary>
      string Name { get; }

      /// <summary>
      /// Processes a bitmap and returns a new processed WriteabelBitmap.
      /// </summary>
      /// <param name="input">The input bitmap.</param>
      /// <returns>The result of WriteabelBitmap processing.</returns>
      WriteableBitmap Process(WriteableBitmap input);

      /// <summary>
      /// Processes an ARGB32 integer bitmap and returns the new processed bitmap data.
      /// </summary>
      /// <param name="inputPixels">The input bitmap as integer array.</param>
      /// <param name="width">The width of the bitmap.</param>
      /// <param name="height">The height of the bitmap.</param>
      /// <returns>The result of the processing.</returns>
      int[] Process(int[] inputPixels, int width, int height);
   }
}
