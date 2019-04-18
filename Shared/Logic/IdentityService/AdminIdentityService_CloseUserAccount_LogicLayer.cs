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
    public static class AdminIdentityService_CloseUserAccount_LogicLayer
    {
        
        /**
         * Close user account.
         */
        internal static async Task CloseUserAccount(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);

            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                // User exists
                Debug.Tested();
                Debug.AssertNull(user.Deleted);
                if (user.Closed == null) {
                    // User account is not already closed
                    Debug.Tested();
                    user.Closed = DateTime.Now;
                    await IdentityServiceLogicLayer.InvalidateUserAccessTokens(dbClient, userId);
                } else {
                    // User account is already closed
                    Debug.Untested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_USER_ACCOUNT_CLOSED);
                }
            } else {
                // User does not exist (or is soft deleted)
                Debug.Tested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
            }
        }

    }   // AdminIdentityService_CloseUserAccount_LogicLayer

}   // BDDReferenceService.Logic
