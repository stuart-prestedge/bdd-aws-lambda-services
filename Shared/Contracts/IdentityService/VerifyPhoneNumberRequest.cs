using System;
namespace BDDReferenceService.Contracts
{
    public class VerifyPhoneNumberRequest
    {
    
        /**
         * A mandatory string that contains the one-time-password sent to the user.
         */
        public string oneTimePassword { get; set; }

    }   // VerifyPhoneNumberRequest

}   // BDDReferenceService.Contracts
