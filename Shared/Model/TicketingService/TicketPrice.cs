using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a ticket price.
     */
    public class TicketPrice
    {
        
        /**
         * The currency code.
         */
        public string Currency { get; set; }

        /**
         * The ticket price in the currency (smallest denominations, e.g. cents, pence etc.).
         */
        public UInt64 Price { get; set; }

        /**
         * Are tickets available to purchase in this currency?
         */
        public bool Available { get; set; }

    }   // TicketPrice

}   // BDDReferenceService.Model
