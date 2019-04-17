using System;
namespace BDDReferenceService.Contracts
{
    public class LoginRequest
    {
    
        /**
         * A mandatory string (must exist and must not be null or empty) containing a valid email address.
         */
        public string emailAddress { get; set; }

        /**
         * A mandatory string containing a password.
         */
        public string password { get; set; }

    }   // LoginRequest

}   // BDDReferenceService.Contracts
