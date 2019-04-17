using System;
namespace BDDReferenceService.Contracts
{
    public class GetLogsRequest
    {
    
        /**
         * The ID of the user who's audit logs are being retrieved. Current user is inferred if missing.
         * Can get audit logs for another user if logged in as an administrator with the correct permission.
         */
        //???public string userId { get; set; }

        /**
         * The start time on or after which audits are retrieved.
         */
        public string from { get; set; }

        /**
         * The after time before which audits are retrieved.
         */
        public string to { get; set; }

    }   // GetLogsRequest

}   // BDDReferenceService.Contracts
