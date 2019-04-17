using System;
namespace BDDReferenceService.Contracts
{
    public class BuyTicketsBaseRequest
    {
    
        /**
         * This is the ID of the user that the purchase is for. If missing or null then the currently logged in user is inferred.
         */
        public string purchasingUserId { get; set; }

        /**
         * The hash of the ID of the game that the ticket is being bought for.
         */
        public string gameIdHash { get; set; }

        /**
         * This is the ID of the syndicate that the purchase is for.
         */
        public string syndicateId { get; set; }

        /**
         * The optional string that contains the currency code of the currency that the user would prefer the ticket to be purchased in. If missing or null or the currency is not available then the service will decide which currency to use to purchase the ticket.
         */
        public string currency { get; set; }

        /**
         * The email address of the person the ticket is being immediately gifted to.
         */
        public string offerToEmail { get; set; }

        /**
         * The message for the person the ticket is being offered to.
         */
        public string offerMessage { get; set; }

    }   // BuyTicketsBaseRequest

}   // BDDReferenceService.Contracts
