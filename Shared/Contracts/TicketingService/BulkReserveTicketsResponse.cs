using System;

namespace BDDReferenceService.Contracts {

    public class BulkReserveTicketsResponse {
    
        /**
         * This is the first ticket number.
         */
        public UInt32 firstTicketNumber { get; set; }

        /**
         * This is the last ticket number.
         */
        public UInt32 lastTicketNumber { get; set; }

        /**
         * The number of tickets reserved.
         */
        public UInt32 tickets { get; set; }

        /**
         * The UTC date/time of when the ticket is reserved until.
         */
        public string reservedUntil { get; set; }

    }   // BulkReserveTicketsResponse

}   // BDDReferenceService.Contracts
