using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a game audit.
     */
    public class GameAuditRecord
    {

        /**
         * The audit change type.
         */
        public enum AuditChangeType
        {
            create = 1,
            update = 2,
            delete = 3
        }

        /**
         * The audit data type.
         */
        public enum AuditDataType
        {
            game = 1,
            draw = 2
        }

        /**
         * The ID of the game audit record.
         */
        public string ID { get; set; }

        /**
         * The date/time this audit record was created.
         */
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /**
         * The type of game audit record.
         */
        public AuditChangeType ChangeType { get; set; }

        /**
         * The ID of the game.
         */
        public string GameID { get; set; }

        /**
         * The type of data being audited.
         */
        public AuditDataType DataType { get; set; }

        /**
         * The ID of the object being changed.
         */
        public string TargetID { get; set; }

        /**
         * The ID of the admin user that performed the change.
         */
        public string UserID { get; set; }

        /**
         * The game/draw state.
         * Null if type is delete.
         */
        public object TargetCopy { get; set; }
            
    }   // GameAuditRecord

}   // BDDReferenceService.Model
