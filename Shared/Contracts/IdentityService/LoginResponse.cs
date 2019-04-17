using System;
namespace BDDReferenceService.Contracts
{
    public class LoginResponse
    {
    
        /**
         * An optional string that contains more information about the error if login was unsuccessful. Valid values are 'ACCOUNT_CLOSED', 'USER_BLOCKED', 'USER_LOCKED' and 'USER_NOT_FOUND'.
         */
        public string error { get; set; }

        /**
         * An optional string that contains an access token if login was successful. This is an opaque string, meaningless to the client but used for authorization of authenticated endpoints.
         */
        public string accessToken { get; set; }

        /**
         * The expiry date/time of the access token. This is a UTC timestamp string in the form "YYYY-MM-DDTHH:MM:SSZ".
         */
        public string expiryTime { get; set; }

    }   // LoginResponse

}   // BDDReferenceService.Contracts
