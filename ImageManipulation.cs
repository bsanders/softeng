/*
 * Change Log:
 * Julian Nguyen(5/2/13)
 * This class was made.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace SoftwareEng
{
    /// By Julian Nguyen
    /// Edited: Julian Nguyen(5/2/13)
    /// <summary>
    /// This call is from image manipulation.
    /// </summary>
    class ImageManipulation
    {


        /// By: http://tech.pro/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale
        /// Edited: Julian Nguyen(5/2/13)
        /// <summary>
        /// This is will grauscale a bitmap.
        /// </summary>
        /// <param name="original"></param>
        /// <returns>A grauscale bitmap.</returns>
        public Bitmap makeGrayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][] 
                {
                     new float[] {.3f, .3f, .3f, 0, 0},
                     new float[] {.59f, .59f, .59f, 0, 0},
                     new float[] {.11f, .11f, .11f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }



        /// By: http://stackoverflow.com/questions/3386749/loading-a-file-to-a-bitmap-but-leaving-the-original-file-intact
        /// Edited: Julian Nguyen(5/6/13)
        /// <summary>
        /// This will load an image into memory and it will not lock the file.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <returns>The loaded bitmap in memory.</returns>
        public Bitmap LoadImageNoLock(String path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path)); // Don't use using!!
            return new Bitmap(Image.FromStream(ms));
        }


        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        /// By: http://stackoverflow.com/questions/180030/how-can-i-find-out-when-a-picture-was-actually-taken-in-c-sharp-running-on-vista
        /// Edited Julian Nguyen(5/4/13)
        /// <summary>
        /// retrieves the datetime WITHOUT loading the whole image
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PropertyItem[] getImageProperty(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Image myImage = Bitmap.FromStream(fs, false, false);

            return myImage.PropertyItems;

        }

       


    } // End of ImageManipulation.
}
