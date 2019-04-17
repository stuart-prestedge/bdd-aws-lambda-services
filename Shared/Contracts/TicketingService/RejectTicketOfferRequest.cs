using System;
namespace BDDReferenceService.Contracts
{
    public class RejectTicketOfferRequest
    {
    
        /**
         * This is the ID of the user that the ticket is rejected on behalf of. If missing or null then the currently logged in user is inferred.
         */
        public string rejectingUserId { get; set; }

        /**
         * The email of the user rejecting the ticket offer.
         * For extra security, this must match the email stored in the ticket offer record.
         */
        public string rejectingUserEmail { get; set; }

        /**
         * The ID of the ticket gift offer record.
         */
        public string ticketGiftOfferId { get; set; }

    }   // RejectTicketOfferRequest

}   // BDDReferenceService.Contracts
