using System;
namespace BDDReferenceService.Contracts
{
    public class BuyTicketsRequest : BuyTicketsBaseRequest
    {
    
        /**
         * The ticket numbers to buy.
         */
        public UInt32?[] ticketNumbers { get; set; }

    }   // BuyTicketsRequest

}   // BDDReferenceService.Contracts
