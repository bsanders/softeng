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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainGUI());//main gui
            //Application.Run(new Form1());//backend testing gui
        }
    }
}
