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
    public static class UserIdentityService_SetUserAllowNonEssentialEmails_LogicLayer
    {
        
        /**
         * Check validity of set user allow non-essential emails request inputs.
         */
        internal static SetUserAllowNonEssentialEmailsRequest CheckValidSetUserAllowNonEssentialEmailsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                bool ?allowNonEssentialEmails = null;
                if (APIHelper.RequestBodyContainsField(requestBody, "allowNonEssentialEmails", out JToken allowNonEssentialEmailsField)) {
                    if (allowNonEssentialEmailsField.Type == JTokenType.Boolean) {
                        allowNonEssentialEmails = (bool)allowNonEssentialEmailsField;
                    } else if (allowNonEssentialEmailsField.Type == JTokenType.String) {
                        if (bool.TryParse((string)allowNonEssentialEmailsField, out bool allowNonEssentialEmails_)) {
                            allowNonEssentialEmails = allowNonEssentialEmails_;
                        }
                    }
                } else {
                    allowNonEssentialEmails = false;
                }
                if (allowNonEssentialEmails != null) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "allowNonEssentialEmails"
                                                        }))) {
                        return new SetUserAllowNonEssentialEmailsRequest {
                            allowNonEssentialEmails = (bool)allowNonEssentialEmails
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_ALLOW_NON_ESSENTIAL_EMAILS;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user allow non-essential emails.
         */
        internal static async Task SetUserAllowNonEssentialEmails(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserAllowNonEssentialEmailsRequest setUserAllowNonEssentialEmailsRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserAllowNonEssentialEmailsRequest);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if (user.AllowNonEssentialEmails != setUserAllowNonEssentialEmailsRequest.allowNonEssentialEmails)
            {
                user.AllowNonEssentialEmails = setUserAllowNonEssentialEmailsRequest.allowNonEssentialEmails;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

    }   // UserIdentityService_SetUserAllowNonEssentialEmails_LogicLayer

}   // BDDReferenceService.Logic
