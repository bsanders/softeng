using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SoftwareEng
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            const bool ownThisMutex = true; 

            //when specified as 'out' no need to initialize
            bool mutexCreated;

            System.Threading.Mutex mutex = new System.Threading.Mutex(ownThisMutex, "PhotoBomberMutex", out mutexCreated);

            //thus another instance is already running
            if (mutexCreated == false)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainGUI());//main gui
            //Application.Run(new Form1());//backend testing gui

            //keeps hold of mutex until program is done
            GC.KeepAlive(mutex);
        }
    }
}
