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
    public static class UserIdentityService_VerifyEmail_LogicLayer
    {
        
        /**
         * Check validity of verify email request inputs.
         */
        public static VerifyEmailRequest CheckValidVerifyEmailRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                    Debug.Tested();
                    if (!String.IsNullOrEmpty((string)requestBody["verifyEmailLinkId"])) {
                        Debug.Tested();
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "verifyEmailLinkId",
                                                            "emailAddress"
                                                            }))) {
                            Debug.Tested();
                            return new VerifyEmailRequest {
                                verifyEmailLinkId = (string)requestBody["verifyEmailLinkId"],
                                emailAddress = (string)requestBody["emailAddress"]
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = IdentityServiceLogicLayer.INVALID_VERIFY_EMAIL_LINK_ID;
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
         * Verify email.
         * ??++BREAK INTO SMALLER METHODS
         */
        public static async Task VerifyEmail(AmazonDynamoDBClient dbClient, VerifyEmailRequest verifyEmailRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(verifyEmailRequest);
            Debug.AssertEmail(verifyEmailRequest.emailAddress);
            Debug.AssertString(verifyEmailRequest.verifyEmailLinkId);

            // Find a valid link
            Link link = await IdentityServiceLogicLayer.FindValidLink(dbClient, verifyEmailRequest.verifyEmailLinkId, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_EMAIL_ADDRESS);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                // Valid link exits
                Debug.Tested();
                Debug.AssertID(link.UserID);

                // Find user
                User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, link.UserID, true);
                Debug.AssertValidOrNull(user);
                if (user != null) {
                    // User exists
                    Debug.Tested();
                    Debug.AssertEmail(user.EmailAddress);
                    Debug.AssertValidOrNull(user.EmailAddressVerified);
                    if (user.NewEmailAddress == verifyEmailRequest.emailAddress) {
                        // Verifying new email address
                        Debug.Tested();

                        // Make changes to the user
                        user.EmailAddress = verifyEmailRequest.emailAddress;
                        user.NewEmailAddress = null;
                        user.EmailAddressVerified = DateTime.Now;

                        // Save the user
                        await IdentityServiceDataLayer.SaveUser(dbClient, user);

                        // Revoke the link                        
                        link.Revoked = true;
                        //??++SAVE LINK
                    } else {
                        // Possibly verifying main email address
                        Debug.Tested();
                        if (user.EmailAddressVerified == null) {
                            // Main email address not verified
                            Debug.Tested();
                            if (user.EmailAddress == verifyEmailRequest.emailAddress) {
                                // Verifying main email address
                                Debug.Tested();
                                Debug.AssertNull(user.NewEmailAddress);

                                // Make changes to the user
                                user.EmailAddressVerified = DateTime.Now;

                                // Save the user
                                await IdentityServiceDataLayer.SaveUser(dbClient, user);

                                // Revoke the link                        
                                link.Revoked = true;
                                //??++SAVE LINK
                            } else {
                                // Verifying wrong email address
                                Debug.Tested();
                                throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS, new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS));
                            }
                        } else {
                            // Main email address already verified
                            Debug.Tested();
                            throw new Exception(IdentityServiceLogicLayer.ERROR_EMAIL_ALREADY_VERIFIED, new Exception(IdentityServiceLogicLayer.EMAIL_ALREADY_VERIFIED));
                        }
                    }
                } else {
                    // User does not exist (may have been closed)
                    Debug.Tested();
                    throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER, new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER));
                }
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK, new Exception(SharedLogicLayer.ERROR_INVALID_LINK));
            }
        }

    }   // UserIdentityService_VerifyEmail_LogicLayer

}   // BDDReferenceService.Logic
