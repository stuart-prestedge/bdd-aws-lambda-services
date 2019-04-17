using System;
namespace BDDReferenceService.Contracts
{
    public class VerifyEmailRequest
    {
    
        /**
         * A mandatory string that contains a link ID sent in the create account and set user email APIs.
         */
        public string verifyEmailLinkId { get; set; }

        /**
         * A mandatory string that contains the email address that the verify email link was sent to.
         */
        public string emailAddress { get; set; }

    }   // VerifyEmailRequest

}   // BDDReferenceService.Contracts
