using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a ticket price audit.
     */
    internal class TicketPriceAuditRecord
    {

        /**
         * The audit change type.
         */
        internal enum AuditChangeType
        {
            create = 1,
            update = 2
        }

        /**
         * The ID of the ticket price audit record.
         */
        internal string ID { get; set; }

        /**
         * The date/time this audit record was created.
         */
        internal DateTime Timestamp { get; set; } = DateTime.Now;

        /**
         * The ID of the administrator who made the change.
         */
        internal string AdministratorID { get; set; }

        /**
         * The type of ticket price audit record.
         */
        internal AuditChangeType ChangeType { get; set; }

        /**
         * The currency code.
         */
        public string Currency { get; set; }

        /**
         * The ticket price in the currency.
         */
        public UInt64 Price { get; set; }

        /**
         * Are tickets available to purchase in this currency?
         */
        public bool Available { get; set; }

    }   // TicketPriceAuditRecord

}   // BDDReferenceService.Model
