using System;
namespace BDDReferenceService.Contracts
{
    public class SetUserPasswordRequest
    {
    
        /**
         * A mandatory string that contains the user's existing password.
         */
        public string oldPassword { get; set; }

        /**
         * A mandatory string that contains a valid new password.
         */
        public string newPassword { get; set; }

    }   // SetUserPasswordRequest

}   // BDDReferenceService.Contracts
