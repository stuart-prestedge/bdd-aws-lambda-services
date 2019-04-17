using System;
namespace BDDReferenceService.Model
{

    /**
     * Class representing a ticket audit.
     */
    public class TicketAudit
    {

        /**
         * The audit type.
         */
        public enum TicketAuditType
        {
            purchase,
            offer,
            revoke,
            accept,
            reject,
            reserve,
            unreserve,
            bulk_reserve,
            bulk_assign
        }

        /**
         * The ID of the ticket audit record.
         */
        public string ID { get; set; }

        /**
         * The date/time that this audit record was created.
         * This is the official timestamp of the recorded change.
         */
        public DateTime Timestamp { get; set; }

        /**
         * The type of audit.
         */
        public TicketAuditType Type { get; set; }

        /**
         * The ID of the ticket being audited.
         * In the case of bulk reservation, this is null (IS THIS RIGHT? the first ticket number?).
         */
        public string TicketID { get; set; }

        /**
         * The ID of the user that made the change. This is not necessarily the user for whom the reservation/purchase etc. is for (e.g. it may be an administrator).
         */
        public string UserID { get; set; }

        /**
         * The ID of a user.
         * If Type is purchase, this is the owner of the ticket. 
         * If Type is offer, this is the user being offered the ticket. 
         * If Type is revoke, this is the user that was offered the ticket. 
         * If Type is accept, this is the user that offered the ticket. 
         * If Type is reject, this is the user that offered the ticket. 
         * If Type is reserve, this is the user that the ticket is reserved for. 
         * If Type is unreserve, this is the user that the reservation was for. 
         * If Type is bulk reservation, this is the user that the bulk reservation is for. 
         * If Type is bulk assignment, this is the user that is assigned the tickets. 
         * TargetUserID may differ from the user making the change to the ticket.
         */
        public string TargetUserID { get; set; }

        /**
         * The ID of the syndicate that the ticket is purchased for or gifted to.
         */
        internal string SyndicateID { get; set; }

        /**
         * The email of the offered to person.
         * Valid (i.e. non-null) when type is offered.
         */
        public string OfferedToEmail { get; set; }

    }   // TicketAudit

}   // BDDReferenceService.Model
