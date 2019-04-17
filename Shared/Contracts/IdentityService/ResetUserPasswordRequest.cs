using System;
namespace BDDReferenceService.Contracts
{
    public class ResetUserPasswordRequest
    {
    
        /**
         * A mandatory string with the user's email address. It must be a valid email address.
         */
        public string emailAddress { get; set; }

        /**
         * A mandatory string used to determine that the caller is a human.
         */
        public string reCaptcha { get; set; }

    }   // ResetUserPasswordRequest

}   // BDDReferenceService.Contracts
