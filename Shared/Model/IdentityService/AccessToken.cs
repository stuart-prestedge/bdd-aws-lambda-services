using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing an access token.
     */
    internal class AccessToken
    {
        
        /**
         * The unique identifier of the access token.
         */
        internal string ID { get; set; }

        /**
         * The ID of the user this access token is for.
         */
        //??--internal User User { get; set; }
        internal string UserID { get; set; }

        /**
         * The access token expiry date/time.
         */
        internal DateTime? Expires { get; set; }

        /**
         * The maximum access token expiry date/time.
         */
        internal DateTime? MaxExpiry { get; set; }

    }   // AccessToken

}   // BDDReferenceService.Model
