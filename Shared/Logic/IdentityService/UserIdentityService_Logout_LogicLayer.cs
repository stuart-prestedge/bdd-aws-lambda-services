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
    public static class UserIdentityService_Logout_LogicLayer
    {
        
        /**
         * Logout.
         */
        public static async Task Logout(AmazonDynamoDBClient dbClient, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes
            user.LastLoggedOut = DateTime.Now;

            // Save the user
            await IdentityServiceDataLayer.SaveUser(dbClient, user);

            //
            await IdentityServiceLogicLayer.InvalidateUserAccessTokens(dbClient, loggedInUserId);
        }

    }   // UserIdentityService_Logout_LogicLayer

}   // BDDReferenceService.Logic
