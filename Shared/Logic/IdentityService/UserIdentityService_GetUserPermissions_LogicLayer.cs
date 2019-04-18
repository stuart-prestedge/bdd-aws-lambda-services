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
    public static class UserIdentityService_GetUserPermissions_LogicLayer
    {
        
        /**
         * Get user permissions.
         */
        public static async Task<string[]> GetUserPermissions(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId);
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
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
            }
        }

    }   // UserIdentityService_GetUserPermissions_LogicLayer

}   // BDDReferenceService.Logic
