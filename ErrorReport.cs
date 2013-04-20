/**
 * By: Ryan Moe
 * Edited Last: 
 * 
 * Think of this more of like an "Error Report" class...
 * This is a standardized error class to use
 * for communicating with the gui when things go back.
 * These will probably be passed to the GUI through callbacks.
 * 
 * 
 * Change Log:
 * 4/19/13 Julian Nguyen: Added ErrorReport.ReportTypes to replace the old CONSTANTS for ReportID.
 * 
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    
    /// <summary>
    /// 
    /// </summary>
    public class ErrorReport
    {

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum ReportTypes : int
        {
            SUCCESS = 0,
            SUCCESS_WITH_WARNINGS = 1,
            FAILURE = 3
        }
        


        //Error object vars.
        //These get set by the backend.
        private ReportTypes _reportID;
        private string _description;
        private List<string> _warningList;
        //---------------------------
        //Default to success, of course!

        /// <summary>
        /// This is the default constructor. 
        /// Set the reportID to SUCCESS. 
        /// </summary>
        public ErrorReport()
        {
            this._reportID = ReportTypes.SUCCESS;
            this._description = String.Empty;
            this._warningList = new List<string>();
        }


        /*
         * Setter and getters. 
         */ 

        public ReportTypes reportID
        {
            get
            {
                return this._reportID;
            }
            set
            {
                this._reportID = value;
            }
        }

        public String description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        public List<String> warnings
        {
            get
            {
                return this._warningList;
            }
            set
            {
                this._warningList = value;
            }
        }

    } // End of ErrorReport class. 

}
