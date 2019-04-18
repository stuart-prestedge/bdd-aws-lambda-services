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
    public static class UserIdentityService_SetUserGender_LogicLayer
    {
        
        /**
         * Check validity of set user gender request inputs.
         */
        internal static SetUserGenderRequest CheckValidSetUserGenderRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (APIHelper.RequestBodyContainsField(requestBody, "gender", out JToken genderField)) {
                    int? gender = null;
                    if (genderField.Type == JTokenType.Integer) {
                        gender = (int)genderField;
                    } else if (genderField.Type == JTokenType.String) {
                        if (int.TryParse((string)genderField, out int gender_)) {
                            gender = gender_;
                        }
                    }
                    if (gender != null) {
                        if ((gender == 1) ||
                            (gender == 2) ||
                            (gender == 3)) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "gender"
                                                                }))) {
                                return new SetUserGenderRequest {
                                    gender = (UInt16)gender
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = IdentityServiceLogicLayer.INVALID_GENDER;
                        }
                    } else {
                        Debug.Untested();
                        error = IdentityServiceLogicLayer.INVALID_GENDER;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_GENDER;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user gender.
         */
        internal static async Task SetUserGender(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserGenderRequest setUserGenderRequest) {
            Debug.Tested();
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserGenderRequest);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if (user.Gender != setUserGenderRequest.gender)
            {
                user.Gender = setUserGenderRequest.gender;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

    }   // UserIdentityService_SetUserGender_LogicLayer

}   // BDDReferenceService.Logic
