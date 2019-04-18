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
    public static class UserIdentityService_RefreshAccessToken_LogicLayer
    {
        
        /**
         * Refresh access token.
         * The specified user ID must exist.
         */
        public static async Task<DateTime?> RefreshAccessToken(AmazonDynamoDBClient dbClient, string loggedInUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);

            DateTime? retVal = null;
            AccessToken accessToken = await IdentityServiceLogicLayer.FindAccessTokenByUserID(dbClient, loggedInUserId);
            Debug.AssertValidOrNull(accessToken);
            if (accessToken != null)
            {
                //??--Int64 accessTokenLifetime = (Int64)GetIdentityGlobalSetting(GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME, DEFAULT_ACCESS_TOKEN_LIFETIME);
                Int64 accessTokenLifetime = await IdentityServiceLogicLayer.GetInt64IdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_ACCESS_TOKEN_LIFETIME, IdentityServiceLogicLayer.DEFAULT_ACCESS_TOKEN_LIFETIME);
                if (accessToken.MaxExpiry == null)
                {
                    accessToken.Expires = DateTime.Now.AddSeconds(accessTokenLifetime);
                }
                else
                {
                    if (accessToken.Expires == accessToken.MaxExpiry)
                    {
                        throw new Exception(IdentityServiceLogicLayer.ERROR_CANNOT_EXTEND_ACCESS_TOKEN);
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

    }   // UserIdentityService_RefreshAccessToken_LogicLayer

}   // BDDReferenceService.Logic
