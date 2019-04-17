using System;
namespace BDDReferenceService.Contracts {

    public class GetTicketPricesResponse {
    
        /**
         * The ID of the purchased ticket.
         */
        public GetTicketPriceResponse[] ticketPrices { get; set; }

    }   // GetTicketPricesResponse

    public class GetTicketPriceResponse {
    
        /**
         * The currency.
         */
        public string currency { get; set; }

        /**
         * The price.
         */
        public UInt64 price { get; set; }

    }   // GetTicketPriceResponse

}   // BDDReferenceService.Contracts
