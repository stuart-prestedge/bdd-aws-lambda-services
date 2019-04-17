using System;
namespace BDDReferenceService.Contracts
{
    public class SetUserGenderRequest
    {
    
        /**
         * A mandatory number. 1 = male, 2 = female, 3 = unspecified.
         */
        public UInt16 gender { get; set; }

    }   // SetUserGenderRequest

}   // BDDReferenceService.Contracts
