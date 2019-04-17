using System;
namespace BDDReferenceService.Contracts
{
    public class BuyTicketRequest : BuyTicketsBaseRequest
    {
    
        /**
         * The optional ticket number to buy.
         */
        public UInt32? ticketNumber { get; set; }

    }   // BuyTicketRequest

}   // BDDReferenceService.Contracts
