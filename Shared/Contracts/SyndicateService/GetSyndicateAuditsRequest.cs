using System;
namespace BDDReferenceService.Contracts
{
    public class GetSyndicateAuditsRequest
    {
    
        /**
         * The from date/time.
         */
        public string from { get; set; }

        /**
         * The to date/time.
         */
        public string to { get; set; }

        /**
         * This is an optional syndicate ID.
         */
        public string syndicateId { get; set; }

    }   // GetSyndicateAuditsRequest

}   // BDDReferenceService.Contracts
