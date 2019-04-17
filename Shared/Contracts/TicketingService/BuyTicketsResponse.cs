using System;
namespace BDDReferenceService.Contracts
{
    public class BuyTicketsResponse
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
         * The IDs of the purchased tickets.
         */
        public string[] ticketIds { get; set; }

        /**
         * The numbers of the purchased tickets.
         */
        public UInt32[] ticketNumbers { get; set; }

    }   // BuyTicketsResponse

}   // BDDReferenceService.Contracts
