/**
 * By: Ryan Moe
 * Edited Last: 
 * 
 * Think of this more of like an "Error Report" class...
 * This is a standardized error class to use
 * for communicating with the gui when things go back.
 * These will probably be passed to the GUI through callbacks.
 * 
 * Change log: 
 * Julian Nguyen(4/30/13)
 * ErrorReports constants numbers removed and replaced with ReportStatus enums.
 * Comments added.
 * 
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    /// By Julian Nguyen
    /// Edited: Julian Nguyen(4/30/13)
    /// <summary>
    /// Standardized enums for telling the gui what happened.
    /// </summary>
    public enum ReportStatus
    {
        SUCCESS,
        SUCCESS_WITH_WARNINGS,
        FAILURE
    } // End of ReportStatus.

    public class ErrorReport
    {
        // The status of the report.
        public ReportStatus reportStatus;

        // If there was an error, this is the description.
        public string description;


        // If there was an warning, here is a list of them, why not??
        public List<string> warnings;


        /// By Ryan Moe
        /// Edited: Julian Nguyen(4/30/13)
        /// <summary>
        /// Default constructor. 
        /// Defaults to success.
        /// </summary>
        public ErrorReport()
        {
            reportStatus = ReportStatus.SUCCESS;
            description = String.Empty;
            warnings = new List<string>();
        }


    } // End of ErrorReport.

}
