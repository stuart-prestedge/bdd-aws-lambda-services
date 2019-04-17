using System;
namespace BDDReferenceService.Contracts
{
    public class SetUserAddressRequest
    {
    
        /**
         * A mandatory string.
         */
        public string address1 { get; set; }

        /**
         * An optional string.
         */
        public string address2 { get; set; }

        /**
         * An optional string.
         */
        public string address3 { get; set; }

        /**
         * An optional string.
         */
        public string address4 { get; set; }

        /**
         * An optional string.
         */
        public string city { get; set; }

        /**
         * An optional string.
         */
        public string region { get; set; }

        /**
         * A mandatory string containing a valid country code (see 9.1. Country Codes).
         */
        public string country { get; set; }

        /**
         * A mandatory string containing a valid postal code. For now, valid simply means not empty but this may change in the future.
         */
        public string postalCode { get; set; }

    }   // SetUserAddressRequest

}   // BDDReferenceService.Contracts
