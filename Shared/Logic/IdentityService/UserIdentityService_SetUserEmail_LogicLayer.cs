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
    public static class UserIdentityService_SetUserEmail_LogicLayer
    {
        
        /**
         * Check validity of set user email request inputs.
         */
        public static SetUserEmailRequest CheckValidSetUserEmailRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "emailAddress"
                                                        }))) {
                        return new SetUserEmailRequest {
                            emailAddress = (string)requestBody["emailAddress"]
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
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
         * Set user email.
         * If email is already in use then throw an exception.
         */
        public static async Task SetUserEmail(AmazonDynamoDBClient dbClient, string userId, SetUserEmailRequest setUserEmailRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(setUserEmailRequest);
            Debug.AssertEmail(setUserEmailRequest.emailAddress);

            // Check that the specified email address is not already in use.
            User user = await IdentityServiceLogicLayer.FindUserByEmailAddressOrNewEmailAddress(dbClient, setUserEmailRequest.emailAddress);
            Debug.AssertValidOrNull(user);
            if (user != null)
            {
                Debug.Untested();
                if (user.ID != userId)
                {
                    Debug.Untested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_EMAIL_IN_USE);
                }
            }
            //??-- foreach (var user_ in Users) {
            //     Debug.Tested();
            //     Debug.AssertValid(user_);
            //     if (user_.Deleted == null) {
            //         if (user_.ID != userId) {
            //             Debug.Tested();
            //             if ((user_.EmailAddress == setUserEmailRequest.emailAddress) ||
            //                 (user_.NewEmailAddress == setUserEmailRequest.emailAddress)) {
            //                     Debug.Tested();
            //                 throw new Exception(ERROR_EMAIL_IN_USE);
            //             } else {
            //                 Debug.Tested();
            //             }
            //         } else {
            //             Debug.Tested();
            //         }
            //     } else {
            //         Debug.Untested();
            //     }
            // }
            else
            {
                Debug.Untested();

                // Load the user
                user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId, true);
                Debug.AssertValidOrNull(user);
            }
            if (user != null) {
                // User exists (not closed and not soft deleted)
                Debug.Tested();
                Debug.AssertNull(user.Closed);
                Debug.AssertNull(user.Deleted);
                if (user.NewEmailAddress == null) {
                    // The user's email address is not already being changed.
                    Debug.Tested();
                    if (user.EmailAddress != setUserEmailRequest.emailAddress)
                    {
                        // The email address being changed to is different from the existing one.
                        Debug.Tested();
                        user.NewEmailAddress = setUserEmailRequest.emailAddress;

                        // Save the user
                        await IdentityServiceDataLayer.SaveUser(dbClient, user);

                        // Send the validate link.
                        Link link = await IdentityServiceLogicLayer.CreateLink(dbClient, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_EMAIL_ADDRESS, user.ID);
                        Debug.AssertValid(link);
                        Debug.AssertString(link.ID);
                        //??++SEND VERIFICATION EMAIL?
                        //??--IdentityServiceLogicLayer.VerifyEmailLinkId = link.ID;
                        LoggingHelper.LogMessage($"EMAIL VERIFICATION LINK ID: {link.ID}");
                    } else {
                        // The email address being changed to is the same as the existing one.
                        Debug.Tested();
                    }
                } else if (user.NewEmailAddress != setUserEmailRequest.emailAddress) {
                    // The user's email address is already being changed but to a different address than the specified value.
                    Debug.Tested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_EMAIL_ALREADY_BEING_CHANGED);
                } else {
                    // The user's email address is already being changed to the specified value.
                    Debug.Tested();
                }
            } else {
                // User does not exist (or is closed or soft deleted)
                Debug.Tested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
            }
        }

    }   // UserIdentityService_SetUserEmail_LogicLayer

}   // BDDReferenceService.Logic
