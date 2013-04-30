/********************************************************************************
 * Edited this class to implement ISingleInstanceApp to make it so that the
 * application can only launch one instance.
 * 
 * Uses SingleInstance.cs
 * 
 * Source: http://blogs.microsoft.co.il/blogs/arik/archive/2010/05/28/wpf-single-instance-application.aspx
 * -Ryan Causey
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Shell;

namespace SoftwareEng
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "Photobomber Unique String";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }


        public ResourceDictionary ThemeDictionary
        {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
        }

        public void setTheme(string desiredTheme)
        {
            Uri themeURI = new Uri(desiredTheme, UriKind.Relative);

            Resources.MergedDictionaries.Clear();

            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = (themeURI) });
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // handle command line arguments of second instance
            // ...

            return true;
        }

        #endregion
    }
}
