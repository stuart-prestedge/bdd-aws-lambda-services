using System;
namespace BDDReferenceService.Model
{
    public class PublicDraw
    {

        /**
         * The hash of the ID of the draw record.
         */
        public string IDHash { get; set; }

        /**
         * The hash of the ID of the game record that this draw belongs to.
         */
        public string GameIDHash { get; set; }

        /**
         * The date/time the draw should happen on. Null if not set.
         */
        public string DrawDate { get; set; }

        /**
         * The date/time the draw actually happened on.
         * Null if not happened yet.
         */
        public string DrawnDate { get; set; }

        /**
         * The amount, in cents, that this draw will (or did) pay out.
         * Null if not decided yet.
         */
        public UInt64 Amount { get; set; }

        /**
         * A flag indicating if the amount is an 'up to' amount.
         */
        public bool AmountIsUpTo { get; set; }

        /**
         * The winning number.
         * Null if not drawn yet.
         */
        public UInt32? WinningNumber { get; set; }

    }   // PublicDraw

}   // BDDReferenceService.Model
