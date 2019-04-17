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
        internal const string ERROR_INCORRECT_PASSWORD = "INCORRECT_PASSWORD";
        public const string ERROR_USER_ACCOUNT_CLOSED = "USER_ACCOUNT_CLOSED";
        internal const string ERROR_USER_BLOCKED = "USER_BLOCKED";
        internal const string ERROR_USER_LOCKED = "USER_LOCKED";
        internal const string ERROR_CANNOT_EXTEND_ACCESS_TOKEN = "CANNOT_EXTEND_ACCESS_TOKEN";
        internal const string ERROR_EMAIL_ALREADY_BEING_CHANGED = "EMAIL_ALREADY_BEING_CHANGED";
        internal const string ERROR_EMAIL_NOT_VERIFIED = "EMAIL_NOT_VERIFIED";
        internal const string ERROR_EMAIL_ALREADY_VERIFIED = "EMAIL_ALREADY_VERIFIED";
        internal const string ERROR_NO_PHONE_NUMBER_SET = "NO_PHONE_NUMBER_SET";
        internal const string ERROR_PHONE_NUMBER_VERIFIED = "PHONE_NUMBER_VERIFIED";
        internal const string ERROR_USER_NOT_FOUND = "USER_NOT_FOUND";
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

        internal const string USER_NOT_FOUND = "USER_NOT_FOUND";
        internal const string INCORRECT_PASSWORD = "INCORRECT_PASSWORD";
        internal const string ACCOUNT_CLOSED = "ACCOUNT_CLOSED";
        internal const string USER_BLOCKED = "USER_BLOCKED";
        internal const string USER_LOCKED = "USER_LOCKED";
        internal const string CANNOT_EXTEND_ACCESS_TOKEN = "CANNOT_EXTEND_ACCESS_TOKEN";
        internal const string EMAIL_IN_USE = "EMAIL_IN_USE";
        internal const string EMAIL_ALREADY_BEING_CHANGED = "EMAIL_ALREADY_BEING_CHANGED";
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
        private const string LINK_TYPE_RESET_PASSWORD = "RESET_PASSWORD";
        private const string LINK_TYPE_VERIFY_EMAIL_ADDRESS = "VERIFY_EMAIL_ADDRESS";
        private const string LINK_TYPE_VERIFY_PHONE_NUMBER = "VERIFY_PHONE_NUMBER";
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
        private const string GLOBAL_SETTING_USERS_CAN_REOPEN_ACCOUNT = "UsersCanReopenAccount";
        private const string GLOBAL_SETTING_LOCK_ON_FAILED_LOGIN_ATTEMPTS = "LockOnFailedLoginAttempts";
        private const string GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME = "AccessTokenLifetime";
        //??++ResetPasswordLinkLifetime
        //??++EmailVerificationLifetime
        private const string GLOBAL_SETTING_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED = "FailForgotPasswordIfEmailNotVerified";
        private const string GLOBAL_SETTING_MAX_USER_LOGIN_TIME = "MaxUserLoginTime";
        private const string GLOBAL_SETTING_CLOSE_UNVERIFIED_ACCOUNTS_AFTER_N_DAYS = "CloseUnverifiedAccountsAfterNDays";
        //??++CloseInactiveAccountsAfterNDays
        #endregion Global setting names

        /**
         * Default values.
         */
        #region Default values
        internal const bool DEFAULT_SYSTEM_LOCKED = false;
        private const Int16 DEFAULT_MAX_LOGIN_ATTEMPTS = 10;
        private const bool DEFAULT_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED = false;
        private const Int64 DEFAULT_ACCESS_TOKEN_LIFETIME = 600;
        private const Int64 DEFAULT_MAX_USER_LOGIN_TIME = 0;
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
        internal static string VerifyEmailLinkId = null;

        /**
         * The ID of the reset password link to be sent. This is debug code.
         */
        internal static string ResetPasswordLinkId = null;

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
            VerifyEmailLinkId = null;
            ResetPasswordLinkId = null;
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
        private static async Task<User> FindUserByEmailAddressOrNewEmailAddress(AmazonDynamoDBClient dbClient, string emailAddress) {
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
        private static async Task<AccessToken> FindAccessTokenByUserID(AmazonDynamoDBClient dbClient, string userId) {
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

        /**
         * Revoke the links for the specified user of the specified type.
         */
        private static async Task RevokeUserLinks(AmazonDynamoDBClient dbClient, string userId, string type) {
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
         * Search for users.
         */
        internal static async Task<SearchUsersResponse> SearchUsers(AmazonDynamoDBClient dbClient,
                                                                    string loggedInUserId,
                                                                    string searchText) {
            Debug.Untested();
            Debug.AssertID(loggedInUserId);
            Debug.AssertString(searchText);

            //??++NEEDS TO BE MORE EFFICIENT
            List<SearchUserResponse> users = new List<SearchUserResponse>();
            string lowerSearchText = searchText.ToLower();
            List<string> attributes = new List<string>();
            attributes.Add(IdentityServiceDataLayer.FIELD_USERS_ID);
            attributes.Add(IdentityServiceDataLayer.FIELD_USERS_DELETED);
            attributes.Add(IdentityServiceDataLayer.FIELD_USERS_GIVEN_NAME);
            attributes.Add(IdentityServiceDataLayer.FIELD_USERS_FAMILY_NAME);
            attributes.Add(IdentityServiceDataLayer.FIELD_USERS_FULL_NAME);
            attributes.Add(IdentityServiceDataLayer.FIELD_USERS_PREFERRED_NAME);
            attributes.Add(IdentityServiceDataLayer.FIELD_USERS_EMAIL_ADDRESS);
            ScanResponse response = await dbClient.ScanAsync(IdentityServiceDataLayer.DATASET_USERS, attributes);
            Debug.AssertValid(response);
            //??++CHECK RESPONSE?
            LoggingHelper.LogMessage($"Get users: {response.Count} items");
            foreach (var item in response.Items)
            {
                Debug.Untested();
                Debug.AssertValid(item);
                if (item[IdentityServiceDataLayer.FIELD_USERS_DELETED] == null) {
                    // User is not soft deleted.
                    Debug.Tested();
                    if (Match(item[IdentityServiceDataLayer.FIELD_USERS_GIVEN_NAME].S, lowerSearchText) ||
                        Match(item[IdentityServiceDataLayer.FIELD_USERS_FAMILY_NAME].S, lowerSearchText) ||
                        Match(item[IdentityServiceDataLayer.FIELD_USERS_FULL_NAME].S, lowerSearchText) ||
                        Match(item[IdentityServiceDataLayer.FIELD_USERS_PREFERRED_NAME].S, lowerSearchText) ||
                        Match(item[IdentityServiceDataLayer.FIELD_USERS_EMAIL_ADDRESS].S, lowerSearchText)) {
                        // User matches search text.
                        Debug.Tested();
                        users.Add(new SearchUserResponse() {
                            id = item[IdentityServiceDataLayer.FIELD_USERS_ID].S,
                            givenName = item[IdentityServiceDataLayer.FIELD_USERS_GIVEN_NAME].S,
                            familyName = item[IdentityServiceDataLayer.FIELD_USERS_FAMILY_NAME].S,
                            fullName = item[IdentityServiceDataLayer.FIELD_USERS_FULL_NAME].S,
                            emailAddress = item[IdentityServiceDataLayer.FIELD_USERS_EMAIL_ADDRESS].S
                        });
                    } else {
                        // User does not match search text.
                        Debug.Tested();
                    }
                } else {
                    // User is soft deleted.
                    Debug.Untested();
                }
                //??-- countries.Add(new Country {
                //     Code = item[FIELD_COUNTRIES_CODE].S,
                //     Name = item[FIELD_COUNTRIES_NAME].S,
                //     Currencies = item[FIELD_COUNTRIES_CURRENCIES].SS,
                //     Available = item[FIELD_COUNTRIES_AVAILABLE].BOOL
                // });
            }
            //??-- foreach (var user in Users) {
            //     Debug.Tested();
            //     Debug.AssertValid(user);
            //     if (user.Deleted == null) {
            //         // User is not soft deleted.
            //         Debug.Tested();
            //         if (Match(user.GivenName, lowerSearchText) ||
            //             Match(user.FamilyName, lowerSearchText) ||
            //             Match(user.FullName, lowerSearchText) ||
            //             Match(user.PreferredName, lowerSearchText) ||
            //             Match(user.EmailAddress, lowerSearchText)) {
            //             // User matches search text.
            //             Debug.Tested();
            //             users.Add(new SearchUserResponse() {
            //                 id = user.ID,
            //                 givenName = user.GivenName,
            //                 familyName = user.FamilyName,
            //                 fullName = user.FullName,
            //                 emailAddress = user.EmailAddress
            //             });
            //         } else {
            //             // User does not match search text.
            //             Debug.Tested();
            //         }
            //     } else {
            //         // User is soft deleted.
            //         Debug.Untested();
            //     }
            // }
            return new SearchUsersResponse() {
                users = users.ToArray()
            };
        }

        /**
         * Does a field value match the search text?
         */
        private static bool Match(string field, string lowerSearchText) {
            Debug.Tested();
            Debug.AssertValidOrNull(field);
            Debug.AssertString(lowerSearchText);

            bool retVal = false;
            if (!string.IsNullOrEmpty(field)) {
                string lowerField = field.ToLower();
                if (lowerField.Contains(lowerSearchText)) {
                    retVal = true;
                }
            }
            return retVal;
        }

        /**
         * Get full user details.
         * This includes closed (but not deleted) users.
         */
        internal static async Task<User> GetFullUserDetails(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Untested();
            Debug.AssertID(userId);

            User user = await FindUserByID(dbClient, userId, false);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Tested();
                return user;
            } else {
                Debug.Tested();
                throw new Exception(ERROR_USER_NOT_FOUND);
            }
            //??? GetUserDetailsResponse retVal = new GetUserDetailsResponse() {
            //     emailAddress = user.EmailAddress,
            //     givenName = user.GivenName,
            //     familyName = user.FamilyName,
            //     preferredName = user.PreferredName,
            //     fullName = user.FullName,
            //     dateOfBirth = APIHelper.APIDateStringFromDate(user.DateOfBirth),
            //     gender = user.Gender,
            //     address1 = user.Address1,
            //     address2 = user.Address2,
            //     address3 = user.Address3,
            //     address4 = user.Address4,
            //     city = user.City,
            //     region = user.Region,
            //     country = user.Country,
            //     postalCode = user.PostalCode,
            //     phoneNumber = user.PhoneNumber,
            //     phoneNumberVerified = APIHelper.APIDateTimeStringFromDateTime(user.PhoneNumberVerified),
            //     newEmailAddress = user.NewEmailAddress,
            //     allowNonEssentialEmails = user.AllowNonEssentialEmails,
            //     totalTicketsPurchased = user.TotalTicketsPurchased,
            //     ticketsPurchasedInCurrentGame = user.TicketsPurchasedInCurrentGame,
            //     preferredLanguage = user.PreferredLanguage,
            //     preferredCurrency = user.PreferredCurrency,
            //     preferredTimeZone = user.PreferredTimeZone,
            //     maxDailySpendingAmount = user.MaxDailySpendingAmount,
            //     newMaxDailySpendingAmount = user.NewMaxDailySpendingAmount,
            //     newMaxDailySpendingAmountTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxDailySpendingAmountTime),
            //     maxTimeLoggedIn = user.MaxTimeLoggedIn,
            //     newMaxTimeLoggedIn = user.NewMaxTimeLoggedIn,
            //     newMaxTimeLoggedInTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxTimeLoggedInTime),
            //     excludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.ExcludeUntil),
            //     newExcludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntil),
            //     newExcludeUntilTime = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntilTime)
            // };
        }

        /**
         * Check validity of update user details request inputs.
         */
        internal static void CheckValidUpdateUserDetailsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (Helper.AllFieldsRecognized(requestBody,
                                                new List<string>(new String[]{
                                                    // "emailAddress",
                                                    // "verifyEmailLinkId",
                                                    "givenName",
                                                    "familyName",
                                                    "preferredName",
                                                    "fullName",
                                                    "blocked",
                                                    "locked",
                                                    // "password",
                                                    "dateOfBirth",
                                                    "gender",
                                                    "address1",
                                                    "address2",
                                                    "address3",
                                                    "address4",
                                                    "city",
                                                    "region",
                                                    "country",
                                                    "postalCode",
                                                    "phoneNumber",
                                                    "allowNonEssentialEmails",
                                                    "preferredLanguage",
                                                    "preferredCurrency",
                                                    "preferredTimeZone"
                                                    }))) {
                    Debug.Tested();
                    return;
                } else {
                    // Unrecognised field(s)
                    Debug.Tested();
                    error = APIHelper.UNRECOGNISED_FIELD;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Update user details.
         */
        internal static async Task UpdateUserDetails(AmazonDynamoDBClient dbClient, string userId, JObject requestBody) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(requestBody);

            //??++Fail if email, EmailAddressVerified, NewEmailAddress, PasswordHash, LastLoggedIn, LastLoggedOut, IsAnonymised, ANEEOnTimestamp, ANEEOffTimestamp, TotalTicketsPurchased, TicketsPurchasedInCurrentGame, FailedLoginAttempts,
            //      KYCStatus, KCYTimestamp, MaxDailySpendingAmount etc.
            //      is specified as per specification
            //???What about Closed, Deleted
            User user = await FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Tested();
                if (requestBody["givenName"] != null) {
                    user.GivenName = (string)requestBody["givenName"];
                }
                if (requestBody["familyName"] != null) {
                    user.FamilyName = (string)requestBody["familyName"];
                }
                if (requestBody["preferredName"] != null) {
                    user.PreferredName = (string)requestBody["preferredName"];
                }
                if (requestBody["fullName"] != null) {
                    user.FullName = (string)requestBody["fullName"];
                }
                if (requestBody["blocked"] != null) {
                    user.Blocked = (bool)requestBody["blocked"];
                }
                if (requestBody["locked"] != null) {
                    user.Locked = (bool)requestBody["locked"];
                }
                if (requestBody["dateOfBirth"] != null) {
                    user.DateOfBirth = (DateTime)APIHelper.DateFromAPIDateString((string)requestBody["dateOfBirth"]);
                }
                if (requestBody["gender"] != null) {
                    user.Gender = (UInt16)requestBody["gender"];
                }
                if (requestBody["address1"] != null) {
                    user.Address1 = (string)requestBody["address1"];
                }
                if (requestBody["address2"] != null) {
                    user.Address2 = (string)requestBody["address2"];
                }
                if (requestBody["address3"] != null) {
                    user.Address3 = (string)requestBody["address3"];
                }
                if (requestBody["address4"] != null) {
                    user.Address4 = (string)requestBody["address4"];
                }
                if (requestBody["city"] != null) {
                    user.City = (string)requestBody["city"];
                }
                if (requestBody["region"] != null) {
                    user.Region = (string)requestBody["region"];
                }
                if (requestBody["country"] != null) {
                    user.Country = (string)requestBody["country"];
                }
                if (requestBody["postalCode"] != null) {
                    user.PostalCode = (string)requestBody["postalCode"];
                }
                if (requestBody["phoneNumber"] != null) {
                    user.PhoneNumber = (string)requestBody["phoneNumber"];
                }
                if (requestBody["allowNonEssentialEmails"] != null) {
                    user.AllowNonEssentialEmails = (bool)requestBody["allowNonEssentialEmails"];
                    //ANEEOnTimestamp, ANEEOffTimestamp
                }
                if (requestBody["preferredLanguage"] != null) {
                    user.PreferredLanguage = (string)requestBody["preferredLanguage"];
                }
                if (requestBody["preferredCurrency"] != null) {
                    user.PreferredCurrency = (string)requestBody["preferredCurrency"];
                }
                if (requestBody["preferredTimeZone"] != null) {
                    user.PreferredTimeZone = (string)requestBody["preferredTimeZone"];
                }
                //??++UPDATE THE DATABASE!!!
            } else {
                Debug.Tested();
                throw new Exception(ERROR_USER_NOT_FOUND);
            }
            //??? GetUserDetailsResponse retVal = new GetUserDetailsResponse() {
            //     emailAddress = user.EmailAddress,
            //     givenName = user.GivenName,
            //     familyName = user.FamilyName,
            //     preferredName = user.PreferredName,
            //     fullName = user.FullName,
            //     dateOfBirth = APIHelper.APIDateStringFromDate(user.DateOfBirth),
            //     gender = user.Gender,
            //     address1 = user.Address1,
            //     address2 = user.Address2,
            //     address3 = user.Address3,
            //     address4 = user.Address4,
            //     city = user.City,
            //     region = user.Region,
            //     country = user.Country,
            //     postalCode = user.PostalCode,
            //     phoneNumber = user.PhoneNumber,
            //     phoneNumberVerified = APIHelper.APIDateTimeStringFromDateTime(user.PhoneNumberVerified),
            //     newEmailAddress = user.NewEmailAddress,
            //     allowNonEssentialEmails = user.AllowNonEssentialEmails,
            //     totalTicketsPurchased = user.TotalTicketsPurchased,
            //     ticketsPurchasedInCurrentGame = user.TicketsPurchasedInCurrentGame,
            //     preferredLanguage = user.PreferredLanguage,
            //     preferredCurrency = user.PreferredCurrency,
            //     preferredTimeZone = user.PreferredTimeZone,
            //     maxDailySpendingAmount = user.MaxDailySpendingAmount,
            //     newMaxDailySpendingAmount = user.NewMaxDailySpendingAmount,
            //     newMaxDailySpendingAmountTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxDailySpendingAmountTime),
            //     maxTimeLoggedIn = user.MaxTimeLoggedIn,
            //     newMaxTimeLoggedIn = user.NewMaxTimeLoggedIn,
            //     newMaxTimeLoggedInTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxTimeLoggedInTime),
            //     excludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.ExcludeUntil),
            //     newExcludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntil),
            //     newExcludeUntilTime = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntilTime)
            // };
        }

        /**
         * Check validity of set user password request inputs.
         */
        internal static void CheckValidAdminSetUserPasswordRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidPassword((string)requestBody["password"], true)) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "password"
                                                        }))) {
                        return;
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    ;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user password.
         */
        internal static async Task SetUserPasswordDirect(AmazonDynamoDBClient dbClient, string userId, JToken requestBody) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(requestBody);
            Debug.AssertString((string)requestBody["password"]);

            User user = await FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Tested();
                user.PasswordHash = Helper.Hash((string)requestBody["password"]);
            } else {
                Debug.Tested();
                throw new Exception(ERROR_USER_NOT_FOUND);
            }
        }

        /**
         * Check validity of set user permissions request inputs.
         */
        internal static void CheckValidSetUserPermissionsRequest(JToken requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (requestBody["permissions"] != null) {
                    if (requestBody["permissions"].Type == JTokenType.Array) {
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "permissions"
                                                            }))) {
                            return;
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    }
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user permissions.
         */
        internal static async Task SetUserPermissions(AmazonDynamoDBClient dbClient, string userId, JToken requestBody) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(requestBody);
            Debug.AssertValid((JArray)requestBody["permissions"]);

            // Load the user
            User user = await FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Untested();
                Debug.AssertValid(user.Permissions);

                // Change the permissions
                //??--user.Permissions.RemoveAll(permission => (permission.Item1 == userId));
                user.Permissions.Clear();
                foreach (var permission in (JArray)requestBody["permissions"])
                {
                    Debug.AssertString((string)permission);
                    user.Permissions.Add((string)permission);//??--new Tuple<string, string>(userId, (string)permission));
                }
                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);

            } else {
                Debug.Untested();
                throw new Exception(ERROR_USER_NOT_FOUND);
            }
        }

        /**
         * Close user account.
         */
        internal static async Task CloseUserAccount(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);

            User user = await FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                // User exists
                Debug.Tested();
                Debug.AssertNull(user.Deleted);
                if (user.Closed == null) {
                    // User account is not already closed
                    Debug.Tested();
                    user.Closed = DateTime.Now;
                    await InvalidateUserAccessTokens(dbClient, userId);
                } else {
                    // User account is already closed
                    Debug.Untested();
                    throw new Exception(ERROR_USER_ACCOUNT_CLOSED);
                }
            } else {
                // User does not exist (or is soft deleted)
                Debug.Tested();
                throw new Exception(ERROR_USER_NOT_FOUND);
            }
        }

        /**
         * Get global settings.
         */
        //??? internal static Dictionary<string, object> GetIdentityGlobalSettings() {
        //     Debug.Tested();
        //     Debug.AssertValid(IdentityGlobalSettings);

        //     return IdentityGlobalSettings;
        // }

        /**
         * Check validity of update global settings request inputs.
         */
        internal static void CheckValidUpdateIdentityGlobalSettingsRequest(JToken requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                return;
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Update global settings.
         */
        internal static async Task UpdateIdentityGlobalSettings(AmazonDynamoDBClient dbClient,
                                                                string loggedInUserId,
                                                                JToken requestBody) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(requestBody);

            foreach (JProperty globalSetting in requestBody) {
                Debug.Tested();
                Debug.AssertValid(globalSetting);
                Debug.AssertString(globalSetting.Name);
                string name = globalSetting.Name;
                string value = globalSetting.Value.ToString();//??--Object<object>();
                await UpdateIdentityGlobalSetting(dbClient, loggedInUserId, name, value);
            }
        }

        /**
         * Update global setting.
         */
        internal static async Task UpdateIdentityGlobalSetting(AmazonDynamoDBClient dbClient, string loggedInUserId, string name, string value) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertIDOrNull(loggedInUserId);
            Debug.AssertString(name);

            if (name == GLOBAL_SETTING_SYSTEM_LOCKED) {
                // Setting 'system locked' flag.
                Debug.Tested();
                await UpdateSystemLockedGlobalSetting(dbClient, loggedInUserId, bool.Parse(value), false);
            } else {
                // Not setting 'system locked' flag.
                Debug.Tested();
                await IdentityServiceDataLayer.AddIdentityGlobalSetting(dbClient, name, value);
                //??--IdentityGlobalSettings[name] = value;
            }
        }

        /**
         * Update system locked global setting.
         */
        internal static async Task UpdateSystemLockedGlobalSetting(AmazonDynamoDBClient dbClient,
                                                                   string loggedInUserId,
                                                                   bool value,
                                                                   bool force = false) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertIDOrNull(loggedInUserId);

            // Setting 'system locked' flag.
            if (value) {
                // Setting to true (allowed)
                Debug.Tested();
                User loggedInUser = await FindUserByID(dbClient, loggedInUserId);
                Debug.AssertValidOrNull(loggedInUser);
                if (IdentityServiceLogicLayer.UserHasPermission(loggedInUser, IdentityServiceLogicLayer.PERMISSION_CAN_LOCK_SYSTEM)) {
                    Debug.Tested();
                    //??--if (!IdentityGlobalSettings.ContainsKey(GLOBAL_SETTING_SYSTEM_LOCKED) || !(bool)IdentityGlobalSettings[GLOBAL_SETTING_SYSTEM_LOCKED])
                    bool systemLocked = await GetBoolIdentityGlobalSetting(dbClient, GLOBAL_SETTING_SYSTEM_LOCKED, false);                   
                    if (!systemLocked)
                    {
                        Debug.Tested();
                        //??--IdentityGlobalSettings[GLOBAL_SETTING_SYSTEM_LOCKED] = value;
                        await IdentityServiceDataLayer.AddIdentityGlobalSetting(dbClient, GLOBAL_SETTING_SYSTEM_LOCKED, value.ToString());
                    } else {
                        Debug.Tested();
                        throw new Exception(SharedLogicLayer.ERROR_SYSTEM_ALREADY_LOCKED);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(SharedLogicLayer.ERROR_NO_PERMISSION);
                }
            } else if (force) {
                // Forcing the set to false.
                Debug.Tested();
                //??--IdentityGlobalSettings[GLOBAL_SETTING_SYSTEM_LOCKED] = value;
                await IdentityServiceDataLayer.AddIdentityGlobalSetting(dbClient, GLOBAL_SETTING_SYSTEM_LOCKED, value.ToString());
            } else {
                // Setting to false (not allowed)
                Debug.Untested();
                throw new Exception(SharedLogicLayer.ERROR_NO_PERMISSION);
            }
        }

        #endregion Admin identity service

        /**
         * User identity service methods.
         */
        #region User identity service

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
                                                        error = INVALID_ALLOW_NON_ESSENTIAL_EMAILS;
                                                    }
                                                } else {
                                                    Debug.Untested();
                                                    error = INVALID_POSTAL_CODE;
                                                }
                                            } else {
                                                Debug.Untested();
                                                error = INVALID_COUNTRY_CODE;
                                            }
                                        } else {
                                            Debug.Untested();
                                            error = INVALID_ADDRESS_1;
                                        }
                                    } else {
                                        Debug.Untested();
                                        error = INVALID_DATE_OF_BIRTH;
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
                            error = INVALID_FAMILY_NAME;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_GIVEN_NAME;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_CLIENT_ID;
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
            User user = await FindUserByEmailAddressOrNewEmailAddress(dbClient, createAccountRequest.emailAddress);
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
                await AddUserCreatingSyndicate(dbClient, user);

                // Create an email verification link
                Link link = await CreateLink(dbClient, LINK_TYPE_VERIFY_EMAIL_ADDRESS, user.ID);
                Debug.AssertValid(link);
                Debug.AssertString(link.ID);
                IdentityServiceLogicLayer.VerifyEmailLinkId = link.ID;

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
                    if (await GetBoolIdentityGlobalSetting(dbClient, GLOBAL_SETTING_USERS_CAN_REOPEN_ACCOUNT, true))
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
                        throw new Exception(ERROR_USER_ACCOUNT_CLOSED);
                    }
                } else {
                    // Account is not closed so throw an error.
                    throw new Exception(ERROR_EMAIL_IN_USE);
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

        /**
         * Check validity of login request inputs.
         */
        internal static LoginRequest CheckValidLoginRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                    if (Helper.IsValidPassword((string)requestBody["password"], false)) {
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "emailAddress",
                                                            "password"
                                                            }))) {
                            return new LoginRequest {
                                emailAddress = (string)requestBody["emailAddress"],
                                password = (string)requestBody["password"]
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
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
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Login.
         * Returns an access token if successful, along with the access token's expiry time.
         */
        internal static async Task<Tuple<string, DateTime>> Login(AmazonDynamoDBClient dbClient, LoginRequest loginRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(loginRequest);
            Debug.AssertEmail(loginRequest.emailAddress);
            Debug.AssertString(loginRequest.password);

            // Find user with email and password hash
            //??--User user = IdentityServiceLogicLayer.Users.Find(u => (u.EmailAddress == loginRequest.emailAddress));
            User user = await IdentityServiceDataLayer.FindUserByEmailAddress(dbClient, loginRequest.emailAddress);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                // User found with specified email address
                Debug.Tested();
                Debug.AssertID(user.ID);
                if (user.Closed != null) {
                    Debug.Tested();
                    throw new Exception(ERROR_USER_ACCOUNT_CLOSED);
                } else if (user.Blocked) {
                    Debug.Tested();
                    throw new Exception(ERROR_USER_BLOCKED);
                } else if (user.Locked) {
                    Debug.Tested();
                    throw new Exception(ERROR_USER_LOCKED);
                } else {
                    // User is not closed, blocked or locked.
                    Debug.Tested();
                    if (user.PasswordHash == Helper.Hash(loginRequest.password)) {
                        // Correct password - log the user in.
                        Debug.Tested();
                        // Invalidate any existing access tokens
                        await InvalidateUserAccessTokens(dbClient, user.ID);
                        // Set the last login time to now
                        user.LastLoggedIn = DateTime.Now;
                        // Mark failed login attempts as zero
                        user.FailedLoginAttempts = 0;

                        // Save the user
                        await IdentityServiceDataLayer.SaveUser(dbClient, user);

                        // Create a new access token
                        //??--Int64 accessTokenLifetime = (Int64)GetIdentityGlobalSetting(GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME, DEFAULT_ACCESS_TOKEN_LIFETIME);
                        Int64 accessTokenLifetime = await GetInt64IdentityGlobalSetting(dbClient, GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME, DEFAULT_ACCESS_TOKEN_LIFETIME);
                        AccessToken accessToken = new AccessToken() {
                            ID = RandomHelper.Next().ToString(),
                            UserID = user.ID,
                            Expires = DateTime.Now.AddSeconds(accessTokenLifetime)
                        };
                        // Setup the access token max expiry time
                        //??--Int64 maxUserLoginTime = (Int64)GetIdentityGlobalSetting(GLOBAL_SETTING_MAX_USER_LOGIN_TIME, DEFAULT_MAX_USER_LOGIN_TIME);
                        Int64 maxUserLoginTime = await GetInt64IdentityGlobalSetting(dbClient, GLOBAL_SETTING_MAX_USER_LOGIN_TIME, DEFAULT_MAX_USER_LOGIN_TIME);
                        if ((user.MaxTimeLoggedIn == null) || (user.MaxTimeLoggedIn == 0)) {
                            Debug.Tested();
                            if (maxUserLoginTime != 0) {
                                Debug.Tested();
                                accessToken.MaxExpiry = DateTime.Now.AddMinutes((UInt64)maxUserLoginTime);
                            } else {
                                Debug.Tested();
                            }
                        } else {
                            Debug.Tested();
                            if (maxUserLoginTime != 0) {
                                Debug.Untested();
                                UInt64 maxUserLoginTime_ = Math.Min((UInt64)maxUserLoginTime, (UInt64)user.MaxTimeLoggedIn);
                                accessToken.MaxExpiry = DateTime.Now.AddMinutes(maxUserLoginTime_);
                            } else {
                                Debug.Tested();
                                accessToken.MaxExpiry = DateTime.Now.AddMinutes((UInt64)user.MaxTimeLoggedIn);
                            }
                        }
                        // Ensure the max expiry has not been exceeded
                        if ((accessToken.MaxExpiry != null) && (accessToken.Expires > accessToken.MaxExpiry)) {
                            Debug.Untested();
                            accessToken.Expires = (DateTime)accessToken.MaxExpiry;
                        } else {
                            Debug.Tested();
                        }
                        // Add the access token
                        //??--AccessTokens.Add(accessToken.ID, accessToken);
                        await IdentityServiceDataLayer.AddAccessToken(dbClient, accessToken);
                        // Return the access token ID and expiry date/time
                        //??--expiryTime = accessToken.Expires;
                        return new Tuple<string, DateTime>(accessToken.ID, (DateTime)accessToken.Expires);
                    } else {
                        // Incorrect password
                        Debug.Tested();
                        //??--Int16 maxLoginAttempts = (Int16)GetIdentityGlobalSetting(GLOBAL_SETTING_LOCK_ON_FAILED_LOGIN_ATTEMPTS, DEFAULT_MAX_LOGIN_ATTEMPTS);
                        Int64 maxLoginAttempts = await GetInt64IdentityGlobalSetting(dbClient, GLOBAL_SETTING_LOCK_ON_FAILED_LOGIN_ATTEMPTS, DEFAULT_MAX_LOGIN_ATTEMPTS);
                        if (++user.FailedLoginAttempts == maxLoginAttempts) {
                            // Too many password attempts - user locked.
                            Debug.Tested();
                            user.Locked = true;

                            // Save the user
                            await IdentityServiceDataLayer.SaveUser(dbClient, user);

                            throw new Exception(ERROR_USER_LOCKED);
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_INCORRECT_PASSWORD);
                        }
                    }
                }
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS);
            }
        }

        /**
         * Check validity of check password request inputs.
         */
        internal static CheckPasswordRequest CheckValidCheckPasswordRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidPassword((string)requestBody["password"], false)) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "password"
                                                        }))) {
                        return new CheckPasswordRequest {
                            password = (string)requestBody["password"]
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                    error = APIHelper.INVALID_PASSWORD;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Check password.
         */
        internal static async Task CheckPassword(AmazonDynamoDBClient dbClient, CheckPasswordRequest checkPasswordRequest, string loggedInUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(checkPasswordRequest);
            Debug.AssertString(checkPasswordRequest.password);
            Debug.AssertID(loggedInUserId);

            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);
            if (user.Locked) {
                // The user is locked.
                // This code should never be called as locked users cannot login.
                Debug.Unreachable();
                throw new Exception(ERROR_USER_LOCKED);
            } else if (user.Blocked) {
                // The user is blocked.
                // This code should never be called as blocked users cannot login.
                Debug.Unreachable();
                throw new Exception(ERROR_USER_BLOCKED);
            } else if (user.PasswordHash != Helper.Hash(checkPasswordRequest.password)) {
                Debug.Tested();
                throw new Exception(ERROR_INCORRECT_PASSWORD);
            } else {
                Debug.Tested();
            }
        }

        /**
         * Logout.
         */
        internal static async Task Logout(AmazonDynamoDBClient dbClient, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes
            user.LastLoggedOut = DateTime.Now;

            // Save the user
            await IdentityServiceDataLayer.SaveUser(dbClient, user);

            //
            await InvalidateUserAccessTokens(dbClient, loggedInUserId);
        }

        /**
         * Refresh access token.
         * The specified user ID must exist.
         */
        internal static async Task<DateTime?> RefreshAccessToken(AmazonDynamoDBClient dbClient, string loggedInUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);

            DateTime? retVal = null;
            AccessToken accessToken = await FindAccessTokenByUserID(dbClient, loggedInUserId);
            Debug.AssertValidOrNull(accessToken);
            if (accessToken != null)
            {
                //??--Int64 accessTokenLifetime = (Int64)GetIdentityGlobalSetting(GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME, DEFAULT_ACCESS_TOKEN_LIFETIME);
                Int64 accessTokenLifetime = await GetInt64IdentityGlobalSetting(dbClient, GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME, DEFAULT_ACCESS_TOKEN_LIFETIME);
                if (accessToken.MaxExpiry == null)
                {
                    accessToken.Expires = DateTime.Now.AddSeconds(accessTokenLifetime);
                }
                else
                {
                    if (accessToken.Expires == accessToken.MaxExpiry)
                    {
                        throw new Exception(ERROR_CANNOT_EXTEND_ACCESS_TOKEN);
                    }
                    else
                    {
                        // Extend the expiry time.
                        accessToken.Expires = DateTime.Now.AddSeconds(accessTokenLifetime);
                        // Ensure the max expiry has not been exceeded
                        if (accessToken.Expires > accessToken.MaxExpiry)
                        {
                            Debug.Untested();
                            accessToken.Expires = (DateTime)accessToken.MaxExpiry;
                        }
                        else
                        {
                            Debug.Untested();
                        }
                    }
                }
                retVal = accessToken.Expires;
            }
            else
            {
                Debug.Untested();
            }
            return retVal;
        }

        /**
         * Get user details.
         */
        internal static async Task<GetUserDetailsResponse> GetUserDetails(AmazonDynamoDBClient dbClient, string loggedInUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);

            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);
            GetUserDetailsResponse retVal = new GetUserDetailsResponse() {
                emailAddress = user.EmailAddress,
                givenName = user.GivenName,
                familyName = user.FamilyName,
                preferredName = user.PreferredName,
                fullName = user.FullName,
                dateOfBirth = APIHelper.APIDateStringFromDate(user.DateOfBirth),
                gender = user.Gender,
                address1 = user.Address1,
                address2 = user.Address2,
                address3 = user.Address3,
                address4 = user.Address4,
                city = user.City,
                region = user.Region,
                country = user.Country,
                postalCode = user.PostalCode,
                phoneNumber = user.PhoneNumber,
                phoneNumberVerified = APIHelper.APIDateTimeStringFromDateTime(user.PhoneNumberVerified),
                newEmailAddress = user.NewEmailAddress,
                allowNonEssentialEmails = user.AllowNonEssentialEmails,
                totalTicketsPurchased = user.TotalTicketsPurchased,
                ticketsPurchasedInCurrentGame = user.TicketsPurchasedInCurrentGame,
                preferredLanguage = user.PreferredLanguage,
                preferredCurrency = user.PreferredCurrency,
                preferredTimeZone = user.PreferredTimeZone,
                maxDailySpendingAmount = user.MaxDailySpendingAmount,
                newMaxDailySpendingAmount = user.NewMaxDailySpendingAmount,
                newMaxDailySpendingAmountTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxDailySpendingAmountTime),
                maxTimeLoggedIn = user.MaxTimeLoggedIn,
                newMaxTimeLoggedIn = user.NewMaxTimeLoggedIn,
                newMaxTimeLoggedInTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxTimeLoggedInTime),
                excludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.ExcludeUntil),
                newExcludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntil),
                newExcludeUntilTime = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntilTime)
            };
            return retVal;
        }

        /**
         * Get user permissions.
         */
        internal static async Task<string[]> GetUserPermissions(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);

            // Load the user
            User user = await FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                // User exists (account may be open or closed)
                Debug.Tested();
                Debug.AssertValid(user.Permissions);
                //??-- List<string> permissions = new List<string>();
                // foreach (var permission in Permissions) {
                //     Debug.Tested();
                //     Debug.AssertValid(permission);
                //     Debug.AssertID(permission.Item1);
                //     Debug.AssertString(permission.Item2);
                //     if (permission.Item1 == userId) {
                //         Debug.Tested();
                //         permissions.Add(permission.Item2);
                //     } else {
                //         Debug.Tested();
                //     }
                // }
                return user.Permissions.ToArray();
            } else {
                // User does not exist (or is soft deleted)
                Debug.Tested();
                throw new Exception(ERROR_USER_NOT_FOUND);
            }
        }

        /**
         * Check validity of close account request inputs.
         */
        internal static CloseAccountRequest CheckValidCloseAccountRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidPassword((string)requestBody["password"], false)) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "password"
                                                        }))) {
                        return new CloseAccountRequest {
                            password = (string)requestBody["password"]
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                    error = APIHelper.INVALID_PASSWORD;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Close account.
         */
        internal static async Task CloseAccount(AmazonDynamoDBClient dbClient, string loggedInUserId, CloseAccountRequest closeAccountRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(closeAccountRequest);
            Debug.AssertString(closeAccountRequest.password);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);
            Debug.AssertNull(user.Closed);

            // Check the password
            if (user.PasswordHash == Helper.Hash(closeAccountRequest.password)) {

                // Make changes
                user.Closed = DateTime.Now;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);

                // Actually log user out
                await InvalidateUserAccessTokens(dbClient, loggedInUserId);
            } else {
                throw new Exception(ERROR_INCORRECT_PASSWORD);
            }
        }

        /**
         * Check validity of set user name request inputs.
         */
        internal static SetUserNameRequest CheckValidSetUserNameRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!String.IsNullOrEmpty((string)requestBody["givenName"])) {
                    if (!String.IsNullOrEmpty((string)requestBody["familyName"])) {
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "givenName",
                                                            "familyName",
                                                            "fullName",
                                                            "preferredName"
                                                            }))) {
                            return new SetUserNameRequest {
                                givenName = (string)requestBody["givenName"],
                                familyName = (string)requestBody["familyName"],
                                fullName = (string)requestBody["fullName"],
                                preferredName = (string)requestBody["preferredName"]
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_FAMILY_NAME;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_GIVEN_NAME;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user name.
         */
        internal static async Task SetUserName(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserNameRequest setUserNameRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserNameRequest);
            Debug.AssertString(setUserNameRequest.givenName);
            Debug.AssertString(setUserNameRequest.familyName);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if ((user.GivenName != setUserNameRequest.givenName) ||
                (user.FamilyName != setUserNameRequest.familyName) ||
                (user.FullName != setUserNameRequest.fullName) ||
                (user.PreferredName != setUserNameRequest.preferredName))
            {
                user.GivenName = setUserNameRequest.givenName;
                user.FamilyName = setUserNameRequest.familyName;
                user.FullName = setUserNameRequest.fullName;
                user.PreferredName = setUserNameRequest.preferredName;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

        /**
         * Check validity of set user email request inputs.
         */
        internal static SetUserEmailRequest CheckValidSetUserEmailRequest(JObject requestBody) {
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
        internal static async Task SetUserEmail(AmazonDynamoDBClient dbClient, string userId, SetUserEmailRequest setUserEmailRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(setUserEmailRequest);
            Debug.AssertEmail(setUserEmailRequest.emailAddress);

            // Check that the specified email address is not already in use.
            User user = await FindUserByEmailAddressOrNewEmailAddress(dbClient, setUserEmailRequest.emailAddress);
            Debug.AssertValidOrNull(user);
            if (user != null)
            {
                Debug.Untested();
                if (user.ID != userId)
                {
                    Debug.Untested();
                    throw new Exception(ERROR_EMAIL_IN_USE);
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
                user = await FindUserByID(dbClient, userId, true);
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
                        Link link = await CreateLink(dbClient, LINK_TYPE_VERIFY_EMAIL_ADDRESS, user.ID);
                        Debug.AssertValid(link);
                        Debug.AssertString(link.ID);
                        //??++SEND VERIFICATION EMAIL?
                        IdentityServiceLogicLayer.VerifyEmailLinkId = link.ID;
                        LoggingHelper.LogMessage($"EMAIL VERIFICATION LINK ID: {link.ID}");
                    } else {
                        // The email address being changed to is the same as the existing one.
                        Debug.Tested();
                    }
                } else if (user.NewEmailAddress != setUserEmailRequest.emailAddress) {
                    // The user's email address is already being changed but to a different address than the specified value.
                    Debug.Tested();
                    throw new Exception(ERROR_EMAIL_ALREADY_BEING_CHANGED);
                } else {
                    // The user's email address is already being changed to the specified value.
                    Debug.Tested();
                }
            } else {
                // User does not exist (or is closed or soft deleted)
                Debug.Tested();
                throw new Exception(ERROR_USER_NOT_FOUND);
            }
        }

        /**
         * Check validity of set user password request inputs.
         */
        internal static SetUserPasswordRequest CheckValidSetUserPasswordRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidPassword((string)requestBody["oldPassword"], false)) {
                    if (Helper.IsValidPassword((string)requestBody["newPassword"], true)) {//??++WHAT TO DO IF PASSWORDS SAME?
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "oldPassword",
                                                            "newPassword"
                                                            }))) {
                            return new SetUserPasswordRequest {
                                oldPassword = (string)requestBody["oldPassword"],
                                newPassword = (string)requestBody["newPassword"]
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_NEW_PASSWORD;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_OLD_PASSWORD;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user password.
         */
        internal static async Task SetUserPassword(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserPasswordRequest setUserPasswordRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserPasswordRequest);
            Debug.AssertString(setUserPasswordRequest.oldPassword);
            Debug.AssertString(setUserPasswordRequest.newPassword);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Check password
            if (user.PasswordHash == Helper.Hash(setUserPasswordRequest.oldPassword)) {

                // Make changes (if necessary)
                string newPasswordHash = Helper.Hash(setUserPasswordRequest.newPassword);
                if (user.PasswordHash != newPasswordHash)
                {
                    user.PasswordHash = newPasswordHash;

                    // Save the user
                    await IdentityServiceDataLayer.SaveUser(dbClient, user);
                }
            } else {
                throw new Exception(ERROR_INCORRECT_PASSWORD);
            }
        }

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
                            error = INVALID_GENDER;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_GENDER;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_GENDER;
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
            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if (user.Gender != setUserGenderRequest.gender)
            {
                user.Gender = setUserGenderRequest.gender;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

        /**
         * Check validity of set user address request inputs.
         */
        internal static SetUserAddressRequest CheckValidSetUserAddressRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!String.IsNullOrEmpty((string)requestBody["address1"])) {
                    if (Helper.IsValidCountryCode((string)requestBody["country"])) {
                        if (!String.IsNullOrEmpty((string)requestBody["postalCode"])) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "address1",
                                                                "address2",
                                                                "address3",
                                                                "address4",
                                                                "city",
                                                                "region",
                                                                "country",
                                                                "postalCode"
                                                                }))) {
                                return new SetUserAddressRequest {
                                    address1 = (string)requestBody["address1"],
                                    address2 = (string)requestBody["address2"],
                                    address3 = (string)requestBody["address3"],
                                    address4 = (string)requestBody["address4"],
                                    city = (string)requestBody["city"],
                                    region = (string)requestBody["region"],
                                    country = (string)requestBody["country"],
                                    postalCode = (string)requestBody["postalCode"]
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_POSTAL_CODE;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_COUNTRY_CODE;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_ADDRESS_1;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user address.
         */
        internal static async Task SetUserAddress(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserAddressRequest setUserAddressRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserAddressRequest);
            Debug.AssertString(setUserAddressRequest.address1);
            Debug.AssertString(setUserAddressRequest.country);
            Debug.AssertString(setUserAddressRequest.postalCode);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if ((user.Address1 != setUserAddressRequest.address1) ||
                (user.Address2 != setUserAddressRequest.address2) ||
                (user.Address3 != setUserAddressRequest.address3) ||
                (user.Address4 != setUserAddressRequest.address4) ||
                (user.City != setUserAddressRequest.city) ||
                (user.Region != setUserAddressRequest.region) ||
                (user.Country != setUserAddressRequest.country) ||
                (user.PostalCode != setUserAddressRequest.postalCode))
            {
                user.Address1 = setUserAddressRequest.address1;
                user.Address2 = setUserAddressRequest.address2;
                user.Address3 = setUserAddressRequest.address3;
                user.Address4 = setUserAddressRequest.address4;
                user.City = setUserAddressRequest.city;
                user.Region = setUserAddressRequest.region;
                user.Country = setUserAddressRequest.country;
                user.PostalCode = setUserAddressRequest.postalCode;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

        /**
         * Check validity of set user phone number request inputs.
         */
        internal static SetUserPhoneNumberRequest CheckValidSetUserPhoneNumberRequest(JObject requestBody) {
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
                    error = INVALID_PHONE_NUMBER;
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
        internal static async Task SetUserPhoneNumber(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserPhoneNumberRequest setUserPhoneNumberRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserPhoneNumberRequest);
            Debug.AssertString(setUserPhoneNumberRequest.phoneNumber);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
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
                Link link = await CreateLink(dbClient, LINK_TYPE_VERIFY_PHONE_NUMBER, loggedInUserId, oneTimePassword);
                Debug.AssertValid(link);
                //??++SMS OTP
                LoggingHelper.LogMessage($"PHONE NUMBER VERIFICATION LINK ID: {link.ID}");
            }
        }

        /**
         * Check validity of set user allow non-essential emails request inputs.
         */
        internal static SetUserAllowNonEssentialEmailsRequest CheckValidSetUserAllowNonEssentialEmailsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                bool ?allowNonEssentialEmails = null;
                if (APIHelper.RequestBodyContainsField(requestBody, "allowNonEssentialEmails", out JToken allowNonEssentialEmailsField)) {
                    if (allowNonEssentialEmailsField.Type == JTokenType.Boolean) {
                        allowNonEssentialEmails = (bool)allowNonEssentialEmailsField;
                    } else if (allowNonEssentialEmailsField.Type == JTokenType.String) {
                        if (bool.TryParse((string)allowNonEssentialEmailsField, out bool allowNonEssentialEmails_)) {
                            allowNonEssentialEmails = allowNonEssentialEmails_;
                        }
                    }
                } else {
                    allowNonEssentialEmails = false;
                }
                if (allowNonEssentialEmails != null) {
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "allowNonEssentialEmails"
                                                        }))) {
                        return new SetUserAllowNonEssentialEmailsRequest {
                            allowNonEssentialEmails = (bool)allowNonEssentialEmails
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_ALLOW_NON_ESSENTIAL_EMAILS;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user allow non-essential emails.
         */
        internal static async Task SetUserAllowNonEssentialEmails(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserAllowNonEssentialEmailsRequest setUserAllowNonEssentialEmailsRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserAllowNonEssentialEmailsRequest);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if (user.AllowNonEssentialEmails != setUserAllowNonEssentialEmailsRequest.allowNonEssentialEmails)
            {
                user.AllowNonEssentialEmails = setUserAllowNonEssentialEmailsRequest.allowNonEssentialEmails;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

        /**
         * Check validity of set user preferences request inputs.
         */
        internal static SetUserPreferencesRequest CheckValidSetUserPreferencesRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!APIHelper.RequestBodyContainsField(requestBody, "preferredLanguage", out JToken preferredLanguageField) || (preferredLanguageField.Type == JTokenType.Null) || Helper.IsValidLanguageCode((string)preferredLanguageField)) {
                    if (!APIHelper.RequestBodyContainsField(requestBody, "preferredCurrency", out JToken preferredCurrencyField) || (preferredCurrencyField.Type == JTokenType.Null) || Helper.IsValidCurrencyCode((string)preferredCurrencyField)) {
                        if (!APIHelper.RequestBodyContainsField(requestBody, "preferredTimeZone", out JToken preferredTimeZoneField) || (preferredTimeZoneField.Type == JTokenType.Null) || Helper.IsValidTimeZone((string)preferredTimeZoneField)) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "preferredLanguage",
                                                                "preferredCurrency",
                                                                "preferredTimeZone"
                                                                }))) {
                                return new SetUserPreferencesRequest {
                                    preferredLanguage = (string)preferredLanguageField,
                                    preferredCurrency = (string)preferredCurrencyField,
                                    preferredTimeZone = (string)preferredTimeZoneField
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_PREFERRED_TIME_ZONE;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_PREFERRED_CURRENCY;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_PREFERRED_LANGUAGE;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user preferences.
         */
        internal static async Task SetUserPreferences(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserPreferencesRequest setUserPreferencesRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserPreferencesRequest);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if ((user.PreferredLanguage != setUserPreferencesRequest.preferredLanguage) ||
                (user.PreferredCurrency != setUserPreferencesRequest.preferredCurrency) ||
                (user.PreferredTimeZone != setUserPreferencesRequest.preferredTimeZone))
            {
                user.PreferredLanguage = setUserPreferencesRequest.preferredLanguage;
                user.PreferredCurrency = setUserPreferencesRequest.preferredCurrency;
                user.PreferredTimeZone = setUserPreferencesRequest.preferredTimeZone;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

        /**
         * Check validity of set user limits request inputs.
         */
        internal static SetUserLimitsRequest CheckValidSetUserLimitsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                bool validMaxDailySpendingAmount = false;
                UInt32? maxDailySpendingAmount = null;
                if (!APIHelper.RequestBodyContainsField(requestBody, "maxDailySpendingAmount", out JToken maxDailySpendingAmountField) || (maxDailySpendingAmountField.Type == JTokenType.Null)) {
                    validMaxDailySpendingAmount = true;
                } else if (maxDailySpendingAmountField.Type == JTokenType.Integer) {
                    validMaxDailySpendingAmount = true;
                    maxDailySpendingAmount = (UInt32)maxDailySpendingAmountField;
                }
                if (validMaxDailySpendingAmount) {
                    bool validMaxTimeLoggedIn = false;
                    UInt64? maxTimeLoggedIn = null;
                    if (!APIHelper.RequestBodyContainsField(requestBody, "maxTimeLoggedIn", out JToken maxTimeLoggedInField) || (maxTimeLoggedInField.Type == JTokenType.Null)) {
                        validMaxTimeLoggedIn = true;
                    } else if (maxTimeLoggedInField.Type == JTokenType.Integer) {
                        validMaxTimeLoggedIn = true;
                        maxTimeLoggedIn = (UInt64)maxTimeLoggedInField;
                    }
                    if (validMaxTimeLoggedIn) {
                        bool validExcludeUntil = false;
                        string excludeUntil = null;
                        if (!APIHelper.RequestBodyContainsField(requestBody, "excludeUntil", out JToken excludeUntilField) || (excludeUntilField.Type == JTokenType.Null)) {
                            validExcludeUntil = true;
                        } else if (excludeUntilField.Type == JTokenType.Date) {
                            validExcludeUntil = true;
                            excludeUntil = APIHelper.APIDateTimeStringFromDateTime((DateTime)excludeUntilField);
                        } else if (APIHelper.IsValidAPIDateTimeString((string)excludeUntilField)) {
                            validExcludeUntil = true;
                            excludeUntil = (string)excludeUntilField;
                        }
                        if (validExcludeUntil) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "maxDailySpendingAmount",
                                                                "maxTimeLoggedIn",
                                                                "excludeUntil"
                                                                }))) {
                                return new SetUserLimitsRequest {
                                    maxDailySpendingAmount = maxDailySpendingAmount,
                                    maxTimeLoggedIn = maxTimeLoggedIn,
                                    excludeUntil = excludeUntil
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_EXCLUDE_UNTIL;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_MAX_TIME_LOGGED_IN;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_MAX_DAILY_SPENDING_AMOUNT;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user limits.
         */
        internal static async Task SetUserLimits(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserLimitsRequest setUserLimitsRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserLimitsRequest);

            if ((setUserLimitsRequest.maxDailySpendingAmount != null) ||
                (setUserLimitsRequest.maxTimeLoggedIn != null) ||
                (setUserLimitsRequest.excludeUntil != null)) {
                // At least one of the limits may be being changed
                Debug.Tested();
                
                // Load the user
                User user = await FindUserByID(dbClient, loggedInUserId);
                Debug.AssertValid(user);

                // Make changes
                SetUserMaxDailySpendingAmount(user, setUserLimitsRequest);
                SetUserMaxTimeLoggedIn(user, setUserLimitsRequest);
                SetUserExcludeUntil(user, setUserLimitsRequest);

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            } else {
                Debug.Tested();
            }
        }

        /**
         * Set user max daily spending amount.
         */
        private static void SetUserMaxDailySpendingAmount(User user, SetUserLimitsRequest setUserLimitsRequest) {
            Debug.Tested();
            Debug.AssertValid(user);
            Debug.AssertValid(setUserLimitsRequest);

            if (setUserLimitsRequest.maxDailySpendingAmount != null) {
                // Max daily spending amount is being changed
                Debug.Tested();
                if (user.MaxDailySpendingAmount == null) {
                    // Max daily spending amount not set yet
                    Debug.Tested();
                    user.MaxDailySpendingAmount = setUserLimitsRequest.maxDailySpendingAmount;
                } else if (setUserLimitsRequest.maxDailySpendingAmount <= user.MaxDailySpendingAmount) {
                    // Max daily spending amount set and is being reduced (or staying the same)
                    Debug.Tested();
                    user.MaxDailySpendingAmount = setUserLimitsRequest.maxDailySpendingAmount;
                    user.NewMaxDailySpendingAmount = null;
                    user.NewMaxDailySpendingAmountTime = null;
                } else {
                    // Max daily spending amount set and is being increased
                    Debug.Tested();
                    user.NewMaxDailySpendingAmount = setUserLimitsRequest.maxDailySpendingAmount;
                    user.NewMaxDailySpendingAmountTime = DateTime.Now.AddDays(7);
                }
            } else {
                // Max daily spending amount is not being changed
                Debug.Tested();
            }
        }

        /**
         * Set user max time logged in.
         */
        private static void SetUserMaxTimeLoggedIn(User user, SetUserLimitsRequest setUserLimitsRequest) {
            Debug.Tested();
            Debug.AssertValid(user);
            Debug.AssertValid(setUserLimitsRequest);

            if (setUserLimitsRequest.maxTimeLoggedIn != null) {
                // Max time logged in is being changed
                Debug.Tested();
                if (user.MaxTimeLoggedIn == null) {
                    // Max time logged in not set yet
                    Debug.Tested();
                    user.MaxTimeLoggedIn = setUserLimitsRequest.maxTimeLoggedIn;
                } else if (setUserLimitsRequest.maxTimeLoggedIn <= user.MaxTimeLoggedIn) {
                    // Max time logged in set and is being reduced (or staying the same)
                    Debug.Tested();
                    user.MaxTimeLoggedIn = setUserLimitsRequest.maxTimeLoggedIn;
                    user.NewMaxTimeLoggedIn = null;
                    user.NewMaxTimeLoggedInTime = null;
                } else {
                    // Max time logged in set and is being increased
                    Debug.Tested();
                    user.NewMaxTimeLoggedIn = setUserLimitsRequest.maxTimeLoggedIn;
                    user.NewMaxTimeLoggedInTime = DateTime.Now.AddDays(7);
                }
            } else {
                // Max time logged in is not being changed
                Debug.Tested();
            }
        }
        
        /**
         * Set user exclude until.
         */
        private static void SetUserExcludeUntil(User user, SetUserLimitsRequest setUserLimitsRequest) {
            Debug.Tested();
            Debug.AssertValid(user);
            Debug.AssertValid(setUserLimitsRequest);

            if (setUserLimitsRequest.excludeUntil != null) {
                // Exclude until time is being changed
                Debug.Tested();
                DateTime excludeUntil = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(setUserLimitsRequest.excludeUntil);
                if (user.ExcludeUntil == null) {
                    // Exclude until time not set yet
                    Debug.Tested();
                    user.ExcludeUntil = excludeUntil;
                } else if (user.ExcludeUntil <= excludeUntil) {
                    // Exclude until time set and is being increased (or staying the same)
                    Debug.Tested();
                    user.ExcludeUntil = excludeUntil;
                    user.NewExcludeUntil = null;
                    user.NewExcludeUntilTime = null;
                } else {
                    // Exclude until time set and is being reduced
                    Debug.Tested();
                    user.NewExcludeUntil = excludeUntil;
                    user.NewExcludeUntilTime = DateTime.Now.AddDays(7);
                }
            } else {
                // Exclude until time is not being changed
                Debug.Tested();
            }
        }
        
        /**
         * Check validity of reset user password request inputs.
         */
        internal static ResetUserPasswordRequest CheckValidResetUserPasswordRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                    if (APIHelper.GetStringFromRequestBody(requestBody, "reCaptcha", out string reCaptcha, true)) {
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "emailAddress"
                                                            }))) {
                            return new ResetUserPasswordRequest {
                                emailAddress = (string)requestBody["emailAddress"]
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = APIHelper.INVALID_RECAPTCHA;
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
         * Reset user password.
         */
        internal static async Task ResetUserPassword(AmazonDynamoDBClient dbClient, ResetUserPasswordRequest resetUserPasswordRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(resetUserPasswordRequest);
            Debug.AssertString(resetUserPasswordRequest.emailAddress);

            // Load the user
            User user = await IdentityServiceDataLayer.FindUserByEmailAddress(dbClient, resetUserPasswordRequest.emailAddress);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                // User with email address exists
                Debug.Tested();
                Debug.AssertID(user.ID);
                if (user.EmailAddressVerified != null)
                {
                    Debug.Tested();
                    await DoResetUserPassword(dbClient, user);
                }
                else
                {
                    //??--bool failForgotPasswordIfEmailNotVerified = (bool)GetIdentityGlobalSetting(GLOBAL_SETTING_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED, DEFAULT_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED);
                    bool failForgotPasswordIfEmailNotVerified = await GetBoolIdentityGlobalSetting(dbClient, GLOBAL_SETTING_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED, DEFAULT_FAIL_FORGOT_PASSWORD_IF_EMAIL_NOT_VERIFIED);
                    if (!failForgotPasswordIfEmailNotVerified)
                    {
                        Debug.Tested();
                        await DoResetUserPassword(dbClient, user);
                    }
                    else
                    {
                        Debug.Tested();
                        throw new Exception(ERROR_EMAIL_NOT_VERIFIED);
                    }
                }
            } else {
                // User with email address not found
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS);
            }
        }

        /**
         * Reset user password.
         */
        private static async Task DoResetUserPassword(AmazonDynamoDBClient dbClient, User user) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(user);

            // Email the reset password link
            Link link = await CreateLink(dbClient, LINK_TYPE_RESET_PASSWORD, user.ID);
            Debug.AssertValid(link);
            Debug.AssertString(link.ID);
            //??++EMAIL LINK ID TO ADDRESS

            // Debug code
            IdentityServiceLogicLayer.ResetPasswordLinkId = link.ID;
            LoggingHelper.LogMessage($"PASSWORD RESET LINK ID: {link.ID}");
        }

        /**
         * Check validity of set user password after reset request inputs.
         */
        internal static SetUserPasswordAfterResetRequest CheckValidSetUserPasswordAfterResetRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!String.IsNullOrEmpty((string)requestBody["resetPasswordLinkId"])) {
                    if (Helper.IsValidEmail((string)requestBody["emailAddress"])) {
                        if (Helper.IsValidPassword((string)requestBody["newPassword"], true)) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "resetPasswordLinkId",
                                                                "emailAddress",
                                                                "newPassword"
                                                                }))) {
                                return new SetUserPasswordAfterResetRequest {
                                    resetPasswordLinkId = (string)requestBody["resetPasswordLinkId"],
                                    emailAddress = (string)requestBody["emailAddress"],
                                    newPassword = (string)requestBody["newPassword"]
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_NEW_PASSWORD;
                        }
                    } else {
                        Debug.Untested();
                        error = APIHelper.INVALID_EMAIL_ADDRESS;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_RESET_PASSWORD_LINK_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user password after reset.
         */
        internal static async Task SetUserPasswordAfterReset(AmazonDynamoDBClient dbClient, SetUserPasswordAfterResetRequest setUserPasswordAfterResetRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(setUserPasswordAfterResetRequest);
            Debug.AssertString(setUserPasswordAfterResetRequest.resetPasswordLinkId);
            Debug.AssertEmail(setUserPasswordAfterResetRequest.emailAddress);
            Debug.AssertPassword(setUserPasswordAfterResetRequest.newPassword);

            // Find the valid link
            Link link = await FindValidLink(dbClient, setUserPasswordAfterResetRequest.resetPasswordLinkId, LINK_TYPE_RESET_PASSWORD);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                // Valid link exists
                Debug.Tested();
                Debug.AssertID(link.UserID);
    
                // Load the user
                User user = await FindUserByID(dbClient, link.UserID, true);
                Debug.AssertValidOrNull(user);
                if (user != null) {
                    Debug.Tested();
                    Debug.AssertEmail(user.EmailAddress);
                    if (user.EmailAddress == setUserPasswordAfterResetRequest.emailAddress) {
                        // Email address matches user's email address
                        Debug.Tested();

                        // Make changes
                        user.PasswordHash = Helper.Hash(setUserPasswordAfterResetRequest.newPassword);
                        user.Locked = false;

                        // Save the user
                        await IdentityServiceDataLayer.SaveUser(dbClient, user);

                        // Revoke link
                        link.Revoked = true;
                        //??++SAVE LINK
                    } else {
                        Debug.Tested();
                        throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS, new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS));
                    }
                } else {
                    // User does not exist - may have been closed (and possibly subsequently deleted).
                    Debug.Tested();
                    throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER, new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER));
                }
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK, new Exception(SharedLogicLayer.ERROR_INVALID_LINK));
            }
        }

        /**
         * Check validity of resend email verification request inputs.
         */
        internal static ResendEmailVerificationRequest CheckValidResendEmailVerificationRequest(JObject requestBody) {
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
        internal static async Task ResendEmailVerification(AmazonDynamoDBClient dbClient, ResendEmailVerificationRequest resendEmailVerificationRequest)
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
                    throw new Exception(ERROR_EMAIL_ALREADY_VERIFIED, new Exception(EMAIL_ALREADY_VERIFIED));
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
            await RevokeUserLinks(dbClient, user.ID, LINK_TYPE_VERIFY_EMAIL_ADDRESS);

            // Create a new link
            Link link = await CreateLink(dbClient, LINK_TYPE_VERIFY_EMAIL_ADDRESS, user.ID);
            Debug.AssertValid(link);
            Debug.AssertString(link.ID);

            // Email the new link
            Dictionary<string, string> replacementFields = new Dictionary<string, string>();
            replacementFields["link"] = link.ID;
            EmailHelper.EmailTemplate(EmailHelper.EMAIL_TEMPLATE_EMAIL_VERIFICATION, user.EmailAddress, replacementFields);

            // Debug code
            IdentityServiceLogicLayer.VerifyEmailLinkId = link.ID;
            LoggingHelper.LogMessage($"EMAIL VERIFICATION LINK ID: {link.ID}");
        }

        /**
         * Check validity of verify email request inputs.
         */
        internal static VerifyEmailRequest CheckValidVerifyEmailRequest(JObject requestBody) {
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
                        error = INVALID_VERIFY_EMAIL_LINK_ID;
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
        internal static async Task VerifyEmail(AmazonDynamoDBClient dbClient, VerifyEmailRequest verifyEmailRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(verifyEmailRequest);
            Debug.AssertEmail(verifyEmailRequest.emailAddress);
            Debug.AssertString(verifyEmailRequest.verifyEmailLinkId);

            // Find a valid link
            Link link = await FindValidLink(dbClient, verifyEmailRequest.verifyEmailLinkId, LINK_TYPE_VERIFY_EMAIL_ADDRESS);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                // Valid link exits
                Debug.Tested();
                Debug.AssertID(link.UserID);

                // Find user
                User user = await FindUserByID(dbClient, link.UserID, true);
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
                            throw new Exception(ERROR_EMAIL_ALREADY_VERIFIED, new Exception(EMAIL_ALREADY_VERIFIED));
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

        /**
         * Check validity of verify email with details request inputs.
         */
        internal static VerifyEmailWithDetailsRequest CheckValidVerifyEmailWithDetailsRequest(JObject requestBody) {
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
                                                        error = INVALID_ALLOW_NON_ESSENTIAL_EMAILS;
                                                    }
                                                } else {
                                                    Debug.Untested();
                                                    error = INVALID_POSTAL_CODE;
                                                }
                                            } else {
                                                Debug.Untested();
                                                error = INVALID_COUNTRY_CODE;
                                            }
                                        } else {
                                            Debug.Untested();
                                            error = INVALID_ADDRESS_1;
                                        }
                                    } else {
                                        Debug.Untested();
                                        error = INVALID_DATE_OF_BIRTH;
                                    }
                                } else {
                                    Debug.Untested();
                                    error = APIHelper.INVALID_PASSWORD;
                                }
                            } else {
                                Debug.Untested();
                                error = INVALID_FAMILY_NAME;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_GIVEN_NAME;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_VERIFY_EMAIL_LINK_ID;
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
        internal static async Task VerifyEmailWithDetails(AmazonDynamoDBClient dbClient, VerifyEmailWithDetailsRequest verifyEmailWithDetailsRequest) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(verifyEmailWithDetailsRequest);
            Debug.AssertString(verifyEmailWithDetailsRequest.verifyEmailLinkId);
            Debug.AssertEmail(verifyEmailWithDetailsRequest.emailAddress);

            // Find a valid link
            Link link = await FindValidLink(dbClient, verifyEmailWithDetailsRequest.verifyEmailLinkId, LINK_TYPE_VERIFY_EMAIL_ADDRESS);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                // Valid link exits
                Debug.Tested();
                Debug.AssertID(link.UserID);

                // Find user
                User user = await FindUserByID(dbClient, link.UserID);
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
                        throw new Exception(ERROR_EMAIL_ALREADY_VERIFIED, new Exception(EMAIL_ALREADY_VERIFIED));
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

        /**
         * Resend phone number verification.
         */
        internal static async Task ResendPhoneNumberVerification(AmazonDynamoDBClient dbClient, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);

            // Load the user
            User user = await FindUserByID(dbClient, loggedInUserId);
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
                    await RevokeUserLinks(dbClient, loggedInUserId, LINK_TYPE_VERIFY_PHONE_NUMBER);

                    // Create and send new 'verify phone number' link
                    string oneTimePassword = "<OTP>";//??++GENERATE OTP
                    Link link = await CreateLink(dbClient, LINK_TYPE_VERIFY_PHONE_NUMBER, loggedInUserId, oneTimePassword);
                    Debug.AssertValid(link);
                    //??++SMS OTP
                    LoggingHelper.LogMessage($"PHONE NUMBER VERIFICATION LINK ID: {link.ID}");
                } else {
                    Debug.Untested();
                    throw new Exception(ERROR_PHONE_NUMBER_VERIFIED, new Exception(PHONE_NUMBER_VERIFIED));
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_NO_PHONE_NUMBER_SET, new Exception(NO_PHONE_NUMBER_SET));
            }
        }

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
                    error = INVALID_ONE_TIME_PASSWORD;
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
            Link link = await FindValidLinkByUserID(dbClient, loggedInUserId, LINK_TYPE_VERIFY_PHONE_NUMBER);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                // Valid link exists
                Debug.Tested();
                Debug.AssertString(link.OneTimePassword);
                if (link.OneTimePassword == verifyPhoneNumberRequest.oneTimePassword) {
                    // One-time password matches
                    Debug.Tested();

                    // Load the user
                    User user = await FindUserByID(dbClient, loggedInUserId);
                    Debug.AssertValid(user);
                    Debug.AssertValidOrNull(user.PhoneNumberVerified);
                    if (user.PhoneNumberVerified == null) {
                        // User's phone number not verified
                        Debug.Untested();

                        // Change the user and revoke the link
                        await DoVerifyPhoneNumber(dbClient, user, link);
                    } else {
                        Debug.Untested();
                        throw new Exception(ERROR_PHONE_NUMBER_VERIFIED, new Exception(PHONE_NUMBER_VERIFIED));
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_INCORRECT_PASSWORD, new Exception(INCORRECT_PASSWORD));
                }
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK, new Exception(SharedLogicLayer.ERROR_INVALID_LINK));
            }
        }

        /**
         * Verify phone number.
         */
        internal static async Task DoVerifyPhoneNumber(AmazonDynamoDBClient dbClient, User user, Link link) {
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

        #endregion User identity service

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
