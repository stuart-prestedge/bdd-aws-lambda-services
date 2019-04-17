using System;
namespace BDDReferenceService.Contracts
{
    public class AcceptTicketOfferRequest
    {
    
        /**
         * This is the ID of the user that the ticket is accepted on behalf of. If missing or null then the currently logged in user is inferred.
         */
        public string acceptingUserId { get; set; }

        /**
         * The email of the user accepting the ticket offer.
         * For extra security, this must match the email stored in the ticket offer record.
         */
        public string acceptingUserEmail { get; set; }

        /**
         * The ID of the ticket gift offer record.
         */
        public string ticketGiftOfferId { get; set; }

        /**
         * The ID of the syndicate the ticket is placed in.
         */
        public string syndicateId { get; set; }

    }   // AcceptTicketOfferRequest

}   // BDDReferenceService.Contracts
