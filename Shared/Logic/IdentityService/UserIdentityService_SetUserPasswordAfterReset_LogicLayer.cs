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
    public static class UserIdentityService_SetUserPasswordAfterReset_LogicLayer
    {
        
        /**
         * Check validity of set user password after reset request inputs.
         */
        public static SetUserPasswordAfterResetRequest CheckValidSetUserPasswordAfterResetRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!String.IsNullOrEmpty((string)requestBody["resetPasswordLinkId"])) {
                    if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                        if (Helper.IsValidPassword((string)requestBody["newPassword"], true)) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "resetPasswordLinkId",
                                                                "emailAddress",
                                                                "newPassword"
                                                                }))) {
                                return new SetUserPasswordAfterResetRequest {
                                    resetPasswordLinkId = (string)requestBody["resetPasswordLinkId"],
                                    emailAddress = (string)requestBody["emailAddress"],
                                    newPassword = (string)requestBody["newPassword"]
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = IdentityServiceLogicLayer.INVALID_NEW_PASSWORD;
                        }
                    } else {
                        Debug.Untested();
                        error = APIHelper.INVALID_EMAIL_ADDRESS;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_RESET_PASSWORD_LINK_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user password after reset.
         */
        public static async Task SetUserPasswordAfterReset(AmazonDynamoDBClient dbClient, SetUserPasswordAfterResetRequest setUserPasswordAfterResetRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(setUserPasswordAfterResetRequest);
            Debug.AssertString(setUserPasswordAfterResetRequest.resetPasswordLinkId);
            Debug.AssertEmail(setUserPasswordAfterResetRequest.emailAddress);
            Debug.AssertPassword(setUserPasswordAfterResetRequest.newPassword);

            // Find the valid link
            Link link = await IdentityServiceLogicLayer.FindValidLink(dbClient, setUserPasswordAfterResetRequest.resetPasswordLinkId, IdentityServiceLogicLayer.LINK_TYPE_RESET_PASSWORD);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                // Valid link exists
                Debug.Tested();
                Debug.AssertID(link.UserID);
    
                // Load the user
                User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, link.UserID, true);
                Debug.AssertValidOrNull(user);
                if (user != null) {
                    Debug.Tested();
                    Debug.AssertEmail(user.EmailAddress);
                    if (user.EmailAddress == setUserPasswordAfterResetRequest.emailAddress) {
                        // Email address matches user's email address
                        Debug.Tested();

                        // Make changes
                        user.PasswordHash = Helper.Hash(setUserPasswordAfterResetRequest.newPassword);
                        user.Locked = false;

                        // Save the user
                        await IdentityServiceDataLayer.SaveUser(dbClient, user);

                        // Revoke link
                        link.Revoked = true;
                        //??++SAVE LINK
                    } else {
                        Debug.Tested();
                        throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS, new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS));
                    }
                } else {
                    // User does not exist - may have been closed (and possibly subsequently deleted).
                    Debug.Tested();
                    throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER, new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER));
                }
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK, new Exception(SharedLogicLayer.ERROR_INVALID_LINK));
            }
        }

    }   // UserIdentityService_SetUserPasswordAfterReset_LogicLayer

}   // BDDReferenceService.Logic
