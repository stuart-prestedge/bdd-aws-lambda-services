using System;
namespace BDDReferenceService.Contracts
{
    public class BuyTicketResponse
    {
    
        /**
         * The amount, in the purchase currency, that the ticket(s) was/were purchased for.
         */
        public UInt64 amount { get; set; }

        /**
         * The currency that the purchase was made in.
         */
        public string currency { get; set; }

        /**
         * The ID of the purchased ticket.
         */
        public string ticketId { get; set; }

        /**
         * The number of the purchased ticket.
         */
        public UInt32 ticketNumber { get; set; }

    }   // BuyTicketResponse

}   // BDDReferenceService.Contracts
