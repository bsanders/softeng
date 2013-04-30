using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace SoftwareEng
{

    //Both classes courtesy of Svetoslav Savov   @ http://svetoslavsavov.blogspot.com/2009/07/switching-wpf-interface-themes-at.html

    public class ThemeResourceDictionary : ResourceDictionary
    {
    }

    class ThemeSelector : DependencyObject
    {

        public static readonly DependencyProperty CurrentThemeDictionaryProperty = DependencyProperty.RegisterAttached("CurrentThemeDictionary", typeof(Uri), typeof(ThemeSelector), new UIPropertyMetadata(null, CurrentThemeDictionaryChanged));

        public static Uri GetCurrentThemeDictionary(DependencyObject obj)
        {
            return (Uri)obj.GetValue(CurrentThemeDictionaryProperty);
        }

        public static void SetCurrentThemeDictionary(DependencyObject obj, Uri value)
        {
            obj.SetValue(CurrentThemeDictionaryProperty, value);
        }

        private static void CurrentThemeDictionaryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is FrameworkElement) // works only on FrameworkElement objects  
            {
                ApplyTheme(obj as FrameworkElement, GetCurrentThemeDictionary(obj));
            }
        }

        private static void ApplyTheme(FrameworkElement targetElement, Uri dictionaryUri)
        {
            if (targetElement == null) return;

            try
            {
                ThemeResourceDictionary themeDictionary = null;
                if (dictionaryUri != null)
                {
                    themeDictionary = new ThemeResourceDictionary();
                    try
                    {
                        themeDictionary.Source = dictionaryUri;
                    }
                    catch (FileNotFoundException e)
                    {

                        ErrorWindow debugWindow = new ErrorWindow("debugging error: Selected Theme Xaml not found \n \n"+e.FusionLog);

                        debugWindow.ShowDialog();
                    }

                    // add the new dictionary to the collection of merged dictionaries of the target object  
                    targetElement.Resources.MergedDictionaries.Insert(0, themeDictionary);
                }

                // find if the target element already has a theme applied  
                List<ThemeResourceDictionary> existingDictionaries =
                    (from dictionary in targetElement.Resources.MergedDictionaries.OfType<ThemeResourceDictionary>()
                     select dictionary).ToList();

                // remove the existing dictionaries  
                foreach (ThemeResourceDictionary thDictionary in existingDictionaries)
                {
                    if (themeDictionary == thDictionary) continue;  // don't remove the newly added dictionary  
                    targetElement.Resources.MergedDictionaries.Remove(thDictionary);
                }
            }
            finally { }
        }
    }
}
