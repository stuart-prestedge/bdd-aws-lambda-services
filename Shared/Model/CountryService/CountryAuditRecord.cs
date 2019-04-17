using System;
using System.Collections.Generic;

namespace BDDReferenceService.Model
{

    /**
     * Class representing a country audit.
     */
    internal class CountryAuditRecord
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
         * The ID of the country audit record.
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
         * The type of country audit record.
         */
        internal AuditChangeType ChangeType { get; set; }

        /**
         * The country code.
         */
        public string Code { get; set; }

        /**
         * The name.
         */
        public string Name { get; set; }

        /**
         * The country in the currency.
         */
        public List<string> Currencies { get; set; }

        /**
         * Is the country available?
         */
        public bool Available { get; set; }

    }   // CountryAuditRecord

}   // BDDReferenceService.Model
