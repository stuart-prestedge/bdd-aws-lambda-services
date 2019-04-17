using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a syndicate audit.
     */
    public class SyndicateAuditRecord
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
            syndicate = 1,
            member = 2
        }

        /**
         * The ID of the syndicate audit record.
         */
        public string ID { get; set; }

        /**
         * The date/time this audit record was created.
         */
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /**
         * The type of syndicate audit record.
         */
        public AuditChangeType ChangeType { get; set; }

        /**
         * The ID of the syndicate.
         */
        public string SyndicateID { get; set; }

        /**
         * The type of data being audited.
         */
        public AuditDataType DataType { get; set; }

        /**
         * The ID of the object being changed.
         */
        public string TargetID { get; set; }

        /**
         * The ID of the user that performed the change.
         */
        public string UserID { get; set; }

        /**
         * The syndicate/syndicate member state.
         */
        public object TargetCopy { get; set; }
            
    }   // SyndicateAuditRecord

}   // BDDReferenceService.Model
