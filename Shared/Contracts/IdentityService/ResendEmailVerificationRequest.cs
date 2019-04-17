using System;
namespace BDDReferenceService.Contracts
{
    public class ResendEmailVerificationRequest
    {
    
        /**
         * A mandatory string that contains a valid email address that belongs to an existing user.
         */
        public string emailAddress { get; set; }

    }   // ResendEmailVerificationRequest

}   // BDDReferenceService.Contracts
