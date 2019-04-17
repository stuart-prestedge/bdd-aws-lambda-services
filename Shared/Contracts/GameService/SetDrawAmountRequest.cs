using System;
namespace BDDReferenceService.Contracts
{
    public class SetDrawAmountRequest
    {
    
        /**
         * The ID of the game record that this draw belongs to.
         */
        public string gameId { get; set; }

        /**
         * The amount, in cents, that this draw will (or did) pay out.
         */
        public UInt64 amount { get; set; }

        /**
         * A flag indicating if the amount is an 'up to' amount.
         */
        public bool amountIsUpTo { get; set; }

    }   // SetDrawAmountRequest

}   // BDDReferenceService.Contracts
