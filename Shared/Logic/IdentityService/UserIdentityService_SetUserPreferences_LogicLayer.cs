using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BDDReferenceService.Contracts;
using BDDReferenceService.Data;
using BDDReferenceService.Model;
using Newtonsoft.Json.Linq;

namespace BDDReferenceService.Logic
{

    /**
     * Logic layer with helper methods.
     */
    public static class UserIdentityService_SetUserPreferences_LogicLayer
    {
        
        /**
         * Check validity of set user preferences request inputs.
         */
        public static SetUserPreferencesRequest CheckValidSetUserPreferencesRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!APIHelper.RequestBodyContainsField(requestBody, "preferredLanguage", out JToken preferredLanguageField) || (preferredLanguageField.Type == JTokenType.Null) || Helper.IsValidLanguageCode((string)preferredLanguageField)) {
                    if (!APIHelper.RequestBodyContainsField(requestBody, "preferredCurrency", out JToken preferredCurrencyField) || (preferredCurrencyField.Type == JTokenType.Null) || Helper.IsValidCurrencyCode((string)preferredCurrencyField)) {
                        if (!APIHelper.RequestBodyContainsField(requestBody, "preferredTimeZone", out JToken preferredTimeZoneField) || (preferredTimeZoneField.Type == JTokenType.Null) || Helper.IsValidTimeZone((string)preferredTimeZoneField)) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "preferredLanguage",
                                                                "preferredCurrency",
                                                                "preferredTimeZone"
                                                                }))) {
                                return new SetUserPreferencesRequest {
                                    preferredLanguage = (string)preferredLanguageField,
                                    preferredCurrency = (string)preferredCurrencyField,
                                    preferredTimeZone = (string)preferredTimeZoneField
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = IdentityServiceLogicLayer.INVALID_PREFERRED_TIME_ZONE;
                        }
                    } else {
                        Debug.Untested();
                        error = IdentityServiceLogicLayer.INVALID_PREFERRED_CURRENCY;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_PREFERRED_LANGUAGE;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user preferences.
         */
        public static async Task SetUserPreferences(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserPreferencesRequest setUserPreferencesRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserPreferencesRequest);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if ((user.PreferredLanguage != setUserPreferencesRequest.preferredLanguage) ||
                (user.PreferredCurrency != setUserPreferencesRequest.preferredCurrency) ||
                (user.PreferredTimeZone != setUserPreferencesRequest.preferredTimeZone))
            {
                user.PreferredLanguage = setUserPreferencesRequest.preferredLanguage;
                user.PreferredCurrency = setUserPreferencesRequest.preferredCurrency;
                user.PreferredTimeZone = setUserPreferencesRequest.preferredTimeZone;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

    }   // UserIdentityService_SetUserPreferences_LogicLayer

}   // BDDReferenceService.Logic
