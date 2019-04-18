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
    public static class UserIdentityService_ResetUserPassword_LogicLayer
    {
        
        /**
         * Check validity of reset user password request inputs.
         */
        internal static ResetUserPasswordRequest CheckValidResetUserPasswordRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                    if (APIHelper.GetStringFromRequestBody(requestBody, "reCaptcha", out string reCaptcha, true)) {
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "emailAddress"
                                                            }))) {
                            return new ResetUserPasswordRequest {
                                emailAddress = (string)requestBody["emailAddress"]
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = APIHelper.INVALID_RECAPTCHA;
                    }
                } else {
                    Debug.Untested();
                    error = APIHelper.INVALID_EMAIL_ADDRESS;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Reset user password.
         */
        internal static async Task ResetUserPassword(AmazonDynamoDBClient dbClient, ResetUserPasswordRequest resetUserPasswordRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(resetUserPasswordRequest);
            Debug.AssertString(resetUserPasswordRequest.emailAddress);

            // Load the user
            User user = await IdentityServiceDataLayer.FindUserByEmailAddress(dbClient, resetUserPasswordRequest.emailAddress);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                // User with email address exists
                Debug.Tested();
                Debug.AssertID(user.ID);
                if (user.EmailAddressVerified != null)
                {
                    Debug.Tested();
                    await DoResetUserPassword(dbClient, user);
                }
                else
                {
                    //??--bool failForgotPasswordIfEmailNotVerified = (bool)GetIdentityGlobalSetting(GLOBAL_SETTING_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED, DEFAULT_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED);
                    bool failForgotPasswordIfEmailNotVerified = await IdentityServiceLogicLayer.GetBoolIdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED, IdentityServiceLogicLayer.DEFAULT_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED);
                    if (!failForgotPasswordIfEmailNotVerified)
                    {
                        Debug.Tested();
                        await DoResetUserPassword(dbClient, user);
                    }
                    else
                    {
                        Debug.Tested();
                        throw new Exception(IdentityServiceLogicLayer.ERROR_EMAIL_NOT_VERIFIED);
                    }
                }
            } else {
                // User with email address not found
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS);
            }
        }

        /**
         * Reset user password.
         */
        private static async Task DoResetUserPassword(AmazonDynamoDBClient dbClient, User user) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(user);

            // Email the reset password link
            Link link = await IdentityServiceLogicLayer.CreateLink(dbClient, IdentityServiceLogicLayer.LINK_TYPE_RESET_PASSWORD, user.ID);
            Debug.AssertValid(link);
            Debug.AssertString(link.ID);
            //??++EMAIL LINK ID TO ADDRESS

            // Debug code
            //??--IdentityServiceLogicLayer.ResetPasswordLinkId = link.ID;
            LoggingHelper.LogMessage($"PASSWORD RESET LINK ID: {link.ID}");
        }

    }   // UserIdentityService_ResetUserPassword_LogicLayer

}   // BDDReferenceService.Logic
