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
    public static class UserIdentityService_GetUserDetails_LogicLayer
    {
        
        /**
         * Get user details.
         */
        public static async Task<GetUserDetailsResponse> GetUserDetails(AmazonDynamoDBClient dbClient, string loggedInUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);

            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, loggedInUserId);
            Debug.AssertValid(user);
            GetUserDetailsResponse retVal = new GetUserDetailsResponse() {
                emailAddress = user.EmailAddress,
                givenName = user.GivenName,
                familyName = user.FamilyName,
                preferredName = user.PreferredName,
                fullName = user.FullName,
                dateOfBirth = APIHelper.APIDateStringFromDate(user.DateOfBirth),
                gender = user.Gender,
                address1 = user.Address1,
                address2 = user.Address2,
                address3 = user.Address3,
                address4 = user.Address4,
                city = user.City,
                region = user.Region,
                country = user.Country,
                postalCode = user.PostalCode,
                phoneNumber = user.PhoneNumber,
                phoneNumberVerified = APIHelper.APIDateTimeStringFromDateTime(user.PhoneNumberVerified),
                newEmailAddress = user.NewEmailAddress,
                allowNonEssentialEmails = user.AllowNonEssentialEmails,
                totalTicketsPurchased = user.TotalTicketsPurchased,
                ticketsPurchasedInCurrentGame = user.TicketsPurchasedInCurrentGame,
                preferredLanguage = user.PreferredLanguage,
                preferredCurrency = user.PreferredCurrency,
                preferredTimeZone = user.PreferredTimeZone,
                maxDailySpendingAmount = user.MaxDailySpendingAmount,
                newMaxDailySpendingAmount = user.NewMaxDailySpendingAmount,
                newMaxDailySpendingAmountTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxDailySpendingAmountTime),
                maxTimeLoggedIn = user.MaxTimeLoggedIn,
                newMaxTimeLoggedIn = user.NewMaxTimeLoggedIn,
                newMaxTimeLoggedInTime = APIHelper.APIDateTimeStringFromDateTime(user.NewMaxTimeLoggedInTime),
                excludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.ExcludeUntil),
                newExcludeUntil = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntil),
                newExcludeUntilTime = APIHelper.APIDateTimeStringFromDateTime(user.NewExcludeUntilTime)
            };
            return retVal;
        }

    }   // UserIdentityService_GetUserDetails_LogicLayer

}   // BDDReferenceService.Logic
