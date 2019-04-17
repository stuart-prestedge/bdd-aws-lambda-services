using System;
using System.Collections.Generic;
using BDDReferenceService.Logic;

namespace BDDReferenceService
{
    
    internal static class EmailHelper {

        /**
         * Email templates.
         * Should eventually match https://billiondollardraw.atlassian.net/wiki/spaces/TEC/pages/517308460/Platform+Email+Templates
         */
        internal const string EMAIL_TEMPLATE_CREATE_ACCOUNT = "create-account";
        internal const string EMAIL_TEMPLATE_ACCOUNT_REOPENED = "account-re-opened";
        internal const string EMAIL_TEMPLATE_EMAIL_VERIFICATION = "EMAIL_VERIFICATION";
        internal const string EMAIL_TEMPLATE_INVITE_SYNDICATE_MEMBER = "INVITE_SYNDICATE_MEMBER";

        /**
         * Email a template to an email address.
         */
        internal static bool EmailTemplate(string template, string emailAddress, Dictionary<string, string> replacementFields) {
            Debug.Tested();

            LoggingHelper.LogMessage($"EMAIL TEMPLATE: '{template}' TO '{emailAddress}'");

            return true;
        }

    }   // EmailHelper

}   // BDDReferenceService
