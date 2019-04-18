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
    public static class UserIdentityService_ResendEmailVerification_LogicLayer
    {
        
        /**
         * Check validity of resend email verification request inputs.
         */
        public static ResendEmailVerificationRequest CheckValidResendEmailVerificationRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "emailAddress"
                                                        }))) {
                        return new ResendEmailVerificationRequest {
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
         * Resend email verification.
         */
        public static async Task ResendEmailVerification(AmazonDynamoDBClient dbClient, ResendEmailVerificationRequest resendEmailVerificationRequest)
        {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(resendEmailVerificationRequest);
            Debug.AssertString(resendEmailVerificationRequest.emailAddress);

            // Find the user by email address
            User user = await IdentityServiceDataLayer.FindUserByEmailAddress(dbClient, resendEmailVerificationRequest.emailAddress);
            Debug.AssertValidOrNull(user);
            if (user != null)
            {
                // User exists
                Debug.AssertID(user.ID);
                if (user.EmailAddressVerified == null)
                {
                    Debug.Tested();
                    await DoResendEmailVerification(dbClient, user);
                }
                else if (user.NewEmailAddress != null)
                {
                    Debug.Tested();
                    await DoResendEmailVerification(dbClient, user);
                }
                else
                {
                    Debug.Untested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_EMAIL_ALREADY_VERIFIED, new Exception(IdentityServiceLogicLayer.EMAIL_ALREADY_VERIFIED));
                }
            }
            else
            {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS, new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS));
            }
        }

        /**
         * Resend email verification.
         */
        private static async Task DoResendEmailVerification(AmazonDynamoDBClient dbClient, User user)
        {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(user);
            Debug.AssertEmail(user.EmailAddress);

            // Revoke existing link(s)
            await IdentityServiceLogicLayer.RevokeUserLinks(dbClient, user.ID, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_EMAIL_ADDRESS);

            // Create a new link
            Link link = await IdentityServiceLogicLayer.CreateLink(dbClient, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_EMAIL_ADDRESS, user.ID);
            Debug.AssertValid(link);
            Debug.AssertString(link.ID);

            // Email the new link
            Dictionary<string, string> replacementFields = new Dictionary<string, string>();
            replacementFields["link"] = link.ID;
            EmailHelper.EmailTemplate(EmailHelper.EMAIL_TEMPLATE_EMAIL_VERIFICATION, user.EmailAddress, replacementFields);

            // Debug code
            //??--IdentityServiceLogicLayer.VerifyEmailLinkId = link.ID;
            LoggingHelper.LogMessage($"EMAIL VERIFICATION LINK ID: {link.ID}");
        }

    }   // UserIdentityService_ResendEmailVerification_LogicLayer

}   // BDDReferenceService.Logic
