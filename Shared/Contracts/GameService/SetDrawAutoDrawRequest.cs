using System;
namespace BDDReferenceService.Contracts
{
    public class SetDrawAutoDrawRequest
    {
    
        /**
         * The ID of the game record that this draw belongs to.
         */
        public string gameId { get; set; }

        /**
         * A flag indicating if the draw is to be auto-drawn.
         */
        public bool autoDraw { get; set; }

    }   // SetDrawAutoDrawRequest

}   // BDDReferenceService.Contracts
