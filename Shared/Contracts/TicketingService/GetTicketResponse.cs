using System;
namespace BDDReferenceService.Contracts {

    public class GetTicketResponse {
    
        /**
         * The ID of the ticket record.
         */
        public string id { get; set; }

        /**
         * The ID of the user that owns this ticket.
         */
        public string ownerId { get; set; }

        /**
         * The hash of the ID of the game.
         */
        public string gameIdHash { get; set; }

        /**
         * The number of this ticket.
         */
        public UInt32 number { get; set; }

        /**
         * The ID of the syndicate that the ticket belongs to.
         */
        public string syndicateId { get; set; }

        /**
         * The date/time that this ticket was purchased.
         */
        public string purchasedDate { get; set; }

        /**
         * This is the display name of the user that offered this ticket.
         * It need only exist if the user getting the ticket information is not the owner of the ticket (i.e. is the user who the ticketed is being offered to).
         */
        public string offeredBy { get; set; }

        /**
         * The ID of the user that this ticket has been offered to as a gift.
         * Null if not offered as a gift.
         */
        public string offeredToEmail { get; set; }

        /**
         * The date/time that this ticket was offered as a gift.
         */
        public string offeredDate { get; set; }

    }   // GetTicketResponse

}   // BDDReferenceService.Contracts
