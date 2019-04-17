using System;
namespace BDDReferenceService.Contracts {

    public class SyndicateDetailsResponse : SyndicateSummaryResponse {
    
        /**
         * The member details.
         */
        public SyndicateMemberDetails[] members { get; set; }

        /**
         * The pending member details.
         */
        public SyndicateMemberDetails[] pendingMembers { get; set; }

        /**
         * The IDs of the tickets.
         */
        public string[] tickets { get; set; }

    }   // SyndicateDetailsResponse

    public class SyndicateMemberDetails {

        /**
         * The ID of the user record.
         */
        public string userId { get; set; }

        /**
         * The display name of the members.
         */
        public string name { get; set; }

        /**
         * The email address of the members.
         */
        public string emailAddress { get; set; }

    }

}   // BDDReferenceService.Contracts
