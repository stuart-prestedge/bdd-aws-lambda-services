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
    public static class AdminIdentityService_UpdateUserDetails_LogicLayer
    {
        
        /**
         * Check validity of update user details request inputs.
         */
        internal static void CheckValidUpdateUserDetailsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (Helper.AllFieldsRecognized(requestBody,
                                                new List<string>(new String[]{
                                                    // "emailAddress",
                                                    // "verifyEmailLinkId",
                                                    "givenName",
                                                    "familyName",
                                                    "preferredName",
                                                    "fullName",
                                                    "blocked",
                                                    "locked",
                                                    // "password",
                                                    "dateOfBirth",
                                                    "gender",
                                                    "address1",
                                                    "address2",
                                                    "address3",
                                                    "address4",
                                                    "city",
                                                    "region",
                                                    "country",
                                                    "postalCode",
                                                    "phoneNumber",
                                                    "allowNonEssentialEmails",
                                                    "preferredLanguage",
                                                    "preferredCurrency",
                                                    "preferredTimeZone"
                                                    }))) {
                    Debug.Tested();
                    return;
                } else {
                    // Unrecognised field(s)
                    Debug.Tested();
                    error = APIHelper.UNRECOGNISED_FIELD;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Update user details.
         */
        internal static async Task UpdateUserDetails(AmazonDynamoDBClient dbClient, string userId, JObject requestBody) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(requestBody);

            //??++Fail if email, EmailAddressVerified, NewEmailAddress, PasswordHash, LastLoggedIn, LastLoggedOut, IsAnonymised, ANEEOnTimestamp, ANEEOffTimestamp, TotalTicketsPurchased, TicketsPurchasedInCurrentGame, FailedLoginAttempts,
            //      KYCStatus, KCYTimestamp, MaxDailySpendingAmount etc.
            //      is specified as per specification
            //???What about Closed, Deleted
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Tested();
                if (requestBody["givenName"] != null) {
                    user.GivenName = (string)requestBody["givenName"];
                }
                if (requestBody["familyName"] != null) {
                    user.FamilyName = (string)requestBody["familyName"];
                }
                if (requestBody["preferredName"] != null) {
                    user.PreferredName = (string)requestBody["preferredName"];
                }
                if (requestBody["fullName"] != null) {
                    user.FullName = (string)requestBody["fullName"];
                }
                if (requestBody["blocked"] != null) {
                    user.Blocked = (bool)requestBody["blocked"];
                }
                if (requestBody["locked"] != null) {
                    user.Locked = (bool)requestBody["locked"];
                }
                if (requestBody["dateOfBirth"] != null) {
                    user.DateOfBirth = (DateTime)APIHelper.DateFromAPIDateString((string)requestBody["dateOfBirth"]);
                }
                if (requestBody["gender"] != null) {
                    user.Gender = (UInt16)requestBody["gender"];
                }
                if (requestBody["address1"] != null) {
                    user.Address1 = (string)requestBody["address1"];
                }
                if (requestBody["address2"] != null) {
                    user.Address2 = (string)requestBody["address2"];
                }
                if (requestBody["address3"] != null) {
                    user.Address3 = (string)requestBody["address3"];
                }
                if (requestBody["address4"] != null) {
                    user.Address4 = (string)requestBody["address4"];
                }
                if (requestBody["city"] != null) {
                    user.City = (string)requestBody["city"];
                }
                if (requestBody["region"] != null) {
                    user.Region = (string)requestBody["region"];
                }
                if (requestBody["country"] != null) {
                    user.Country = (string)requestBody["country"];
                }
                if (requestBody["postalCode"] != null) {
                    user.PostalCode = (string)requestBody["postalCode"];
                }
                if (requestBody["phoneNumber"] != null) {
                    user.PhoneNumber = (string)requestBody["phoneNumber"];
                }
                if (requestBody["allowNonEssentialEmails"] != null) {
                    user.AllowNonEssentialEmails = (bool)requestBody["allowNonEssentialEmails"];
                    //ANEEOnTimestamp, ANEEOffTimestamp
                }
                if (requestBody["preferredLanguage"] != null) {
                    user.PreferredLanguage = (string)requestBody["preferredLanguage"];
                }
                if (requestBody["preferredCurrency"] != null) {
                    user.PreferredCurrency = (string)requestBody["preferredCurrency"];
                }
                if (requestBody["preferredTimeZone"] != null) {
                    user.PreferredTimeZone = (string)requestBody["preferredTimeZone"];
                }
                //??++UPDATE THE DATABASE!!!
            } else {
                Debug.Tested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
            }
            //??? GetUserDetailsResponse retVal = new GetUserDetailsResponse() {
            //     emailAddress = user.EmailAddress,
            //     givenName = user.GivenName,
            //     familyName = user.FamilyName,
            //     preferredName = user.PreferredName,
            //     fullName = user.FullName,
            //     dateOfBirth = APIHelper.APIDateStringFromDate(user.DateOfBirth),
            //     gender = user.Gender,
            //     address1 = user.Address1,
            //     address2 = user.Address2,
            //     address3 = user.Address3,
            //     address4 = user.Address4,
            //     city = user.City,
            //     region = user.Region,
            //     country = user.Country,
            //     postalCode = user.PostalCode,
            //     phoneNumber = user.PhoneNumber,
            //     phoneNumberVerified = APIHelper.APIDateTimeStringFromDateTime(user.PhoneNumberVerified),
            //     newEmailAddress = user.NewEmailAddress,
            //     allowNonEssentialEmails = user.AllowNonEssentialEmails,
            //     totalTicketsPurchased = user.TotalTicketsPurchased,
            //     ticketsPurchasedInCurrentGame = user.TicketsPurchasedInCurrentGame,
            //     preferredLanguage = user.PreferredLanguage,
            //     preferredCurrency = user.PreferredCurrency,
            //     preferredTimeZone = user.PreferredTimeZone,
            //     maxDailySpendingAmount = user.MaxDailySpendingAmount,
            //     newMaxDailySpendingAmount = user.NewMaxDailySpendingAmount,
            //     newMaxDailySpendingAmountTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxDailySpendingAmountTime),
            //     maxTimeLoggedIn = user.MaxTimeLoggedIn,
            //     newMaxTimeLoggedIn = user.NewMaxTimeLoggedIn,
            //     newMaxTimeLoggedInTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxTimeLoggedInTime),
            //     excludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.ExcludeUntil),
            //     newExcludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntil),
            //     newExcludeUntilTime = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntilTime)
            // };
        }

    }   // AdminIdentityService_UpdateUserDetails_LogicLayer

}   // BDDReferenceService.Logic
