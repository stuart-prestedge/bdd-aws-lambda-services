using System;
namespace BDDReferenceService.Contracts {

    public class SyndicateSummaryResponse {
    
        /**
         * The ID of the syndicate record.
         */
        public string id { get; set; }

        /**
         * The name of the syndicate.
         */
        public string name { get; set; }

        /**
         * The status of the current user in this syndicate. Possible 'values' are 1 (pending) and 2 (full). Invited members that have not accepted the invitation are pending.
         */
        public int status { get; set; }

        /**
         * The number of full members in the syndicate. It is only included in the response if the includeCounts input parameter is set to true.
         */
        public int? memberCount { get; set; }

        /**
         * The number of tickets belonging to the syndicate. It is only included in the response if the includeCounts input parameter is set to true.
         */
        public int? ticketCount { get; set; }

    }   // SyndicateSummaryResponse

}   // BDDReferenceService.Contracts
