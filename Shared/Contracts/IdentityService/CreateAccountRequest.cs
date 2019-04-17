using System;
namespace BDDReferenceService.Contracts
{
    public class CreateAccountRequest
    {
    
        /**
         * An optional string (is not required but must be non-empty if specified) that indicates the client used to create the account.
         */
        public string clientId { get; set; }

        /**
         * A mandatory string (must exist and must not be null or empty) with the user's given name.
         */
        public string givenName { get; set; }

        /**
         * A mandatory string with the user's family name.
         */
        public string familyName { get; set; }

        /**
         * A mandatory string with the user's email address. It must be a valid email address. It must be unique (i.e. not already used in another account, either validated, not validated or new (when a user is in the process of changing their email address).
         */
        public string emailAddress { get; set; }

        /**
         * A mandatory string with the user's password. It must be a strong password. Exact requirements TBD but for now at least 6 characters, at least one lower case letter, at least one upper case letter and at least one digit. It need not be unique.
         */
        public string password { get; set; }

        /**
         * A mandatory string in the form "YYYY-MM-DD". It must not be in the future. The age of the new user will be checked against the age rules (at least 18+ globally. 21+ in Estonia). The minimum age for each country is obtained from the Countries database table.
         */
        public string dateOfBirth { get; set; }

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

        /**
         * An optional boolean indicating if the user allows marketing emails. The default value is false.
         */
        public bool allowNonEssentialEmails { get; set; }

    }   // CreateAccountRequest

}   // BDDReferenceService.Contracts
