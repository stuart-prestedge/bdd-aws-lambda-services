using System;
using System.Collections.Generic;

namespace BDDReferenceService.Contracts {

    public class GetCountriesResponse {
    
        /**
         * The ID of the purchased ticket.
         */
        public GetCountryResponse[] countries { get; set; }

    }   // GetCountriesResponse

    public class GetCountryResponse {
    
        /**
         * The country code.
         */
        public string code { get; set; }

        /**
         * The name.
         */
        public string name { get; set; }

        /**
         * The currencies.
         */
        public string[] currencies { get; set; }

    }   // GetCountryResponse

}   // BDDReferenceService.Contracts
