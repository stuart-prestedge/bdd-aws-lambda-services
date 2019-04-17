using System;
namespace BDDReferenceService.Contracts
{
    public class GameRequest
    {
    
        /**
         * The name of the game.
         */
        public string name { get; set; }

        /**
         * The date/time that the game opens.
         */
        public string openDate { get; set; }

        /**
         * The date/time that the game closes.
         */
        public string closeDate { get; set; }

        /**
         * The number of tickets available to purchase in the game.
         */
        public UInt32 ticketCount { get; set; }

        /**
         * The ticket price in cents.
         */
        public UInt16 ticketPrice { get; set; }

        /**
         * The URL of the ticket service that manages tickets for this game.
         */
        public string ticketServiceURL { get; set; }

        /**
         * How much (in US$) has been donated to charity. Null or missing means zero.
         */
        public UInt32? donatedToCharity { get; set; }

    }   // GameRequest

}   // BDDReferenceService.Contracts
