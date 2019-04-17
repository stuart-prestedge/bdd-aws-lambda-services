using System;
namespace BDDReferenceService.Contracts
{
    public class ReserveTicketResponse
    {
    
        /**
         * The UTC date/time of when the ticket is reserved until.
         */
        public string reservedUntil { get; set; }

        /**
         * The reserved ticket number.
         */
        public UInt32 ticketNumber { get; set; }

    }   // ReserveTicketResponse

}   // BDDReferenceService.Contracts
