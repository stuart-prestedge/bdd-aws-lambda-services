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
    public static class UserIdentityService_VerifyEmailWithDetails_LogicLayer
    {
        
        /**
         * Check validity of verify email with details request inputs.
         */
        public static VerifyEmailWithDetailsRequest CheckValidVerifyEmailWithDetailsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                    Debug.Tested();
                    if (!String.IsNullOrEmpty((string)requestBody["verifyEmailLinkId"])) {
                        Debug.Tested();
                        if (!String.IsNullOrEmpty((string)requestBody["givenName"])) {
                            Debug.Tested();
                            if (!String.IsNullOrEmpty((string)requestBody["familyName"])) {
                                Debug.Tested();
                                if (Helper.IsValidPassword((string)requestBody["password"], true)) {
                                    Debug.Tested();
                                    if (APIHelper.IsValidAPIDateString((string)requestBody["dateOfBirth"])) {
                                        Debug.Tested();
                                        if (!String.IsNullOrEmpty((string)requestBody["address1"])) {
                                            Debug.Tested();
                                            if (Helper.IsValidCountryCode((string)requestBody["country"])) {
                                                Debug.Tested();
                                                if (!String.IsNullOrEmpty((string)requestBody["postalCode"])) {
                                                    Debug.Tested();
                                                    if (APIHelper.GetOptionalBooleanFromRequestBody(requestBody, "allowNonEssentialEmails", out bool? allowNonEssentialEmails)) {
                                                        Debug.Tested();
                                                        if (Helper.AllFieldsRecognized(requestBody,
                                                                                        new List<string>(new String[]{
                                                                                            "emailAddress",
                                                                                            "verifyEmailLinkId",
                                                                                            "givenName",
                                                                                            "familyName",
                                                                                            "password",
                                                                                            "dateOfBirth",
                                                                                            "address1",
                                                                                            "address2",
                                                                                            "address3",
                                                                                            "address4",
                                                                                            "city",
                                                                                            "region",
                                                                                            "country",
                                                                                            "postalCode",
                                                                                            "allowNonEssentialEmails"
                                                                                            }))) {
                                                            Debug.Tested();
                                                            return new VerifyEmailWithDetailsRequest {
                                                                emailAddress = (string)requestBody["emailAddress"],
                                                                verifyEmailLinkId = (string)requestBody["verifyEmailLinkId"],
                                                                givenName = (string)requestBody["givenName"],
                                                                familyName = (string)requestBody["familyName"],
                                                                password = (string)requestBody["password"],
                                                                dateOfBirth = (string)requestBody["dateOfBirth"],
                                                                address1 = (string)requestBody["address1"],
                                                                address2 = (string)requestBody["address2"],
                                                                address3 = (string)requestBody["address3"],
                                                                address4 = (string)requestBody["address4"],
                                                                city = (string)requestBody["city"],
                                                                region = (string)requestBody["region"],
                                                                country = (string)requestBody["country"],
                                                                postalCode = (string)requestBody["postalCode"],
                                                                allowNonEssentialEmails = (bool)allowNonEssentialEmails
                                                            };
                                                        } else {
                                                            // Unrecognised field(s)
                                                            Debug.Tested();
                                                            error = APIHelper.UNRECOGNISED_FIELD;
                                                        }
                                                    } else {
                                                        Debug.Untested();
                                                        error = IdentityServiceLogicLayer.INVALID_ALLOW_NON_ESSENTIAL_EMAILS;
                                                    }
                                                } else {
                                                    Debug.Untested();
                                                    error = IdentityServiceLogicLayer.INVALID_POSTAL_CODE;
                                                }
                                            } else {
                                                Debug.Untested();
                                                error = IdentityServiceLogicLayer.INVALID_COUNTRY_CODE;
                                            }
                                        } else {
                                            Debug.Untested();
                                            error = IdentityServiceLogicLayer.INVALID_ADDRESS_1;
                                        }
                                    } else {
                                        Debug.Untested();
                                        error = IdentityServiceLogicLayer.INVALID_DATE_OF_BIRTH;
                                    }
                                } else {
                                    Debug.Untested();
                                    error = APIHelper.INVALID_PASSWORD;
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
         * Verify email with details.
         */
        public static async Task VerifyEmailWithDetails(AmazonDynamoDBClient dbClient, VerifyEmailWithDetailsRequest verifyEmailWithDetailsRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(verifyEmailWithDetailsRequest);
            Debug.AssertString(verifyEmailWithDetailsRequest.verifyEmailLinkId);
            Debug.AssertEmail(verifyEmailWithDetailsRequest.emailAddress);

            // Find a valid link
            Link link = await IdentityServiceLogicLayer.FindValidLink(dbClient, verifyEmailWithDetailsRequest.verifyEmailLinkId, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_EMAIL_ADDRESS);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                // Valid link exits
                Debug.Tested();
                Debug.AssertID(link.UserID);

                // Find user
                User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, link.UserID);
                Debug.AssertValidOrNull(user);
                if (user != null) {
                    // User exists
                    Debug.Untested();
                    Debug.AssertEmail(user.EmailAddress);
                    Debug.AssertValidOrNull(user.EmailAddressVerified);

                    if (user.EmailAddressVerified == null) {
                        // Email address not verified
                        Debug.Untested();

                        if (user.EmailAddress == verifyEmailWithDetailsRequest.emailAddress) {
                            // Verifying correct email address
                            Debug.Untested();

                            // Change user and revoke link
                            await DoVerifyEmailWithDetails(dbClient, user, link, verifyEmailWithDetailsRequest);
                        } else {
                            Debug.Tested();
                            throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS, new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS));
                        }
                    } else {
                        Debug.Untested();
                        throw new Exception(IdentityServiceLogicLayer.ERROR_EMAIL_ALREADY_VERIFIED, new Exception(IdentityServiceLogicLayer.EMAIL_ALREADY_VERIFIED));
                    }
                } else {
                    // User does not exist (may have been deleted)
                    Debug.Untested();
                    throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER, new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER));
                }
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK, new Exception(SharedLogicLayer.ERROR_INVALID_LINK));
            }
        }

        /**
         * Verify email with details.
         */
        private static async Task DoVerifyEmailWithDetails(AmazonDynamoDBClient dbClient, User user, Link link, VerifyEmailWithDetailsRequest verifyEmailWithDetailsRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(user);
            Debug.AssertValid(link);
            Debug.AssertValid(verifyEmailWithDetailsRequest);

            // Make changes
            user.EmailAddressVerified = DateTime.Now;
            user.GivenName = verifyEmailWithDetailsRequest.givenName;
            user.FamilyName = verifyEmailWithDetailsRequest.familyName;
            user.PasswordHash = Helper.Hash(verifyEmailWithDetailsRequest.password);
            user.DateOfBirth = (DateTime)APIHelper.DateFromAPIDateString(verifyEmailWithDetailsRequest.dateOfBirth);
            user.Address1 = verifyEmailWithDetailsRequest.address1;
            user.Address2 = verifyEmailWithDetailsRequest.address2;
            user.Address3 = verifyEmailWithDetailsRequest.address3;
            user.Address4 = verifyEmailWithDetailsRequest.address4;
            user.City = verifyEmailWithDetailsRequest.city;
            user.Region = verifyEmailWithDetailsRequest.region;
            user.Country = verifyEmailWithDetailsRequest.country;
            user.PostalCode = verifyEmailWithDetailsRequest.postalCode;
            user.AllowNonEssentialEmails = verifyEmailWithDetailsRequest.allowNonEssentialEmails;

            // Save the user
            await IdentityServiceDataLayer.SaveUser(dbClient, user);

            // Revoke the link                        
            link.Revoked = true;
            //??++SAVE LINK
        }

    }   // UserIdentityService_VerifyEmailWithDetails_LogicLayer

}   // BDDReferenceService.Logic
