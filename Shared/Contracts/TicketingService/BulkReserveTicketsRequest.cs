using System;
namespace BDDReferenceService.Contracts
{
    public class BulkReserveTicketsRequest
    {
    
        /**
         * This is the ID of the user that the tickets are reserved for.
         */
        public string userId { get; set; }

        /**
         * The hash of the ID of the game that the tickets are being reserved for.
         */
        public string gameIdHash { get; set; }

        /**
         * The minimum ticket number to reserve.
         */
        public UInt32 minTicketNumber { get; set; }

        /**
         * The maximum ticket number to reserve.
         */
        public UInt32 maxTicketNumber { get; set; }

        /**
         * The number of tickets to reserve.
         */
        public UInt32 tickets { get; set; }

        /**
         * The allowed ticket boundary to select.
         * The first ticket number, modulo the boundary, must be zero.
         */
        public UInt32 boundary { get; set; }

    }   // BulkReserveTicketsRequest

}   // BDDReferenceService.Contracts
