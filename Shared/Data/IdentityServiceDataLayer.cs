using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BDDReferenceService.Contracts;
using BDDReferenceService.Model;
using Newtonsoft.Json.Linq;

namespace BDDReferenceService.Data
{

    /**
     * Data layer with helper methods.
     */
    public static class IdentityServiceDataLayer
    {

        /**
         * Identity service dataset and field names.
         */
        #region Identity service dataset names
        
        // Users
        internal const string DATASET_USERS = "Users";
        internal const string DATASET_USERS_INDEX_EMAIL_ADDRESS = "EmailAddress-index";
        internal const string DATASET_USERS_INDEX_NEW_EMAIL_ADDRESS = "NewEmailAddress-index";
        internal const string FIELD_USERS_ID = "Id";
        internal const string FIELD_USERS_CLIENT_ID = "ClientId";
        internal const string FIELD_USERS_GIVEN_NAME = "GivenName";
        internal const string FIELD_USERS_FAMILY_NAME = "FamilyName";
        internal const string FIELD_USERS_PREFERRED_NAME = "PreferredName";
        internal const string FIELD_USERS_FULL_NAME = "FullName";
        internal const string FIELD_USERS_EMAIL_ADDRESS = "EmailAddress";
        internal const string FIELD_USERS_EMAIL_ADDRESS_VERIFIED = "EmailAddressVerified";
        internal const string FIELD_USERS_NEW_EMAIL_ADDRESS = "NewEmailAddress";
        internal const string FIELD_USERS_PASSWORD_HASH = "PasswordHash";
        internal const string FIELD_USERS_BLOCKED = "Blocked";
        internal const string FIELD_USERS_LOCKED = "Locked";
        internal const string FIELD_USERS_DATE_OF_BIRTH = "DateOfBirth";
        internal const string FIELD_USERS_GENDER = "Gender";
        internal const string FIELD_USERS_ADDRESS_1 = "Address1";
        internal const string FIELD_USERS_ADDRESS_2 = "Address2";
        internal const string FIELD_USERS_ADDRESS_3 = "Address3";
        internal const string FIELD_USERS_ADDRESS_4 = "Address4";
        internal const string FIELD_USERS_CITY = "City";
        internal const string FIELD_USERS_REGION = "Region";
        internal const string FIELD_USERS_COUNTRY = "Country";
        internal const string FIELD_USERS_POSTAL_CODE = "PostalCode";
        internal const string FIELD_USERS_PHONE_NUMBER = "PhoneNumber";
        internal const string FIELD_USERS_PHONE_NUMBER_VERIFIED = "PhoneNumberVerified";
        internal const string FIELD_USERS_LAST_LOGGED_IN = "LastLoggedIn";
        internal const string FIELD_USERS_LAST_LOGGED_OUT = "LastLoggedOut";
        internal const string FIELD_USERS_CLOSED = "Closed";
        internal const string FIELD_USERS_IS_ANONYMISED = "IsAnonymised";
        internal const string FIELD_USERS_DELETED = "Deleted";
        internal const string FIELD_USERS_ALLOW_NON_ESSENTIAL_EMAILS = "AllowNonEssentialEmails";
        internal const string FIELD_USERS_ANEE_ON_TIMESTAMP = "ANEEOnTimestamp";
        internal const string FIELD_USERS_ANEE_OFF_TIMESTAMP = "ANEEOffTimestamp";
        internal const string FIELD_USERS_TOTAL_TICKETS_PURCHASED = "TotalTicketsPurchased";
        internal const string FIELD_USERS_TICKETS_PURCHASED_IN_CURRENT_GAME = "TicketsPurchasedInCurrentGame";
        internal const string FIELD_USERS_PREFERRED_LANGUAGE = "PreferredLanguage";
        internal const string FIELD_USERS_PREFERRED_CURRENCY = "PreferredCurrency";
        internal const string FIELD_USERS_PREFERRED_TIME_ZONE = "PreferredTimeZone";
        internal const string FIELD_USERS_FAILED_LOGIN_ATTEMPTS = "FailedLoginAttempts";
        internal const string FIELD_USERS_KYC_STATUS = "KYCStatus";
        internal const string FIELD_USERS_KYC_TIMESTAMP = "KCYTimestamp";
        internal const string FIELD_USERS_MAX_DAILY_SPENDING_AMOUNT = "MaxDailySpendingAmount";
        internal const string FIELD_USERS_NEW_MAX_DAILY_SPENDING_AMOUNT = "NewMaxDailySpendingAmount";
        internal const string FIELD_USERS_NEW_MAX_DAILY_SPENDING_AMOUNT_TIME = "NewMaxDailySpendingAmountTime";
        internal const string FIELD_USERS_MAX_TIME_LOGGED_IN = "MaxTimeLoggedIn";
        internal const string FIELD_USERS_NEW_MAX_TIME_LOGGED_IN = "NewMaxTimeLoggedIn";
        internal const string FIELD_USERS_NEW_MAX_TIME_LOGGED_IN_TIME = "NewMaxTimeLoggedInTime";
        internal const string FIELD_USERS_EXCLUDE_UNTIL = "ExcludeUntil";
        internal const string FIELD_USERS_NEW_EXCLUDE_UNTIL = "NewExcludeUntil";
        internal const string FIELD_USERS_NEW_EXCLUDE_UNTIL_TIME = "NewExcludeUntilTime";
        internal const string FIELD_USERS_PERMISSIONS = "Permissions";
        
        // Access tokens
        internal const string DATASET_ACCESS_TOKENS = "AccessTokens";
        internal const string DATASET_ACCESS_TOKENS_INDEX_USER_ID = "UserId-index";
        internal const string FIELD_ACCESS_TOKENS_ID = "Id";
        internal const string FIELD_ACCESS_TOKENS_USER_ID = "UserId";
        internal const string FIELD_ACCESS_TOKENS_EXPIRES = "Expires";
        internal const string FIELD_ACCESS_TOKENS_MAX_EXPIRY = "MaxExpiry";
        
        // Links
        internal const string DATASET_LINKS = "Links";
        internal const string DATASET_LINKS_INDEX_USER_ID = "UserId-index";
        internal const string FIELD_LINKS_ID = "Id";
        internal const string FIELD_LINKS_TYPE = "Type";
        internal const string FIELD_LINKS_EXPIRES = "Expires";
        internal const string FIELD_LINKS_USER_ID = "UserId";
        internal const string FIELD_LINKS_REVOKED = "Revoked";
        internal const string FIELD_LINKS_ONE_TIME_PASSWORD = "OneTimePassword";

        // Identity global settings
        internal const string DATASET_IDENTITY_GLOBAL_SETTINGS = "IdentityGlobalSettings";
        internal const string FIELD_IDENTITY_GLOBAL_SETTINGS_NAME = "Name";
        internal const string FIELD_IDENTITY_GLOBAL_SETTINGS_VALUE = "Value";

        #endregion Identity service dataset names

        /**
         * User record helper methods.
         */
        #region User records

        /**
         * Get a user from the database item.
         */
        internal static User UserFromDBItem(Dictionary<string, AttributeValue> item) {
            Debug.Untested();
            Debug.AssertValid(item);

            return new User {
                ID = item[FIELD_USERS_ID].S,
                ClientID = item[FIELD_USERS_CLIENT_ID].S,
                GivenName = item[FIELD_USERS_GIVEN_NAME].S,
                FamilyName = item[FIELD_USERS_FAMILY_NAME].S,
                PreferredName = item[FIELD_USERS_PREFERRED_NAME].S,
                FullName = item[FIELD_USERS_FULL_NAME].S,
                EmailAddress = item[FIELD_USERS_EMAIL_ADDRESS].S,
                EmailAddressVerified = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_EMAIL_ADDRESS_VERIFIED].S),
                NewEmailAddress = item[FIELD_USERS_NEW_EMAIL_ADDRESS].S,
                PasswordHash = item[FIELD_USERS_PASSWORD_HASH].S,
                Blocked = item[FIELD_USERS_BLOCKED].BOOL,
                Locked = item[FIELD_USERS_LOCKED].BOOL,
                DateOfBirth = APIHelper.DateFromAPIDateString(item[FIELD_USERS_DATE_OF_BIRTH].S),
                Gender = UInt16.Parse(item[FIELD_USERS_GENDER].N),
                Address1 = item[FIELD_USERS_ADDRESS_1].S,
                Address2 = item[FIELD_USERS_ADDRESS_2].S,
                Address3 = item[FIELD_USERS_ADDRESS_3].S,
                Address4 = item[FIELD_USERS_ADDRESS_4].S,
                City = item[FIELD_USERS_CITY].S,
                Region = item[FIELD_USERS_REGION].S,
                Country = item[FIELD_USERS_COUNTRY].S,
                PostalCode = item[FIELD_USERS_POSTAL_CODE].S,
                PhoneNumber = item[FIELD_USERS_PHONE_NUMBER].S,
                PhoneNumberVerified = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_PHONE_NUMBER_VERIFIED].S),
                LastLoggedIn = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_LAST_LOGGED_IN].S),
                LastLoggedOut = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_LAST_LOGGED_OUT].S),
                Closed = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_CLOSED].S),
                IsAnonymised = item[FIELD_USERS_IS_ANONYMISED].BOOL,
                Deleted = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_DELETED].S),
                AllowNonEssentialEmails = item[FIELD_USERS_ALLOW_NON_ESSENTIAL_EMAILS].BOOL,
                ANEEOnTimestamp = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_ANEE_ON_TIMESTAMP].S),
                ANEEOffTimestamp = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_ANEE_OFF_TIMESTAMP].S),
                TotalTicketsPurchased = UInt32.Parse(item[FIELD_USERS_TOTAL_TICKETS_PURCHASED].N),
                TicketsPurchasedInCurrentGame = UInt32.Parse(item[FIELD_USERS_TICKETS_PURCHASED_IN_CURRENT_GAME].N),
                PreferredLanguage = item[FIELD_USERS_PREFERRED_LANGUAGE].S,
                PreferredCurrency = item[FIELD_USERS_PREFERRED_CURRENCY].S,
                PreferredTimeZone = item[FIELD_USERS_PREFERRED_TIME_ZONE].S,
                FailedLoginAttempts = UInt16.Parse(item[FIELD_USERS_FAILED_LOGIN_ATTEMPTS].N),
                KYCStatus = item[FIELD_USERS_KYC_STATUS].S,
                KCYTimestamp = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_KYC_TIMESTAMP].S),
                MaxDailySpendingAmount = UInt32.Parse(item[FIELD_USERS_MAX_DAILY_SPENDING_AMOUNT].N),
                NewMaxDailySpendingAmount = UInt32.Parse(item[FIELD_USERS_NEW_MAX_DAILY_SPENDING_AMOUNT].N),
                NewMaxDailySpendingAmountTime = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_NEW_MAX_DAILY_SPENDING_AMOUNT_TIME].S),
                MaxTimeLoggedIn = UInt64.Parse(item[FIELD_USERS_MAX_TIME_LOGGED_IN].N),
                NewMaxTimeLoggedIn = UInt64.Parse(item[FIELD_USERS_NEW_MAX_TIME_LOGGED_IN].N),
                NewMaxTimeLoggedInTime = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_NEW_MAX_TIME_LOGGED_IN_TIME].S),
                ExcludeUntil = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_EXCLUDE_UNTIL].S),
                NewExcludeUntil = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_NEW_EXCLUDE_UNTIL].S),
                NewExcludeUntilTime = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_NEW_EXCLUDE_UNTIL_TIME].S)
                //??++PERMISSIONS
            };
        }

        /**
         * Get a user database item by ID.
         */
        internal static async Task<Dictionary<string, AttributeValue>> GetUserDBItemById(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);

            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(FIELD_USERS_ID, new AttributeValue(userId));
            GetItemResponse getResponse = await dbClient.GetItemAsync(DATASET_USERS, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            return getResponse.Item;
        }

        /**
         * Find a user by email address.
         */
        internal static async Task<User> FindUserByEmailAddress(AmazonDynamoDBClient dbClient, string emailAddress) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertEmail(emailAddress);

            User retVal = null;
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(FIELD_USERS_EMAIL_ADDRESS, new AttributeValue(emailAddress));
            GetItemResponse getResponse = await dbClient.GetItemAsync(DATASET_USERS_INDEX_EMAIL_ADDRESS, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            if (getResponse.Item != null)
            {
                // A user with the specified ID exists.
                Debug.Untested();
                retVal = UserFromDBItem(getResponse.Item);
                Debug.AssertValid(retVal);
            }
            return retVal;//??--Users.Find(user_ => (user_.EmailAddress == emailAddress));
        }

        /**
         * Find a user by new email address.
         */
        internal static async Task<User> FindUserByNewEmailAddress(AmazonDynamoDBClient dbClient, string newEmailAddress) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertEmail(newEmailAddress);

            User retVal = null;
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(FIELD_USERS_EMAIL_ADDRESS, new AttributeValue(newEmailAddress));
            GetItemResponse getResponse = await dbClient.GetItemAsync(DATASET_USERS_INDEX_NEW_EMAIL_ADDRESS, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            if (getResponse.Item != null)
            {
                // A user with the specified ID exists.
                Debug.Untested();
                retVal = UserFromDBItem(getResponse.Item);
                Debug.AssertValid(retVal);
            }
            return retVal;//??--Users.Find(user_ => (user_.EmailAddress == emailAddress));
        }

        /**
         * Get a user database item by ID.
         */
        internal static DateTime? GetUserDBItemDeleted(Dictionary<string, AttributeValue> item) {
            Debug.Untested();
            Debug.AssertValidOrNull(item);

            DateTime ?retVal = null;
            if (item[FIELD_USERS_DELETED].S != null)
            {
                retVal = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_DELETED].S);
                Debug.AssertValid(retVal);
            }
            return retVal;
        }

        /**
         * Get a user database item by ID.
         */
        internal static DateTime? GetUserDBItemClosed(Dictionary<string, AttributeValue> item) {
            Debug.Untested();
            Debug.AssertValidOrNull(item);

            DateTime ?retVal = null;
            if (item[FIELD_USERS_CLOSED].S != null)
            {
                retVal = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_USERS_CLOSED].S);
                Debug.AssertValid(retVal);
            }
            return retVal;
        }

        /**
         * Add a user record to the data store.
         */
        internal static async Task AddUser(AmazonDynamoDBClient dbClient, User user) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(user);
            Debug.AssertID(user.ID);

            Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>();
            item.Add(FIELD_USERS_ID, new AttributeValue(user.ID));
            item.Add(FIELD_USERS_CLIENT_ID, new AttributeValue(user.ClientID));
            item.Add(FIELD_USERS_GIVEN_NAME, new AttributeValue(user.GivenName));
            item.Add(FIELD_USERS_FAMILY_NAME, new AttributeValue(user.FamilyName));
            item.Add(FIELD_USERS_PREFERRED_NAME, new AttributeValue(user.PreferredName));
            item.Add(FIELD_USERS_FULL_NAME, new AttributeValue(user.FullName));
            item.Add(FIELD_USERS_EMAIL_ADDRESS, new AttributeValue(user.EmailAddress));
            item.Add(FIELD_USERS_EMAIL_ADDRESS_VERIFIED, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.EmailAddressVerified)));
            item.Add(FIELD_USERS_NEW_EMAIL_ADDRESS, new AttributeValue(user.NewEmailAddress));
            item.Add(FIELD_USERS_PASSWORD_HASH, new AttributeValue(user.PasswordHash));
            item.Add(FIELD_USERS_BLOCKED, new AttributeValue { BOOL = user.Blocked });
            item.Add(FIELD_USERS_LOCKED, new AttributeValue { BOOL = user.Locked });
            item.Add(FIELD_USERS_DATE_OF_BIRTH, new AttributeValue(APIHelper.APIDateStringFromDate(user.DateOfBirth)));
            item.Add(FIELD_USERS_GENDER, new AttributeValue { N = user.Gender.ToString() });
            item.Add(FIELD_USERS_ADDRESS_1, new AttributeValue(user.Address1));
            item.Add(FIELD_USERS_ADDRESS_2, new AttributeValue(user.Address2));
            item.Add(FIELD_USERS_ADDRESS_3, new AttributeValue(user.Address3));
            item.Add(FIELD_USERS_ADDRESS_4, new AttributeValue(user.Address4));
            item.Add(FIELD_USERS_CITY, new AttributeValue(user.City));
            item.Add(FIELD_USERS_REGION, new AttributeValue(user.Region));
            item.Add(FIELD_USERS_COUNTRY, new AttributeValue(user.Country));
            item.Add(FIELD_USERS_POSTAL_CODE, new AttributeValue(user.PostalCode));
            item.Add(FIELD_USERS_PHONE_NUMBER, new AttributeValue(user.PhoneNumber));
            item.Add(FIELD_USERS_PHONE_NUMBER_VERIFIED, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.PhoneNumberVerified)));
            item.Add(FIELD_USERS_LAST_LOGGED_IN, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.LastLoggedIn)));
            item.Add(FIELD_USERS_LAST_LOGGED_OUT, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.LastLoggedOut)));
            item.Add(FIELD_USERS_CLOSED, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.Closed)));
            item.Add(FIELD_USERS_IS_ANONYMISED, new AttributeValue { BOOL = user.IsAnonymised });
            item.Add(FIELD_USERS_DELETED, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.Deleted)));
            item.Add(FIELD_USERS_ALLOW_NON_ESSENTIAL_EMAILS, new AttributeValue { BOOL = user.AllowNonEssentialEmails});
            item.Add(FIELD_USERS_ANEE_ON_TIMESTAMP, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.ANEEOnTimestamp)));
            item.Add(FIELD_USERS_ANEE_OFF_TIMESTAMP, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.ANEEOffTimestamp)));
            item.Add(FIELD_USERS_TOTAL_TICKETS_PURCHASED, new AttributeValue { N = user.TotalTicketsPurchased.ToString() });
            item.Add(FIELD_USERS_TICKETS_PURCHASED_IN_CURRENT_GAME, new AttributeValue { N = user.TicketsPurchasedInCurrentGame.ToString() });
            item.Add(FIELD_USERS_PREFERRED_LANGUAGE, new AttributeValue(user.PreferredLanguage));
            item.Add(FIELD_USERS_PREFERRED_CURRENCY, new AttributeValue(user.PreferredCurrency));
            item.Add(FIELD_USERS_PREFERRED_TIME_ZONE, new AttributeValue(user.PreferredTimeZone));
            item.Add(FIELD_USERS_FAILED_LOGIN_ATTEMPTS, new AttributeValue { N = user.FailedLoginAttempts.ToString() });
            item.Add(FIELD_USERS_KYC_STATUS, new AttributeValue(user.KYCStatus));
            item.Add(FIELD_USERS_KYC_TIMESTAMP, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.KCYTimestamp)));
            item.Add(FIELD_USERS_MAX_DAILY_SPENDING_AMOUNT, new AttributeValue { N = user.MaxDailySpendingAmount.ToString() });
            item.Add(FIELD_USERS_NEW_MAX_DAILY_SPENDING_AMOUNT, new AttributeValue { N = user.NewMaxDailySpendingAmount.ToString() });
            item.Add(FIELD_USERS_NEW_MAX_DAILY_SPENDING_AMOUNT_TIME, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.NewMaxDailySpendingAmountTime)));
            item.Add(FIELD_USERS_MAX_TIME_LOGGED_IN, new AttributeValue { N = user.MaxTimeLoggedIn.ToString() });
            item.Add(FIELD_USERS_NEW_MAX_TIME_LOGGED_IN, new AttributeValue { N = user.NewMaxTimeLoggedIn.ToString() });
            item.Add(FIELD_USERS_NEW_MAX_TIME_LOGGED_IN_TIME, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.NewMaxTimeLoggedInTime)));
            item.Add(FIELD_USERS_EXCLUDE_UNTIL, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.ExcludeUntil)));
            item.Add(FIELD_USERS_NEW_EXCLUDE_UNTIL, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntil)));
            item.Add(FIELD_USERS_NEW_EXCLUDE_UNTIL_TIME, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntilTime)));
            item.Add(FIELD_USERS_PERMISSIONS, new AttributeValue { SS = user.Permissions });
            PutItemResponse putResponse = await dbClient.PutItemAsync(DATASET_USERS, item);
            Debug.AssertValid(putResponse);
            //??++Check response?
        }

        /**
         * Save the user to the database.
         */
        internal static async Task SaveUser(AmazonDynamoDBClient dbClient, User user)
        {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(user);

            Debug.Uncoded();
        }   
        
        #endregion User records

        /**
         * Access token record helper methods.
         */
        #region AccessToken records

        /**
         * Get a access token from the database item.
         */
        internal static AccessToken AccessTokenFromDBItem(Dictionary<string, AttributeValue> item) {
            Debug.Untested();
            Debug.AssertValid(item);

            return new AccessToken {
                ID = item[FIELD_ACCESS_TOKENS_ID].S,
                UserID = item[FIELD_ACCESS_TOKENS_USER_ID].S,
                Expires = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_ACCESS_TOKENS_EXPIRES].S),
                MaxExpiry = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_ACCESS_TOKENS_MAX_EXPIRY].S)
            };
        }

        /**
         * Get a access token database item by ID.
         */
        internal static async Task<Dictionary<string, AttributeValue>> GetAccessTokenDBItemById(AmazonDynamoDBClient dbClient, string accessTokenId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(accessTokenId);

            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(FIELD_ACCESS_TOKENS_ID, new AttributeValue(accessTokenId));
            GetItemResponse getResponse = await dbClient.GetItemAsync(DATASET_ACCESS_TOKENS, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            return getResponse.Item;
        }

        /**
         * Add a access token record to the data store.
         */
        internal static async Task AddAccessToken(AmazonDynamoDBClient dbClient, AccessToken accessToken)
        {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(accessToken);
            Debug.AssertID(accessToken.ID);

            Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>();
            item.Add(FIELD_ACCESS_TOKENS_ID, new AttributeValue(accessToken.ID));
            item.Add(FIELD_ACCESS_TOKENS_USER_ID, new AttributeValue(accessToken.UserID));
            item.Add(FIELD_ACCESS_TOKENS_EXPIRES, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(accessToken.Expires)));
            item.Add(FIELD_ACCESS_TOKENS_MAX_EXPIRY, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(accessToken.MaxExpiry)));
            PutItemResponse putResponse = await dbClient.PutItemAsync(DATASET_ACCESS_TOKENS, item);
            Debug.AssertValid(putResponse);
            //??++Check response?
        }

        #endregion AccessToken records

        /**
         * Link record helper methods.
         */
        #region Link records

        /**
         * Get a link from the database item.
         */
        internal static Link LinkFromDBItem(Dictionary<string, AttributeValue> item) {
            Debug.Untested();
            Debug.AssertValid(item);

            return new Link {
                ID = item[FIELD_LINKS_ID].S,
                Type = item[FIELD_LINKS_TYPE].S,
                Expires = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_LINKS_EXPIRES].S),
                UserID = item[FIELD_LINKS_USER_ID].S,
                Revoked = item[FIELD_LINKS_REVOKED].BOOL,
                OneTimePassword = item[FIELD_LINKS_ONE_TIME_PASSWORD].S
            };
        }

        /**
         * Get a link database item by ID.
         */
        internal static async Task<Dictionary<string, AttributeValue>> GetLinkDBItemById(AmazonDynamoDBClient dbClient, string linkId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(linkId);

            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(FIELD_LINKS_ID, new AttributeValue(linkId));
            GetItemResponse getResponse = await dbClient.GetItemAsync(DATASET_LINKS, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            return getResponse.Item;
        }

        /**
         * Add a link record to the data store.
         */
        internal static async Task AddLink(AmazonDynamoDBClient dbClient, Link link)
        {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(link);
            Debug.AssertID(link.ID);

            Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>();
            item.Add(FIELD_LINKS_ID, new AttributeValue(link.ID));
            item.Add(FIELD_LINKS_TYPE, new AttributeValue(link.Type));
            item.Add(FIELD_LINKS_EXPIRES, new AttributeValue(APIHelper.APIDateTimeStringFromDateTime(link.Expires)));
            item.Add(FIELD_LINKS_USER_ID, new AttributeValue(link.UserID));
            item.Add(FIELD_LINKS_REVOKED, new AttributeValue { BOOL = link.Revoked });
            item.Add(FIELD_LINKS_ONE_TIME_PASSWORD, new AttributeValue(link.OneTimePassword));
            PutItemResponse putResponse = await dbClient.PutItemAsync(DATASET_LINKS, item);
            Debug.AssertValid(putResponse);
            //??++Check response?
        }

        #endregion Link records

        /**
         * Identity global setting record helper methods.
         */
        #region Identity global setting records

        /**
         * Get an identity global setting from the database item.
         */
        internal static string ValueFromDBItem(Dictionary<string, AttributeValue> item) {
            Debug.Untested();
            Debug.AssertValid(item);

            string retVal = null;
            AttributeValue attributeValue = item[FIELD_IDENTITY_GLOBAL_SETTINGS_VALUE];
            if (!attributeValue.NULL)
            {
                retVal = attributeValue.S;
                //??-- if (attributeValue.IsBOOLSet)
                // {
                //     retVal = attributeValue.BOOL;
                // }
                // else if (attributeValue.N != null)
                // {
                //     retVal = int.Parse(attributeValue.N);
                // }
                // else if (attributeValue.S != null)
                // {
                //     retVal = attributeValue.S;
                // }
                // else
                // {
                //     Debug.Unreachable();
                // }
            }
            return retVal;
            //??-- return new Link {
            //     ID = item[FIELD_LINKS_ID].S,
            //     Type = item[FIELD_LINKS_TYPE].S,
            //     Expires = APIHelper.DateTimeFromAPIDateTimeString(item[FIELD_LINKS_EXPIRES].S),
            //     UserID = item[FIELD_LINKS_USER_ID].S,
            //     Revoked = item[FIELD_LINKS_REVOKED].BOOL,
            //     OneTimePassword = item[FIELD_LINKS_ONE_TIME_PASSWORD].S
            // };
        }

        /**
         * Get an identity global setting database item by ID.
         */
        internal static async Task<Dictionary<string, AttributeValue>> GetIdentityGlobalSettingDBItemByName(AmazonDynamoDBClient dbClient, string name) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(name);

            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(FIELD_IDENTITY_GLOBAL_SETTINGS_NAME, new AttributeValue(name));
            GetItemResponse getResponse = await dbClient.GetItemAsync(DATASET_IDENTITY_GLOBAL_SETTINGS, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            return getResponse.Item;
        }

        /**
         * Add an identity global setting record to the data store.
         */
        internal static async Task AddIdentityGlobalSetting(AmazonDynamoDBClient dbClient, string name, string value)
        {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(name);
            Debug.AssertStringOrNull(value);

            Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>();
            item.Add(FIELD_IDENTITY_GLOBAL_SETTINGS_NAME, new AttributeValue(name));
            item.Add(FIELD_IDENTITY_GLOBAL_SETTINGS_VALUE, new AttributeValue(value));
            PutItemResponse putResponse = await dbClient.PutItemAsync(DATASET_IDENTITY_GLOBAL_SETTINGS, item);
            Debug.AssertValid(putResponse);
            //??++Check response?
        }

        #endregion Identity global setting records

    }   // IdentityServiceDataLayer

}   // BDDReferenceService.Data
