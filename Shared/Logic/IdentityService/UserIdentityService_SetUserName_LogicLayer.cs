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
    public static class UserIdentityService_SetUserName_LogicLayer
    {
        
        /**
         * Check validity of set user name request inputs.
         */
        public static SetUserNameRequest CheckValidSetUserNameRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!String.IsNullOrEmpty((string)requestBody["givenName"])) {
                    if (!String.IsNullOrEmpty((string)requestBody["familyName"])) {
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "givenName",
                                                            "familyName",
                                                            "fullName",
                                                            "preferredName"
                                                            }))) {
                            return new SetUserNameRequest {
                                givenName = (string)requestBody["givenName"],
                                familyName = (string)requestBody["familyName"],
                                fullName = (string)requestBody["fullName"],
                                preferredName = (string)requestBody["preferredName"]
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = IdentityServiceLogicLayer.INVALID_FAMILY_NAME;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_GIVEN_NAME;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user name.
         */
        public static async Task SetUserName(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserNameRequest setUserNameRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserNameRequest);
            Debug.AssertString(setUserNameRequest.givenName);
            Debug.AssertString(setUserNameRequest.familyName);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if ((user.GivenName != setUserNameRequest.givenName) ||
                (user.FamilyName != setUserNameRequest.familyName) ||
                (user.FullName != setUserNameRequest.fullName) ||
                (user.PreferredName != setUserNameRequest.preferredName))
            {
                user.GivenName = setUserNameRequest.givenName;
                user.FamilyName = setUserNameRequest.familyName;
                user.FullName = setUserNameRequest.fullName;
                user.PreferredName = setUserNameRequest.preferredName;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

    }   // UserIdentityService_SetUserName_LogicLayer

}   // BDDReferenceService.Logic
