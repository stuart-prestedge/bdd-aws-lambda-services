using System;
namespace BDDReferenceService.Contracts
{
    public class SetUserLimitsRequest
    {
    
        /**
         * This is ...
         */
        public UInt32? maxDailySpendingAmount { get; set; }

        /**
         * This is ...
         */
        public UInt64? maxTimeLoggedIn { get; set; }

        /**
         * This is ...
         */
        public string excludeUntil { get; set; }

    }   // SetUserLimitsRequest

}   // BDDReferenceService.Contracts
