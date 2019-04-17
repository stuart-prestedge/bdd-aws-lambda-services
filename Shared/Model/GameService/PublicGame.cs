using System;
using System.Collections.Generic;

namespace BDDReferenceService.Model
{
    
    public class PublicGame
    {

        /**
         * The hash of the ID of the game.
         */
        public string IDHash { get; set; }

        /**
         * The name of the game.
         */
        public string Name { get; set; }

        /**
         * The date/time that the game opens.
         */
        public string OpenDate { get; set; }

        /**
         * The date/time that the game closes.
         */
        public string CloseDate { get; set; }

        /**
         * The number of tickets available to purchase in the game.
         */
        public UInt32 TicketCount { get; set; }

        /**
         * The ticket price in cents.
         */
        public UInt16 TicketPrice { get; set; }

        /**
         * The URL of the ticket service that manages tickets for this game.
         * Public information.
         */
        public string TicketServiceURL { get; set; }

    }   // PublicGame

}   // BDDReferenceService.Model
