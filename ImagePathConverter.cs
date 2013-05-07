/*
 * Change Log:
 * Ryan Causey(4/6/13)
 * This class was made,
 * 
 */
using System;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace SoftwareEng
{
    /*
     * Created By: Ryan Causey(4/6/13)
     * Last Edited By: Ryan Causey
     * Last Edited Date: 4/7/13
     */
    /// By Ryan Causey
    /// Edited Julian Nguyen(5/7/13)
    /// <summary>
    /// Converter to allow data binding to be used in the BitmapImage UriSource attribute with the
    /// cache option on.
    /// </summary>
    public class ImagePathConverter : IValueConverter
    {
        /// By Ryan Causey
        /// Edited Julian Nguyen(5/7/13)
        /// <summary>
        /// Converter to allow data binding to be used in the BitmapImage UriSource attribute with the
        /// cache option on
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // value contains the full path to the image
            String path = (String)value;
            //handled the path not being loaded yet.
            if (path != "")
            {
                if (File.Exists(path))
                {
                    // load the image, convert to bitmap, set cache option so it
                    //does not lock out the file, then return the new image.
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(path);
                    image.EndInit();

                    return image;
                }
            }

            return DependencyProperty.UnsetValue;
        }


        /// By Ryan Causey
        /// Edited Julian Nguyen(5/7/13)
        /// <summary>
        /// put this here so that if someone tries to convert back we throw an exception as
        /// the operation is not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
