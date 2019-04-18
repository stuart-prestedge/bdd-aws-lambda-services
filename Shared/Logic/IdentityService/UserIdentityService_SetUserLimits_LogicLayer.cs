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
    public static class UserIdentityService_SetUserLimits_LogicLayer
    {
        
        /**
         * Check validity of set user limits request inputs.
         */
        public static SetUserLimitsRequest CheckValidSetUserLimitsRequest(JObject requestBody) {
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
                            error = IdentityServiceLogicLayer.INVALID_EXCLUDE_UNTIL;
                        }
                    } else {
                        Debug.Untested();
                        error = IdentityServiceLogicLayer.INVALID_MAX_TIME_LOGGED_IN;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_MAX_DAILY_SPENDING_AMOUNT;
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
        public static async Task SetUserLimits(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserLimitsRequest setUserLimitsRequest) {
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
                User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
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
        
    }   // UserIdentityService_SetUserLimits_LogicLayer

}   // BDDReferenceService.Logic
