using System;
using BDDReferenceService.Logic;

namespace BDDReferenceService
{
    
    internal static class NameHelper
    {

        /**
         * Get a display name.
         */
        internal static string GetDisplayName(string givenName, string familyName) {
            Debug.Tested();

            string retVal = null;
            if (!string.IsNullOrEmpty(givenName)) {
                retVal = givenName;
            }
            if (!string.IsNullOrEmpty(familyName)) {
                if (!string.IsNullOrEmpty(retVal)) {
                    retVal += " " + familyName;
                } else {
                    retVal = familyName;
                }
            }
            return retVal;
        }

    }   // NameHelper

}   // BDDReferenceService
