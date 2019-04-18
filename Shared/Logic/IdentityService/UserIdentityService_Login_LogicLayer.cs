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
    public static class UserIdentityService_Login_LogicLayer
    {
        
        /**
         * Check validity of login request inputs.
         */
        public static LoginRequest CheckValidLoginRequest(JObject requestBody) {
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
        public static async Task<Tuple<string, DateTime>> Login(AmazonDynamoDBClient dbClient, LoginRequest loginRequest) {
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
                    throw new Exception(IdentityServiceLogicLayer.ERROR_USER_ACCOUNT_CLOSED);
                } else if (user.Blocked) {
                    Debug.Tested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_USER_BLOCKED);
                } else if (user.Locked) {
                    Debug.Tested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_USER_LOCKED);
                } else {
                    // User is not closed, blocked or locked.
                    Debug.Tested();
                    if (user.PasswordHash == Helper.Hash(loginRequest.password)) {
                        // Correct password - log the user in.
                        Debug.Tested();
                        // Invalidate any existing access tokens
                        await IdentityServiceLogicLayer.InvalidateUserAccessTokens(dbClient, user.ID);
                        // Set the last login time to now
                        user.LastLoggedIn = DateTime.Now;
                        // Mark failed login attempts as zero
                        user.FailedLoginAttempts = 0;

                        // Save the user
                        await IdentityServiceDataLayer.SaveUser(dbClient, user);

                        // Create a new access token
                        //??--Int64 accessTokenLifetime = (Int64)GetIdentityGlobalSetting(GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME, DEFAULT_ACCESS_TOKEN_LIFETIME);
                        Int64 accessTokenLifetime = await IdentityServiceLogicLayer.GetInt64IdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME, IdentityServiceLogicLayer.DEFAULT_ACCESS_TOKEN_LIFETIME);
                        AccessToken accessToken = new AccessToken() {
                            ID = RandomHelper.Next().ToString(),
                            UserID = user.ID,
                            Expires = DateTime.Now.AddSeconds(accessTokenLifetime)
                        };
                        // Setup the access token max expiry time
                        //??--Int64 maxUserLoginTime = (Int64)GetIdentityGlobalSetting(GLOBAL_SETTING_MAX_USER_LOGIN_TIME, DEFAULT_MAX_USER_LOGIN_TIME);
                        Int64 maxUserLoginTime = await IdentityServiceLogicLayer.GetInt64IdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_MAX_USER_LOGIN_TIME, IdentityServiceLogicLayer.DEFAULT_MAX_USER_LOGIN_TIME);
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
                        Int64 maxLoginAttempts = await IdentityServiceLogicLayer.GetInt64IdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_LOCK_ON_FAILED_LOGIN_ATTEMPTS, IdentityServiceLogicLayer.DEFAULT_MAX_LOGIN_ATTEMPTS);
                        if (++user.FailedLoginAttempts == maxLoginAttempts) {
                            // Too many password attempts - user locked.
                            Debug.Tested();
                            user.Locked = true;

                            // Save the user
                            await IdentityServiceDataLayer.SaveUser(dbClient, user);

                            throw new Exception(IdentityServiceLogicLayer.ERROR_USER_LOCKED);
                        } else {
                            Debug.Tested();
                            throw new Exception(IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD);
                        }
                    }
                }
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS);
            }
        }

    }   // UserIdentityService_Login_LogicLayer

}   // BDDReferenceService.Logic
