using System;
namespace BDDReferenceService.Contracts
{
    public class InviteMemberRequest
    {
    
        /**
         * The email address of the person being invited to the syndicate.
         */
        public string emailAddress { get; set; }

        /**
         * An optional string containing the message to the invited person.
         */
        public string message { get; set; }

    }   // InviteMemberRequest

}   // BDDReferenceService.Contracts
