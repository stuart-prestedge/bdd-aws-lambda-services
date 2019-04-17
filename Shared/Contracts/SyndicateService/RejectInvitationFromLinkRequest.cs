using System;
namespace BDDReferenceService.Contracts
{
    public class RejectInvitationFromLinkRequest
    {
    
        /**
         * The ID of the link used to reject the invitation.
         */
        public string linkId { get; set; }

        /**
         * The email address that the link is associated with.
         */
        public string emailAddress { get; set; }

    }   // RejectInvitationFromLinkRequest

}   // BDDReferenceService.Contracts
