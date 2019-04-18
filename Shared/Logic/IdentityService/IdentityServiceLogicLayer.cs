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
    public static class IdentityServiceLogicLayer
    {
        
        /**
         * Identity service internal errors.
         */
        #region Identity service internal errors
        public const string ERROR_EMAIL_IN_USE = "EMAIL_IN_USE";
        public const string ERROR_INCORRECT_PASSWORD = "INCORRECT_PASSWORD";
        public const string ERROR_USER_ACCOUNT_CLOSED = "USER_ACCOUNT_CLOSED";
        public const string ERROR_USER_BLOCKED = "USER_BLOCKED";
        public const string ERROR_USER_LOCKED = "USER_LOCKED";
        public const string ERROR_CANNOT_EXTEND_ACCESS_TOKEN = "CANNOT_EXTEND_ACCESS_TOKEN";
        public const string ERROR_EMAIL_ALREADY_BEING_CHANGED = "EMAIL_ALREADY_BEING_CHANGED";
        public const string ERROR_EMAIL_NOT_VERIFIED = "EMAIL_NOT_VERIFIED";
        public const string ERROR_EMAIL_ALREADY_VERIFIED = "EMAIL_ALREADY_VERIFIED";
        public const string ERROR_NO_PHONE_NUMBER_SET = "NO_PHONE_NUMBER_SET";
        public const string ERROR_PHONE_NUMBER_VERIFIED = "PHONE_NUMBER_VERIFIED";
        public const string ERROR_USER_NOT_FOUND = "USER_NOT_FOUND";
        #endregion Identity service internal errors

        /**
         * Identity service response body errors.
         */
        #region Identity service response body errors
        internal const string INVALID_POSTAL_CODE = "INVALID_POSTAL_CODE";
        internal const string INVALID_COUNTRY_CODE = "INVALID_COUNTRY_CODE";
        internal const string INVALID_ADDRESS_1 = "INVALID_ADDRESS_1";
        internal const string INVALID_DATE_OF_BIRTH = "INVALID_DATE_OF_BIRTH";
        internal const string INVALID_FAMILY_NAME = "INVALID_FAMILY_NAME";
        internal const string INVALID_GIVEN_NAME = "INVALID_GIVEN_NAME";
        internal const string INVALID_CLIENT_ID = "INVALID_CLIENT_ID";
        internal const string INVALID_OLD_PASSWORD = "INVALID_OLD_PASSWORD";
        internal const string INVALID_NEW_PASSWORD = "INVALID_NEW_PASSWORD";
        internal const string INVALID_GENDER = "INVALID_GENDER";
        internal const string INVALID_PHONE_NUMBER = "INVALID_PHONE_NUMBER";
        internal const string INVALID_ALLOW_NON_ESSENTIAL_EMAILS = "INVALID_ALLOW_NON_ESSENTIAL_EMAILS";
        internal const string INVALID_PREFERRED_TIME_ZONE = "INVALID_PREFERRED_TIME_ZONE";
        internal const string INVALID_PREFERRED_CURRENCY = "INVALID_PREFERRED_CURRENCY";
        internal const string INVALID_PREFERRED_LANGUAGE = "INVALID_PREFERRED_LANGUAGE";
        internal const string INVALID_MAX_DAILY_SPENDING_AMOUNT = "INVALID_MAX_DAILY_SPENDING_AMOUNT";
        internal const string INVALID_MAX_TIME_LOGGED_IN = "INVALID_MAX_TIME_LOGGED_IN";
        internal const string INVALID_EXCLUDE_UNTIL = "INVALID_EXCLUDE_UNTIL";
        internal const string INVALID_RESET_PASSWORD_LINK_ID = "INVALID_RESET_PASSWORD_LINK_ID";

        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string INCORRECT_PASSWORD = "INCORRECT_PASSWORD";
        public const string ACCOUNT_CLOSED = "ACCOUNT_CLOSED";
        public const string USER_BLOCKED = "USER_BLOCKED";
        public const string USER_LOCKED = "USER_LOCKED";
        public const string CANNOT_EXTEND_ACCESS_TOKEN = "CANNOT_EXTEND_ACCESS_TOKEN";
        public const string EMAIL_IN_USE = "EMAIL_IN_USE";
        public const string EMAIL_ALREADY_BEING_CHANGED = "EMAIL_ALREADY_BEING_CHANGED";
        internal const string EMAIL_ALREADY_VERIFIED = "EMAIL_ALREADY_VERIFIED";
        internal const string INVALID_VERIFY_EMAIL_LINK_ID = "INVALID_VERIFY_EMAIL_LINK_ID";
        internal const string PHONE_NUMBER_VERIFIED = "PHONE_NUMBER_VERIFIED";
        internal const string NO_PHONE_NUMBER_SET = "NO_PHONE_NUMBER_SET";
        internal const string INVALID_ONE_TIME_PASSWORD = "INVALID_ONE_TIME_PASSWORD";

        #endregion Identity service response body errors

        /**
         * Identity service link types.
         */
        #region Identity service link types
        internal const string LINK_TYPE_RESET_PASSWORD = "RESET_PASSWORD";
        internal const string LINK_TYPE_VERIFY_EMAIL_ADDRESS = "VERIFY_EMAIL_ADDRESS";
        internal const string LINK_TYPE_VERIFY_PHONE_NUMBER = "VERIFY_PHONE_NUMBER";
        #endregion Identity service link types

        /**
         * Standard permissions.
         */
        #region Standard permissions
        internal const string PERMISSION_IS_SUPER_ADMIN = "IS_SUPER_ADMIN";
        internal const string PERMISSION_IS_ADMIN = "IS_ADMIN";
        internal const string PERMISSION_CAN_LOCK_SYSTEM = "CAN_LOCK_SYSTEM";
        #endregion Standard permissions

        /**
         * Global setting names.
         */
        #region Global setting names
        internal const string GLOBAL_SETTING_SYSTEM_LOCKED = "SystemLocked";
        internal const string GLOBAL_SETTING_USERS_CAN_REOPEN_ACCOUNT = "UsersCanReopenAccount";
        internal const string GLOBAL_SETTING_LOCK_ON_FAILED_LOGIN_ATTEMPTS = "LockOnFailedLoginAttempts";
        internal const string GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME = "AccessTokenLifetime";
        //??++ResetPasswordLinkLifetime
        //??++EmailVerificationLifetime
        internal const string GLOBAL_SETTING_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED = "FailForgotPasswordIfEmailNotVerified";
        internal const string GLOBAL_SETTING_MAX_USER_LOGIN_TIME = "MaxUserLoginTime";
        internal const string GLOBAL_SETTING_CLOSE_UNVERIFIED_ACCOUNTS_AFTER_N_DAYS = "CloseUnverifiedAccountsAfterNDays";
        //??++CloseInactiveAccountsAfterNDays
        #endregion Global setting names

        /**
         * Default values.
         */
        #region Default values
        internal const bool DEFAULT_SYSTEM_LOCKED = false;
        internal const Int16 DEFAULT_MAX_LOGIN_ATTEMPTS = 10;
        internal const bool DEFAULT_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED = false;
        internal const Int64 DEFAULT_ACCESS_TOKEN_LIFETIME = 600;
        internal const Int64 DEFAULT_MAX_USER_LOGIN_TIME = 0;
        #endregion Default values

        /*
         * The in-memory data.
         */
        #region Data

        /**
         * The users.
         */
        //??--internal static List<User> Users = new List<User>();

        /**
         * The access tokens.
         */
        //??--internal static Dictionary<string, AccessToken> AccessTokens = new Dictionary<string, AccessToken>();

        /**
         * The permissions.
         */
        //??--internal static List<Tuple<string, string>> Permissions = new List<Tuple<string, string>>();

        /**
         * The links.
         */
        //??--internal static Dictionary<string, Link> Links = new Dictionary<string, Link>();

        /**
         * The identity service global settings.
         */
        //??--internal static Dictionary<string, object> IdentityGlobalSettings = new Dictionary<string, object>();

        /**
         * The ID of the verify email link to be sent. This is debug code.
         */
        //??--internal static string VerifyEmailLinkId = null;

        /**
         * The ID of the reset password link to be sent. This is debug code.
         */
        //??--internal static string ResetPasswordLinkId = null;

        #endregion Data

        /**
         * Helper methods
         */
        #region Identity service helper methods

        /**
         * Class constructor.
         * Sets up the test data
         */
        static IdentityServiceLogicLayer() {
            Debug.Tested();

            SetupTestData();
        }

        /**
         * Reset the data.
         */
        internal static void Reset() {
            Debug.Tested();

            ClearTestData();
            SetupTestData();
        }

        /**
         * Clear the data.
         */
        private static void ClearTestData() {
            Debug.Tested();

            //??--Users.Clear();
            //??--AccessTokens.Clear();
            //??--Permissions.Clear();
            //??--Links.Clear();
            //??--IdentityGlobalSettings.Clear();
            //??--VerifyEmailLinkId = null;
            //??--ResetPasswordLinkId = null;
        }

        /**
         * Setup the test data.
         */
        internal static void SetupTestData() {
            Debug.Tested();

            //???SetupTestUsers();
        }

        /**
         * Setup the test users.
         */
        //??? internal static void SetupTestUsers() {
        //     Debug.Tested();

        //     int userNumber = 1;
        //     string userId = userNumber.ToString();

        //     // Add a super admin (with ID 1)
        //     AddUserCreatingSyndicate(new User() {
        //         ID = userId,
        //         EmailAddress = "stuart.prestedge@billiondraw.com",
        //         PasswordHash = Helper.Hash("Passw0rd"),
        //         GivenName = "Stuart",
        //         FamilyName = "Prestedge"
        //     });
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_IS_SUPER_ADMIN));
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_IS_ADMIN));
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_CAN_LOCK_SYSTEM));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_CREATE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UPDATE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_LOCK_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UNLOCK_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_PUBLISH_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_FREEZE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_GAME_CLOSE_DATE));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_GAME_DONATED_TO_CHARITY));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_DELETE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_CREATE_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UPDATE_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_DRAW_DATE));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_DRAW_AMOUNT));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_DRAW_AUTO_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_DRAW_WINNING_NUMBER));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_CLEAR_DRAW_WINNING_NUMBER));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_DELETE_DRAW));
        //     userId = (++userNumber).ToString();

        //     // Add an admin (all permissions, with ID 2)
        //     AddUserCreatingSyndicate(new User() {
        //         ID = userId,
        //         EmailAddress = "admin@billiondraw.com",
        //         PasswordHash = Helper.Hash("Passw0rd"),
        //         GivenName = "BDD",
        //         FamilyName = "Admin"
        //     });
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_IS_ADMIN));
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_CAN_LOCK_SYSTEM));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_CREATE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UPDATE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_LOCK_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UNLOCK_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_PUBLISH_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_FREEZE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_GAME_CLOSE_DATE));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_GAME_DONATED_TO_CHARITY));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_DELETE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_CREATE_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UPDATE_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_DRAW_DATE));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_DRAW_AMOUNT));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_DRAW_AUTO_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_SET_DRAW_WINNING_NUMBER));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_CLEAR_DRAW_WINNING_NUMBER));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_DELETE_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, TicketingServiceLogicLayer.PERMISSION_CAN_BUY_TICKETS));
        //     Permissions.Add(new Tuple<string, string>(userId, TicketingServiceLogicLayer.PERMISSION_CAN_OFFER_TICKET));
        //     Permissions.Add(new Tuple<string, string>(userId, TicketingServiceLogicLayer.PERMISSION_CAN_REVOKE_TICKET));
        //     Permissions.Add(new Tuple<string, string>(userId, TicketingServiceLogicLayer.PERMISSION_CAN_ACCEPT_TICKET));
        //     Permissions.Add(new Tuple<string, string>(userId, TicketingServiceLogicLayer.PERMISSION_CAN_REJECT_TICKET));
        //     Permissions.Add(new Tuple<string, string>(userId, TicketingServiceLogicLayer.PERMISSION_CAN_RESERVE_TICKET));
        //     Permissions.Add(new Tuple<string, string>(userId, TicketingServiceLogicLayer.PERMISSION_CAN_GET_TICKET_AUDITS));
        //     userId = (++userNumber).ToString();

        //     // Add an admin (no permissions, with ID 3)
        //     AddUserCreatingSyndicate(new User() {
        //         ID = userId,
        //         EmailAddress = "admin2@billiondraw.com",
        //         PasswordHash = Helper.Hash("Passw0rd"),
        //         GivenName = "BDD",
        //         FamilyName = "Admin (no permissions)"
        //     });
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_IS_ADMIN));
        //     userId = (++userNumber).ToString();

        //     // Add a user (with ID 4)
        //     AddUserCreatingSyndicate(new User() {
        //         ID = userId,
        //         EmailAddress = "user@billiondraw.com",
        //         PasswordHash = Helper.Hash("Passw0rd"),
        //         GivenName = "BDD",
        //         FamilyName = "User"
        //     });
        //     userId = (++userNumber).ToString();

        //     // Add a second user (with ID 5)
        //     AddUserCreatingSyndicate(new User() {
        //         ID = userId,
        //         EmailAddress = "user2@billiondraw.com",
        //         PasswordHash = Helper.Hash("Passw0rd"),
        //         GivenName = "BDD",
        //         FamilyName = "User 2"
        //     });
        //     userId = (++userNumber).ToString();

        //     // Add a editor user (with ID 6)
        //     AddUserCreatingSyndicate(new User() {
        //         ID = userId,
        //         EmailAddress = "editor@billiondraw.com",
        //         PasswordHash = Helper.Hash("Passw0rd"),
        //         GivenName = "BDD",
        //         FamilyName = "User 6"
        //     });
            
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_IS_ADMIN));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_CREATE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UPDATE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_LOCK_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_DELETE_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_CREATE_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UPDATE_DRAW));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_DELETE_DRAW));
        //     userId = (++userNumber).ToString();

        //     // Add a publisher user (with ID 7)
        //     AddUserCreatingSyndicate(new User() {
        //         ID = userId,
        //         EmailAddress = "publisher@billiondraw.com",
        //         PasswordHash = Helper.Hash("Passw0rd"),
        //         GivenName = "BDD",
        //         FamilyName = "User 2"
        //     });
            
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_IS_ADMIN));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_PUBLISH_GAME));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_UNLOCK_GAME));
        //     userId = (++userNumber).ToString();

        //     // Add a freezer user (with ID 7)
        //     AddUserCreatingSyndicate(new User() {
        //         ID = userId,
        //         EmailAddress = "freezer@billiondraw.com",
        //         PasswordHash = Helper.Hash("Passw0rd"),
        //         GivenName = "BDD",
        //         FamilyName = "User 2"
        //     });
            
        //     Permissions.Add(new Tuple<string, string>(userId, PERMISSION_IS_ADMIN));
        //     Permissions.Add(new Tuple<string, string>(userId, GameServiceLogicLayer.PERMISSION_CAN_FREEZE_GAME));
        //     userId = (++userNumber).ToString();
        // }

        /**
         * Find a user by ID.
         */
        internal static async Task<User> FindUserByID(AmazonDynamoDBClient dbClient, string userId, bool ignoreClosed = false) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);

            User retVal = null;
            var userDBItem = await IdentityServiceDataLayer.GetUserDBItemById(dbClient, userId);
            Debug.AssertValidOrNull(userDBItem);
            if (userDBItem != null)
            {
                // A user with the specified ID exists.
                Debug.Untested();
                if (IdentityServiceDataLayer.GetUserDBItemDeleted(userDBItem) == null)
                {
                    // The user is not deleted
                    if (!ignoreClosed || (IdentityServiceDataLayer.GetUserDBItemClosed(userDBItem) == null))
                    {
                        // Not ignoring closed or the user is not closed
                        retVal = IdentityServiceDataLayer.UserFromDBItem(userDBItem);
                        Debug.AssertValid(retVal);
                    }
                    else
                    {
                        // The user is closed and we are ignoring closed users.
                        Debug.Untested();
                    }
                }
                else
                {
                    // The user is deleted
                    Debug.Untested();
                }
            }
            else
            {
                // A user with the specified ID does not exist.
                Debug.Untested();
            }
            //??-- foreach (User user in Users) {
            //     Debug.Tested();
            //     Debug.AssertValid(user);
            //     if (user.Deleted == null) {
            //         // The user record is not deleted
            //         Debug.Tested();
            //         if (user.ID == userId) {
            //             // Found user with specified ID.
            //             Debug.Tested();
            //             if (user.Closed == null) {
            //                 // The user account is not closed so return it.
            //                 Debug.Tested();
            //                 retVal = user;
            //             } else if (!ignoreClosed) {
            //                 // The user account is closed but, because we are not ignoring closed accounts, return it.
            //                 Debug.Untested();
            //                 retVal = user;
            //             } else {
            //                 // Ignore the closed user account (i.e. return null).
            //                 Debug.Tested();
            //             }
            //             break;
            //         } else {
            //             // User is not the one with the specified ID.
            //             Debug.Tested();
            //         }
            //     } else {
            //         // The user record has been soft deleted so ignore it.
            //         Debug.Untested();
            //     }
            // }
            return retVal;
        }

        /**
         * Find a user by email address or new email address.
         * //??++MAKE THIS MORE EFFICIENT!
         */
        internal static async Task<User> FindUserByEmailAddressOrNewEmailAddress(AmazonDynamoDBClient dbClient, string emailAddress) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertEmail(emailAddress);

            User retVal = await IdentityServiceDataLayer.FindUserByEmailAddress(dbClient, emailAddress);
            Debug.AssertValidOrNull(retVal);
            if (retVal == null)
            {
                retVal = await IdentityServiceDataLayer.FindUserByNewEmailAddress(dbClient, emailAddress);
                Debug.AssertValidOrNull(retVal);
            }
            return retVal;
        }

        /**
         * Find a user by email address.
         */
        internal static async Task<User> GetOrCreateUserByEmailAddress(AmazonDynamoDBClient dbClient, string emailAddress) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertEmail(emailAddress);

            User retVal = await IdentityServiceDataLayer.FindUserByEmailAddress(dbClient, emailAddress);
            Debug.AssertValidOrNull(retVal);
            if (retVal == null) {
                Debug.Tested();
                retVal = new User {
                    ID = RandomHelper.Next(),
                    EmailAddress = emailAddress
                };
                await AddUserCreatingSyndicate(dbClient, retVal);
                //??++EMAIL THE PERSON?
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Find a user by email address.
         */
        internal static async Task AddUserCreatingSyndicate(AmazonDynamoDBClient dbClient, User user) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(user);
            Debug.AssertID(user.ID);

            await IdentityServiceDataLayer.AddUser(dbClient, user);
            //??++Check response?

            // Create the syndicate for the user.
            string name = NameHelper.GetDisplayName(user.GivenName, user.FamilyName);
            await SyndicateServiceLogicLayer.CreateSyndicate(dbClient, name, user.ID);
        }

        /**
         * Find an access token.
         */
        //??? internal static AccessToken FindAccessToken(string accessTokenId) {
        //     Debug.Tested();
        //     Debug.AssertString(accessTokenId);

        //     AccessToken retVal = null;
        //     if (AccessTokens.ContainsKey(accessTokenId)) {
        //         retVal = AccessTokens[accessTokenId];
        //         Debug.AssertValid(retVal);
        //     }
        //     return retVal;
        // }

        /**
         * Find a user's access token.
         * Returns null if an access token is not found for the user.
         */
        internal static async Task<AccessToken> FindAccessTokenByUserID(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);

            AccessToken retVal = null;
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(IdentityServiceDataLayer.FIELD_ACCESS_TOKENS_USER_ID, new AttributeValue(userId));
            GetItemResponse getResponse = await dbClient.GetItemAsync(IdentityServiceDataLayer.DATASET_ACCESS_TOKENS_INDEX_USER_ID, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            if (getResponse.Item != null)
            {
                // An access token with the specified ID exists.
                Debug.Untested();
                retVal = IdentityServiceDataLayer.AccessTokenFromDBItem(getResponse.Item);
                Debug.AssertValid(retVal);
            }
            return retVal;
            /*??--AccessToken retVal = null;
            foreach (var item in AccessTokens) {
                Debug.AssertValid(item);
                Debug.AssertString(item.Key);
                Debug.AssertValidOrNull(item.Value);
                if (item.Value != null) {
                    Debug.Assert(item.Value.ID == item.Key);
                    Debug.AssertValidOrNull(item.Value.User);
                    if (item.Value.User != null) {
                        if (item.Value.User.ID == userId) {
                            retVal = item.Value;
                            break;
                        }
                    }
                }
            }
            return retVal;*/
        }

        /**
         * Invalidate (delete) any access tokens that exist for the specified user.
         */
        internal static async Task InvalidateUserAccessTokens(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);

            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(IdentityServiceDataLayer.FIELD_ACCESS_TOKENS_USER_ID, new AttributeValue(userId));
            DeleteItemResponse deleteResponse = await dbClient.DeleteItemAsync(IdentityServiceDataLayer.DATASET_ACCESS_TOKENS_INDEX_USER_ID, key);
            Debug.AssertValid(deleteResponse);
            //??++CHECK RESPONSE?
            //??-- List<string> accessTokens = new List<string>();
            // foreach (var item in AccessTokens) {
            //     Debug.AssertValid(item);
            //     Debug.AssertString(item.Key);
            //     Debug.AssertValidOrNull(item.Value);
            //     if (item.Value != null) {
            //         Debug.Assert(item.Value.ID == item.Key);
            //         Debug.AssertValidOrNull(item.Value.User);
            //         if (item.Value.User != null) {
            //             if (item.Value.User.ID == userId) {
            //                 accessTokens.Add(item.Key);
            //             }
            //         }
            //     }
            // }
            // foreach (var accessToken in accessTokens) {
            //     Debug.AssertString(accessToken);
            //     AccessTokens.Remove(accessToken);
            // }
        }

        /**
         * Get the ID of the logged in user from the access token.
         If the access token has expired then treat it as not existing and return zero (i.e. invalid ID).
         */
        internal static async Task<string> UserIDFromAccessToken(AmazonDynamoDBClient dbClient, string accessTokenID) {
            Debug.Untested();
            Debug.AssertValidOrNull(accessTokenID);

            string retVal = Helper.INVALID_ID;
            if (!string.IsNullOrEmpty(accessTokenID))
            {
                Debug.Untested();
                var accessTokenDBItem = await IdentityServiceDataLayer.GetAccessTokenDBItemById(dbClient, accessTokenID);
                Debug.AssertValidOrNull(accessTokenDBItem);
                if (accessTokenDBItem != null)
                {
                    // An access token with the specified ID exists.
                    Debug.Untested();
                    AccessToken accessToken = IdentityServiceDataLayer.AccessTokenFromDBItem(accessTokenDBItem);
                    Debug.AssertValid(accessToken);
                    if (accessToken.Expires > DateTime.Now)
                    {
                        // The access token has not expired.
                        Debug.Untested();
                        Debug.AssertID(accessToken.UserID);
                        retVal = accessToken.UserID;
                    }
                    else
                    {
                        // The access token has expired.
                        Debug.Untested();
                    }
                }
                else
                {
                    // Access token not found.
                    Debug.Tested();
                }
            }
            else
            {
                // Invalid (empty or null) access token.
                Debug.Untested();
            }
            //??--     if (AccessTokens.ContainsKey(accessTokenID)) {
            //         Debug.Tested();
            //         AccessToken accessToken = AccessTokens[accessTokenID];
            //         Debug.AssertValid(accessToken);
            //         Debug.AssertValidOrNull(accessToken.User);
            //         if (accessToken.Expires > DateTime.Now) {
            //             // The access token has not expired.
            //             Debug.Tested();
            //             if (accessToken.User != null) {
            //                 // Access token associated with a user.
            //                 Debug.Tested();
            //                 Debug.AssertID(accessToken.User.ID);
            //                 retVal = accessToken.User.ID;
            //             } else {
            //                 // Access token not associated with a user.
            //                 Debug.Untested();
            //             }
            //         }
            //         else
            //         {
            //             // The access token has expired.
            //             Debug.Untested();
            //         }
            //     } else {
            //         // Access token not found.
            //         Debug.Tested();
            //     }
            // } else {
            //     // Invalid (empty or null) access token.
            //     Debug.Untested();
            // }
            return retVal;
        }

        /**
         * Does the specified user have the specified permission?
         */
        internal static bool UserHasPermission(User user, string permission) {
            Debug.Tested();
            Debug.AssertValid(user);
            Debug.AssertValid(user.Permissions);
            Debug.AssertString(permission);

            bool retVal = false;
            foreach (string userPermission in user.Permissions)
            {
                Debug.AssertString(userPermission);
                if (userPermission == permission)
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        /**
        * Check that the user is logged in and that they can perform the action on behalf of the requested user.
        * They either must be logged in as the requested user or be logged in as an administrator with the
        *  specified permission.
        */
        //??? internal static async Task<Tuple<string, string>> CheckLoggedInWithPermission(AmazonDynamoDBClient dbClient,
        //                                                                               IDictionary<string, string> requestHeaders,
        //                                                                               string adminPermission,
        //                                                                               string requestedUserId)
        // {
        //     Debug.Untested();
        //     Debug.AssertValid(dbClient);
        //     Debug.AssertValid(requestHeaders);
        //     Debug.AssertString(adminPermission);
        //     Debug.AssertIDOrNull(requestedUserId);

        //     string actualUserId = null;
        //     APIHelper.CheckLoggedIn(requestHeaders, out string loggedInUserId);
        //     if (requestedUserId == null) {
        //         Debug.Tested();
        //         actualUserId = loggedInUserId;
        //     } else if (IdentityServiceLogicLayer.UserHasPermission(loggedInUserId, IdentityServiceLogicLayer.PERMISSION_IS_ADMIN)) {
        //         Debug.Tested();
        //         if (IdentityServiceLogicLayer.UserHasPermission(loggedInUserId, adminPermission)) {
        //             Debug.Tested();
        //             actualUserId = requestedUserId;
        //             User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, actualUserId);
        //             Debug.AssertValidOrNull(user);
        //             if (user == null) {
        //                 Debug.Tested();
        //                 throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
        //             } else {
        //                 Debug.Tested();
        //             }
        //         } else {
        //             Debug.Tested();
        //             throw new Exception(SharedLogicLayer.ERROR_NO_PERMISSION);
        //         }
        //     } else {
        //         Debug.Tested();
        //         throw new Exception(SharedLogicLayer.ERROR_NOT_AN_ADMIN, new Exception(SharedLogicLayer.ERROR_NOT_AN_ADMIN));
        //     }
        //     return new Tuple<string, string>(loggedInUserId, actualUserId);
        // }

        /**
         * Create a link of the specified type.
        */
        internal static async Task<Link> CreateLink(AmazonDynamoDBClient dbClient,
                                                    string type,
                                                    string userId,
                                                    string oneTimePassword = null) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(type);
            Debug.AssertID(userId);
            Debug.AssertStringOrNull(oneTimePassword);

            Link link = new Link() {
                ID = RandomHelper.Next().ToString(),
                Type = type,
                Expires = DateTime.Now.AddHours(1),//??++TAKE FROM SETTING?
                UserID = userId,
                Revoked = false,
                OneTimePassword = oneTimePassword
            };
            //??--Links.Add(link.ID, link);
            await IdentityServiceDataLayer.AddLink(dbClient, link);
            return link;
        }

        /**
         * Find a valid link with the ID and type passed in.
         */
        internal static async Task<Link> FindValidLink(AmazonDynamoDBClient dbClient, string linkID, string type) {
            Debug.Untested();
            Debug.AssertString(linkID);
            Debug.AssertString(type);
            //??--Debug.AssertValid(Links);

            Link retVal = null;
            var linkDBItem = await IdentityServiceDataLayer.GetLinkDBItemById(dbClient, linkID);
            Debug.AssertValidOrNull(linkDBItem);
            if (linkDBItem != null)
            {
                // A link with the specified ID exists.
                Debug.Untested();
                Link link = IdentityServiceDataLayer.LinkFromDBItem(linkDBItem);
                Debug.AssertValid(link);
                if (link.Type == type)
                {
                    // Correct type
                    Debug.Untested();
                    if (link.Expires > DateTime.Now)
                    {
                        // Not expired
                        Debug.Untested();
                        if (!link.Revoked)
                        {
                            // Not revoked
                            Debug.Untested();
                            retVal = link;
                        }
                        else
                        {
                            // Revoked
                            Debug.Untested();
                        }
                    }
                    else
                    {
                        // Expired
                        Debug.Untested();
                    }
                }
                else
                {
                    // Wrong type
                    Debug.Untested();
                }
            }
            else
            {
                // Link not found
                Debug.Untested();
            }
            //??-- if (Links.ContainsKey(linkID)) {
            //     Debug.Tested();
            //     retVal = Links[linkID];
            //     Debug.AssertValid(retVal);
            //     if (retVal.Type != type) {
            //         Debug.Tested();
            //         retVal = null;
            //     } else if (retVal.Expires <= DateTime.Now) {
            //         Debug.Tested();
            //         retVal = null;
            //     } else if (retVal.Revoked) {
            //         Debug.Tested();
            //         retVal = null;
            //     } else {
            //         Debug.Tested();
            //     }
            // } else {
            //     Debug.Tested();
            // }
            return retVal;
        }

        /**
         * Revoke the links for the specified user of the specified type.
         */
        internal static async Task RevokeUserLinks(AmazonDynamoDBClient dbClient, string userId, string type) {
            Debug.Untested();
            Debug.AssertID(userId);
            Debug.AssertString(type);

            // Get all links for the user
            // Delete the ones of the correct type.
            //??++UNCODED
            Debug.Uncoded();
            //??++ QueryRequest request = new QueryRequest {
            //     TableName = IdentityServiceDataLayer.DATASET_LINKS_INDEX_USER_ID,
            //     KeyConditionExpression = 
            // }
            // dbClient.QueryAsync(request);

            //??-- await dbClient.DeleteItemAsync();
            // List<string> linkIDs = new List<string>();
            // foreach (var item in Links)
            // {
            //     Debug.AssertValid(item);
            //     Debug.AssertString(item.Key);
            //     Debug.AssertValid(item.Value);
            //     if ((item.Value.Type == type) && (item.Value.UserID == userId)) {
            //         linkIDs.Add(item.Key);
            //     }
            // }
            // foreach (string linkID in linkIDs)
            // {
            //     Debug.AssertString(linkID);
            //     Links.Remove(linkID);
            // }
        }

        /**
         * Get global setting.
         */
        internal static async Task<string> GetIdentityGlobalSetting(AmazonDynamoDBClient dbClient, string globalSettingName, string defaultValue = null)
        {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(globalSettingName);
            Debug.AssertValidOrNull(defaultValue);
            //??--Debug.AssertValid(IdentityGlobalSettings);

            string retVal = defaultValue;
            var globalSettingDBItem = await IdentityServiceDataLayer.GetIdentityGlobalSettingDBItemByName(dbClient, globalSettingName);
            Debug.AssertValidOrNull(globalSettingDBItem);
            if (globalSettingDBItem != null)
            {
                // A global setting with the specified name exists.
                Debug.Untested();
                retVal = IdentityServiceDataLayer.ValueFromDBItem(globalSettingDBItem);
            }
            //??-- if (IdentityGlobalSettings.ContainsKey(globalSettingName)) {
            //     retVal = IdentityGlobalSettings[globalSettingName];
            //     Debug.AssertValidOrNull(retVal);
            // }
            return retVal;
        }

        /**
         * Get boolean global setting.
         */
        internal static async Task<bool> GetBoolIdentityGlobalSetting(AmazonDynamoDBClient dbClient, string globalSettingName, bool defaultValue = false)
        {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(globalSettingName);

            string globalSetting = await GetIdentityGlobalSetting(dbClient, globalSettingName, defaultValue.ToString());
            Debug.AssertValidOrNull(globalSetting);
            return (globalSetting == true.ToString());
        }

        /**
         * Get numeric global setting.
         */
        internal static async Task<Int64> GetInt64IdentityGlobalSetting(AmazonDynamoDBClient dbClient, string globalSettingName, Int64 defaultValue = 0)
        {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(globalSettingName);

            Int64 retVal = defaultValue;
            string globalSetting = await GetIdentityGlobalSetting(dbClient, globalSettingName, defaultValue.ToString());
            Debug.AssertValidOrNull(globalSetting);
            if (globalSetting != null)
            {
                retVal = Int64.Parse(globalSetting);
            }
            return retVal;
        }

        #endregion Identity service helper methods

        /**
         * Admin identity service methods.
         */
        #region Admin identity service

        /**
         * Get global settings.
         */
        //??? internal static Dictionary<string, object> GetIdentityGlobalSettings() {
        //     Debug.Tested();
        //     Debug.AssertValid(IdentityGlobalSettings);

        //     return IdentityGlobalSettings;
        // }

        #endregion Admin identity service

        /*
         * Worker methods
         */
        #region Worker methods

        /*
         * Close unverified accounts worker method.
         */
        internal static void CloseUnverifiedAccountsWorker() {
            Debug.Untested();

            Debug.Uncoded();//??++NEEDS CODING
            /*foreach (User user in Users) {
                Debug.Untested();
                Debug.AssertValid(user);
                if (user.Deleted == null) {
                    // User has not been deleted.
                    Debug.Untested();
                    if (user.Closed == null) {
                        // User account not closed.
                        Debug.Untested();
                        ;
                        Debug.Uncoded();
                    } else {
                        // User account closed (so ignore).
                        Debug.Untested();
                    }
                } else {
                    // Ignore soft deleted users.
                    Debug.Untested();
                }
            }*/
        }

        #endregion Worker methods

        /*
         * The test methods
         */
        #region Test methods
        
        /*
         * The main test entry point.
         */
        internal static void Test() {
            Debug.Tested();
            
            TestFindValidLink();
        }

        /*
         * Test the FindValidLink() method.
         */
        internal static void TestFindValidLink() {
            Debug.Tested();

            //??++ string unknownLinkId = "UNKNOWN";
            // string type1 = "UNKNOWN-TYPE";
            // string type2 = "TEST-TYPE";
            // string userId = "1";
            // Link link = FindValidLink(unknownLinkId, type1);
            // Debug.AssertNull(link);
            // link = CreateLink(type2, userId);
            // Debug.AssertValid(link);
            // string linkId = link.ID;
            // link = FindValidLink(linkId, type1);
            // Debug.AssertNull(link);
            // link = FindValidLink(linkId, type2);
            // Debug.AssertValid(link);
            // link.Expires = DateTime.Now.AddSeconds(-1);
            // link = FindValidLink(linkId, type2);
            // Debug.AssertNull(link);
            // link = CreateLink(type2, userId);
            // Debug.AssertValid(link);
            // linkId = link.ID;
            // link.Revoked = true;
            // link = FindValidLink(linkId, type2);
            // Debug.AssertNull(link);
        }

        #endregion Test methods

    }   // IdentityServiceLogicLayer

}   // BDDReferenceService.Logic
