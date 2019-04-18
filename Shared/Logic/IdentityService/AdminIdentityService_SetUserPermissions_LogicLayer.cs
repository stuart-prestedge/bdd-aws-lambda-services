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
    public static class AdminIdentityService_SetUserPermissions_LogicLayer
    {
        
        /**
         * Check validity of set user permissions request inputs.
         */
        internal static void CheckValidSetUserPermissionsRequest(JToken requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (requestBody["permissions"] != null) {
                    if (requestBody["permissions"].Type == JTokenType.Array) {
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "permissions"
                                                            }))) {
                            return;
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    }
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user permissions.
         */
        internal static async Task SetUserPermissions(AmazonDynamoDBClient dbClient, string userId, JToken requestBody) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(requestBody);
            Debug.AssertValid((JArray)requestBody["permissions"]);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Untested();
                Debug.AssertValid(user.Permissions);

                // Change the permissions
                //??--user.Permissions.RemoveAll(permission => (permission.Item1 == userId));
                user.Permissions.Clear();
                foreach (var permission in (JArray)requestBody["permissions"])
                {
                    Debug.AssertString((string)permission);
                    user.Permissions.Add((string)permission);//??--new Tuple<string, string>(userId, (string)permission));
                }
                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);

            } else {
                Debug.Untested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
            }
        }

    }   // AdminIdentityService_SetUserPermissions_LogicLayer

}   // BDDReferenceService.Logic
