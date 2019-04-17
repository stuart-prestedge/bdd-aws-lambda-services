using System;
namespace BDDReferenceService.Contracts
{
    public class SetUserNameRequest
    {
    
        /**
         * A mandatory string (must exist and must not be null or empty) with the user's given name.
         */
        public string givenName { get; set; }

        /**
         * A mandatory string with the user's family name.
         */
        public string familyName { get; set; }

        /**
         * An optional string with the user's full name.
         */
        public string fullName { get; set; }

        /**
         * An optional string with the user's preferred name.
         */
        public string preferredName { get; set; }

    }   // SetUserNameRequest

}   // BDDReferenceService.Contracts
