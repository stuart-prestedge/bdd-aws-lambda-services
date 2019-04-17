using System;
namespace BDDReferenceService.Contracts
{
    public class OfferTicketRequest
    {
    
        /**
         * This is the ID of the user that the offer is made on behalf of (i.e. the user that owns the ticket). If missing or null then the currently logged in user is inferred.
         */
        public string offeringUserId { get; set; }

        /**
         * The ID of the ticket being offered.
         */
        public string ticketId { get; set; }

        /**
         * The email address of the person the ticket is being immediately gifted to.
         */
        public string offerToEmail { get; set; }

        /**
         * The message for the person the ticket is being offered to.
         */
        public string offerMessage { get; set; }

    }   // OfferTicketRequest

}   // BDDReferenceService.Contracts
