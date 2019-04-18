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
    public static class UserIdentityService_VerifyPhoneNumber_LogicLayer
    {
        
        /**
         * Check validity of verify phone number request inputs.
         */
        internal static VerifyPhoneNumberRequest CheckValidVerifyPhoneNumberRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (!String.IsNullOrEmpty((string)requestBody["oneTimePassword"])) {
                    Debug.Tested();
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "oneTimePassword"
                                                        }))) {
                        Debug.Tested();
                        return new VerifyPhoneNumberRequest {
                            oneTimePassword = (string)requestBody["oneTimePassword"]
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_ONE_TIME_PASSWORD;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Verify phone number.
         */
        internal static async Task VerifyPhoneNumber(AmazonDynamoDBClient dbClient, string loggedInUserId, VerifyPhoneNumberRequest verifyPhoneNumberRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(verifyPhoneNumberRequest);
            Debug.AssertString(verifyPhoneNumberRequest.oneTimePassword);

            // Find valid link
            Link link = await FindValidLinkByUserID(dbClient, loggedInUserId, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_PHONE_NUMBER);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                // Valid link exists
                Debug.Tested();
                Debug.AssertString(link.OneTimePassword);
                if (link.OneTimePassword == verifyPhoneNumberRequest.oneTimePassword) {
                    // One-time password matches
                    Debug.Tested();

                    // Load the user
                    User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
                    Debug.AssertValid(user);
                    Debug.AssertValidOrNull(user.PhoneNumberVerified);
                    if (user.PhoneNumberVerified == null) {
                        // User's phone number not verified
                        Debug.Untested();

                        // Change the user and revoke the link
                        await DoVerifyPhoneNumber(dbClient, user, link);
                    } else {
                        Debug.Untested();
                        throw new Exception(IdentityServiceLogicLayer.ERROR_PHONE_NUMBER_VERIFIED, new Exception(IdentityServiceLogicLayer.PHONE_NUMBER_VERIFIED));
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD, new Exception(IdentityServiceLogicLayer.INCORRECT_PASSWORD));
                }
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK, new Exception(SharedLogicLayer.ERROR_INVALID_LINK));
            }
        }

        /**
         * Verify phone number.
         */
        private static async Task DoVerifyPhoneNumber(AmazonDynamoDBClient dbClient, User user, Link link) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(user);
            Debug.AssertValid(link);

            // Make changes to the user
            user.PhoneNumberVerified = DateTime.Now;

            // Save the user
            await IdentityServiceDataLayer.SaveUser(dbClient, user);

            // Revoke the link                        
            link.Revoked = true;
            //??++SAVE LINK
        }

        /**
         * Find a valid link with the user ID and type passed in.
         */
        private static async Task<Link> FindValidLinkByUserID(AmazonDynamoDBClient dbClient, string userId, string type) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertString(type);

            Link retVal = null;
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(IdentityServiceDataLayer.FIELD_LINKS_USER_ID, new AttributeValue(userId));
            GetItemResponse getResponse = await dbClient.GetItemAsync(IdentityServiceDataLayer.DATASET_LINKS_INDEX_USER_ID, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            if (getResponse.Item != null)
            {
                // A link with the specified ID exists.
                Debug.Untested();
                retVal = IdentityServiceDataLayer.LinkFromDBItem(getResponse.Item);
                Debug.AssertValid(retVal);
            }
            //??-- foreach (var item in Links)
            // {
            //     Debug.AssertValid(item);
            //     Debug.AssertString(item.Key);
            //     Debug.AssertValid(item.Value);
            //     Debug.AssertString(item.Value.ID);
            //     Debug.Assert(item.Value.ID == item.Key);
            //     Debug.AssertID(item.Value.UserID);
            //     if (item.Value.UserID == userId) {
            //         if ((item.Value.Type == type) && (item.Value.Expires > DateTime.Now) && !item.Value.Revoked) {
            //             retVal = item.Value;
            //             break;
            //         }
            //     }
            // }
            return retVal;
        }

    }   // UserIdentityService_VerifyPhoneNumber_LogicLayer

}   // BDDReferenceService.Logic
