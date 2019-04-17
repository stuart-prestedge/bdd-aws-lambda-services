using System;
namespace BDDReferenceService.Contracts
{
    public class ReserveTicketRequest
    {
    
        /**
         * This is the ID of the user that the ticket is reserved for. If missing or null then the currently logged in user is inferred.
         */
        public string reservingUserId { get; set; }

        /**
         * The hash of the ID of the game that the ticket is being reserved for.
         */
        public string gameIdHash { get; set; }

        /**
         * The optional ticket number to reserve.
         */
        public UInt32? ticketNumber { get; set; }

    }   // ReserveTicketRequest

}   // BDDReferenceService.Contracts
