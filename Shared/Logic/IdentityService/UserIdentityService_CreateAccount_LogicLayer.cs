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
    public static class UserIdentityService_CreateAccount_LogicLayer
    {
        
        /**
         * Check validity of create account request inputs.
         */
        public static CreateAccountRequest CheckValidCreateAccountRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if ((requestBody["clientId"] == null) || ((string)requestBody["clientId"] != "")) {
                    if (!String.IsNullOrEmpty((string)requestBody["givenName"])) {
                        if (!String.IsNullOrEmpty((string)requestBody["familyName"])) {
                            if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                                if (Helper.IsValidPassword((string)requestBody["password"], true)) {
                                    if (APIHelper.IsValidAPIDateString((string)requestBody["dateOfBirth"])) {
                                        if (!String.IsNullOrEmpty((string)requestBody["address1"])) {
                                            if (Helper.IsValidCountryCode((string)requestBody["country"])) {
                                                if (!String.IsNullOrEmpty((string)requestBody["postalCode"])) {
                                                    if (APIHelper.GetOptionalBooleanFromRequestBody(requestBody, "allowNonEssentialEmails", out bool? allowNonEssentialEmails)) {
                                                        if (Helper.AllFieldsRecognized(requestBody,
                                                                                        new List<string>(new String[]{
                                                                                            "clientId",
                                                                                            "givenName",
                                                                                            "familyName",
                                                                                            "emailAddress",
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
                                                            return new CreateAccountRequest {
                                                                clientId = (string)requestBody["clientId"],
                                                                givenName = (string)requestBody["givenName"],
                                                                familyName = (string)requestBody["familyName"],
                                                                emailAddress = (string)requestBody["emailAddress"],
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
                                                                allowNonEssentialEmails = (requestBody["allowNonEssentialEmails"] == null) ? false : (bool)requestBody["allowNonEssentialEmails"]
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
                                error = APIHelper.INVALID_EMAIL_ADDRESS;
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
                    error = IdentityServiceLogicLayer.INVALID_CLIENT_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Creates an account.
         */
        public static async Task<Tuple<User, bool>> CreateAccount(AmazonDynamoDBClient dbClient, CreateAccountRequest createAccountRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(createAccountRequest);
            //??--Debug.AssertValid(IdentityServiceLogicLayer.Users);

            // Is there an existing user?
            User user = await IdentityServiceLogicLayer.FindUserByEmailAddressOrNewEmailAddress(dbClient, createAccountRequest.emailAddress);
            //??--User user = IdentityServiceLogicLayer.Users.Find(u => ((u.EmailAddress == createAccountRequest.emailAddress) || (u.NewEmailAddress == createAccountRequest.emailAddress)));
            Debug.AssertValidOrNull(user);
            bool bReOpened = false;
            if (user == null) {
                // No user exists with the supplied email address.
                // Create a new user           
                user = new User() {
                    ID = RandomHelper.Next(),
                    EmailAddress = createAccountRequest.emailAddress,
                    PasswordHash = Helper.Hash(createAccountRequest.password)
                };
                SetupUserFromCreateAccountRequest(user, createAccountRequest);

                // Add the new user to the data store           
                await IdentityServiceLogicLayer.AddUserCreatingSyndicate(dbClient, user);

                // Create an email verification link
                Link link = await IdentityServiceLogicLayer.CreateLink(dbClient, IdentityServiceLogicLayer.LINK_TYPE_VERIFY_EMAIL_ADDRESS, user.ID);
                Debug.AssertValid(link);
                Debug.AssertString(link.ID);
                //??--IdentityServiceLogicLayer.VerifyEmailLinkId = link.ID;

                // Send email verification email
                Dictionary<string, string> replacementFields = new Dictionary<string, string>();
                replacementFields["link"] = link.ID;
                EmailHelper.EmailTemplate(EmailHelper.EMAIL_TEMPLATE_CREATE_ACCOUNT, createAccountRequest.emailAddress, replacementFields);

                // Indicate that a new user was created.
                bReOpened = false;
            } else {
                // A user already exists with the supplied email address.
                if (user.Closed != null) {
                    // Account is closed so possibly re-open.
                    //??--if ((bool)GetIdentityGlobalSetting(GLOBAL_SETTING_USERS_CAN_REOPEN_ACCOUNT, true))
                    if (await IdentityServiceLogicLayer.GetBoolIdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_USERS_CAN_REOPEN_ACCOUNT, true))
                    {
                        // Accounts allowed to be re-opened
                        user.Closed = null;

                        // Setup the user
                        SetupUserFromCreateAccountRequest(user, createAccountRequest);

                        // Save the user
                        await IdentityServiceDataLayer.SaveUser(dbClient, user);

                        // Send account re-opened email
                        Dictionary<string, string> replacementFields = new Dictionary<string, string>();
                        EmailHelper.EmailTemplate(EmailHelper.EMAIL_TEMPLATE_ACCOUNT_REOPENED, createAccountRequest.emailAddress, replacementFields);

                        // Indicate that an existing, closed, user was re-opened.
                        bReOpened = true;
                    } else {
                        // Accounts cannot be re-opened so throw an error.
                        throw new Exception(IdentityServiceLogicLayer.ERROR_USER_ACCOUNT_CLOSED);
                    }
                } else {
                    // Account is not closed so throw an error.
                    throw new Exception(IdentityServiceLogicLayer.ERROR_EMAIL_IN_USE);
                }
            }
            return new Tuple<User, bool>(user, bReOpened);
        }

        /**
         * Creates an account.
         */
        private static void SetupUserFromCreateAccountRequest(User user, CreateAccountRequest createAccountRequest) {
            Debug.Untested();
            Debug.AssertValid(user);
            Debug.AssertValid(createAccountRequest);

            user.ClientID = createAccountRequest.clientId;
            user.GivenName = createAccountRequest.givenName;
            user.FamilyName = createAccountRequest.familyName;
            user.DateOfBirth = (DateTime)APIHelper.DateFromAPIDateString(createAccountRequest.dateOfBirth);
            user.Address1 = createAccountRequest.address1;
            user.Address2 = createAccountRequest.address2;
            user.Address3 = createAccountRequest.address3;
            user.Address4 = createAccountRequest.address4;
            user.City = createAccountRequest.city;
            user.Region = createAccountRequest.region;
            user.Country = createAccountRequest.country;
            user.PostalCode = createAccountRequest.postalCode;
            user.AllowNonEssentialEmails = createAccountRequest.allowNonEssentialEmails;
            if (createAccountRequest.allowNonEssentialEmails) {
                user.ANEEOnTimestamp = DateTime.Now;
            } else {
                user.ANEEOffTimestamp = DateTime.Now;
            }
        }

    }   // UserIdentityService_CreateAccount_LogicLayer

}   // BDDReferenceService.Logic
