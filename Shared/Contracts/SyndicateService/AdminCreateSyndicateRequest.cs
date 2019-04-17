using System;
namespace BDDReferenceService.Contracts
{
    public class AdminCreateSyndicateRequest
    {
    
        /**
         * The syndicate name.
         */
        public string name { get; set; }

        /**
         * The syndicate members.
         */
        public string[] userIds { get; set; }

    }   // AdminCreateSyndicateRequest

}   // BDDReferenceService.Contracts
