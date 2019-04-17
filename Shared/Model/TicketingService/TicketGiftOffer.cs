using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a ticket.
     */
    internal class TicketGiftOffer
    {
        
        /**
         * The ID of the ticket gift offer record.
         */
        internal string ID { get; set; }

        /**
         * The date/time that this ticket was offered.
         * Null if not offered.
         */
        internal DateTime Created { get; set; }

        /**
         * The ID of the ticket record that is being gifted.
         */
        internal string TicketID { get; set; }

        /**
         * The email of the offered to person.
         */
        internal string OfferedToEmail { get; set; }

        /**
         * The offer message.
         */
        internal string OfferMessage { get; set; }

    }   // TicketGiftOffer

}   // BDDReferenceService.Model
