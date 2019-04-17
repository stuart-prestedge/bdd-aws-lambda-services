using System;
using System.Collections.Generic;
using BDDReferenceService.Contracts;
using BDDReferenceService.Model;

namespace BDDReferenceService
{

    /**
     * Logging helper class.
     */
    public static class LoggingHelper
    {

        /**
         * Log a message line.
         */
        public static void LogMessage(string message)
        {
            Debug.Untested();
            Debug.AssertString(message);

            Console.WriteLine(message);
        }

    }   // LoggingHelper

}   // BDDReferenceService
