using System;
namespace BDDReferenceService.Contracts
{
    public class SetUserEmailRequest
    {
    
        /**
         * A mandatory string that contains the new email address. It must be a valid email address.
         */
        public string emailAddress { get; set; }

    }   // SetUserEmailRequest

}   // BDDReferenceService.Contracts
