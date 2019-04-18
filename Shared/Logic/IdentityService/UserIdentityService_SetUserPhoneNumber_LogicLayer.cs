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
    public static class UserIdentityService_SetUserPhoneNumber_LogicLayer
    {
        
        /**
         * Check validity of set user phone number request inputs.
         */
        public static SetUserPhoneNumberRequest CheckValidSetUserPhoneNumberRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!String.IsNullOrEmpty((string)requestBody["phoneNumber"])) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "phoneNumber"
                                                        }))) {
                        return new SetUserPhoneNumberRequest {
                            phoneNumber = (string)requestBody["phoneNumber"]
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_PHONE_NUMBER;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user phone number.
         */
        public static async Task SetUserPhoneNumber(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserPhoneNumberRequest setUserPhoneNumberRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserPhoneNumberRequest);
            Debug.AssertString(setUserPhoneNumberRequest.phoneNumber);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if (user.PhoneNumber != setUserPhoneNumberRequest.phoneNumber) {

                // Set the new phone number
                user.PhoneNumber = setUserPhoneNumberRequest.phoneNumber;

                // Mark it as not verified
                user.PhoneNumberVerified = null;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);

                // Send the OTP via SMS
                string oneTimePassword = "<OTP>";//??++GENERATE OTP
                Link link = await IdentityServiceLogicLayer.CreateLink(dbClient, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_PHONE_NUMBER, loggedInUserId, oneTimePassword);
                Debug.AssertValid(link);
                //??++SMS OTP
                LoggingHelper.LogMessage($"PHONE NUMBER VERIFICATION LINK ID: {link.ID}");
            }
        }

    }   // UserIdentityService_SetUserPhoneNumber_LogicLayer

}   // BDDReferenceService.Logic
