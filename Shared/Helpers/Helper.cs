using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace BDDReferenceService
{
    internal static class Helper
    {

        /*
         * Constants.
         */
        internal static string INVALID_ID = null;

        internal static bool AllFieldsRecognized(JToken obj, List<string> fields = null) {
            Debug.Tested();
            Debug.AssertValid(obj);
            Debug.AssertValidOrNull(fields);

            bool retVal = true;
            foreach (JProperty property in obj) {
                Debug.Tested();
                Debug.AssertValid(property);
                Debug.AssertString(property.Name);
                if ((fields == null) || !fields.Contains(property.Name)) {
                    Debug.Tested();
                    retVal = false;
                    break;
                } else {
                    Debug.Tested();
                }
            }
            return retVal;
        }

        /**
         * Is the specified ID valid?
         */
        internal static bool IsValidID(string id) {
            Debug.Tested();
            Debug.AssertValidOrNull(id);

            return (!string.IsNullOrEmpty(id));
        }

        /**
         * Is the email valid?
         */
        internal static bool IsValidEmail(string email) {
            Debug.Tested();
            Debug.AssertValidOrNull(email);

            bool retVal = false;
            if (!string.IsNullOrEmpty(email)) {
                Debug.Tested();
                try {
                    var addr = new System.Net.Mail.MailAddress(email);
                    Debug.AssertValid(addr);
                    retVal = (addr.Address == email);
                } catch {
                    // Invalid email
                    Debug.Tested();
                    retVal = false;
                }
            } else {
                // Null or empty email
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Is the password valid?
         //??++CHECK FOR upper, lower, digit if strict.
         */
        internal static bool IsValidPassword(string password, bool strict)
        {
            Debug.Tested();
            Debug.AssertValidOrNull(password);

            bool retVal = false;
            if (!string.IsNullOrEmpty(password)) {
                try {
                    if (strict) {
                        retVal = (password.Length >= 6);
                    } else {
                        retVal = true;
                    }
                } catch {
                    retVal = false;
                }
            }
            return retVal;
        }

        /**
         * Is the country code valid?
         * See https://billiondollardraw.atlassian.net/wiki/spaces/TEC/pages/132317198/9.1.+Country+Codes
         */
        internal static bool IsValidCountryCode(string countryCode)
        {
            Debug.Tested();
            Debug.AssertValidOrNull(countryCode);

            bool retVal = false;
            if (!string.IsNullOrEmpty(countryCode))
            {
                try
                {
                    if ((countryCode == "lu") ||
                        (countryCode == "no") ||
                        (countryCode == "ie") ||
                        (countryCode == "ch") ||
                        (countryCode == "nl") ||
                        (countryCode == "se") ||
                        (countryCode == "is") ||
                        (countryCode == "de") ||
                        (countryCode == "at") ||
                        (countryCode == "dk") ||
                        (countryCode == "be") ||
                        (countryCode == "gb") ||
                        (countryCode == "fr") ||
                        (countryCode == "fi") ||
                        (countryCode == "mt") ||
                        (countryCode == "it") ||
                        (countryCode == "es") ||
                        (countryCode == "cy") ||
                        (countryCode == "cz") ||
                        (countryCode == "pt") ||
                        (countryCode == "pl") ||
                        (countryCode == "hu") ||
                        (countryCode == "gr") ||
                        (countryCode == "tr") ||
                        (countryCode == "hr") ||
                        (countryCode == "ro") ||
                        (countryCode == "bg") ||
                        (countryCode == "al") ||
                        (countryCode == "us") ||
                        (countryCode == "ca") ||
                        (countryCode == "mx") ||
                        (countryCode == "sg") ||
                        (countryCode == "hk") ||
                        (countryCode == "jp") ||
                        (countryCode == "kr") ||
                        (countryCode == "il") ||
                        (countryCode == "cn") ||
                        (countryCode == "in") ||
                        (countryCode == "au") ||
                        (countryCode == "nz") ||
                        (countryCode == "cl") ||
                        (countryCode == "pa") ||
                        (countryCode == "uy") ||
                        (countryCode == "ar") ||
                        (countryCode == "br") ||
                        (countryCode == "mu") ||
                        (countryCode == "za") ||
                        (countryCode == "ma") ||
                        (countryCode == "ru")) {
                        retVal = true;
                    }
                }
                catch
                {
                    retVal = false;
                }
            }
            return retVal;
        }

        /**
         * Is the language code valid?
         See https://billiondollardraw.atlassian.net/wiki/spaces/TEC/pages/47218775/9.2.+Language+Codes
         */
        internal static bool IsValidLanguageCode(string languageCode) {
            Debug.Tested();
            Debug.AssertValidOrNull(languageCode);

            bool retVal = false;
            if (!string.IsNullOrEmpty(languageCode)) {
                try {
                    if ((languageCode == "en-us") ||
                        (languageCode == "en-gb") ||
                        (languageCode == "fr") ||
                        (languageCode == "de") ||
                        (languageCode == "es") ||
                        (languageCode == "it") ||
                        (languageCode == "zh-Hant") ||
                        (languageCode == "zh-Hans") ||
                        (languageCode == "hi") ||
                        (languageCode == "ar") ||
                        (languageCode == "pt-pt") ||
                        (languageCode == "pt-br") ||
                        (languageCode == "bn") ||
                        (languageCode == "ru") ||
                        (languageCode == "ja") ||
                        (languageCode == "pa") ||
                        (languageCode == "ko") ||
                        (languageCode == "pl") ||
                        (languageCode == "nl") ||
                        (languageCode == "cs")) {
                        retVal = true;
                    }
                } catch {
                    retVal = false;
                }
            }
            return retVal;
        }

        /**
         * Is the currency code valid?
         See https://billiondollardraw.atlassian.net/wiki/spaces/TEC/pages/126451839/9.3.+Currency+Codes
         */
        internal static bool IsValidCurrencyCode(string currencyCode)
        {
            Debug.Tested();
            Debug.AssertValidOrNull(currencyCode);

            bool retVal = false;
            if (!string.IsNullOrEmpty(currencyCode))
            {
                try
                {
                    if ((currencyCode == "EUR") ||
                        (currencyCode == "USD") ||
                        (currencyCode == "GBP") ||
                        (currencyCode == "NOK") ||
                        (currencyCode == "CHF") ||
                        (currencyCode == "CHE") ||
                        (currencyCode == "CHW") ||
                        (currencyCode == "SEK") ||
                        (currencyCode == "ISK") ||
                        (currencyCode == "DKK") ||
                        (currencyCode == "CZK") ||
                        (currencyCode == "PLN") ||
                        (currencyCode == "HUF") ||
                        (currencyCode == "TRY") ||
                        (currencyCode == "HRK") ||
                        (currencyCode == "RON") ||
                        (currencyCode == "BGN") ||
                        (currencyCode == "ALL") ||
                        (currencyCode == "CAD") ||
                        (currencyCode == "MXN") ||
                        (currencyCode == "MXV") ||
                        (currencyCode == "SGD") ||
                        (currencyCode == "HKD") ||
                        (currencyCode == "JPY") ||
                        (currencyCode == "KRW") ||
                        (currencyCode == "ILS") ||
                        (currencyCode == "CNY") ||
                        (currencyCode == "INR") ||
                        (currencyCode == "AUD") ||
                        (currencyCode == "NZD") ||
                        (currencyCode == "CLP") ||
                        (currencyCode == "CLF") ||
                        (currencyCode == "PAB") ||
                        (currencyCode == "UYU") ||
                        (currencyCode == "UYI") ||
                        (currencyCode == "ARS") ||
                        (currencyCode == "BRL") ||
                        (currencyCode == "MUR") ||
                        (currencyCode == "ZAR") ||
                        (currencyCode == "MAD") ||
                        (currencyCode == "RUB")) {
                        retVal = true;
                    }
                }
                catch
                {
                    retVal = false;
                }
            }
            return retVal;
        }

        /**
         * Is the time zone code valid?
         See https://billiondollardraw.atlassian.net/wiki/spaces/TEC/pages/417497098/9.4.+Time+Zones
         */
        internal static bool IsValidTimeZone(string timeZone)
        {
            Debug.Tested();
            Debug.AssertValidOrNull(timeZone);

            bool retVal = false;
            if (!string.IsNullOrEmpty(timeZone))
            {
                try
                {
                    if ((timeZone == "gb")) {
                            //??++OTHERS
                        retVal = true;
                    }
                }
                catch
                {
                    retVal = false;
                }
            }
            return retVal;
        }

        /**
         * One-way hash of the input string.
         */
        internal static string Hash(string input)
        {
            return "HASH(" + input + ")";
        }

    }   // Helper

}   // BDDReferenceService
