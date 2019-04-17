using System;
namespace BDDReferenceService.Contracts
{
    public class BulkAssignTicketsRequest
    {
    
        /**
         * This is the ID of the user that the tickets are assigned to.
         */
        public string userId { get; set; }

        /**
         * The hash of the ID of the game that the tickets are being assigned to.
         */
        public string gameIdHash { get; set; }

        /**
         * The first ticket number to assign.
         */
        public UInt32 firstTicketNumber { get; set; }

        /**
         * The last ticket number to assign.
         */
        public UInt32 lastTicketNumber { get; set; }

        /**
         * This is the ID of the syndicate that the tickets are assigned to.
         */
        public string syndicateId { get; set; }

        /**
         * The currency to attempt purchase in.
         */
        public string currency { get; set; }

    }   // BulkAssignTicketsRequest

}   // BDDReferenceService.Contracts
