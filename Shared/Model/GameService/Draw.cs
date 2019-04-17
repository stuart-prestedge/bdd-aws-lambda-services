using System;
namespace BDDReferenceService.Model
{
    public class Draw
    {

        /**
         * The ID of the draw record.
         * Private information.
         */
        public string ID { get; set; }

        /**
         * The ID of the game record that this draw belongs to.
         * Private information.
         */
        public string GameID { get; set; }

        /**
         * The date/time the draw should happen on. Can be null if not set.
         * Public information.
         */
        public DateTime? DrawDate { get; set; }

        /**
         * The date/time the draw actually happened on.
         * Null if not happened yet.
         * Public information.
         */
        public DateTime? DrawnDate { get; set; }

        /**
         * The amount, in cents, that this draw will (or did) pay out.
         * Null if not decided yet.
         * Public information.
         */
        public UInt64 Amount { get; set; }

        /**
         * A flag indicating if the amount is an 'up to' amount.
         */
        public bool AmountIsUpTo { get; set; }

        /**
         * The winning number.
         * Null if not drawn yet.
         * Public information.
         */
        public UInt32? WinningNumber { get; set; }

        /**
         * The winning user ID.
         * Null if not drawn yet.
         * Private information.
         */
        public string WinningUserID { get; set; }

        /**
         * The winning ticket ID.
         * Null if not drawn yet.
         * Private information.
         */
        public string WinningTicketID { get; set; }

        /**
         * Does this draw happen automatically or manually?
         * Private information.
         */
        public bool AutoDraw { get; set; }

        /**
         * Did this draw happen automatically or manually?
         * Private information.
         */
        public bool AutoDrawn { get; set; }

    }   // Draw

}   // BDDReferenceService.Model
