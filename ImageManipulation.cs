using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SoftwareEng
{
    /// By Julian Nguyen
    /// Edited: Julian Nguyen(5/2/13)
    /// <summary>
    /// 
    /// </summary>
    class ImageManipulation
    {


        /// By: http://tech.pro/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale
        /// Edited: Julian Nguyen(5/2/13)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
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
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Bitmap LoadImageNoLock(String path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path)); // Don't use using!!
            return new Bitmap(Image.FromStream(ms));
        }


    } // End of ImageManipulation.
}
