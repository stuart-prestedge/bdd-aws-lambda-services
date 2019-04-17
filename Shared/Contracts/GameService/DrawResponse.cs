using System;
namespace BDDReferenceService.Model
{
    public class DrawResponse
    {

        /**
         * The ID of the draw record.
         * Private information.
         */
        public string id { get; set; }

        /**
         * The ID of the game record that this draw belongs to.
         * Private information.
         */
        public string gameId { get; set; }

        /**
         * The date/time the draw should happen on. Can be null if not set.
         * Public information.
         */
        public string drawDate { get; set; }

        /**
         * The date/time the draw actually happened on.
         * Null if not happened yet.
         * Public information.
         */
        public string drawnDate { get; set; }

        /**
         * The amount, in cents, that this draw will (or did) pay out.
         * Null if not decided yet.
         * Public information.
         */
        public UInt64 amount { get; set; }

        /**
         * A flag indicating if the amount is an 'up to' amount.
         */
        public bool amountIsUpTo { get; set; }

        /**
         * The winning number.
         * Null if not drawn yet.
         * Public information.
         */
        public UInt32? winningNumber { get; set; }

        /**
         * The winning user ID.
         * Null if not drawn yet.
         * Private information.
         */
        public string winningUserId { get; set; }

        /**
         * The winning ticket ID.
         * Null if not drawn yet.
         * Private information.
         */
        public string winningTicketId { get; set; }

        /**
         * Does this draw happen automatically or manually?
         * Private information.
         */
        public bool autoDraw { get; set; }

        /**
         * Did this draw happen automatically or manually?
         * Private information.
         */
        public bool autoDrawn { get; set; }

    }   // DrawResponse

}   // BDDReferenceService.Model
