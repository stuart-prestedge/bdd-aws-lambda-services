using System;
namespace BDDReferenceService.Contracts
{
    public class RefreshAccessTokenResponse
    {
    
        /**
         * The expiry date/time of the access token. This is a UTC timestamp string in the form "YYYY-MM-DDTHH:MM:SSZ".
         */
        public string expiryTime { get; set; }

    }   // RefreshAccessTokenResponse

}   // BDDReferenceService.Contracts
