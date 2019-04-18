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
    public static class UserIdentityService_CheckPassword_LogicLayer
    {
        
        /**
         * Check validity of check password request inputs.
         */
        public static CheckPasswordRequest CheckValidCheckPasswordRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidPassword((string)requestBody["password"], false)) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "password"
                                                        }))) {
                        return new CheckPasswordRequest {
                            password = (string)requestBody["password"]
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                    error = APIHelper.INVALID_PASSWORD;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Check password.
         */
        public static async Task CheckPassword(AmazonDynamoDBClient dbClient, CheckPasswordRequest checkPasswordRequest, string loggedInUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(checkPasswordRequest);
            Debug.AssertString(checkPasswordRequest.password);
            Debug.AssertID(loggedInUserId);

            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);
            if (user.Locked) {
                // The user is locked.
                // This code should never be called as locked users cannot login.
                Debug.Unreachable();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_LOCKED);
            } else if (user.Blocked) {
                // The user is blocked.
                // This code should never be called as blocked users cannot login.
                Debug.Unreachable();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_BLOCKED);
            } else if (user.PasswordHash != Helper.Hash(checkPasswordRequest.password)) {
                Debug.Tested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD);
            } else {
                Debug.Tested();
            }
        }

    }   // UserIdentityService_CheckPassword_LogicLayer

}   // BDDReferenceService.Logic
