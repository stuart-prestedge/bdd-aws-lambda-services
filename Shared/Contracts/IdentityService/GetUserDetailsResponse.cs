using System;
namespace BDDReferenceService.Contracts
{
    public class GetUserDetailsResponse
    {
    
        /**
         * A mandatory string with the user's email address. It must be a valid email address. It must be unique (i.e. not already used in another account, either validated, not validated or new (when a user is in the process of changing their email address).
         */
        public string emailAddress { get; set; }

        /**
         * A mandatory string (must exist and must not be null or empty) with the user's given name.
         */
        public string givenName { get; set; }

        /**
         * A mandatory string with the user's family name.
         */
        public string familyName { get; set; }

        /**
         * The optional preferred name.
         */
        public string preferredName { get; set; }

        /**
         * The optional full name.
         */
        public string fullName { get; set; }

        /**
         * A mandatory string in the form "YYYY-MM-DD". It must not be in the future. The age of the new user will be checked against the age rules (at least 18+ globally. 21+ in Estonia). The minimum age for each country is obtained from the Countries database table.
         */
        public string dateOfBirth { get; set; }

        /**
         * The user's gender.
         */
        public UInt16 gender { get; set; }

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

        public string phoneNumber { get; set; }
        public string phoneNumberVerified { get; set; }
        public string newEmailAddress { get; set; }
        public bool allowNonEssentialEmails { get; set; }
        public UInt64 totalTicketsPurchased { get; set; }
        public UInt64 ticketsPurchasedInCurrentGame { get; set; }
        public string preferredLanguage { get; set; }
        public string preferredCurrency { get; set; }
        public string preferredTimeZone { get; set; }
        public UInt64? maxDailySpendingAmount { get; set; }
        public UInt64? newMaxDailySpendingAmount { get; set; }
        public string newMaxDailySpendingAmountTime { get; set; }
        public UInt64? maxTimeLoggedIn { get; set; }
        public UInt64? newMaxTimeLoggedIn { get; set; }
        public string newMaxTimeLoggedInTime { get; set; }
        public string excludeUntil { get; set; }
        public string newExcludeUntil { get; set; }
        public string newExcludeUntilTime { get; set; }

    }   // GetUserDetailsResponse

}   // BDDReferenceService.Contracts
