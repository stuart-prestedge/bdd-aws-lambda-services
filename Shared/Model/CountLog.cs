using System;
using System.Collections.Generic;

namespace BDDReferenceService.Model
{
    
    internal class CountLog
    {

        /**
         * The ID of the count log.
         */
        internal string ID { get; set; }

        /**
         * The date/time that the count log was created.
         */
        internal DateTime Created { get; set; }

        /**
         * The ticket service instance identifier.
         */
        internal string ServiceInstance { get; set; }

        /**
         * Purchases count.
         */
        internal UInt64 PurchaseCount { get; set; }

        /**
         * Reservations count.
         */
        internal UInt64 ReservationCount { get; set; }

        /**
         * Unreservations count.
         */
        internal UInt64 UnreservationCount { get; set; }

        /**
         * Offers count.
         */
        internal UInt64 OfferCount { get; set; }

        /**
         * Revokes count.
         */
        internal UInt64 RevokeCount { get; set; }

        /**
         * Accepts count.
         */
        internal UInt64 AcceptCount { get; set; }

        /**
         * Rejects count.
         */
        internal UInt64 RejectCount { get; set; }

    }   // CountLog

}   // BDDReferenceService.Model
