using System;
using System.Collections.Generic;

namespace BDDReferenceService.Model
{

    /**
     * Class representing a ticket price.
     */
    public class Country
    {
        
        /**
         * The country code.
         */
        public string Code { get; set; }

        /**
         * The country name.
         */
        public string Name { get; set; }

        /**
         * The currencies available in the country.
         */
        public List<string> Currencies { get; set; }

        /**
         * Is this country available to end-users?
         */
        public bool Available { get; set; }

    }   // Country

}   // BDDReferenceService.Model
