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
    public static class UserIdentityService_SetUserPassword_LogicLayer
    {
        
        /**
         * Check validity of set user password request inputs.
         */
        internal static SetUserPasswordRequest CheckValidSetUserPasswordRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidPassword((string)requestBody["oldPassword"], false)) {
                    if (Helper.IsValidPassword((string)requestBody["newPassword"], true)) {//??++WHAT TO DO IF PASSWORDS SAME?
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "oldPassword",
                                                            "newPassword"
                                                            }))) {
                            return new SetUserPasswordRequest {
                                oldPassword = (string)requestBody["oldPassword"],
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
                    error = IdentityServiceLogicLayer.INVALID_OLD_PASSWORD;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user password.
         */
        internal static async Task SetUserPassword(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserPasswordRequest setUserPasswordRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserPasswordRequest);
            Debug.AssertString(setUserPasswordRequest.oldPassword);
            Debug.AssertString(setUserPasswordRequest.newPassword);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Check password
            if (user.PasswordHash == Helper.Hash(setUserPasswordRequest.oldPassword)) {

                // Make changes (if necessary)
                string newPasswordHash = Helper.Hash(setUserPasswordRequest.newPassword);
                if (user.PasswordHash != newPasswordHash)
                {
                    user.PasswordHash = newPasswordHash;

                    // Save the user
                    await IdentityServiceDataLayer.SaveUser(dbClient, user);
                }
            } else {
                throw new Exception(IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD);
            }
        }

    }   // UserIdentityService_SetUserPassword_LogicLayer

}   // BDDReferenceService.Logic
