using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using BDDReferenceService.Contracts;
using BDDReferenceService.Logic;
using BDDReferenceService.Model;
using Newtonsoft.Json.Linq;

namespace BDDReferenceService {

    /**
     * Logic layer with helper methods.
     */
    internal static class SyndicateServiceLogicLayer {

        /**
         * Syndicate service errors.
         */
        internal const string ERROR_INVALID_SYNDICATE_ID = "INVALID_SYNDICATE_ID";
        internal const string ERROR_SYNDICATE_NOT_FOUND = "SYNDICATE_NOT_FOUND";
        internal const string ERROR_USER_NOT_SYNDICATE_MEMBER = "USER_NOT_SYNDICATE_MEMBER";
        internal const string ERROR_SYNDICATE_MEMBER_NOT_FOUND = "SYNDICATE_MEMBER_NOT_FOUND";
        internal const string ERROR_SYNDICATE_MEMBER_NOT_PENDING = "SYNDICATE_MEMBER_NOT_PENDING";
        internal const string ERROR_SYNDICATE_MEMBER_NOT_FULL = "SYNDICATE_MEMBER_NOT_FULL";
        internal const string ERROR_USER_ALREADY_SYNDICATE_MEMBER = "USER_ALREADY_SYNDICATE_MEMBER";
        internal const string ERROR_MEMBER_LAST_SYNDICATE = "MEMBER_LAST_SYNDICATE";
        internal const string ERROR_SYNDICATE_LAST_MEMBER_WITH_TICKETS = "SYNDICATE_LAST_MEMBER_WITH_TICKETS";

        /**
         * Syndicate service link types.
         */
        #region Syndicate service link types
        private const string LINK_TYPE_REJECT_SYNDICATE_MEMBER_INVITATION = "REJECT_SYNDICATE_MEMBER_INVITATION";
        #endregion Syndicate service link types

        /**
         * The syndicate service global settings.
         */
        internal static Dictionary<string, object> GlobalSettings = new Dictionary<string, object>();

        #region Helper methods

        /**
         * The ID of the reject invitation link to be sent. This is debug code.
         */
        internal static string RejectInvitationLinkId = null;

        /**
         * Reset the data.
         */
        internal static void Reset() {
            Debug.Tested();

            GlobalSettings.Clear();
            RejectInvitationLinkId = null;
        }

        /**
         * Convert a syndicate into a syndicate summary.
         */
        internal static SyndicateSummaryResponse SyndicateSummaryResponseFromSyndicate(string userId, Syndicate syndicate) {
            Debug.Tested();
            Debug.AssertIDOrNull(userId);
            Debug.AssertValid(syndicate);
            Debug.AssertID(syndicate.ID);

            List<SyndicateMember> syndicateMembers = SyndicateServiceLogicLayer.GetSyndicateMembers(syndicate.ID);
            Debug.AssertValid(syndicateMembers);
            SyndicateMember.SyndicateMemberStatus userStatus = SyndicateMember.SyndicateMemberStatus.unknown;
            if (userId != null) {
                Debug.Tested();
                foreach (SyndicateMember syndicateMember in syndicateMembers) {
                    Debug.Tested();
                    Debug.AssertValid(syndicateMember);
                    if (syndicateMember.UserID == userId) {
                        userStatus = syndicateMember.Status;
                        break;
                    }
                }
            }
            SyndicateSummaryResponse response = new SyndicateSummaryResponse {
                id = syndicate.ID,
                name = syndicate.Name,
                status = (int)userStatus
            };
            return response;
        }

        /**
         * Convert a syndicate into a syndicate details.
         */
        internal static async Task<SyndicateDetailsResponse> SyndicateDetailsResponseFromSyndicate(AmazonDynamoDBClient dbClient, string userId, Syndicate syndicate) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertIDOrNull(userId);
            Debug.AssertValid(syndicate);
            Debug.AssertID(syndicate.ID);

            List<SyndicateMember> syndicateMembers = SyndicateServiceLogicLayer.GetSyndicateMembers(syndicate.ID);
            Debug.AssertValid(syndicateMembers);
            SyndicateMember.SyndicateMemberStatus userStatus = SyndicateMember.SyndicateMemberStatus.unknown;
            List<SyndicateMemberDetails> fullMembers = new List<SyndicateMemberDetails>();
            List<SyndicateMemberDetails> pendingMembers = new List<SyndicateMemberDetails>();
            foreach (SyndicateMember syndicateMember in syndicateMembers) {
                Debug.Tested();
                Debug.AssertValid(syndicateMember);
                if ((userId != null) && (syndicateMember.UserID == userId)) {
                    userStatus = syndicateMember.Status;
                }
                User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, syndicateMember.UserID, true);
                Debug.AssertValid(user);//??++CHECK
                string userName = NameHelper.GetDisplayName(user.GivenName, user.FamilyName);
                string userEmailAddress = user.EmailAddress;
                SyndicateMemberDetails syndicateMemberDetails = new SyndicateMemberDetails {
                    userId = syndicateMember.UserID,
                    name = userName,
                    emailAddress = userEmailAddress,
                };
                if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.full) {
                    Debug.Tested();
                    fullMembers.Add(syndicateMemberDetails);
                } else {
                    Debug.Tested();
                    pendingMembers.Add(syndicateMemberDetails);
                }
            }
            List<string> ticketIds = new List<string>();
            List<Ticket> tickets = TicketingServiceLogicLayer.GetSyndicateTickets(syndicate.ID);
            Debug.AssertValid(tickets);
            foreach (Ticket ticket in tickets) {
                Debug.Tested();
                Debug.AssertValid(ticket);
                Debug.AssertID(ticket.ID);
                ticketIds.Add(ticket.ID);
            }
            SyndicateDetailsResponse response = new SyndicateDetailsResponse {
                id = syndicate.ID,
                name = syndicate.Name,
                members = fullMembers.ToArray(),
                pendingMembers = pendingMembers.ToArray(),
                tickets = ticketIds.ToArray()
            };
            return response;
        }

        /**
         * Invite a new member to the syndicate specified by ID.
         */
        private static async Task DoInviteMember(AmazonDynamoDBClient dbClient, string loggedInUserId, Syndicate syndicate, string emailAddress, string message) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(syndicate);
            Debug.AssertID(syndicate.ID);
            Debug.AssertEmail(emailAddress);
            Debug.AssertValidOrNull(message);

            // Get the user being invited.
            User user = await IdentityServiceLogicLayer.GetOrCreateUserByEmailAddress(dbClient, emailAddress);
            Debug.AssertValid(user);
            Debug.AssertID(user.ID);

            // Check for existing syndicate member
            SyndicateMember existingSyndicateMember = TestDataHelper.SyndicateMembers.Find(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicate.ID) && (syndicateMember_.UserID == user.ID)));
            Debug.AssertValidOrNull(existingSyndicateMember);
            if (existingSyndicateMember == null) {
                Debug.Tested();

                // Add the syndicate member
                SyndicateMember syndicateMember = new SyndicateMember {
                    SyndicateID = syndicate.ID,
                    UserID = user.ID,
                    Status = SyndicateMember.SyndicateMemberStatus.pending,
                    Message = message
                };
                AddSyndicateMember(syndicateMember, loggedInUserId);

                // Email the invitation to the email address
                Link link = await IdentityServiceLogicLayer.CreateLink(dbClient, LINK_TYPE_REJECT_SYNDICATE_MEMBER_INVITATION, user.ID);
                Debug.AssertValid(link);
                Debug.AssertString(link.ID);
                Dictionary<string, string> replacementFields = new Dictionary<string, string>();
                string rootURL = "https://app.bdd.com";
                replacementFields["url"] = rootURL + "/syndicate-invitation?syndicate=" + syndicate.ID;
                replacementFields["link"] = link.ID;
                EmailHelper.EmailTemplate(EmailHelper.EMAIL_TEMPLATE_INVITE_SYNDICATE_MEMBER, emailAddress, replacementFields);

                // Debug code
                RejectInvitationLinkId = link.ID;
            } else {
                Debug.Untested();
                throw new Exception(ERROR_USER_ALREADY_SYNDICATE_MEMBER);
            }
            return;
        }

        /**
         * Accept a member invitation to the syndicate specified by ID.
         */
        private static void DoAcceptInvitation(string loggedInUserId, SyndicateMember syndicateMember) {
            Debug.Tested();
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(syndicateMember);

            if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.pending) {
                Debug.Tested();

                // Make the syndicate member full
                syndicateMember.Status = SyndicateMember.SyndicateMemberStatus.full;

                // Remove the message
                syndicateMember.Message = null;

                // Add the audit
                AddSyndicateMemberAudit(SyndicateAuditRecord.AuditChangeType.update, loggedInUserId, syndicateMember);
            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_PENDING);
            }
        }

        /**
         * Reject a member invitation to the syndicate specified by ID.
         */
        private static void DoRejectInvitation(string userId, string syndicateId) {
            Debug.Tested();
            Debug.AssertID(userId);
            Debug.AssertID(syndicateId);

            // Get the syndicate (checks logged in user is a member)
            Syndicate existingSyndicate = SyndicateServiceLogicLayer.GetSyndicate(userId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(existingSyndicate);
            Debug.AssertValid(syndicateMember);

            // Reject the invitation
            DoRejectInvitationEx(userId, syndicateMember);
        }

        /**
         * Reject a member invitation to the syndicate specified by ID.
         */
        private static void DoRejectInvitationEx(string loggedInUserId, SyndicateMember syndicateMember) {
            Debug.Tested();
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(syndicateMember);

            if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.pending) {
                Debug.Tested();

                // Remove the syndicate member
                TestDataHelper.SyndicateMembers.Remove(syndicateMember);

                // Add the audit
                AddSyndicateMemberAudit(SyndicateAuditRecord.AuditChangeType.delete, loggedInUserId, syndicateMember);

            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_PENDING);
            }
        }

        /**
         * Revoke a member invitation to the syndicate specified by ID.
         */
        internal static void DoRevokeMemberInvitation(string loggedInUserId, string syndicateId, string invitedUserId) {
            Debug.Tested();
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);
            Debug.AssertID(invitedUserId);

            // Find the syndicate member for the invited user
            SyndicateMember syndicateMember = TestDataHelper.SyndicateMembers.Find(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicateId) && (syndicateMember_.UserID == invitedUserId)));
            Debug.AssertValidOrNull(syndicateMember);

            if (syndicateMember != null) {
                Debug.Tested();
                if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.pending) {
                    Debug.Tested();

                    // Remove the syndicate member
                    TestDataHelper.SyndicateMembers.Remove(syndicateMember);

                    // Add the audit
                    AddSyndicateMemberAudit(SyndicateAuditRecord.AuditChangeType.delete, loggedInUserId, syndicateMember);
                } else {
                    Debug.Untested();
                    throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_PENDING);
                }
            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_FOUND);
            }
        }

        /**
         * Get global setting.
         */
        internal static object GetGlobalSetting(string globalSettingName, object defaultValue = null) {
            Debug.Untested();
            Debug.AssertString(globalSettingName);
            Debug.AssertValidOrNull(defaultValue);
            Debug.AssertValid(GlobalSettings);

            object retVal = defaultValue;
            if (GlobalSettings.ContainsKey(globalSettingName)) {
                Debug.Untested();
                retVal = GlobalSettings[globalSettingName];
                Debug.AssertValidOrNull(retVal);
            }
            return retVal;
        }

        #endregion Helper methods

        #region Syndicates

        /**
         * Get the specified user's syndicates.
         */
        internal static async Task<int> GetInvitationCount(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(TestDataHelper.SyndicateMembers);
            Debug.AssertValid(TestDataHelper.Syndicates);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            return TestDataHelper.SyndicateMembers.Count(syndicateMember_ => ((syndicateMember_.UserID == userId) && (syndicateMember_.Status == SyndicateMember.SyndicateMemberStatus.pending)));
        }

        /**
         * Get the specified syndicate. The logged in user must be a member (pending or full) of the syndicate.
         */
        internal static async Task<Syndicate> GetSyndicateDetails(AmazonDynamoDBClient dbClient, string loggedInUserId, string syndicateId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);
            Debug.AssertValid(TestDataHelper.Syndicates);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Syndicate syndicate = GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(syndicate);
            return syndicate;
        }

        /**
         * Get the specified syndicate. The specified in user must be a member (pending or full) of the syndicate.
         */
        internal static Syndicate GetSyndicate(string userId, string syndicateId, out SyndicateMember syndicateMember) {
            Debug.Tested();
            Debug.AssertID(userId);
            Debug.AssertID(syndicateId);
            Debug.AssertValid(TestDataHelper.Syndicates);

            syndicateMember = TestDataHelper.SyndicateMembers.Find(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicateId) && (syndicateMember_.UserID == userId)));
            Debug.AssertValidOrNull(syndicateMember);
            if (syndicateMember != null) {
                // The user is a full or pending member of the specified syndicate
                Debug.Tested();
                Syndicate syndicate = TestDataHelper.Syndicates.Find(syndicate_ => (syndicate_.ID == syndicateId));
                Debug.AssertValidOrNull(syndicate);
                if (syndicate != null) {
                    // The syndicate exists
                    Debug.Tested();
                    return syndicate;
                } else {
                    Debug.Untested();
                    throw new Exception(ERROR_SYNDICATE_NOT_FOUND, new Exception(ERROR_SYNDICATE_NOT_FOUND));
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_USER_NOT_SYNDICATE_MEMBER, new Exception(ERROR_USER_NOT_SYNDICATE_MEMBER));
            }
        }

        /**
         * Get all the syndicate members of a syndicate.
         */
        internal static List<SyndicateMember> GetSyndicateMembers(string syndicateId) {
            Debug.Tested();
            Debug.AssertID(syndicateId);
            Debug.AssertValid(TestDataHelper.SyndicateMembers);

            return TestDataHelper.SyndicateMembers.FindAll(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicateId)));
        }

        /**
         * Get the specified user's syndicates.
         */
        internal static async Task<List<Tuple<Syndicate, SyndicateMember>>> GetUserSyndicates(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(TestDataHelper.SyndicateMembers);
            Debug.AssertValid(TestDataHelper.Syndicates);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            List<Tuple<Syndicate, SyndicateMember>> retVal = new List<Tuple<Syndicate, SyndicateMember>>();
            List<SyndicateMember> syndicateMembers = TestDataHelper.SyndicateMembers.FindAll(syndicateMember_ => (syndicateMember_.UserID == userId));
            Debug.AssertValid(syndicateMembers);
            foreach (SyndicateMember syndicateMember in syndicateMembers) {
                Debug.Tested();
                Debug.AssertValid(syndicateMember);
                Syndicate syndicate = TestDataHelper.Syndicates.Find(syndicate_ => (syndicate_.ID == syndicateMember.SyndicateID));
                Debug.AssertValidOrNull(syndicate);
                if (syndicate != null) {
                    Debug.Tested();
                    retVal.Add(new Tuple<Syndicate, SyndicateMember>(syndicate, syndicateMember));
                }
            }
            return retVal;
        }

        /**
         * Get the counts for the specified syndicate.
         */
        internal static void GetSyndicateCounts(string syndicateId, out int memberCount, out int ticketCount) {
            Debug.Tested();
            Debug.AssertID(syndicateId);
            Debug.AssertValid(TestDataHelper.SyndicateMembers);
            Debug.AssertValid(TestDataHelper.Tickets);

            memberCount = TestDataHelper.SyndicateMembers.Count(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicateId) && (syndicateMember_.Status == SyndicateMember.SyndicateMemberStatus.full)));
            ticketCount = TestDataHelper.Tickets.Count(ticket_ => (ticket_.SyndicateID == syndicateId));
        }

        /**
         * Is the syndicate valid as a request to create/update syndicates?
         */
        internal static SyndicateRequest CheckValidRequestSyndicate(JObject requestBody, bool forCreate) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (!string.IsNullOrEmpty((string)requestBody["name"])) {
                    Debug.Tested();
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "name"
                                                        }))) {
                        Debug.Tested();
                        return new SyndicateRequest {
                            name = (string)requestBody["name"]
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Tested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Tested();
                    error = APIHelper.INVALID_NAME;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Creates a syndicate.
         */
        internal static async Task<Syndicate> CreateSyndicate(AmazonDynamoDBClient dbClient, string name, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(name);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Syndicates);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Add the new syndicate
            Syndicate retVal = new Syndicate {
                Name = name
            };
            AddSyndicate(retVal, loggedInUserId);

            // Add the logged in user as a syndicate member
            SyndicateMember syndicateMember = new SyndicateMember {
                SyndicateID = retVal.ID,
                UserID = loggedInUserId,
                Status = SyndicateMember.SyndicateMemberStatus.full
            };
            AddSyndicateMember(syndicateMember, loggedInUserId);

            return retVal;
        }

        /**
         * Adds a syndicate.
         */
        private static void AddSyndicate(Syndicate syndicate, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(syndicate);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Syndicates);
            //??--Debug.Assert(!SystemHelper.GetSystemLocked());

            // Set the syndicate's ID
            if (syndicate.ID == Helper.INVALID_ID) {
                syndicate.ID = RandomHelper.Next();
            }
            // Add the syndicate
            TestDataHelper.Syndicates.Add(syndicate);

            // Add the audit
            AddSyndicateAudit(SyndicateAuditRecord.AuditChangeType.create, loggedInUserId, syndicate);
        }

        /**
         * Adds a syndicate.
         */
        private static void AddSyndicateMember(SyndicateMember syndicateMember, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(syndicateMember);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.SyndicateMembers);
            //??--Debug.Assert(!SystemHelper.GetSystemLocked());

            // Set the syndicate member's ID
            if (syndicateMember.ID == Helper.INVALID_ID) {
                syndicateMember.ID = RandomHelper.Next();
            }
            // Add the syndicate member
            TestDataHelper.SyndicateMembers.Add(syndicateMember);

            // Add the audit
            AddSyndicateMemberAudit(SyndicateAuditRecord.AuditChangeType.create, loggedInUserId, syndicateMember);
        }

        /**
         * Update the syndicate specified by ID.
         */
        internal static async Task<Syndicate> ModifySyndicate(AmazonDynamoDBClient dbClient, string syndicateId, SyndicateRequest syndicateRequest, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(syndicateId);
            Debug.AssertValid(syndicateRequest);
            Debug.AssertID(loggedInUserId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (checks logged in user is a member)
            Syndicate existingSyndicate = SyndicateServiceLogicLayer.GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(existingSyndicate);

            // Check if the syndicate member is a full member
            if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.full) {
                Debug.Tested();

                // Update the syndicate
                existingSyndicate.Name = syndicateRequest.name;
            
                // Add the audit
                AddSyndicateAudit(SyndicateAuditRecord.AuditChangeType.update, loggedInUserId, existingSyndicate);

                return existingSyndicate;
            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_FULL);
            }
        }

        /**
         * Is the invite member request object valid?
         */
        internal static InviteMemberRequest CheckValidInviteMemberRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetStringFromRequestBody(requestBody, "emailAddress", out string emailAddress) && Helper.IsValidEmail(emailAddress)) {
                    Debug.Tested();
                    if (APIHelper.GetOptionalStringFromRequestBody(requestBody, "message", out string message)) {
                        Debug.Tested();
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "emailAddress",
                                                            "message"
                                                            }))) {
                            Debug.Tested();
                            return new InviteMemberRequest {
                                emailAddress = emailAddress,
                                message = message
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Untested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = APIHelper.INVALID_MESSAGE;
                    }
                } else {
                    Debug.Untested();
                    error = APIHelper.INVALID_EMAIL_ADDRESS;
                }
            } else {
                // No body
                Debug.Untested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Invite a new member to the syndicate specified by ID.
         */
        internal static async Task<Syndicate> InviteMember(AmazonDynamoDBClient dbClient, string loggedInUserId, string syndicateId, string emailAddress, string message) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);
            Debug.AssertEmail(emailAddress);
            Debug.AssertValidOrNull(message);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (checks logged in user is a member)
            Syndicate existingSyndicate = SyndicateServiceLogicLayer.GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(existingSyndicate);
            Debug.Assert(existingSyndicate.ID == syndicateId);

            // Check if the syndicate member is a full member
            if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.full) {
                Debug.Tested();

                // Actually invite the member
                await DoInviteMember(dbClient, loggedInUserId, existingSyndicate, emailAddress, message);

                return existingSyndicate;
            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_FULL);
            }
        }

        /**
         * Accept a member invitation to the syndicate specified by ID.
         */
        internal static async Task<Syndicate> AcceptInvitation(AmazonDynamoDBClient dbClient, string loggedInUserId, string syndicateId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (checks logged in user is a member)
            Syndicate existingSyndicate = SyndicateServiceLogicLayer.GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(existingSyndicate);

            // Accept the invitation
            DoAcceptInvitation(loggedInUserId, syndicateMember);

            return existingSyndicate;
        }

        /**
         * Reject a member invitation to the syndicate specified by ID.
         */
        internal static async Task RejectInvitation(AmazonDynamoDBClient dbClient, string loggedInUserId, string syndicateId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            DoRejectInvitation(loggedInUserId, syndicateId);
        }

        /**
         * Is the invite member request object valid?
         */
        internal static RejectInvitationFromLinkRequest CheckValidRejectInvitationFromLinkRequest(JObject requestBody) {
            Debug.Untested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Untested();
                if (APIHelper.GetStringFromRequestBody(requestBody, "linkId", out string linkId)) {
                    Debug.Untested();
                    if (APIHelper.GetStringFromRequestBody(requestBody, "emailAddress", out string emailAddress) && Helper.IsValidEmail(emailAddress)) {
                        Debug.Untested();
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "linkId",
                                                            "emailAddress"
                                                            }))) {
                            Debug.Untested();
                            return new RejectInvitationFromLinkRequest {
                                linkId = linkId,
                                emailAddress = emailAddress
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Untested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = APIHelper.INVALID_EMAIL_ADDRESS;
                    }
                } else {
                    Debug.Untested();
                    error = APIHelper.INVALID_LINK_ID;
                }
            } else {
                // No body
                Debug.Untested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set user password after reset.
         */
        internal static async Task RejectInvitationFromLink(AmazonDynamoDBClient dbClient,
                                                            RejectInvitationFromLinkRequest rejectInvitationFromLinkRequest,
                                                            string syndicateId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(rejectInvitationFromLinkRequest);
            Debug.AssertString(rejectInvitationFromLinkRequest.linkId);
            Debug.AssertEmail(rejectInvitationFromLinkRequest.emailAddress);
            Debug.AssertID(syndicateId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Link link = await IdentityServiceLogicLayer.FindValidLink(dbClient, rejectInvitationFromLinkRequest.linkId, LINK_TYPE_REJECT_SYNDICATE_MEMBER_INVITATION);
            Debug.AssertValidOrNull(link);
            if (link != null) {
                Debug.Untested();
                Debug.AssertID(link.UserID);
                User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, link.UserID, true);
                Debug.AssertValidOrNull(user);
                if (user != null) {
                    Debug.Untested();
                    Debug.Assert(user.ID == link.UserID);
                    Debug.AssertEmail(user.EmailAddress);
                    if (user.EmailAddress == rejectInvitationFromLinkRequest.emailAddress) {
                        Debug.Untested();
                        DoRejectInvitation(user.ID, syndicateId);
                        link.Revoked = true;
                    } else {
                        Debug.Untested();
                        throw new Exception(SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS);
                    }
                } else {
                    // User does not exist - may have been closed (and possibly subsequently deleted).
                    Debug.Untested();
                    throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK_USER);
                }
            } else {
                Debug.Untested();
                throw new Exception(SharedLogicLayer.ERROR_INVALID_LINK, new Exception(SharedLogicLayer.ERROR_INVALID_LINK));
            }
        }

        /**
         * Is the invite member request object valid?
         */
        internal static RevokeMemberInvitationRequest CheckValidRevokeMemberInvitationRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "userId", out string userId) && Helper.IsValidID(userId)) {
                    Debug.Tested();
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "userId"
                                                        }))) {
                        Debug.Tested();
                        return new RevokeMemberInvitationRequest {
                            userId = userId
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Untested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                    error = APIHelper.INVALID_USER_ID;
                }
            } else {
                // No body
                Debug.Untested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Revoke a member invitation to the syndicate specified by ID.
         */
        internal static async Task<Syndicate> RevokeMemberInvitation(AmazonDynamoDBClient dbClient, string loggedInUserId, string syndicateId, string invitedUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);
            Debug.AssertID(invitedUserId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (checks logged in user is a member)
            Syndicate existingSyndicate = SyndicateServiceLogicLayer.GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(existingSyndicate);

            // Check if the syndicate member is a full member
            if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.full) {
                Debug.Tested();

                // Revoke the invitation
                DoRevokeMemberInvitation(loggedInUserId, syndicateId, invitedUserId);
                
                return existingSyndicate;
            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_FULL);
            }
        }

        /**
         * Revoke a member invitation to the syndicate specified by ID.
         */
        internal static async Task LeaveSyndicate(AmazonDynamoDBClient dbClient, string loggedInUserId, string syndicateId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (checks logged in user is a member)
            Syndicate existingSyndicate = SyndicateServiceLogicLayer.GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(existingSyndicate);
            Debug.AssertValid(syndicateMember);

            // Check if the syndicate member is a full member
            if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.full) {
                Debug.Tested();

                // Check that the member can leave
                CheckMemberCanLeaveSyndicate(syndicateMember, existingSyndicate);

                // Remove the syndicate member
                TestDataHelper.SyndicateMembers.Remove(syndicateMember);

                // Add the audit
                AddSyndicateMemberAudit(SyndicateAuditRecord.AuditChangeType.delete, loggedInUserId, syndicateMember);
            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_FULL);
            }
        }

        /**
         * Check if a member can leave a syndicate.
         * If this is the member's only syndicate then an error is thrown.
         * If this is the syndicate's only member and the syndicate has tickets then an error is thrown.
         */
        private static void CheckMemberCanLeaveSyndicate(SyndicateMember syndicateMember, Syndicate syndicate) {
            Debug.Tested();
            Debug.AssertValid(syndicateMember);
            Debug.AssertValid(syndicate);

            // Check whether there is only one syndicate for the member.
            int memberSyndicates = TestDataHelper.SyndicateMembers.Count(syndicateMember_ => ((syndicateMember_.UserID == syndicateMember.UserID) && (syndicateMember_.Status == SyndicateMember.SyndicateMemberStatus.full)));
            Debug.Assert(memberSyndicates > 0);
            if (memberSyndicates == 1) {
                Debug.Untested();
                throw new Exception(ERROR_MEMBER_LAST_SYNDICATE);
            } else {
                Debug.Tested();
            }
            // Check whether there is only one member for the syndicate.
            int syndicateMembers = TestDataHelper.SyndicateMembers.Count(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicate.ID) && (syndicateMember_.Status == SyndicateMember.SyndicateMemberStatus.full)));
            Debug.Assert(syndicateMembers > 0);
            if (syndicateMembers == 1) {
                Debug.Untested();
                int tickets = TestDataHelper.Tickets.Count(ticket_ => (ticket_.SyndicateID == syndicate.ID));
                if (tickets > 0) {
                    Debug.Untested();
                    throw new Exception(ERROR_SYNDICATE_LAST_MEMBER_WITH_TICKETS);
                } else {
                    Debug.Untested();
                }
            } else {
                Debug.Tested();
            }
        }

        /**
         * Check validity of get audit records request inputs.
         */
        internal static GetSyndicateAuditsRequest CheckValidGetSyndicateAuditsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetDateTimeFromRequestBody(requestBody, "from", out string from)) {
                    Debug.Tested();
                    Debug.Assert(APIHelper.IsValidAPIDateTimeString(from));
                    if (APIHelper.GetDateTimeFromRequestBody(requestBody, "to", out string to)) {
                        Debug.Tested();
                        Debug.Assert(APIHelper.IsValidAPIDateTimeString(to));
                        if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "syndicateId", out string syndicateId)) {
                            Debug.Tested();
                            Debug.AssertIDOrNull(syndicateId);
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "from",
                                                                "to",
                                                                "syndicateId"
                                                                }))) {
                                Debug.Tested();
                                return new GetSyndicateAuditsRequest {
                                    from = from,
                                    to = to,
                                    syndicateId = syndicateId
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Untested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                        }
                    } else {
                        Debug.Untested();
                    }
                } else {
                    Debug.Untested();
                }
            } else {
                // No body
                Debug.Untested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Get a list of all the syndicate audits in a time range.
         */
        internal static async Task<List<SyndicateAuditRecord>> GetSyndicateAudits(AmazonDynamoDBClient dbClient, string fromStr, string toStr, string syndicateId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(fromStr));
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(toStr));
            Debug.AssertIDOrNull(syndicateId);
            Debug.AssertValid(TestDataHelper.SyndicateAuditRecords);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            DateTime from = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(fromStr);
            Debug.AssertValid(from);
            DateTime to = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(toStr);
            Debug.AssertValid(to);
            if (from <= to) {
                Debug.Tested();
                List<SyndicateAuditRecord> retVal = new List<SyndicateAuditRecord>();
                foreach (SyndicateAuditRecord syndicateAudit in TestDataHelper.SyndicateAuditRecords) {
                    Debug.Tested();
                    Debug.AssertValid(syndicateAudit);
                    if (from <= syndicateAudit.Timestamp) {
                        Debug.Tested();
                        if (syndicateAudit.Timestamp < to) {
                            Debug.Tested();
                            if ((syndicateId == null) || (syndicateAudit.SyndicateID == syndicateId)) {
                                Debug.Tested();
                                retVal.Add(syndicateAudit);
                            } else {
                                Debug.Untested();
                            }
                        } else {
                            Debug.Untested();
                        }
                    } else {
                        Debug.Untested();
                    }
                }
                return retVal;
            } else {
                Debug.Untested();
                throw new Exception(SharedLogicLayer.ERROR_FROM_GREATER_THAN_TO);
            }
        }

        #endregion Syndicates

        #region Syndicate audits

        /**
         * Add a syndicate audit record.
         */
        private static void AddSyndicateAudit(SyndicateAuditRecord.AuditChangeType changeType,
                                              string userId,
                                              Syndicate syndicate) {
            Debug.Tested();
            Debug.AssertID(userId);
            Debug.AssertValid(syndicate);
            Debug.AssertID(syndicate.ID);
            Debug.AssertValid(TestDataHelper.SyndicateAuditRecords);

            SyndicateAuditRecord syndicateAudit = new SyndicateAuditRecord {
                ID = RandomHelper.Next(),
                Timestamp = DateTime.Now,
                ChangeType = changeType,
                SyndicateID = syndicate.ID,
                DataType = SyndicateAuditRecord.AuditDataType.syndicate,
                TargetID = syndicate.ID,
                UserID = userId
            };
            syndicateAudit.TargetCopy = new Syndicate {
                ID = syndicate.ID,
                Name = syndicate.Name
            };
            TestDataHelper.SyndicateAuditRecords.Add(syndicateAudit);
        }

        /**
         * Add a syndicate audit record.
         */
        private static void AddSyndicateMemberAudit(SyndicateAuditRecord.AuditChangeType changeType,
                                                    string userId,
                                                    SyndicateMember syndicateMember) {
            Debug.Tested();
            Debug.AssertID(userId);
            Debug.AssertValid(syndicateMember);
            Debug.AssertID(syndicateMember.ID);
            Debug.AssertValid(TestDataHelper.SyndicateAuditRecords);

            SyndicateAuditRecord syndicateAudit = new SyndicateAuditRecord {
                ID = RandomHelper.Next(),
                Timestamp = DateTime.Now,
                ChangeType = changeType,
                SyndicateID = syndicateMember.SyndicateID,
                DataType = SyndicateAuditRecord.AuditDataType.member,
                TargetID = syndicateMember.ID,
                UserID = userId
            };
            if (changeType != SyndicateAuditRecord.AuditChangeType.delete) {
                syndicateAudit.TargetCopy = new SyndicateMember {
                    ID = syndicateMember.ID,
                    SyndicateID = syndicateMember.SyndicateID,
                    UserID = syndicateMember.UserID,
                    Status = syndicateMember.Status,
                    Message = syndicateMember.Message
                };
            }
            TestDataHelper.SyndicateAuditRecords.Add(syndicateAudit);
        }

        #endregion Syndicate audits

        #region Admin syndicate service methods
        
        /**
         * Get the specified syndicate. The logged in user need not be a member (pending or full) of the syndicate.
         */
        internal static async Task<Syndicate> GetSyndicateAdmin(AmazonDynamoDBClient dbClient, string syndicateId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(syndicateId);
            Debug.AssertValid(TestDataHelper.Syndicates);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Syndicate syndicate = TestDataHelper.Syndicates.Find(syndicate_ => (syndicate_.ID == syndicateId));
            Debug.AssertValidOrNull(syndicate);
            if (syndicate != null) {
                // The syndicate exists
                Debug.Tested();
                return syndicate;
            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_NOT_FOUND, new Exception(ERROR_SYNDICATE_NOT_FOUND));
            }
        }

        /**
         * Is the invite member request object valid?
         */
        internal static AdminCreateSyndicateRequest CheckValidAdminCreateSyndicateRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (!string.IsNullOrEmpty((string)requestBody["name"])) {
                    Debug.Tested();
                    if (APIHelper.GetIDArrayFromRequestBody(requestBody, "userIds", out string[] userIds) && (userIds.Length > 0)) {
                        Debug.Tested();
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "name",
                                                            "userIds"
                                                            }))) {
                            Debug.Tested();
                            return new AdminCreateSyndicateRequest {
                                name = (string)requestBody["name"],
                                userIds = userIds
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Untested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        //???error = APIHelper.EMPTY_ARRAY;
                    }
                } else {
                    Debug.Untested();
                    ;
                }
            } else {
                // No body
                Debug.Untested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Creates a syndicate.
         */
        internal static async Task<Syndicate> CreateSyndicateAdmin(AmazonDynamoDBClient dbClient, AdminCreateSyndicateRequest adminCreateSyndicateRequest, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(adminCreateSyndicateRequest);
            Debug.AssertValid(adminCreateSyndicateRequest.userIds);
            Debug.Assert(adminCreateSyndicateRequest.userIds.Length > 0);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Syndicates);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Add the new syndicate
            Syndicate retVal = new Syndicate {
                Name = adminCreateSyndicateRequest.name
            };
            AddSyndicate(retVal, loggedInUserId);

            foreach (string userId in adminCreateSyndicateRequest.userIds) {
                Debug.Tested();
                Debug.AssertID(userId);
                // Add the logged in user as a syndicate member
                SyndicateMember syndicateMember = new SyndicateMember {
                    SyndicateID = retVal.ID,
                    UserID = userId,
                    Status = SyndicateMember.SyndicateMemberStatus.full
                };
                AddSyndicateMember(syndicateMember, loggedInUserId);
            }
            return retVal;
        }

        /**
         * Update the syndicate specified by ID.
         */
        internal static async Task<Syndicate> ModifySyndicateAdmin(AmazonDynamoDBClient dbClient, string syndicateId, SyndicateRequest syndicateRequest, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(syndicateId);
            Debug.AssertValid(syndicateRequest);
            Debug.AssertID(loggedInUserId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (no need to be a member as this is an admin method)
            Syndicate existingSyndicate = await SyndicateServiceLogicLayer.GetSyndicateAdmin(dbClient, syndicateId);
            Debug.AssertValid(existingSyndicate);

            // Update the syndicate
            existingSyndicate.Name = syndicateRequest.name;
        
            // Add the audit
            AddSyndicateAudit(SyndicateAuditRecord.AuditChangeType.update, loggedInUserId, existingSyndicate);

            return existingSyndicate;
        }

        /**
         * Is the admin syndicate member request object valid?
         */
        internal static AdminSyndicateMemberRequest CheckValidAdminSyndicateMemberRequest(JObject requestBody) {
            Debug.Untested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "userId", out string userId) && Helper.IsValidID(userId)) {
                    Debug.Tested();
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "userId"
                                                        }))) {
                        Debug.Tested();
                        return new AdminSyndicateMemberRequest {
                            userId = userId
                        };
                    } else {
                        // Unrecognised field(s)
                        Debug.Untested();
                        error = APIHelper.UNRECOGNISED_FIELD;
                    }
                } else {
                    Debug.Untested();
                }
            } else {
                // No body
                Debug.Untested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Invite a new member to the syndicate specified by ID.
         */
        internal static async Task AddSyndicateMember(AmazonDynamoDBClient dbClient, string syndicateId, string userId, string adminUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(syndicateId);
            Debug.AssertID(userId);
            Debug.AssertID(adminUserId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (no need to be a member as this is an admin method)
            Syndicate existingSyndicate = await SyndicateServiceLogicLayer.GetSyndicateAdmin(dbClient, syndicateId);
            Debug.AssertValid(existingSyndicate);

            // Add the syndicate member
            SyndicateMember syndicateMember = new SyndicateMember {
                SyndicateID = syndicateId,
                UserID = userId,
                Status = SyndicateMember.SyndicateMemberStatus.full,
                Message = null
            };
            AddSyndicateMember(syndicateMember, adminUserId);
        }

        /**
         * Remove a member from the syndicate specified by ID.
         */
        internal static async Task RemoveSyndicateMember(AmazonDynamoDBClient dbClient, string syndicateId, string userId, string adminUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(syndicateId);
            Debug.AssertID(userId);
            Debug.AssertID(adminUserId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (no need to be a member as this is an admin method)
            Syndicate existingSyndicate = await SyndicateServiceLogicLayer.GetSyndicateAdmin(dbClient, syndicateId);
            Debug.AssertValid(existingSyndicate);
            Debug.Assert(existingSyndicate.ID == syndicateId);

            // Find the syndicate member for the specified user
            SyndicateMember syndicateMember = TestDataHelper.SyndicateMembers.Find(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicateId) && (syndicateMember_.UserID == userId)));
            Debug.AssertValid(syndicateMember);

            if (syndicateMember.Status == SyndicateMember.SyndicateMemberStatus.full) {
                Debug.Untested();

                // Remove the syndicate member
                TestDataHelper.SyndicateMembers.Remove(syndicateMember);

                // Add the audit
                AddSyndicateMemberAudit(SyndicateAuditRecord.AuditChangeType.delete, adminUserId, syndicateMember);
            } else {
                Debug.Untested();
                throw new Exception(ERROR_SYNDICATE_MEMBER_NOT_FULL);
            }
        }

        /**
         * Invite a new member to the syndicate specified by ID.
         */
        internal static async Task<Syndicate> InviteMemberAdmin(AmazonDynamoDBClient dbClient,
                                                                string loggedInUserId,
                                                                string syndicateId,
                                                                string emailAddress,
                                                                string message) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);
            Debug.AssertEmail(emailAddress);
            Debug.AssertValidOrNull(message);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (no need to be a member as this is an admin method)
            Syndicate existingSyndicate = await SyndicateServiceLogicLayer.GetSyndicateAdmin(dbClient, syndicateId);
            Debug.AssertValid(existingSyndicate);
            Debug.Assert(existingSyndicate.ID == syndicateId);

            // Actually invite the member
            await DoInviteMember(dbClient, loggedInUserId, existingSyndicate, emailAddress, message);

            return existingSyndicate;
        }

        /**
         * Accept a member invitation to the syndicate specified by ID.
         */
        internal static async Task<Syndicate> AcceptInvitationAdmin(AmazonDynamoDBClient dbClient, string loggedInUserId, string invitedUserId, string syndicateId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(invitedUserId);
            Debug.AssertID(syndicateId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (no need to be a member as this is an admin method)
            Syndicate existingSyndicate = await SyndicateServiceLogicLayer.GetSyndicateAdmin(dbClient, syndicateId);
            Debug.AssertValid(existingSyndicate);
            Debug.Assert(existingSyndicate.ID == syndicateId);

            // Find the syndicate member for the invited user
            SyndicateMember syndicateMember = TestDataHelper.SyndicateMembers.Find(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicateId) && (syndicateMember_.UserID == invitedUserId)));
            Debug.AssertValid(syndicateMember);

            // Accept the invitation
            DoAcceptInvitation(loggedInUserId, syndicateMember);

            return existingSyndicate;
        }

        /**
         * Reject a member invitation to the syndicate specified by ID.
         */
        internal static async Task<Syndicate> RejectInvitationAdmin(AmazonDynamoDBClient dbClient, string loggedInUserId, string invitedUserId, string syndicateId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (no need to be a member as this is an admin method)
            Syndicate existingSyndicate = await SyndicateServiceLogicLayer.GetSyndicateAdmin(dbClient, syndicateId);
            Debug.AssertValid(existingSyndicate);
            Debug.Assert(existingSyndicate.ID == syndicateId);

            // Find the syndicate member for the invited user
            SyndicateMember syndicateMember = TestDataHelper.SyndicateMembers.Find(syndicateMember_ => ((syndicateMember_.SyndicateID == syndicateId) && (syndicateMember_.UserID == invitedUserId)));
            Debug.AssertValid(syndicateMember);

            // Reject the invitation
            DoRejectInvitationEx(loggedInUserId, syndicateMember);

            return existingSyndicate;
        }

        /**
         * Revoke a member invitation to the syndicate specified by ID.
         */
        internal static async Task<Syndicate> RevokeMemberInvitationAdmin(AmazonDynamoDBClient dbClient, string loggedInUserId, string syndicateId, string invitedUserId) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(syndicateId);
            Debug.AssertID(invitedUserId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the syndicate (no need to be a member as this is an admin method)
            Syndicate existingSyndicate = await SyndicateServiceLogicLayer.GetSyndicateAdmin(dbClient, syndicateId);
            Debug.AssertValid(existingSyndicate);
            Debug.Assert(existingSyndicate.ID == syndicateId);

            // Revoke the invitation
            DoRevokeMemberInvitation(loggedInUserId, syndicateId, invitedUserId);
            
            return existingSyndicate;
        }

        /**
         * Get global settings.
         */
        internal static async Task<Dictionary<string, object>> GetGlobalSettings(AmazonDynamoDBClient dbClient) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(GlobalSettings);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            return GlobalSettings;
        }

        /**
         * Check validity of update global settings request inputs.
         */
        internal static void CheckValidUpdateGlobalSettingsRequest(JToken requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                return;
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Update global settings.
         */
        internal static async Task UpdateGlobalSettings(AmazonDynamoDBClient dbClient, JToken requestBody) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(requestBody);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            foreach (JProperty globalSetting in requestBody)
            {
                Debug.AssertValid(globalSetting);
                string name = globalSetting.Name;
                object value = globalSetting.Value.ToObject<object>();
                GlobalSettings[name] = value;
            }
        }

        #endregion Admin syndicate service methods

    }   // SyndicateServiceLogicLayer

}   // BDDReferenceService
