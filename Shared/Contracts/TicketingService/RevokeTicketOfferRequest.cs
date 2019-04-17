using System;
namespace BDDReferenceService.Contracts
{
    public class RevokeTicketOfferRequest
    {
    
        /**
         * This is the ID of the user that the offer was made on behalf of (i.e. the user that owns the ticket). If missing or null then the currently logged in user is inferred.
         */
        public string revokingUserId { get; set; }

        /**
         * The ID of the ticket being revoked.
         */
        public string ticketId { get; set; }

    }   // RevokeTicketOfferRequest

}   // BDDReferenceService.Contracts
