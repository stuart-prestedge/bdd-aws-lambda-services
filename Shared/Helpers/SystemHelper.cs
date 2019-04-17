using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using BDDReferenceService.Logic;

namespace BDDReferenceService
{
    
    internal static class SystemHelper
    {

        /**
         * Is the system is not locked?
         */
        internal static async Task<bool> GetSystemLocked(AmazonDynamoDBClient dbClient)
        {
            Debug.Tested();
            Debug.AssertValid(dbClient);

            return await IdentityServiceLogicLayer.GetBoolIdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_SYSTEM_LOCKED, IdentityServiceLogicLayer.DEFAULT_SYSTEM_LOCKED);
        }

        /**
         * Check to see that the system is not locked.
         * Return true if it is not locked.
         * Throw a ERROR_SYSTEM_LOCKED exception if it is locked.
         */
        internal static async Task CheckSystemNotLocked(AmazonDynamoDBClient dbClient)
        {
            Debug.Tested();

            if (await GetSystemLocked(dbClient))
            {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_SYSTEM_LOCKED, new Exception(SharedLogicLayer.ERROR_SYSTEM_LOCKED));
            }
            else
            {
                Debug.Tested();
            }
        }

        /**
         * Unlock the system.
         */
        internal static async Task UnlockSystem(AmazonDynamoDBClient dbClient) {
            Debug.Tested();
            Debug.AssertValid(dbClient);

            await IdentityServiceLogicLayer.UpdateSystemLockedGlobalSetting(dbClient, Helper.INVALID_ID, false, true);
        }

    }   // SystemHelper

}   // BDDReferenceService
