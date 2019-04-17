using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a syndicate.
     */
    internal class SyndicateMember
    {
        
        /**
         * The syndicate member status.
         */
        public enum SyndicateMemberStatus
        {
            unknown = 0,
            pending = 1,
            full = 2
        }

        /**
         * The ID of the syndicate member record.
         */
        public string ID { get; set; }

        /**
         * The ID of the syndicate record.
         */
        public string SyndicateID { get; set; }

        /**
         * The ID of the member user ID.
         */
        public string UserID { get; set; }

        /**
         * Possible 'values' are pending and full. Invited members that have not accepted the invitation are pending.
         */
        public SyndicateMemberStatus Status { get; set; }

        /**
         * The optional message used when the member is invited. This is null for full members
         */
        public string Message { get; set; }

    }   // SyndicateMember

}   // BDDReferenceService.Model
