/********************************************************************************
 * This is a class to take the .dll files we are using and embedd them into the
 * executable produced by visual studio when we hit build. There are a couple steps
 * that need to be taken to get this to work.
 * 
 * Since the .csproj file is tracked by git I can skip that step. If for some
 * reason this is not working, see the source below to change your .csproj file.
 * 
 * Since the project solution is not tracked by git you will need to do the following
 * before you build:
 * Go to Project->SoftwareEng Properties->Application and change the startup object
 * item to point to this class.
 * 
 * Now you should be able to hit build/re-build and the executable produced in the
 * bin/debug folder will be able to run without the .dll's being shipped with it.
 * 
 * Source: http://www.digitallycreated.net/Blog/61/combining-multiple-assemblies-into-a-single-exe-for-a-wpf-application
 * -Ryan Causey
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Shell;

namespace SoftwareEng
{
    class ProgramStarter
    {
        [STAThreadAttribute]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            App.Main();
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);

            string path = assemblyName.Name + ".dll";
            if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
            {
                path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
            }

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                    return null;

                byte[] assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
