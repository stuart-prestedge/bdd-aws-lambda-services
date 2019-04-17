using System;
namespace BDDReferenceService.Contracts
{
    public class ClearDrawWinningNumberRequest
    {
    
        /**
         * The ID of the game record that this draw belongs to.
         */
        public string gameId { get; set; }

        /**
         * A flag indicating if the draw is to be auto-drawn.
         */
        public bool? autoDraw { get; set; }

    }   // ClearDrawWinningNumberRequest

}   // BDDReferenceService.Contracts
