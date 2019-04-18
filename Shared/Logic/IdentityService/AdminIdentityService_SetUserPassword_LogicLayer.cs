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
    public static class AdminIdentityService_SetUserPassword_LogicLayer
    {
        
        /**
         * Check validity of set user password request inputs.
         */
        internal static void CheckValidAdminSetUserPasswordRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidPassword((string)requestBody["password"], true)) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "password"
                                                        }))) {
                        return;
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    ;
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
        internal static async Task SetUserPasswordDirect(AmazonDynamoDBClient dbClient, string userId, JToken requestBody) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(requestBody);
            Debug.AssertString((string)requestBody["password"]);

            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Tested();
                user.PasswordHash = Helper.Hash((string)requestBody["password"]);
            } else {
                Debug.Tested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
            }
        }

    }   // AdminIdentityService_SetUserPassword_LogicLayer

}   // BDDReferenceService.Logic
