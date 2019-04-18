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
    public static class AdminIdentityService_SearchUsers_LogicLayer
    {
        
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

    }   // AdminIdentityService_SearchUsers_LogicLayer

}   // BDDReferenceService.Logic
