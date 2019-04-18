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
    public static class AdminIdentityService_UpdateIdentityGlobalSettings_LogicLayer
    {
        
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

            if (name == IdentityServiceLogicLayer.GLOBAL_SETTING_SYSTEM_LOCKED) {
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
                User loggedInUser = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
                Debug.AssertValidOrNull(loggedInUser);
                if (IdentityServiceLogicLayer.UserHasPermission(loggedInUser, IdentityServiceLogicLayer.PERMISSION_CAN_LOCK_SYSTEM)) {
                    Debug.Tested();
                    //??--if (!IdentityGlobalSettings.ContainsKey(GLOBAL_SETTING_SYSTEM_LOCKED) || !(bool)IdentityGlobalSettings[GLOBAL_SETTING_SYSTEM_LOCKED])
                    bool systemLocked = await IdentityServiceLogicLayer.GetBoolIdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_SYSTEM_LOCKED, false);                   
                    if (!systemLocked)
                    {
                        Debug.Tested();
                        //??--IdentityGlobalSettings[GLOBAL_SETTING_SYSTEM_LOCKED] = value;
                        await IdentityServiceDataLayer.AddIdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_SYSTEM_LOCKED, value.ToString());
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
                await IdentityServiceDataLayer.AddIdentityGlobalSetting(dbClient, IdentityServiceLogicLayer.GLOBAL_SETTING_SYSTEM_LOCKED, value.ToString());
            } else {
                // Setting to false (not allowed)
                Debug.Untested();
                throw new Exception(SharedLogicLayer.ERROR_NO_PERMISSION);
            }
        }

    }   // AdminIdentityService_UpdateIdentityGlobalSettings_LogicLayer

}   // BDDReferenceService.Logic
