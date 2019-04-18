using System;
using System.Collections.Generic;
using BDDReferenceService.Logic;

namespace BDDReferenceService
{
    
    public static class ReCaptchaHelper {

        /*
         * Errors
         */
        public const string ERROR_INVALID_RECAPTCHA_TOKEN = "ERROR_INVALID_RECAPTCHA_TOKEN";

        /**
         * Check reCaptcha token.
         * Will eventually be asynchronous.
         */
        public static void CheckReCaptchaToken(string reCaptchaToken) {
            Debug.Untested();

            if (string.IsNullOrEmpty(reCaptchaToken)) {
                Debug.Untested();
                throw new Exception(ERROR_INVALID_RECAPTCHA_TOKEN);
            }
        }

    }   // ReCaptchaHelper

}   // BDDReferenceService
