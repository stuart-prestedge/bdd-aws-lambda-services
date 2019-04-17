using System;
namespace BDDReferenceService.Contracts
{
    public class DrawRequest
    {
    
        /**
         * The ID of the game record that this draw belongs to.
         */
        public string gameId { get; set; }

        /**
         * The date/time the draw should happen on.
         */
        public string drawDate { get; set; }

        /**
         * The amount, in cents, that this draw will (or did) pay out.
         * Zero if not specified.
         */
        public UInt64 amount { get; set; }

        /**
         * A flag indicating if the amount is an 'up to' amount.
         */
        public bool amountIsUpTo { get; set; }

        /**
         * Does this draw happen automatically or manually?
         */
        public bool autoDraw { get; set; }

    }   // DrawRequest

}   // BDDReferenceService.Contracts
