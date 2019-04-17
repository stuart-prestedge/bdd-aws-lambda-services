using System;
namespace BDDReferenceService.Contracts
{
    public class CreateAccountResponse
    {
    
        /**
         * An optional string (is not required but must be non-empty if specified) that indicates the client used to create the account.
         */
        public ErrorResponse[] error { get; set; }

    }   // CreateAccountResponse

    public class ErrorResponse
    {
    
        /**
         * This is a well known error code.
         */
        public UInt16 code { get; set; }

        /**
         * English description in case error code not recognized.
         */
        public string message { get; set; }

    }   // ErrorResponse

}   // BDDReferenceService.Contracts
