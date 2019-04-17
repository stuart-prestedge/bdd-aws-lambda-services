using System;
using System.Collections.Generic;

namespace BDDReferenceService.Model
{
    
    public class Game
    {

        /**
         * The ID of the game.
         * Public information.
         */
        public string ID { get; set; }

        /**
         * The name of the game.
         * Public information.
         */
        public string Name { get; set; }

        /**
         * The date (no time) that the game opens.
         * Public information.
         */
        public DateTime OpenDate { get; set; }

        /**
         * The date (no time) that the game closes.
         * Public information.
         */
        public DateTime? CloseDate { get; set; }

        /**
         * The number of tickets available to purchase in the game.
         * Public information.
         */
        public UInt32 TicketCount { get; set; }

        /**
         * The ticket price in cents.
         * Public information.
         */
        public UInt16 TicketPrice { get; set; }

        /**
         * Is the game locked.
         * Private information.
         */
        public bool Locked { get; set; } = false;

        /**
         * Is the game published.
         * Private information.
         */
        public bool Published { get; set; } = false;

        /**
         * Is the game frozen.
         * Private information.
         */
        public bool Frozen { get; set; } = false;

        /**
         * The URL of the ticket service that manages tickets for this game.
         * Public information.
         */
        public string TicketServiceURL { get; set; }

        /**
         * How much (in US$) has been donated to charity. Null or missing means zero.
         * Public information.
         */
        public UInt32? DonatedToCharity { get; set; }

    }   // Game

}   // BDDReferenceService.Model
