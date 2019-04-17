using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing an access token.
     */
    internal class Link
    {
        
        /**
         * The unique identifier of the link.
         */
        internal string ID { get; set; }

        /**
         * The type of the link.
         */
        internal string Type { get; set; }

        /**
         * The link expiry date/time.
         */
        internal DateTime? Expires { get; set; }

        /**
         * The ID of the user that the link is associated with.
         */
        internal string UserID { get; set; }

        /**
         * Is this link revoked (or used in the case of single use links)?
         */
        internal bool Revoked { get; set; }

        /**
         * A one-time-password used when SMSing a phone number verification message.
         */
        internal string OneTimePassword { get; set; }

    }   // Link

}   // BDDReferenceService.Model
