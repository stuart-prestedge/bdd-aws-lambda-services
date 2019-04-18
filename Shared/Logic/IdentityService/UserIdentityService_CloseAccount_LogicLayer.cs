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
    public static class UserIdentityService_CloseAccount_LogicLayer
    {
        
        /**
         * Check validity of close account request inputs.
         */
        public static CloseAccountRequest CheckValidCloseAccountRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidPassword((string)requestBody["password"], false)) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "password"
                                                        }))) {
                        return new CloseAccountRequest {
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
         * Close account.
         */
        public static async Task CloseAccount(AmazonDynamoDBClient dbClient, string loggedInUserId, CloseAccountRequest closeAccountRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(closeAccountRequest);
            Debug.AssertString(closeAccountRequest.password);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);
            Debug.AssertNull(user.Closed);

            // Check the password
            if (user.PasswordHash == Helper.Hash(closeAccountRequest.password)) {

                // Make changes
                user.Closed = DateTime.Now;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);

                // Actually log user out
                await IdentityServiceLogicLayer.InvalidateUserAccessTokens(dbClient, loggedInUserId);
            } else {
                Debug.Untested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD);
            }
        }

    }   // UserIdentityService_CloseAccount_LogicLayer

}   // BDDReferenceService.Logic
