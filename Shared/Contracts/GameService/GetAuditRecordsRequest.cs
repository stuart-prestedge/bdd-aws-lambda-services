using System;
namespace BDDReferenceService.Contracts
{
    public class GetAuditRecordsRequest
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
         * This is an optional number (1=game, 2=draw) that allows filtering of audit records by data type.
         */
        public Int32? dataType { get; set; }

    }   // GetAuditRecordsRequest

}   // BDDReferenceService.Contracts
