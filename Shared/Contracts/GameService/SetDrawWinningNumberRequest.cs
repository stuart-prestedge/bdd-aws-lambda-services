using System;
namespace BDDReferenceService.Contracts
{
    public class SetDrawWinningNumberRequest
    {
    
        /**
         * The ID of the game record that this draw belongs to.
         */
        public string gameId { get; set; }

        /**
         * The winning number.
         */
        public UInt32 winningNumber { get; set; }

        /**
         * The amount, in US cents, that this draw will (or did) pay out.
         * Null if not to be set.
         */
        public UInt64? amount { get; set; }

    }   // SetDrawWinningNumberRequest

}   // BDDReferenceService.Contracts
