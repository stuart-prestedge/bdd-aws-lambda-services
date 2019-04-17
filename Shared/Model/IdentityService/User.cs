using System;
using System.Collections.Generic;

namespace BDDReferenceService.Model
{

    /**
     * Class representing a user.
     */
    public class User {
        
        /**
         * The unique identifier of the user record.
         */
        public string ID { get; set; }

        /**
         * An optional string (can be empty or null) that identifies the client used to create the account.
         */
        public string ClientID { get; set; }

        /**
         * A string containing the given (first) name of the user.
         */
        public string GivenName { get; set; }

        /**
         * A string containing the family name (surname) of the user.
         */
        public string FamilyName { get; set; }

        /**
         * A string containing the user's preferred name.
         */
        public string PreferredName { get; set; }

        /**
         * A string containing the user's full name.
         */
        public string FullName { get; set; }

        /**
         * A string containing the user's email address.
         */
        public string EmailAddress { get; set; }

        /**
         * A timestamp containing the date/time that the user's email address is verified (null if not verified).
         */
        public DateTime? EmailAddressVerified { get; set; }

        /**
         * Used when changing a user's email address to store the new address before it is verified.
         */
        public string NewEmailAddress { get; set; }

        /**
         * The hash of the user's password.
         */
        public string PasswordHash { get; set; }

        /**
         * A flag indicating if a user is blocked. Blocked users cannot login or revalidate their authentication token. If the user is blocked and they try to do this, the appropriate error is returned.
         */
        public bool Blocked { get; set; }

        /**
         * A flag indicating if a user is locked. Locked users cannot login or revalidate their authentication token. If the user is locked and they try to do this, the appropriate error is returned.
         */
        public bool Locked { get; set; }

        /**
         * A date that represents the user's date of birth. Has no time component.
         */
        public DateTime? DateOfBirth { get; set; }

        /**
         * A number representing gender. 0=unset, 1=male, 2=female, 3=other/unspecified
         */
        public UInt16 Gender { get; set; }

        /**
         * .
         */
        public string Address1 { get; set; }

        /**
         * .
         */
        public string Address2 { get; set; }

        /**
         * .
         */
        public string Address3 { get; set; }

        /**
         * .
         */
        public string Address4 { get; set; }

        /**
         * .
         */
        public string City { get; set; }

        /**
         * .
         */
        public string Region { get; set; }

        /**
         * A string with the users country code (REF).
         */
        public string Country { get; set; }

        /**
         * .
         */
        public string PostalCode { get; set; }

        /**
         * .
         */
        public string PhoneNumber { get; set; }

        /**
         * .
         */
        public DateTime? PhoneNumberVerified { get; set; }

        /**
         * A timestamp containing the date/time when the user last logged in. Null if never logged in.
         */
        public DateTime? LastLoggedIn { get; set; }

        /**
         * A timestamp containing the date/time that the user last logged out. Null if never logged out.
         */
        public DateTime? LastLoggedOut { get; set; }

        /**
         * A timestamp containing the date/time that the account was closed.
         */
        public DateTime? Closed { get; set; }

        /**
         * A flag indicating if the closed account has been anonymized yet.
         */
        public bool IsAnonymised { get; set; }

        /**
         * A timestamp containing the date/time that this record was soft deleted. Null if not deleted.
         */
        public DateTime? Deleted { get; set; }

        /**
         * A flag indicating if non-essential notifications (emails/SMS) can be sent to this user.
         */
        public bool AllowNonEssentialEmails { get; set; }

        /**
         * A timestamp containing the date/time that AllowNonEssentialEmails was last turned on. Null if never set to on.
         */
        public DateTime? ANEEOnTimestamp { get; set; }

        /**
         * A timestamp containing the date/time that AllowNonEssentialEmails was last turned off. Null if never set to off.
         */
        public DateTime? ANEEOffTimestamp { get; set; }

        /**
         * The total number of tickets this user has purchased.
         */
        public UInt32 TotalTicketsPurchased { get; set; }

        /**
         * The number of tickets the current user has purchased in the current game.
         */
        public UInt32 TicketsPurchasedInCurrentGame { get; set; }

        /**
         * The preferred language (see Language Codes). Null if not specified.
         */
        public string PreferredLanguage { get; set; }

        /**
         * The preferred currency (see Currency Codes). Null if not specified.
         */
        public string PreferredCurrency { get; set; }

        /**
         * The preferred time zone (see Time Zones). Null if not specified.
         */
        public string PreferredTimeZone { get; set; }

        /**
         * A number indicating how many consecutive failed attempts have occurred.
         */
        public UInt16 FailedLoginAttempts { get; set; }

        /**
         * The result of a KYC operation for this user (type TBD). Null if not performed.
         */
        public string KYCStatus { get; set; }

        /**
         * A timestamp of when the KYC was performed. Null if not performed.
         */
        public DateTime? KCYTimestamp { get; set; }

        /**
         * The maximum amount, in US dollars, that the user is allowed to spend in any 24 hour period. Null if there is no limit.
         */
        public UInt32? MaxDailySpendingAmount { get; set; }

        /**
         * The new maximum amount, in US dollars, that the user is allowed to spend in any 24 hour period. This is set if the user increases their limit. The new limit only takes effect 7 days after the request.
         */
        public UInt32? NewMaxDailySpendingAmount { get; set; }

        /**
         * The timestamp of the date/time when the NewMaxDailySpendingAmount will take effect.
         */
        public DateTime? NewMaxDailySpendingAmountTime { get; set; }

        /**
         * The number of minutes a user remains logged in before they are automatically logged out. The logout is done by revoking the user's access token.
         */
        public UInt64? MaxTimeLoggedIn { get; set; }

        /**
         * The new number of minutes a user remains logged in before they are automatically logged out. This is set if the user increases the time they can remain logged in. The new limit only takes effect 7 days after the request.
         */
        public UInt64? NewMaxTimeLoggedIn { get; set; }

        /**
         * The timestamp of the date/time when the NewMaxTimeLoggedIn will take effect.
         */
        public DateTime? NewMaxTimeLoggedInTime { get; set; }

        /**
         * The timestamp of the date/time that a user will be excluded until. Null if not excluded. MAX-TIMESTAMP-VALUE if excluded indefinitely.
         */
        public DateTime? ExcludeUntil { get; set; }

        /**
         * The new timestamp of the date/time that a user will be excluded for. This is set if the user reduces their exclusion time. The new exclusion time only takes effect 7 days after the request.
         */
        public DateTime? NewExcludeUntil { get; set; }

        /**
         * The timestamp of the date/time when the NewExcludeUntil will take effect
         */
        public DateTime? NewExcludeUntilTime { get; set; }

        /**
         * The permissions of the user.
         */
        public List<string> Permissions { get; set; }

    }   // User

}   // BDDReferenceService.Model
