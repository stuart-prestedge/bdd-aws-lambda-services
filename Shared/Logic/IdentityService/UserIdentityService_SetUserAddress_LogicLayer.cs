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
    public static class UserIdentityService_SetUserAddress_LogicLayer
    {
        
        /**
         * Check validity of set user address request inputs.
         */
        internal static SetUserAddressRequest CheckValidSetUserAddressRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                if (!String.IsNullOrEmpty((string)requestBody["address1"])) {
                    if (Helper.IsValidCountryCode((string)requestBody["country"])) {
                        if (!String.IsNullOrEmpty((string)requestBody["postalCode"])) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "address1",
                                                                "address2",
                                                                "address3",
                                                                "address4",
                                                                "city",
                                                                "region",
                                                                "country",
                                                                "postalCode"
                                                                }))) {
                                return new SetUserAddressRequest {
                                    address1 = (string)requestBody["address1"],
                                    address2 = (string)requestBody["address2"],
                                    address3 = (string)requestBody["address3"],
                                    address4 = (string)requestBody["address4"],
                                    city = (string)requestBody["city"],
                                    region = (string)requestBody["region"],
                                    country = (string)requestBody["country"],
                                    postalCode = (string)requestBody["postalCode"]
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = IdentityServiceLogicLayer.INVALID_POSTAL_CODE;
                        }
                    } else {
                        Debug.Untested();
                        error = IdentityServiceLogicLayer.INVALID_COUNTRY_CODE;
                    }
                } else {
                    Debug.Untested();
                    error = IdentityServiceLogicLayer.INVALID_ADDRESS_1;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user address.
         */
        internal static async Task SetUserAddress(AmazonDynamoDBClient dbClient, string loggedInUserId, SetUserAddressRequest setUserAddressRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setUserAddressRequest);
            Debug.AssertString(setUserAddressRequest.address1);
            Debug.AssertString(setUserAddressRequest.country);
            Debug.AssertString(setUserAddressRequest.postalCode);

            // Load the user
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);

            // Make changes (if necessary)
            if ((user.Address1 != setUserAddressRequest.address1) ||
                (user.Address2 != setUserAddressRequest.address2) ||
                (user.Address3 != setUserAddressRequest.address3) ||
                (user.Address4 != setUserAddressRequest.address4) ||
                (user.City != setUserAddressRequest.city) ||
                (user.Region != setUserAddressRequest.region) ||
                (user.Country != setUserAddressRequest.country) ||
                (user.PostalCode != setUserAddressRequest.postalCode))
            {
                user.Address1 = setUserAddressRequest.address1;
                user.Address2 = setUserAddressRequest.address2;
                user.Address3 = setUserAddressRequest.address3;
                user.Address4 = setUserAddressRequest.address4;
                user.City = setUserAddressRequest.city;
                user.Region = setUserAddressRequest.region;
                user.Country = setUserAddressRequest.country;
                user.PostalCode = setUserAddressRequest.postalCode;

                // Save the user
                await IdentityServiceDataLayer.SaveUser(dbClient, user);
            }
        }

    }   // UserIdentityService_SetUserAddress_LogicLayer

}   // BDDReferenceService.Logic
