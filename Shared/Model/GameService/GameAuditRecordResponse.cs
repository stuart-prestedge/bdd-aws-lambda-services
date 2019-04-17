using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a game audit.
     */
    public class GameAuditRecordResponse
    {

        /**
         * The ID of the game audit record.
         */
        public string id { get; set; }

        /**
         * The date/time this audit record was created.
         */
        public string timestamp { get; set; }

        /**
         * The type of game audit record.
         */
        public Int32 changeType { get; set; }

        /**
         * The ID of the game.
         */
        public string gameId { get; set; }

        /**
         * The ID of the game.
         */
        public Int32 dataType { get; set; }

        /**
         * The ID of the object being changed.
         */
        public string targetId { get; set; }

        /**
         * The ID of the admin user that performed the change.
         */
        public string userId { get; set; }

        /**
         * The game/draw state.
         * Null if type is delete.
         */
        public object targetCopy { get; set; }
            
    }   // GameAuditRecordResponse

}   // BDDReferenceService.Model
