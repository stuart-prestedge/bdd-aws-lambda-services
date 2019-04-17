using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a ticket.
     */
    internal class Ticket
    {
        
        /**
         * The ID of the ticket record.
         */
        internal string ID { get; set; }

        /**
         * The ID of the game record that this draw belongs to.
         */
        internal string GameID { get; set; }

        /**
         * The number of this ticket.
         */
        internal UInt32 Number { get; set; }

        /**
         * The ID of the syndicate that the ticket belongs to.
         */
        internal string SyndicateID { get; set; }

        /**
         * The ID of the user that has reserved or owns this ticket.
         * 0 if not owned and not reserved.
         */
        internal string ReserverOrOwnerID { get; set; }

        /**
         * The date/time that this ticket was reserved.
         * Null if not reserved.
         */
        internal DateTime? ReservedDate { get; set; }

        /**
         * The date/time that this ticket was reserved.
         * Null if not reserved.
         */
        internal DateTime? ReservedUntil { get; set; }

        /**
         * The date/time that this ticket was purchased.
         * Null if not purchased (may be reserved).
         */
        internal DateTime? PurchasedDate { get; set; }

        /**
         * The ID of the user that this ticket has been offered to as a gift.
         * Null if not offered as a gift.
         */
        internal string OfferedToID { get; set; }

        /**
         * The date/time that this ticket was offered as a gift.
         */
        internal DateTime? OfferedDate { get; set; }

        /**
         * A boolean indicating if the number was chosen by the system (true) or the user (false).
         */
        internal bool ChosenBySystem { get; set; }

        /**
         * The currency used to purchase the ticket.
         * Null if not purchased.
         */
        internal string Currency { get; set; }

        /**
         * The amount used to purchase the ticket.
         * Null if not purchased.
         */
        internal UInt32 Amount { get; set; }

    }   // Ticket

}   // BDDReferenceService.Model
