using System;
namespace BDDReferenceService.Contracts
{
    public class SetUserPasswordAfterResetRequest
    {
    
        /**
         * A mandatory string that contains a link ID sent in the reset password API.
         */
        public string resetPasswordLinkId { get; set; }

        /**
         * A mandatory string that contains the email address that the reset password link was sent to.
         */
        public string emailAddress { get; set; }

        /**
         * A mandatory string that contains the valid new password.
         */
        public string newPassword { get; set; }

    }   // SetUserPasswordAfterResetRequest

}   // BDDReferenceService.Contracts
