#region Header
//
//   Project:           Windows Phone 7 extensions.
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
using System;

namespace PhotoHub
{
   /// <summary>
   /// WriteableBitmap extension for WP7 media library
   /// </summary>
    public static class ArrayExtensions
   {

      /// <summary>
      /// Creates a WriteableBitmap from an integer array that is interpreted as ARGB32.
      /// </summary>
      /// <param name="input">The pixels as ARGB32.</param>
      /// <param name="width">The width of the bitmap.</param>
      /// <param name="height">The height of the bitmap.</param>
      /// <returns>The result WriteableBitmap.</returns>
      public static WriteableBitmap ToWritableBitmap(this int[] input, int width, int height)
      {
          if (input == null)
          {
              return null;
          }

         var result = new WriteableBitmap(width, height);
         Buffer.BlockCopy(input, 0, result.Pixels, 0, input.Length * 4);
         return result;
      }
   }
}
