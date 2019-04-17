using System;
namespace BDDReferenceService.Contracts
{
    public class GetAllTicketAuditsRequest
    {
    
        /**
         * The start time on or after which audits are retrieved.
         */
        public string from { get; set; }

        /**
         * The after time before which audits are retrieved.
         */
        public string to { get; set; }

    }   // GetAllTicketAuditsRequest

}   // BDDReferenceService.Contracts
