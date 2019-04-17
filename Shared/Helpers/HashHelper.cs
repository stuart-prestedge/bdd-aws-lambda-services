using System;

namespace BDDReferenceService
{
    
    internal static class HashHelper
    {

        /**
         * Hash an ID.
         */
        internal static string GetIDHash(string id) {
            Debug.Untested();

            return Helper.Hash(id);
        }

    }   // HashHelper

}   // BDDReferenceService
