/**
 * By: Ryan Moe
 * Edited Last: 
 * 
 * Think of this more of like an "Error Report" class...
 * This is a standardized error class to use
 * for communicating with the gui when things go back.
 * These will probably be passed to the GUI through callbacks.
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    

    public class ErrorReport
    {
        //---------------------------
        //CONSTANTS, these are standardized numbers for telling
        //the gui what happen in the backend.
        public const int SUCCESS = 0;
        public const int SUCCESS_WITH_WARNINGS = 1;
        public const int FAILURE = 2;
        //---------------------------
        //Error object vars.
        //These get set by the backend.
        public int reportID;
        public string description;
        public List<string> warnings;
        //---------------------------
        //Default to success, of course!
        public ErrorReport()
        {
            reportID = 0;
            description = "";
            warnings = new List<string>();
        }


    }

}
