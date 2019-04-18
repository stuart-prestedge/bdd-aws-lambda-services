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
    public static class UserIdentityService_ResendPhoneNumberVerification_LogicLayer
    {
        
        /**
         * Resend phone number verification.
         */
        internal static async Task ResendPhoneNumberVerification(AmazonDynamoDBClient dbClient, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);//??++CHECK HARE AND ELSEWHERE FOR NULL - COULD PASS IN USER?
            Debug.AssertStringOrNull(user.PhoneNumber);
            Debug.AssertValidOrNull(user.PhoneNumberVerified);
            if (!string.IsNullOrEmpty(user.PhoneNumber)) {
                // User's phone number exists
                Debug.Tested();
                if (user.PhoneNumberVerified == null) {
                    // User's phone number not verified
                    Debug.Tested();

                    // Revoke all 'verify phone number' links
                    await IdentityServiceLogicLayer.RevokeUserLinks(dbClient, loggedInUserId, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_PHONE_NUMBER);

                    // Create and send new 'verify phone number' link
                    string oneTimePassword = "<OTP>";//??++GENERATE OTP
                    Link link = await IdentityServiceLogicLayer.CreateLink(dbClient, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_PHONE_NUMBER, loggedInUserId, oneTimePassword);
                    Debug.AssertValid(link);
                    //??++SMS OTP
                    LoggingHelper.LogMessage($"PHONE NUMBER VERIFICATION LINK ID: {link.ID}");
                } else {
                    Debug.Untested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_PHONE_NUMBER_VERIFIED, new Exception(IdentityServiceLogicLayer.PHONE_NUMBER_VERIFIED));
                }
            } else {
                Debug.Tested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_NO_PHONE_NUMBER_SET, new Exception(IdentityServiceLogicLayer.NO_PHONE_NUMBER_SET));
            }
        }

    }   // UserIdentityService_ResendPhoneNumberVerification_LogicLayer

}   // BDDReferenceService.Logic
