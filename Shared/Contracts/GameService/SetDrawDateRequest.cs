using System;
namespace BDDReferenceService.Contracts
{
    public class SetDrawDateRequest
    {
    
        /**
         * The ID of the game record that this draw belongs to.
         */
        public string gameId { get; set; }

        /**
         * The date/time the draw should happen on.
         */
        public string drawDate { get; set; }

    }   // SetDrawDateRequest

}   // BDDReferenceService.Contracts
