using System;
namespace BDDReferenceService.Contracts
{
    public class SetUserPreferencesRequest
    {
    
        /**
         * This is the language code (9.2. Language Codes) of the user's preferred language. If missing, the user record's field is not changed. If specified (even if null) then the PreferredLanguage field is set appropriately.
         */
        public string preferredLanguage { get; set; }

        /**
         * This is the currency code (9.3. Currency Codes) of the user's preferred currency. If missing, the user record's field is not changed. If specified (even if null) then the PreferredCurrency field is set appropriately.
         */
        public string preferredCurrency { get; set; }

        /**
         * This is the time zone (see 9.4. Time Zones) of the user's preferred time zone. If missing, the user record's field is not changed. If specified (even if null) then the PreferredTimeZone field is set appropriately.
         */
        public string preferredTimeZone { get; set; }

    }   // SetUserPreferencesRequest

}   // BDDReferenceService.Contracts
