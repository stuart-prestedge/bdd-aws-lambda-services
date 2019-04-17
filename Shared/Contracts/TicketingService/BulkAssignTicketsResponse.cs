using System;

namespace BDDReferenceService.Contracts {

    public class BulkAssignTicketsResponse {
    
        /**
         * The amount, in the purchase currency, that the ticket(s) was/were purchased for.
         */
        public UInt64 amount { get; set; }

        /**
         * The currency that the purchase was made in.
         */
        public string currency { get; set; }

    }   // BulkAssignTicketsResponse

}   // BDDReferenceService.Contracts
