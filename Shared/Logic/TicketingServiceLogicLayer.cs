using System;
using System.Collections.Generic;
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
    internal static class TicketingServiceLogicLayer {

        /**
         * Permissions.
         */
        #region Ticketing service permissions
        internal const string PERMISSION_CAN_BUY_TICKETS = "can-buy-tickets";
        internal const string PERMISSION_CAN_OFFER_TICKET = "can-offer-ticket";
        internal const string PERMISSION_CAN_REVOKE_TICKET = "can-revoke-ticket";
        internal const string PERMISSION_CAN_ACCEPT_TICKET = "can-accept-ticket";
        internal const string PERMISSION_CAN_REJECT_TICKET = "can-reject-ticket";
        internal const string PERMISSION_CAN_RESERVE_TICKET = "can-reserve-ticket";
        internal const string PERMISSION_CAN_GET_TICKET_AUDITS = "can-get-ticket-audits";
        #endregion Ticketing service permissions

        /**
         * Ticket service syntactical errors.
         */
        #region Ticketing service syntactical errors
        internal const string INVALID_TICKET_ID = "INVALID_TICKET_ID";
        internal const string INVALID_PURCHASING_USER_ID = "INVALID_PURCHASING_USER_ID";
        internal const string INVALID_GAME_ID_HASH = "INVALID_GAME_ID_HASH";
        internal const string INVALID_SYNDICATE_ID = "INVALID_SYNDICATE_ID";
        internal const string INVALID_CURRENCY = "INVALID_CURRENCY";
        internal const string INVALID_OFFER_EMAIL = "INVALID_OFFER_EMAIL";
        internal const string INVALID_OFFER_MESSAGE = "INVALID_OFFER_MESSAGE";
        internal const string INVALID_TICKET_NUMBER = "INVALID_TICKET_NUMBER";
        internal const string INVALID_TICKET_NUMBERS = "INVALID_TICKET_NUMBERS";
        internal const string INVALID_TICKET_GIFT_OFFER_ID = "INVALID_TICKET_GIFT_OFFER_ID";
        internal const string INVALID_ACCEPTING_USER_EMAIL = "INVALID_ACCEPTING_USER_EMAIL";
        internal const string INVALID_ACCEPTING_USER_ID = "INVALID_ACCEPTING_USER_ID";
        internal const string INVALID_TICKET_RANGE = "INVALID_TICKET_RANGE";
        internal const string INVALID_LAST_TICKET_NUMBER = "INVALID_LAST_TICKET_NUMBER";
        internal const string INVALID_FIRST_TICKET_NUMBER = "INVALID_FIRST_TICKET_NUMBER";
        internal const string INVALID_USER_ID = "INVALID_USER_ID";
        internal const string INVALID_OFFERING_USER_ID = "INVALID_OFFERING_USER_ID";
        internal const string INVALID_REVOKING_USER_ID = "INVALID_REVOKING_USER_ID";
        internal const string INVALID_REJECTING_USER_ID = "INVALID_REJECTING_USER_ID";
        internal const string INVALID_REJECTING_USER_EMAIL = "INVALID_REJECTING_USER_EMAIL";
        internal const string INVALID_RESERVING_USER_ID = "INVALID_RESERVING_USER_ID";
        internal const string INVALID_FROM = "INVALID_FROM";
        internal const string INVALID_TO = "INVALID_TO";
        #endregion Ticketing service syntactical errors

        /**
         * Ticket service logical errors.
         */
        #region Ticketing service logical errors
        internal const string ERROR_TICKET_NOT_FOUND = "TICKET_NOT_FOUND";
        internal const string ERROR_TICKET_NUMBER_NOT_FOUND = "TICKET_NUMBER_NOT_FOUND";
        internal const string ERROR_TICKET_ALREADY_RESERVED_OR_OWNED = "TICKET_ALREADY_RESERVED_OR_OWNED";
        internal const string ERROR_TICKET_ALREADY_OFFERED = "TICKET_ALREADY_OFFERED";
        internal const string ERROR_TICKET_NOT_OFFERED = "TICKET_NOT_OFFERED";
        internal const string ERROR_TICKET_NOT_OFFERED_TO_EMAIL = "TICKET_NOT_OFFERED_TO_EMAIL";
        internal const string ERROR_TICKET_NOT_OFFERED_TO_USER = "TICKET_NOT_OFFERED_TO_USER";
        internal const string ERROR_TICKET_NOT_OWNED = "TICKET_NOT_OWNED";
        internal const string ERROR_TICKET_GIFT_OFFER_NOT_FOUND = "TICKET_GIFT_OFFER_NOT_FOUND";
        internal const string ERROR_TICKET_NUMBER_TOO_LARGE = "TICKET_NUMBER_TOO_LARGE";
        internal const string ERROR_USER_NOT_TICKET_OWNER_OR_OFFEREE = "USER_NOT_TICKET_OWNER_OR_OFFEREE";
        internal const string ERROR_TICKET_NOT_RESERVED_OR_OWNED = "TICKET_NOT_RESERVED_OR_OWNED";
        #endregion Ticketing service logical errors

        /**
         * Global setting names.
         */
        private const string GLOBAL_RESERVATION_TIME = "ReservationTime";

        /**
         * Default values.
         */
        private const Int16 DEFAULT_RESERVATION_TIME = 600;

        /**
         * The ticketing service global settings.
         */
        internal static Dictionary<string, object> GlobalSettings = new Dictionary<string, object>();

        /**
         * The ticket prices and audit records.
         */
        internal static List<TicketPrice> TicketPrices = new List<TicketPrice>();
        internal static List<TicketPriceAuditRecord> TicketPriceAuditRecords = new List<TicketPriceAuditRecord>();

        #region Helper methods

        /**
         * Reset the data.
         */
        internal static void Reset() {
            Debug.Tested();

            GlobalSettings.Clear();
            TicketPrices.Clear();
            TicketPriceAuditRecords.Clear();
        }

        /**
         * Gets a game from the game Id hash.
         * If the game is not found an exception will be thrown.
         * The game must be locked, published and not frozen or an exception will be thrown.
         */
        private static Game GetGame(string gameIdHash) {
            Debug.Tested();
            Debug.AssertString(gameIdHash);

            Game game = TestDataHelper.Games.Find(game_ => (Helper.Hash(game_.ID.ToString()) == gameIdHash));
            Debug.AssertValidOrNull(game);
            if (game != null) {
                Debug.Tested();
                if (game.Locked) {
                    Debug.Tested();
                    if (game.Published) {
                        Debug.Tested();
                        if (!game.Frozen) {
                            Debug.Tested();
                            return game;
                        } else {
                            Debug.Tested();
                            throw new Exception(GameServiceLogicLayer.ERROR_GAME_FROZEN, new Exception(GameServiceLogicLayer.ERROR_GAME_FROZEN));
                        }
                    } else {
                        Debug.Tested();
                        throw new Exception(GameServiceLogicLayer.ERROR_GAME_NOT_PUBLISHED, new Exception(GameServiceLogicLayer.ERROR_GAME_NOT_PUBLISHED));
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(GameServiceLogicLayer.ERROR_GAME_NOT_LOCKED, new Exception(GameServiceLogicLayer.ERROR_GAME_NOT_LOCKED));
                }
            } else {
                Debug.Tested();
                throw new Exception(GameServiceLogicLayer.ERROR_GAME_NOT_FOUND, new Exception(GameServiceLogicLayer.ERROR_GAME_NOT_FOUND));
            }
        }

        /**
         * Get global setting.
         */
        internal static object GetGlobalSetting(string globalSettingName, object defaultValue = null) {
            Debug.Tested();
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

        /**
         * Get all the tickets of a syndicate.
         */
        internal static List<Ticket> GetSyndicateTickets(string syndicateId) {
            Debug.Tested();
            Debug.AssertID(syndicateId);
            Debug.AssertValid(TestDataHelper.Tickets);

            return TestDataHelper.Tickets.FindAll(ticket_ => ((ticket_.SyndicateID == syndicateId)));
        }

        #endregion Helper methods

        #region Tickets

        /**
         * Get the specified ticket. It must be owned by or offered to the logged in user.
         */
        internal static async Task<Ticket> GetTicket(AmazonDynamoDBClient dbClient, string userId, string ticketId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertID(ticketId);
            Debug.AssertValid(TestDataHelper.Tickets);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => (ticket_.ID == ticketId));
            Debug.AssertValidOrNull(ticket);
            if (ticket != null) {
                Debug.Tested();
                Debug.AssertValidOrNull(ticket.ReserverOrOwnerID);
                if (ticket.ReserverOrOwnerID != null) {
                    // Reserved or owned by someone
                    Debug.Tested();
                    if (ticket.PurchasedDate != null) {
                        // Owned by someone
                        Debug.Tested();
                        if ((ticket.ReserverOrOwnerID == userId) || (ticket.OfferedToID == userId)) {
                            // Owned by or offered to the specified user.
                            Debug.Tested();
                            return ticket;
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_USER_NOT_TICKET_OWNER_OR_OFFEREE, new Exception(ERROR_USER_NOT_TICKET_OWNER_OR_OFFEREE));
                        }
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_TICKET_NOT_OWNED, new Exception(ERROR_TICKET_NOT_OWNED));
                    }
                } else {
                    Debug.Untested();
                    throw new Exception(ERROR_TICKET_NOT_RESERVED_OR_OWNED, new Exception(ERROR_TICKET_NOT_RESERVED_OR_OWNED));
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_TICKET_NOT_FOUND, new Exception(ERROR_TICKET_NOT_FOUND));
            }
        }

        /**
         * Get the specified user's tickets (owned - not reserved).
         */
        internal static async Task<IEnumerable<Ticket>> GetMyTickets(AmazonDynamoDBClient dbClient, string userId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(userId);
            Debug.AssertValid(TestDataHelper.Tickets);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            return TestDataHelper.Tickets.FindAll(ticket_ => ((ticket_.ReserverOrOwnerID == userId) &&
                                                              (ticket_.PurchasedDate != null)));
        }

        /**
         * Is the buy ticket request object valid?
         */
        internal static BuyTicketRequest CheckValidBuyTicketRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "purchasingUserId", out string purchasingUserId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(purchasingUserId);
                    if (APIHelper.GetStringFromRequestBody(requestBody, "gameIdHash", out string gameIdHash) && !string.IsNullOrEmpty(gameIdHash)) {
                        Debug.Tested();
                        if (APIHelper.GetIDFromRequestBody(requestBody, "syndicateId", out string syndicateId)) {
                            Debug.Tested();
                            Debug.AssertID(syndicateId);
                            if (APIHelper.GetOptionalStringFromRequestBody(requestBody, "currency", out string currency) && ((currency == null) || Helper.IsValidCurrencyCode(currency))) {
                                Debug.Tested();
                                if (APIHelper.GetOptionalStringFromRequestBody(requestBody, "offerToEmail", out string offerToEmail) && ((offerToEmail == null) || Helper.IsValidEmail(offerToEmail))) {
                                    Debug.Tested();
                                    if (APIHelper.GetOptionalStringFromRequestBody(requestBody, "offerMessage", out string offerMessage)) {
                                        Debug.Tested();
                                        if (APIHelper.GetOptionalNumberFromRequestBody(requestBody, "ticketNumber", out int? ticketNumber) && ((ticketNumber == null) || (ticketNumber >= 0))) {
                                            Debug.Tested();
                                            if (Helper.AllFieldsRecognized(requestBody,
                                                                            new List<string>(new String[]{
                                                                                "purchasingUserId",
                                                                                "gameIdHash",
                                                                                "syndicateId",
                                                                                "currency",
                                                                                "offerToEmail",
                                                                                "offerMessage",
                                                                                "ticketNumber"
                                                                                }))) {
                                                Debug.Tested();
                                                return new BuyTicketRequest {
                                                    purchasingUserId = purchasingUserId,
                                                    gameIdHash = gameIdHash,
                                                    syndicateId = syndicateId,
                                                    currency = currency,
                                                    offerToEmail = offerToEmail,
                                                    offerMessage = offerMessage,
                                                    ticketNumber = (UInt32?)ticketNumber
                                                };
                                            } else {
                                                // Unrecognised field(s)
                                                Debug.Tested();
                                                error = APIHelper.UNRECOGNISED_FIELD;
                                            }
                                        } else {
                                            Debug.Tested();
                                            error = INVALID_TICKET_NUMBER;
                                        }
                                    } else {
                                        Debug.Untested();
                                        error = INVALID_OFFER_MESSAGE;
                                    }
                                } else {
                                    Debug.Tested();
                                    error = INVALID_OFFER_EMAIL;
                                }
                            } else {
                                Debug.Tested();
                                error = INVALID_CURRENCY;
                            }
                        } else {
                            Debug.Tested();
                            error = INVALID_SYNDICATE_ID;
                        }
                    } else {
                        Debug.Tested();
                        error = INVALID_GAME_ID_HASH;
                    }
                } else {
                    Debug.Tested();
                    error = INVALID_PURCHASING_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Buy a ticket and possibly gift it to someone.
         */
        internal static async Task<Ticket> BuyTicket(AmazonDynamoDBClient dbClient,
                                                     string loggedInUserId,
                                                     string purchasingUserId,
                                                     string gameIdHash,
                                                     UInt32? ticketNumber,
                                                     string syndicateId,
                                                     string currency,
                                                     string offerToEmail,
                                                     string offerMessage) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(purchasingUserId);
            Debug.AssertString(gameIdHash);
            Debug.AssertID(syndicateId);
            Debug.AssertStringOrNull(currency);
            Debug.AssertEmailOrNull(offerToEmail);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game game = GetGame(gameIdHash);
            Debug.AssertValid(game);
            Syndicate syndicate = SyndicateServiceLogicLayer.GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(syndicate);
            return await BuyTicketInternal(dbClient, loggedInUserId, purchasingUserId, game, ticketNumber, syndicate, currency, offerToEmail, offerMessage);
        }

        /**
         * Buy a ticket and possibly gift it to someone.
         */
        internal static async Task<Ticket> BuyTicketInternal(AmazonDynamoDBClient dbClient,
                                                             string loggedInUserId,
                                                             string purchasingUserId,
                                                             Game game,
                                                             UInt32? ticketNumber,
                                                             Syndicate syndicate,
                                                             string currency,
                                                             string offerToEmail,
                                                             string offerMessage) {
            Debug.Tested();
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(purchasingUserId);
            Debug.AssertValid(game);
            Debug.AssertValid(syndicate);
            Debug.AssertStringOrNull(currency);
            Debug.AssertEmailOrNull(offerToEmail);

            Ticket retVal = null;
            if ((ticketNumber == null) || (ticketNumber < game.TicketCount)) {
                Debug.Tested();
                Ticket ticket = GetOrCreateTicketRecordToBuy(purchasingUserId, game, ticketNumber);
                Debug.AssertValid(ticket);
                retVal = await DoBuyTicket(dbClient, loggedInUserId, purchasingUserId, ticket, game, syndicate, currency, offerToEmail, offerMessage);
                Debug.AssertValid(retVal);
            } else {
                Debug.Tested();
                throw new Exception(ERROR_TICKET_NUMBER_TOO_LARGE, new Exception(ERROR_TICKET_NUMBER_TOO_LARGE));
            }
            return retVal;
        }

        /**
         * Is the buy ticket request object valid?
         */
        internal static BuyTicketsRequest CheckValidBuyTicketsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "purchasingUserId", out string purchasingUserId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(purchasingUserId);
                    if (APIHelper.GetStringFromRequestBody(requestBody, "gameIdHash", out string gameIdHash, true)) {
                        Debug.Tested();
                        if (APIHelper.GetIDFromRequestBody(requestBody, "syndicateId", out string syndicateId)) {
                            Debug.Tested();
                            Debug.AssertID(syndicateId);
                            if (APIHelper.GetOptionalStringFromRequestBody(requestBody, "currency", out string currency) && ((currency == null) || Helper.IsValidCurrencyCode(currency))) {
                                Debug.Tested();
                                if (APIHelper.GetOptionalStringFromRequestBody(requestBody, "offerToEmail", out string offerToEmail) && ((offerToEmail == null) || Helper.IsValidEmail(offerToEmail))) {
                                    Debug.Tested();
                                    if (APIHelper.GetOptionalStringFromRequestBody(requestBody, "offerMessage", out string offerMessage)) {
                                        Debug.Tested();
                                        if (APIHelper.RequestBodyContainsField(requestBody, "ticketNumbers", out JToken ticketNumbersField) && (ticketNumbersField.Type == JTokenType.Array)) {
                                            Debug.Tested();
                                            List<UInt32?> ticketNumbers = new List<UInt32?>();
                                            foreach (int? ticketNumber in ticketNumbersField) {
                                                Debug.Tested();
                                                if ((ticketNumber != null) && (ticketNumber < 0)) {
                                                    // Invalid ticket number
                                                    Debug.Untested();
                                                    error = INVALID_TICKET_NUMBER;
                                                    throw APIHelper.CreateInvalidInputParameterException(error);
                                                }
                                                ticketNumbers.Add((UInt32?)ticketNumber);
                                            }
                                            if (ticketNumbers.Count > 0) {
                                                Debug.Tested();
                                                if (Helper.AllFieldsRecognized(requestBody,
                                                                                new List<string>(new String[]{
                                                                                    "purchasingUserId",
                                                                                    "gameIdHash",
                                                                                    "syndicateId",
                                                                                    "currency",
                                                                                    "offerToEmail",
                                                                                    "offerMessage",
                                                                                    "ticketNumbers"
                                                                                    }))) {
                                                    Debug.Tested();
                                                    return new BuyTicketsRequest {
                                                        purchasingUserId = purchasingUserId,
                                                        gameIdHash = gameIdHash,
                                                        syndicateId = syndicateId,
                                                        currency = currency,
                                                        offerToEmail = offerToEmail,
                                                        offerMessage = offerMessage,
                                                        ticketNumbers = ticketNumbers.ToArray()
                                                    };
                                                } else {
                                                    // Unrecognised field(s)
                                                    Debug.Tested();
                                                    error = APIHelper.UNRECOGNISED_FIELD;
                                                }
                                            } else {
                                                Debug.Tested();
                                                error = INVALID_TICKET_NUMBERS;
                                            }
                                        } else {
                                            Debug.Tested();
                                            error = INVALID_TICKET_NUMBERS;
                                        }
                                    } else {
                                        Debug.Tested();
                                        error = INVALID_OFFER_MESSAGE;
                                    }
                                } else {
                                    Debug.Tested();
                                    error = INVALID_OFFER_EMAIL;
                                }
                            } else {
                                Debug.Tested();
                                error = INVALID_CURRENCY;
                            }
                        } else {
                            Debug.Tested();
                            error = INVALID_SYNDICATE_ID;
                        }
                    } else {
                        Debug.Tested();
                        error = INVALID_GAME_ID_HASH;
                    }
                } else {
                    Debug.Tested();
                    error = INVALID_PURCHASING_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Buy some tickets and possibly gift them to someone.
         */
        internal static async Task<Ticket[]> BuyTickets(AmazonDynamoDBClient dbClient,
                                                        string loggedInUserId,
                                                        string purchasingUserId,
                                                        string gameIdHash,
                                                        UInt32?[] ticketNumbers,
                                                        string syndicateId,
                                                        string currency,
                                                        string offerToEmail,
                                                        string offerMessage) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(purchasingUserId);
            Debug.AssertString(gameIdHash);
            Debug.AssertValid(ticketNumbers);
            Debug.AssertID(syndicateId);
            Debug.AssertStringOrNull(currency);
            Debug.AssertEmailOrNull(offerToEmail);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game game = GetGame(gameIdHash);
            Debug.AssertValid(game);
            Syndicate syndicate = SyndicateServiceLogicLayer.GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
            Debug.AssertValid(syndicate);
            Ticket[] retVal = new Ticket[ticketNumbers.Length];
            for (int i = 0; i < ticketNumbers.Length; i++) {
                Debug.Tested();
                UInt32? ticketNumber = ticketNumbers[i];
                Ticket ticket = await BuyTicketInternal(dbClient, loggedInUserId, purchasingUserId, game, ticketNumber, syndicate, currency, offerToEmail, offerMessage);
                Debug.AssertValid(ticket);
                retVal[i] = ticket;
            }
            return retVal;
        }

        /**
         * Get (or create) the ticket record to purchase.
         * Throws an error if the ticket is already reserved by someone else or owned by anyone (including the user the purchase is for).
         */
        internal static Ticket GetOrCreateTicketRecordToBuy(string purchasingUserId,
                                                            Game game,
                                                            UInt32? ticketNumber) {
            Debug.Tested();
            Debug.AssertID(purchasingUserId);
            Debug.AssertValid(game);
            Debug.AssertID(game.ID);
            Debug.Assert((ticketNumber == null) || (ticketNumber < game.TicketCount));

            // Select a ticket number
            bool chosenBySystem = false;
            if (ticketNumber == null) {
                Debug.Tested();
                ticketNumber = SelectUnusedTicketNumber(game);
                chosenBySystem = true;
            }
            // Find the existing ticket record.
            Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => ((ticket_.GameID == game.ID) && (ticket_.Number == ticketNumber)));
            Debug.AssertValidOrNull(ticket);
            if (ticket != null) {
                Debug.Tested();
                if (ticket.ReserverOrOwnerID != null) {
                    if ((ticket.ReserverOrOwnerID == purchasingUserId) && (ticket.PurchasedDate == null)) {
                        // Reserved by the new owner.
                        Debug.Tested();
                    } else {
                        // Reserved or owned by someone else.
                        Debug.Tested();
                        throw new Exception(ERROR_TICKET_ALREADY_RESERVED_OR_OWNED, new Exception(ERROR_TICKET_ALREADY_RESERVED_OR_OWNED));
                    }
                } else {
                    // Not reserved or owned.
                    Debug.Untested();
                }
            } else {
                // No ticket record created yet (therefore not reserved or owned)
                Debug.Tested();
            }
            // Create the ticket if it does not exist.
            if (ticket == null) {
                Debug.Tested();
                ticket = new Ticket {
                    ID = RandomHelper.Next(),
                    GameID = game.ID,
                    Number = (UInt32)ticketNumber,
                    ChosenBySystem = chosenBySystem
                };
                TestDataHelper.Tickets.Add(ticket);
            } else {
                Debug.Tested();
                ticket.ChosenBySystem = chosenBySystem;
            }
            return ticket;
        }

        /**
         * Select an unused ticket number.
         * The ticket cannot even be reserved.
         */
        private static UInt32 SelectUnusedTicketNumber(Game game) {
            Debug.Tested();
            Debug.AssertValid(game);
            Debug.AssertID(game.ID);
            Debug.Assert(game.TicketCount > 0);

            UInt32? retVal = null;
            do {
                UInt32 ticketNumber = (UInt32)RandomHelper.NextNumber((Int64)game.TicketCount);
                Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => ((ticket_.GameID == game.ID) && (ticket_.Number == ticketNumber)));
                Debug.AssertValidOrNull(ticket);
                if (ticket != null) {
                    Debug.Untested();
                    if (ticket.ReserverOrOwnerID != null) {
                        // Reserved (or owned) - choose another number.
                        Debug.Untested();
                    } else {
                        // Not reserved or owned.
                        Debug.Untested();
                        retVal = ticketNumber;
                    }
                } else {
                    // No ticket record created yet (therefore not reserved or owned)
                    Debug.Tested();
                    retVal = ticketNumber;
                }
            } while (retVal == null);

            return (UInt32)retVal;
        }

        /**
         * Buy a ticket and possibly gift it to someone.
         * The ticket will already exist as it is pre-populated.
         */
        private static async Task<Ticket> DoBuyTicket(AmazonDynamoDBClient dbClient,
                                                      string loggedInUserId,
                                          string purchasingUserId,
                                          Ticket ticket,
                                          Game game,
                                          Syndicate syndicate,
                                          string currency,
                                          string offerToEmail,
                                          string offerMessage) {
            Debug.Tested();
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(purchasingUserId);
            Debug.AssertValid(ticket);
            Debug.AssertValid(game);
            Debug.AssertID(game.ID);
            Debug.AssertValid(syndicate);
            Debug.AssertID(syndicate.ID);
            Debug.AssertStringOrNull(currency);
            Debug.AssertEmailOrNull(offerToEmail);

            // Set the ticket's syndicate ID.
            ticket.SyndicateID = syndicate.ID;

            // Set the ticket purchase timestamp (effectively making it purchased).
            ticket.ReserverOrOwnerID = purchasingUserId;
            ticket.PurchasedDate = DateTime.Now;

            // Determine actual currency
            ticket.Currency = DetermineActualCurrency(currency);
            //??++ticket.Amount = ;

            // Add the audit record
            AddTicketAudit((DateTime)ticket.PurchasedDate, TicketAudit.TicketAuditType.purchase, ticket.ID, loggedInUserId, purchasingUserId);

            // Possibly gift the ticket
            if (!string.IsNullOrEmpty(offerToEmail)) {
                Debug.Untested();
                await OfferTicket(dbClient, loggedInUserId, ticket.ReserverOrOwnerID, ticket.ID, offerToEmail, offerMessage);
            } else {
                Debug.Tested();
            }
            // Return the purchased ticket
            return ticket;
        }

        /**
         * Determine the actual currency to use.
         */
        private static string DetermineActualCurrency(string currency) {
            Debug.Tested();
            Debug.AssertStringOrNull(currency);

            if (!Helper.IsValidCurrencyCode(currency)) {
                Debug.Tested();
                currency = "EUR";
            }
            return currency;
        }

        /**
         * Is the offer ticket request object valid?
         */
        internal static OfferTicketRequest CheckValidOfferTicketRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "offeringUserId", out string offeringUserId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(offeringUserId);
                    if (APIHelper.GetIDFromRequestBody(requestBody, "ticketId", out string ticketId)) {
                        Debug.Tested();
                        Debug.AssertID(ticketId);
                        if (APIHelper.GetStringFromRequestBody(requestBody, "offerToEmail", out string offerToEmail) && Helper.IsValidEmail(offerToEmail)) {
                            Debug.Tested();
                            if (APIHelper.GetOptionalStringFromRequestBody(requestBody, "offerMessage", out string offerMessage)) {
                                Debug.Tested();
                                if (Helper.AllFieldsRecognized(requestBody,
                                                                new List<string>(new String[]{
                                                                    "offeringUserId",
                                                                    "ticketId",
                                                                    "offerToEmail",
                                                                    "offerMessage"
                                                                    }))) {
                                    Debug.Tested();
                                    return new OfferTicketRequest {
                                        offeringUserId = offeringUserId,
                                        ticketId = ticketId,
                                        offerToEmail = offerToEmail,
                                        offerMessage = offerMessage
                                    };
                                } else {
                                    // Unrecognised field(s)
                                    Debug.Tested();
                                    error = APIHelper.UNRECOGNISED_FIELD;
                                }
                            } else {
                                Debug.Untested();
                                error = INVALID_OFFER_MESSAGE;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_OFFER_EMAIL;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_TICKET_ID;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_OFFERING_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Offer a ticket to someone.
         */
        internal static async Task OfferTicket(AmazonDynamoDBClient dbClient,
                                               string loggedInUserId,
                                         string offeringUserId,
                                         string ticketId,
                                         string offerToEmail,
                                         string offerMessage) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(offeringUserId);
            Debug.AssertID(ticketId);
            Debug.AssertEmail(offerToEmail);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Debug.Tested();
            Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => (ticket_.ID == ticketId));
            Debug.AssertValidOrNull(ticket);
            if (ticket != null) {
                Debug.Tested();
                Debug.AssertID(ticket.GameID);
                if ((ticket.ReserverOrOwnerID == offeringUserId) && (ticket.PurchasedDate != null)) {
                    Debug.Tested();
                    TicketGiftOffer ticketGiftOffer = TestDataHelper.TicketGiftOffers.Find(ticketGiftOffer_ => (ticketGiftOffer_.TicketID == ticketId));
                    Debug.AssertValidOrNull(ticketGiftOffer);
                    if (ticketGiftOffer == null) {
                        Debug.Tested();

                        // Get the game to ensure it is not frozen.
                        Game game = GetGame(Helper.Hash(ticket.GameID.ToString()));
                        Debug.AssertValid(game);

                        // Get the user being offered the gift.
                        User user = await IdentityServiceLogicLayer.GetOrCreateUserByEmailAddress(dbClient, offerToEmail);
                        Debug.AssertValid(user);
                        Debug.AssertID(user.ID);

                        // Add the ticket gift offer
                        ticketGiftOffer = new TicketGiftOffer {
                            TicketID = ticketId,
                            OfferedToEmail = offerToEmail,
                            OfferMessage = offerMessage
                        };
                        AddTicketGiftOffer(ticketGiftOffer);

                        // Update the ticket.
                        ticket.OfferedDate = DateTime.Now;
                        ticket.OfferedToID = user.ID;

                        // Add the audit record
                        AddTicketAudit(ticketGiftOffer.Created, TicketAudit.TicketAuditType.offer, ticketId, loggedInUserId, user.ID, offerToEmail);

                        // Email the link to the offeree
                        LoggingHelper.LogMessage($"TICKET GIFT OFFER ID: {ticketGiftOffer.ID}");
                        TestDataHelper.TicketGiftOfferId = ticketGiftOffer.ID;
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_TICKET_ALREADY_OFFERED, new Exception(ERROR_TICKET_ALREADY_OFFERED));
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_TICKET_NOT_OWNED, new Exception(ERROR_TICKET_NOT_OWNED));
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_TICKET_NOT_FOUND, new Exception(ERROR_TICKET_NOT_FOUND));
            }
        }

        /**
         * Is the offer ticket request object valid?
         */
        internal static RevokeTicketOfferRequest CheckValidRevokeTicketOfferRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "revokingUserId", out string revokingUserId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(revokingUserId);
                    if (APIHelper.GetIDFromRequestBody(requestBody, "ticketId", out string ticketId)) {
                        Debug.Tested();
                        Debug.AssertID(ticketId);
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "revokingUserId",
                                                            "ticketId"
                                                            }))) {
                            Debug.Tested();
                            return new RevokeTicketOfferRequest {
                                revokingUserId = revokingUserId,
                                ticketId = ticketId
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_TICKET_ID;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_REVOKING_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Revoke a ticket offer to someone.
         */
        internal static async Task RevokeTicketOffer(AmazonDynamoDBClient dbClient, string loggedInUserId, string revokingUserId, string ticketId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(revokingUserId);
            Debug.AssertID(ticketId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Debug.Tested();
            Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => (ticket_.ID == ticketId));
            Debug.AssertValidOrNull(ticket);
            if (ticket != null) {
                Debug.Tested();
                if ((ticket.ReserverOrOwnerID == revokingUserId) && (ticket.PurchasedDate != null)) {
                    // Ticket is owned by revoking user.
                    Debug.Tested();
                    if ((ticket.OfferedDate != null) && (ticket.OfferedToID != null)) {
                        // Ticket has been offered as a gift.
                        Debug.Tested();
                        Debug.AssertID(ticket.OfferedToID);
                        TicketGiftOffer ticketGiftOffer = TestDataHelper.TicketGiftOffers.Find(ticketGiftOffer_ => (ticketGiftOffer_.TicketID == ticketId));
                        Debug.AssertValidOrNull(ticketGiftOffer);
                        if (ticketGiftOffer != null) {
                            Debug.Tested();

                            // Get the game to ensure it is not frozen.
                            Game game = GetGame(Helper.Hash(ticket.GameID.ToString()));
                            Debug.AssertValid(game);

                            // Remove the offer record
                            TestDataHelper.TicketGiftOffers.Remove(ticketGiftOffer);

                            // Update the ticket
                            string offeredToID = ticket.OfferedToID;
                            ticket.OfferedDate = null;
                            ticket.OfferedToID = null;

                            // Add the audit record
                            AddTicketAudit(DateTime.Now, TicketAudit.TicketAuditType.revoke, ticketId, loggedInUserId, offeredToID);
                        } else {
                            Debug.Untested();
                            throw new Exception(ERROR_TICKET_NOT_OFFERED, new Exception(ERROR_TICKET_NOT_OFFERED));
                        }
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_TICKET_NOT_OFFERED, new Exception(ERROR_TICKET_NOT_OFFERED));
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_TICKET_NOT_OWNED, new Exception(ERROR_TICKET_NOT_OWNED));
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_TICKET_NOT_FOUND, new Exception(ERROR_TICKET_NOT_FOUND));
            }
        }

        /**
         * Is the accept ticket request object valid?
         */
        internal static AcceptTicketOfferRequest CheckValidAcceptTicketRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "acceptingUserId", out string acceptingUserId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(acceptingUserId);
                    if (APIHelper.GetStringFromRequestBody(requestBody, "acceptingUserEmail", out string acceptingUserEmail) && Helper.IsValidEmail(acceptingUserEmail)) {
                        Debug.Tested();
                        if (APIHelper.GetIDFromRequestBody(requestBody, "ticketGiftOfferId", out string ticketGiftOfferId)) {
                            Debug.Tested();
                            Debug.AssertID(ticketGiftOfferId);
                            if (APIHelper.GetIDFromRequestBody(requestBody, "syndicateId", out string syndicateId)) {
                                Debug.Tested();
                                Debug.AssertID(syndicateId);
                                if (Helper.AllFieldsRecognized(requestBody,
                                                                new List<string>(new String[]{
                                                                    "acceptingUserId",
                                                                    "acceptingUserEmail",
                                                                    "ticketGiftOfferId",
                                                                    "syndicateId"
                                                                    }))) {
                                    Debug.Tested();
                                    return new AcceptTicketOfferRequest {
                                        acceptingUserId = acceptingUserId,
                                        acceptingUserEmail = acceptingUserEmail,
                                        ticketGiftOfferId = ticketGiftOfferId,
                                        syndicateId = syndicateId
                                    };
                                } else {
                                    // Unrecognised field(s)
                                    Debug.Tested();
                                    error = APIHelper.UNRECOGNISED_FIELD;
                                }
                            } else {
                                Debug.Tested();
                                error = INVALID_SYNDICATE_ID;
                            }
                        } else {
                            Debug.Tested();
                            error = INVALID_TICKET_GIFT_OFFER_ID;
                        }
                    } else {
                        Debug.Tested();
                        error = INVALID_ACCEPTING_USER_EMAIL;
                    }
                } else {
                    Debug.Tested();
                    error = INVALID_ACCEPTING_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Accept a ticket offer from someone.
         */
        internal static async Task AcceptTicketOffer(AmazonDynamoDBClient dbClient,
                                               string loggedInUserId,
                                               string acceptingUserId,
                                               string acceptingUserEmail,
                                               string ticketGiftOfferId,
                                               string syndicateId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(acceptingUserId);
            Debug.AssertEmail(acceptingUserEmail);
            Debug.AssertID(ticketGiftOfferId);
            Debug.AssertID(syndicateId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Debug.Tested();
            TicketGiftOffer ticketGiftOffer = TestDataHelper.TicketGiftOffers.Find(ticketGiftOffer_ => (ticketGiftOffer_.ID == ticketGiftOfferId));
            Debug.AssertValidOrNull(ticketGiftOffer);
            if (ticketGiftOffer != null) {
                // Found ticket gift offer record.
                Debug.Tested();
                Debug.AssertID(ticketGiftOffer.TicketID);
                if (ticketGiftOffer.OfferedToEmail.Equals(acceptingUserEmail, StringComparison.InvariantCultureIgnoreCase)) {
                    // Offered to specified email address
                    Debug.Tested();
                    Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => (ticket_.ID == ticketGiftOffer.TicketID));
                    Debug.AssertValidOrNull(ticket);
                    if (ticket != null) {
                        // Found matching ticket record.
                        Debug.Tested();
                        Debug.AssertID(ticket.ReserverOrOwnerID);
                        Debug.AssertValid(ticket.OfferedDate);
                        Debug.AssertID(ticket.OfferedToID);

                        if (acceptingUserId == ticket.OfferedToID) {
                            // Offered to accepting user
                            Debug.Tested();
                            Debug.Assert(ticket.ReserverOrOwnerID != acceptingUserId);

                            // Get the game to ensure it is not frozen.
                            Game game = GetGame(Helper.Hash(ticket.GameID.ToString()));
                            Debug.AssertValid(game);

                            // Get the syndicate to ensure user is a member
                            Syndicate syndicate = SyndicateServiceLogicLayer.GetSyndicate(loggedInUserId, syndicateId, out SyndicateMember syndicateMember);
                            Debug.AssertValid(syndicate);
                            Debug.AssertID(syndicate.ID);

                            // Change the owner
                            string giftingUserId = ticket.ReserverOrOwnerID;
                            ticket.ReserverOrOwnerID = acceptingUserId;

                            // Change the syndicate
                            ticket.SyndicateID = syndicate.ID;

                            // Remove the offer record
                            TestDataHelper.TicketGiftOffers.Remove(ticketGiftOffer);

                            // Add the audit record
                            AddTicketAudit(DateTime.Now, TicketAudit.TicketAuditType.accept, ticketGiftOffer.TicketID, loggedInUserId, giftingUserId);
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_TICKET_NOT_OFFERED_TO_USER, new Exception(ERROR_TICKET_NOT_OFFERED_TO_USER));
                        }
                    } else {
                        Debug.Untested();
                        throw new Exception(ERROR_TICKET_NOT_FOUND, new Exception(ERROR_TICKET_NOT_FOUND));
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_TICKET_NOT_OFFERED_TO_EMAIL, new Exception(ERROR_TICKET_NOT_OFFERED_TO_EMAIL));
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_TICKET_GIFT_OFFER_NOT_FOUND, new Exception(ERROR_TICKET_GIFT_OFFER_NOT_FOUND));
            }
        }

        /**
         * Is the reject ticket request object valid?
         */
        internal static RejectTicketOfferRequest CheckValidRejectTicketRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "rejectingUserId", out string rejectingUserId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(rejectingUserId);
                    if (APIHelper.GetStringFromRequestBody(requestBody, "rejectingUserEmail", out string rejectingUserEmail) && Helper.IsValidEmail(rejectingUserEmail)) {
                        Debug.Tested();
                        if (APIHelper.GetIDFromRequestBody(requestBody, "ticketGiftOfferId", out string ticketGiftOfferId)) {
                            Debug.Tested();
                            Debug.AssertID(ticketGiftOfferId);
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "rejectingUserId",
                                                                "rejectingUserEmail",
                                                                "ticketGiftOfferId"
                                                                }))) {
                                Debug.Tested();
                                return new RejectTicketOfferRequest {
                                    rejectingUserId = rejectingUserId,
                                    rejectingUserEmail = rejectingUserEmail,
                                    ticketGiftOfferId = ticketGiftOfferId
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_TICKET_GIFT_OFFER_ID;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_REJECTING_USER_EMAIL;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_REJECTING_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Reject a ticket offer from someone.
         */
        internal static async Task RejectTicketOffer(AmazonDynamoDBClient dbClient,
                                               string loggedInUserId,
                                               string rejectingUserId,
                                               string rejectingUserEmail,
                                               string ticketGiftOfferId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(rejectingUserId);
            Debug.AssertEmail(rejectingUserEmail);
            Debug.AssertID(ticketGiftOfferId);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Debug.Tested();
            TicketGiftOffer ticketGiftOffer = TestDataHelper.TicketGiftOffers.Find(ticketGiftOffer_ => (ticketGiftOffer_.ID == ticketGiftOfferId));
            Debug.AssertValidOrNull(ticketGiftOffer);
            if (ticketGiftOffer != null) {
                Debug.Tested();
                Debug.AssertID(ticketGiftOffer.TicketID);
                if (ticketGiftOffer.OfferedToEmail.Equals(rejectingUserEmail, StringComparison.InvariantCultureIgnoreCase)) {
                    Debug.Tested();
                    Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => (ticket_.ID == ticketGiftOffer.TicketID));
                    Debug.AssertValidOrNull(ticket);
                    if (ticket != null) {
                        Debug.Tested();
                        Debug.AssertID(ticket.ReserverOrOwnerID);
                        Debug.AssertValid(ticket.OfferedDate);
                        Debug.AssertID(ticket.OfferedToID);
                        if (ticket.OfferedToID == rejectingUserId) {
                            Debug.Tested();
                            Debug.Assert(ticket.ReserverOrOwnerID != rejectingUserId);

                            // Get the game to ensure it is not frozen.
                            Game game = GetGame(Helper.Hash(ticket.GameID.ToString()));
                            Debug.AssertValid(game);

                            // Change the owner
                            ticket.OfferedDate = null;
                            ticket.OfferedToID = null;

                            // Remove the offer record
                            TestDataHelper.TicketGiftOffers.Remove(ticketGiftOffer);

                            // Add the audit record
                            AddTicketAudit(DateTime.Now, TicketAudit.TicketAuditType.reject, ticketGiftOffer.TicketID, loggedInUserId, ticket.ReserverOrOwnerID);
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_TICKET_NOT_OFFERED_TO_USER, new Exception(ERROR_TICKET_NOT_OFFERED_TO_USER));
                        }
                    } else {
                        Debug.Untested();
                        throw new Exception(ERROR_TICKET_NOT_FOUND, new Exception(ERROR_TICKET_NOT_FOUND));
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_TICKET_NOT_OFFERED_TO_EMAIL, new Exception(ERROR_TICKET_NOT_OFFERED_TO_EMAIL));
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_TICKET_GIFT_OFFER_NOT_FOUND, new Exception(ERROR_TICKET_GIFT_OFFER_NOT_FOUND));
            }
        }

        /**
         * Is the reserve ticket request object valid?
         */
        internal static ReserveTicketRequest CheckValidReserveTicketRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "reservingUserId", out string reservingUserId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(reservingUserId);
                    if (APIHelper.GetStringFromRequestBody(requestBody, "gameIdHash", out string gameIdHash) && !string.IsNullOrEmpty(gameIdHash)) {
                        Debug.Tested();
                        if (APIHelper.GetOptionalNumberFromRequestBody(requestBody, "ticketNumber", out int? ticketNumber) && ((ticketNumber == null) || (ticketNumber >= 0))) {
                            Debug.Tested();
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "reservingUserId",
                                                                "gameIdHash",
                                                                "ticketNumber"
                                                                }))) {
                                Debug.Tested();
                                return new ReserveTicketRequest {
                                    reservingUserId = reservingUserId,
                                    gameIdHash = gameIdHash,
                                    ticketNumber = (UInt32?)ticketNumber
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_TICKET_NUMBER;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_GAME_ID_HASH;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_RESERVING_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Reserve a ticket.
         * The ticket will already exist as it is pre-populated.
         */
        internal static async Task<Ticket> ReserveTicket(AmazonDynamoDBClient dbClient,
                                             string loggedInUserId,
                                             string reservingUserId,
                                             string gameIdHash,
                                             UInt32? ticketNumber) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(reservingUserId);
            Debug.AssertString(gameIdHash);

            //??++MaxReservationCount
            Ticket retVal = null;

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Debug.Tested();
            Game game = GetGame(gameIdHash);
            Debug.AssertValid(game);
            if ((ticketNumber == null) || (ticketNumber < game.TicketCount)) {
                Debug.Tested();

                // Get the ticket record
                Ticket ticket = GetOrCreateTicketRecordToReserve(game, ticketNumber);
                Debug.AssertValid(ticket);
                Debug.AssertID(ticket.ID);
                Debug.AssertNull(ticket.ReserverOrOwnerID);
                Debug.AssertNull(ticket.ReservedDate);
                Debug.AssertNull(ticket.PurchasedDate);

                // Set the ticket reserved user
                ticket.ReserverOrOwnerID = reservingUserId;
                ticket.ReservedDate = DateTime.Now;
                Int16 reservationTime = (Int16)TicketingServiceLogicLayer.GetGlobalSetting(GLOBAL_RESERVATION_TIME, DEFAULT_RESERVATION_TIME);
                Debug.Assert(reservationTime > 0);
                ticket.ReservedUntil = ((DateTime)ticket.ReservedDate).AddSeconds(reservationTime);

                // Set ticket ID for testing
                TestDataHelper.ReservedTicketId = ticket.ID;

                // Add the audit record
                AddTicketAudit(DateTime.Now, TicketAudit.TicketAuditType.reserve, ticket.ID, loggedInUserId, reservingUserId);

                // Return the ticket
                retVal = ticket;
            } else {
                Debug.Tested();
                throw new Exception(ERROR_TICKET_NUMBER_TOO_LARGE, new Exception(ERROR_TICKET_NUMBER_TOO_LARGE));
            }
            return retVal;
        }

        /**
         * Get (or create) the ticket record to purchase.
         * Throws an error if the ticket is already reserved by someone else or owned by anyone (including the user the purchase is for).
         */
        internal static Ticket GetOrCreateTicketRecordToReserve(Game game,
                                                                UInt32? ticketNumber) {
            Debug.Tested();
            Debug.AssertValid(game);
            Debug.AssertID(game.ID);
            Debug.Assert((ticketNumber == null) || (ticketNumber < game.TicketCount));

            // Select a ticket number
            bool chosenBySystem = false;
            if (ticketNumber == null) {
                Debug.Tested();
                ticketNumber = SelectUnusedTicketNumber(game);
                chosenBySystem = true;
            }
            // Find the existing ticket record.
            Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => ((ticket_.GameID == game.ID) && (ticket_.Number == ticketNumber)));
            Debug.AssertValidOrNull(ticket);
            if (ticket != null) {
                Debug.Tested();
                if (ticket.ReserverOrOwnerID != null) {
                    // Already reserved or owned.
                    Debug.Tested();
                    throw new Exception(ERROR_TICKET_ALREADY_RESERVED_OR_OWNED, new Exception(ERROR_TICKET_ALREADY_RESERVED_OR_OWNED));
                } else {
                    // Not reserved or owned.
                    Debug.Untested();
                }
            } else {
                // No ticket record created yet (therefore not reserved or owned)
                Debug.Tested();
            }
            // Create the ticket if it does not exist.
            if (ticket == null) {
                Debug.Tested();
                ticket = new Ticket {
                    ID = RandomHelper.Next(),
                    GameID = game.ID,
                    Number = (UInt32)ticketNumber,
                    ChosenBySystem = chosenBySystem
                };
                TestDataHelper.Tickets.Add(ticket);
            } else {
                Debug.Untested();
                ticket.ChosenBySystem = false;
            }
            return ticket;
        }

        /**
         * Is the get user ticket audits request object valid?
         */
        internal static GetUserTicketAuditsRequest CheckValidGetUserTicketAuditsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "userId", out string userId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(userId);
                    if (APIHelper.GetDateTimeFromRequestBody(requestBody, "from", out string from)) {
                        Debug.Tested();
                        Debug.Assert(APIHelper.IsValidAPIDateTimeString(from));
                        if (APIHelper.GetDateTimeFromRequestBody(requestBody, "to", out string to)) {
                            Debug.Tested();
                            Debug.Assert(APIHelper.IsValidAPIDateTimeString(to));
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "userId",
                                                                "from",
                                                                "to"
                                                                }))) {
                                Debug.Tested();
                                return new GetUserTicketAuditsRequest {
                                    userId = userId,
                                    from = from,
                                    to = to
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Untested();
                            error = INVALID_TO;
                        }
                    } else {
                        Debug.Untested();
                        error = INVALID_FROM;
                    }
                } else {
                    Debug.Untested();
                    error = INVALID_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Add a ticket gift offer record.
         */
        private static void AddTicketGiftOffer(TicketGiftOffer ticketGiftOffer) {
            Debug.Tested();
            Debug.AssertValid(ticketGiftOffer);

            ticketGiftOffer.ID = RandomHelper.Next();
            ticketGiftOffer.Created = DateTime.Now;
            TestDataHelper.TicketGiftOffers.Add(ticketGiftOffer);
        }

        /**
         * Add a ticket audit record.
         */
        private static void AddTicketAudit(DateTime timestamp,
                                           TicketAudit.TicketAuditType type,
                                           string ticketId,
                                           string userId,
                                           string targetUserId,
                                           string offerToEmail = null) {
            Debug.Tested();
            Debug.AssertValid(timestamp);
            Debug.AssertIDOrNull(ticketId);
            Debug.AssertID(userId);
            Debug.AssertID(targetUserId);
            Debug.AssertEmailOrNull(offerToEmail);
            Debug.AssertValid(TestDataHelper.TicketAudits);

            TicketAudit ticketAudit = new TicketAudit {
                ID = RandomHelper.Next(),
                Timestamp = timestamp,
                Type = type,
                TicketID = ticketId,
                UserID = userId,
                TargetUserID = targetUserId,
                OfferedToEmail = offerToEmail
            };
            TestDataHelper.TicketAudits.Add(ticketAudit);
        }

        #endregion Tickets

        #region Ticket audits

        /**
         * Get a list of all the ticket audits in a time range.
         */
        internal static async Task<List<TicketAudit>> GetTicketAudits(AmazonDynamoDBClient dbClient, string userId, string fromStr, string toStr) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(fromStr));
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(toStr));
            Debug.AssertIDOrNull(userId);
            Debug.AssertValid(TestDataHelper.TicketAudits);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            DateTime from = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(fromStr);
            Debug.AssertValid(from);
            DateTime to = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(toStr);
            Debug.AssertValid(to);
            if (from <= to) {
                Debug.Tested();
                List<TicketAudit> retVal = new List<TicketAudit>();
                foreach (TicketAudit ticketAudit in TestDataHelper.TicketAudits) {
                    Debug.Tested();
                    Debug.AssertValid(ticketAudit);
                    if (from <= ticketAudit.Timestamp) {
                        Debug.Tested();
                        if (ticketAudit.Timestamp < to) {
                            Debug.Tested();
                            if ((userId == null) || (ticketAudit.UserID == userId) || (ticketAudit.TargetUserID == userId)) {
                                Debug.Tested();
                                retVal.Add(ticketAudit);
                            } else {
                                Debug.Tested();
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
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_FROM_GREATER_THAN_TO, new Exception(SharedLogicLayer.ERROR_FROM_GREATER_THAN_TO));
            }
        }

        #endregion Ticket audits

        #region Admin ticketing service methods
        
        /**
         * Is the reserve bulk ticket request object valid?
         */
        internal static BulkReserveTicketsRequest CheckValidBulkReserveTicketsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "userId", out string userId)) {
                    Debug.Tested();
                    Debug.AssertID(userId);
                    if (APIHelper.GetStringFromRequestBody(requestBody, "gameIdHash", out string gameIdHash) && !string.IsNullOrEmpty(gameIdHash)) {
                        Debug.Tested();
                        if (APIHelper.GetNumberFromRequestBody(requestBody, "minTicketNumber", out int? minTicketNumber) && (minTicketNumber >= 0)) {
                            Debug.Tested();
                            if (APIHelper.GetNumberFromRequestBody(requestBody, "maxTicketNumber", out int? maxTicketNumber) && (maxTicketNumber >= 0)) {
                                Debug.Tested();
                                if (minTicketNumber <= maxTicketNumber) {
                                    Debug.Tested();
                                    if (APIHelper.GetNumberFromRequestBody(requestBody, "tickets", out int? tickets) && (tickets > 0)) {
                                        Debug.Tested();
                                        if ((maxTicketNumber - minTicketNumber + 1) >= tickets) {
                                            Debug.Tested();
                                            if (APIHelper.GetNumberFromRequestBody(requestBody, "boundary", out int? boundary) && (boundary > 0)) {
                                                Debug.Tested();
                                                if (Helper.AllFieldsRecognized(requestBody,
                                                                                new List<string>(new String[]{
                                                                                    "userId",
                                                                                    "gameIdHash",
                                                                                    "minTicketNumber",
                                                                                    "maxTicketNumber",
                                                                                    "tickets",
                                                                                    "boundary"
                                                                                    }))) {
                                                    Debug.Tested();
                                                    return new BulkReserveTicketsRequest {
                                                        userId = userId,
                                                        gameIdHash = gameIdHash,
                                                        minTicketNumber = (UInt32)minTicketNumber,
                                                        maxTicketNumber = (UInt32)maxTicketNumber,
                                                        tickets = (UInt32)tickets,
                                                        boundary = (UInt32)boundary
                                                    };
                                                } else {
                                                    // Unrecognised field(s)
                                                    Debug.Tested();
                                                    error = APIHelper.UNRECOGNISED_FIELD;
                                                }
                                            } else {
                                                Debug.Tested();
                                            }
                                        } else {
                                            Debug.Tested();
                                        }
                                    } else {
                                        Debug.Tested();
                                    }
                                } else {
                                    Debug.Tested();
                                }
                            } else {
                                Debug.Tested();
                            }
                        } else {
                            Debug.Tested();
                        }
                    } else {
                        Debug.Tested();
                    }
                } else {
                    Debug.Tested();
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Bulk reserve tickets.
         */
        internal static async Task<BulkReserveTicketsResponse> BulkReserveTickets(AmazonDynamoDBClient dbClient,
                                                                                  string loggedInUserId,
                                                                                  string userId,
                                                                                  string gameIdHash,
                                                                                  UInt32 minTicketNumber,
                                                                                  UInt32 maxTicketNumber,
                                                                                  UInt32 tickets,
                                                                                  UInt32 boundary) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(userId);
            Debug.AssertString(gameIdHash);
            Debug.Assert(minTicketNumber <= maxTicketNumber);
            Debug.Assert((maxTicketNumber - minTicketNumber + 1) >= tickets);
            Debug.Assert(tickets > 0);
            Debug.Assert(boundary > 0);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Tested();
                //???Check the user (e.g. validated, not closed etc.)?
                Game game = GetGame(gameIdHash);
                Debug.AssertValid(game);
                if (maxTicketNumber < game.TicketCount) {
                    Debug.Tested();

                    // Work out the start of the ticket number range
                    UInt32 firstTicketNumber = GetFirstTicketNumberInRange(minTicketNumber, maxTicketNumber, tickets, boundary);
                    Debug.Assert((firstTicketNumber >= minTicketNumber) && (firstTicketNumber <= (maxTicketNumber - tickets + 1)));

                    // Reserve the tickets
                    DateTime reservedDate = DateTime.Now;
                    Int16 reservationTime = (Int16)TicketingServiceLogicLayer.GetGlobalSetting(GLOBAL_RESERVATION_TIME, DEFAULT_RESERVATION_TIME);
                    Debug.Assert(reservationTime > 0);
                    DateTime reservedUntil = reservedDate.AddSeconds(reservationTime);
                    ReserveTickets(user, game, firstTicketNumber, tickets, reservedDate, reservedUntil);

                    // Add the audit record
                    AddTicketAudit(reservedDate, TicketAudit.TicketAuditType.bulk_reserve, null, loggedInUserId, user.ID);

                    // Return the response body
                    return new BulkReserveTicketsResponse {
                        firstTicketNumber = firstTicketNumber,
                        lastTicketNumber = (firstTicketNumber + tickets - 1),
                        tickets = tickets,
                        reservedUntil = APIHelper.APIDateTimeStringFromDateTime(reservedUntil)
                    };
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_TICKET_NUMBER_TOO_LARGE);
                }
            } else {
                Debug.Tested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
            }
        }

        /**
         * Finds a range of numbers that are not owned or reserved and that starts on the specified boundary.
         */
        private static UInt32 GetFirstTicketNumberInRange(UInt32 minTicketNumber, UInt32 maxTicketNumber, UInt32 tickets, UInt32 boundary) {
            Debug.Tested();
            Debug.Assert(minTicketNumber <= maxTicketNumber);
            Debug.Assert((maxTicketNumber - minTicketNumber + 1) >= tickets);
            Debug.Assert(tickets > 0);
            Debug.Assert(boundary > 0);
            Debug.AssertValid(TestDataHelper.Tickets);

            UInt32 lastPossibleFirstTicketNumber = (maxTicketNumber - tickets + 1);
            UInt32? recentBoundary = null;
            for (UInt32 number = minTicketNumber; number <= lastPossibleFirstTicketNumber; number++) {
                Debug.Tested();
                Ticket ticket = TestDataHelper.Tickets.Find(ticket_ => (ticket_.Number == number));
                Debug.AssertValidOrNull(ticket);
                if ((ticket == null) || (ticket.ReserverOrOwnerID == null)) {
                    // The ticket number is not reserved or owned.
                    Debug.Tested();
                    if ((recentBoundary == null) && ((number % boundary) == 0)) {
                        Debug.Tested();
                        recentBoundary = number;
                    } else {
                        Debug.Tested();
                    }
                    if ((recentBoundary != null) && ((number - recentBoundary + 1) == tickets)) {
                        // Range found.
                        Debug.Tested();
                        return (UInt32)recentBoundary;
                    } else {
                        Debug.Tested();
                    }
                } else {
                    // The ticket number is reserved or owned.
                    Debug.Tested();
                    if (recentBoundary != null) {
                        recentBoundary += boundary;
                    }
                }
            }
            throw new Exception(ERROR_TICKET_NUMBER_NOT_FOUND);
        }

        /**
         * Actually reserve the tickets.
         */
        private static void ReserveTickets(User user,
                                           Game game, 
                                           UInt32 firstTicketNumber, 
                                           UInt32 tickets, 
                                           DateTime reservedDate,
                                           DateTime reservedUntil) {
            Debug.Tested();
            Debug.AssertValid(user);
            Debug.AssertID(user.ID);
            Debug.AssertValid(game);
            Debug.AssertID(game.ID);
            Debug.Assert(tickets > 0);

            for (UInt32 ticketNumber = firstTicketNumber; ticketNumber < (firstTicketNumber + tickets); ticketNumber++) {
                Debug.Tested();

                // Get the ticket record
                Ticket ticket = GetOrCreateTicketRecordToReserve(game, ticketNumber);
                Debug.AssertValid(ticket);
                Debug.AssertID(ticket.ID);
                Debug.AssertNull(ticket.ReserverOrOwnerID);
                Debug.AssertNull(ticket.ReservedDate);
                Debug.AssertNull(ticket.PurchasedDate);

                // Set the ticket reserved user
                ticket.ReserverOrOwnerID = user.ID;
                ticket.ReservedDate = reservedDate;
                ticket.ReservedUntil = reservedUntil;
            }
        }

        /**
         * Is the assign bulk ticket request object valid?
         */
        internal static BulkAssignTicketsRequest CheckValidBulkAssignTicketsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "userId", out string userId)) {
                    Debug.Tested();
                    Debug.AssertID(userId);
                    if (APIHelper.GetStringFromRequestBody(requestBody, "gameIdHash", out string gameIdHash) && !string.IsNullOrEmpty(gameIdHash)) {
                        Debug.Tested();
                        if (APIHelper.GetNumberFromRequestBody(requestBody, "firstTicketNumber", out int? firstTicketNumber) && (firstTicketNumber >= 0)) {
                            Debug.Tested();
                            if (APIHelper.GetNumberFromRequestBody(requestBody, "lastTicketNumber", out int? lastTicketNumber) && (lastTicketNumber >= 0)) {
                                Debug.Tested();
                                if (firstTicketNumber <= lastTicketNumber) {
                                    Debug.Tested();
                                    if (APIHelper.GetIDFromRequestBody(requestBody, "syndicateId", out string syndicateId)) {
                                        Debug.Tested();
                                        Debug.AssertID(syndicateId);
                                        if (APIHelper.GetStringFromRequestBody(requestBody, "currency", out string currency) && Helper.IsValidCurrencyCode(currency)) {
                                            Debug.Tested();
                                            if (Helper.AllFieldsRecognized(requestBody,
                                                                            new List<string>(new String[]{
                                                                                "userId",
                                                                                "gameIdHash",
                                                                                "firstTicketNumber",
                                                                                "lastTicketNumber",
                                                                                "syndicateId",
                                                                                "currency"
                                                                                }))) {
                                                Debug.Tested();
                                                return new BulkAssignTicketsRequest {
                                                    userId = userId,
                                                    gameIdHash = gameIdHash,
                                                    firstTicketNumber = (UInt32)firstTicketNumber,
                                                    lastTicketNumber = (UInt32)lastTicketNumber,
                                                    syndicateId = syndicateId,
                                                    currency = currency
                                                };
                                            } else {
                                                // Unrecognised field(s)
                                                Debug.Tested();
                                                error = APIHelper.UNRECOGNISED_FIELD;
                                            }
                                        } else {
                                            Debug.Tested();
                                            error = INVALID_CURRENCY;
                                        }
                                    } else {
                                        Debug.Tested();
                                        error = INVALID_SYNDICATE_ID;
                                    }
                                } else {
                                    Debug.Tested();
                                    error = INVALID_TICKET_RANGE;
                                }
                            } else {
                                Debug.Tested();
                                error = INVALID_LAST_TICKET_NUMBER;
                            }
                        } else {
                            Debug.Tested();
                            error = INVALID_FIRST_TICKET_NUMBER;
                        }
                    } else {
                        Debug.Tested();
                        error = INVALID_GAME_ID_HASH;
                    }
                } else {
                    Debug.Tested();
                    error = INVALID_USER_ID;
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Bulk assign tickets.
         */
        internal static async Task<BulkAssignTicketsResponse> BulkAssignTickets(AmazonDynamoDBClient dbClient,
                                                                                string loggedInUserId,
                                                                                string userId,
                                                                                string gameIdHash,
                                                                                UInt32 firstTicketNumber,
                                                                                UInt32 lastTicketNumber,
                                                                                string syndicateId,
                                                                                string currency) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertID(userId);
            Debug.AssertString(gameIdHash);
            Debug.Assert(firstTicketNumber <= lastTicketNumber);
            Debug.Assert(Helper.IsValidCurrencyCode(currency));

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId);
            Debug.AssertValidOrNull(user);
            if (user != null) {
                Debug.Tested();
                //???Check the user (e.g. validated, not closed etc.)?

                // Check that the game exists
                Game game = GetGame(gameIdHash);
                Debug.AssertValid(game);

                // Check that the syndicate exists and the user is a full member
                Syndicate syndicate = SyndicateServiceLogicLayer.GetSyndicate(userId, syndicateId, out SyndicateMember syndicateMember);
                Debug.AssertValid(syndicate);

                // Check that the last ticket number is not too large
                if (lastTicketNumber < game.TicketCount) {
                    Debug.Tested();

                    // Assign the tickets
                    UInt32 tickets = (lastTicketNumber - firstTicketNumber + 1);
                    DateTime purchasedDate = DateTime.Now;
                    // Int16 reservationTime = (Int16)TicketingServiceLogicLayer.GetGlobalSetting(GLOBAL_RESERVATION_TIME, DEFAULT_RESERVATION_TIME);
                    // Debug.Assert(reservationTime > 0);
                    // DateTime reservedUntil = reservedDate.AddSeconds(reservationTime);
                    currency = DetermineActualCurrency(currency);
                    AssignTickets(user, game, firstTicketNumber, lastTicketNumber, syndicateId, purchasedDate, currency);

                    // Add the audit record
                    AddTicketAudit(purchasedDate, TicketAudit.TicketAuditType.bulk_assign, null, loggedInUserId, user.ID);

                    // Return the response body
                    return new BulkAssignTicketsResponse {
                        amount = 0,//??++
                        currency = currency
                    };
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_TICKET_NUMBER_TOO_LARGE);
                }
            } else {
                Debug.Tested();
                throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND);
            }
        }

        /**
         * Actually assign the tickets.
         */
        private static void AssignTickets(User user,
                                          Game game, 
                                          UInt32 firstTicketNumber, 
                                          UInt32 lastTicketNumber, 
                                          string syndicateId,
                                          DateTime purchasedDate,
                                          string currency) {
            Debug.Tested();
            Debug.AssertValid(user);
            Debug.AssertID(user.ID);
            Debug.AssertValid(game);
            Debug.AssertID(game.ID);
            Debug.Assert(firstTicketNumber <= lastTicketNumber);
            Debug.Assert(Helper.IsValidCurrencyCode(currency));

            for (UInt32 ticketNumber = firstTicketNumber; ticketNumber <= lastTicketNumber; ticketNumber++) {
                Debug.Tested();

                // Get the ticket record
                Ticket ticket = GetOrCreateTicketRecordToBuy(user.ID, game, ticketNumber);
                Debug.AssertValid(ticket);
                Debug.AssertID(ticket.ID);
                Debug.Assert((ticket.ReserverOrOwnerID == null) || (ticket.ReserverOrOwnerID == user.ID));
                Debug.AssertNull(ticket.PurchasedDate);

                // Set the ticket's syndicate ID.
                ticket.SyndicateID = syndicateId;

                // Set the ticket purchase timestamp (effectively making it purchased).
                ticket.ReserverOrOwnerID = user.ID;
                ticket.PurchasedDate = purchasedDate;

                // Determine actual currency
                ticket.Currency = currency;
                //??++ticket.Amount = ;
            }
        }

        /**
         * Is the get logs request object valid?
         */
        internal static void CheckValidGetLogsRequest(GetLogsRequest getLogsRequest) {
            Debug.Tested();
            Debug.AssertValidOrNull(getLogsRequest);

            string error = null;
            if (getLogsRequest != null) {
                Debug.Tested();
                if (APIHelper.IsValidAPIDateTimeString(getLogsRequest.from)) {
                    Debug.Tested();
                    if (APIHelper.IsValidAPIDateTimeString(getLogsRequest.to)) {
                        Debug.Tested();
                        return;
                    } else {
                        Debug.Tested();
                    }
                } else {
                    Debug.Tested();
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Is the get all ticket audits request object valid?
         */
        internal static GetAllTicketAuditsRequest CheckValidGetAllTicketAuditsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetOptionalIDFromRequestBody(requestBody, "userId", out string userId)) {
                    Debug.Tested();
                    Debug.AssertIDOrNull(userId);
                    if (APIHelper.GetDateTimeFromRequestBody(requestBody, "from", out string from)) {
                        Debug.Tested();
                        Debug.Assert(APIHelper.IsValidAPIDateTimeString(from));
                        if (APIHelper.GetDateTimeFromRequestBody(requestBody, "to", out string to)) {
                            Debug.Tested();
                            Debug.Assert(APIHelper.IsValidAPIDateTimeString(to));
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "userId",
                                                                "from",
                                                                "to"
                                                                }))) {
                                Debug.Tested();
                                return new GetAllTicketAuditsRequest {
                                    //??++userId = userId,
                                    from = from,
                                    to = to
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Tested();
                        }
                    } else {
                        Debug.Tested();
                    }
                } else {
                    Debug.Tested();
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Get ticket prices.
         */
        internal static async Task<List<TicketPrice>> GetTicketPrices(AmazonDynamoDBClient dbClient) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(TicketPrices);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            return TicketPrices;
        }

        /**
         * Check validity of set ticket price request inputs.
         */
        internal static TicketPrice CheckValidSetTicketPriceRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetStringFromRequestBody(requestBody, "currency", out string currency) && Helper.IsValidCurrencyCode(currency)) {
                    Debug.Tested();
                    if (APIHelper.GetNumberFromRequestBody(requestBody, "price", out int? price) && (price > 0)) {
                        Debug.Tested();
                        if (APIHelper.GetOptionalBooleanFromRequestBody(requestBody, "available", out bool? available)) {
                            Debug.Tested();
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "currency",
                                                                "price",
                                                                "available"
                                                                }))) {
                                // Valid request
                                Debug.Tested();
                                return new TicketPrice {
                                    Currency = currency,
                                    Price = (UInt64)price,
                                    Available = ((available == null) ? false : (bool)available)
                                };
                            } else {
                                // Unrecognised field(s)
                                Debug.Tested();
                                error = APIHelper.UNRECOGNISED_FIELD;
                            }
                        } else {
                            Debug.Tested();
                            error = "INVALID_FIELD: available";
                        }
                    } else {
                        // Invalid price
                        Debug.Tested();
                        error = "MISSING/INVALID FIELD: price";
                    }
                } else {
                    // Invalid currency code
                    Debug.Tested();
                    error = "MISSING/INVALID FIELD: currency";
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set ticket price.
         */
        internal static async Task SetTicketPrice(AmazonDynamoDBClient dbClient, string loggedInUserId, TicketPrice ticketPrice) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TicketPrices);
            Debug.Assert(Helper.IsValidCurrencyCode(ticketPrice.Currency));
            Debug.Assert(ticketPrice.Price > 0);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            foreach (TicketPrice existingTicketPrice in TicketPrices) {
                Debug.Tested();
                Debug.AssertValid(existingTicketPrice);
                Debug.Assert(Helper.IsValidCurrencyCode(existingTicketPrice.Currency));
                if (existingTicketPrice.Currency == ticketPrice.Currency) {
                    Debug.Tested();
                    existingTicketPrice.Price = ticketPrice.Price;
                    existingTicketPrice.Available = ticketPrice.Available;
                    AddTicketPriceAudit(loggedInUserId, TicketPriceAuditRecord.AuditChangeType.update, ticketPrice);
                    return;
                } else {
                    Debug.Tested();
                }
            }
            TicketPrices.Add(ticketPrice);
            AddTicketPriceAudit(loggedInUserId, TicketPriceAuditRecord.AuditChangeType.create, ticketPrice);
        }

        /**
         * Add a ticket price audit record.
         */
        private static void AddTicketPriceAudit(string loggedInUserId,
                                                TicketPriceAuditRecord.AuditChangeType changeType,
                                                TicketPrice ticketPrice) {
            Debug.Tested();
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(ticketPrice);
            Debug.AssertValid(TicketPriceAuditRecords);

            TicketPriceAuditRecord ticketPriceAudit = new TicketPriceAuditRecord {
                ID = RandomHelper.Next(),
                Timestamp = DateTime.Now,
                AdministratorID = loggedInUserId,
                ChangeType = changeType,
                Currency = ticketPrice.Currency,
                Price = ticketPrice.Price,
                Available = ticketPrice.Available
            };
            TicketPriceAuditRecords.Add(ticketPriceAudit);
        }

        /**
         * Check validity of get ticket price audits request inputs.
         */
        internal static GetTicketPriceAuditsRequest CheckValidGetTicketPriceAuditsRequest(JObject requestBody) {
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
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "adminID",
                                                            "from",
                                                            "to"
                                                            }))) {
                            // Valid request
                            Debug.Tested();
                            return new GetTicketPriceAuditsRequest {
                                //??++adminId = adminID,
                                from = from,
                                to = to
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        Debug.Tested();
                        error = "MISSING/INVALID FIELD: to";
                    }
                } else {
                    Debug.Tested();
                    error = "MISSING/INVALID FIELD: from";
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Get a list of all the audit records in a time range.
         */
        internal static async Task<List<TicketPriceAuditRecord>> GetTicketPriceAuditRecords(AmazonDynamoDBClient dbClient,
                                                                                GetTicketPriceAuditsRequest getTicketPriceAuditsRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(getTicketPriceAuditsRequest);
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(getTicketPriceAuditsRequest.from));
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(getTicketPriceAuditsRequest.to));
            Debug.AssertValid(TicketPriceAuditRecords);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            DateTime from = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(getTicketPriceAuditsRequest.from);
            DateTime to = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(getTicketPriceAuditsRequest.to);
            if (from <= to) {
                Debug.Tested();
                bool limitResults = ((to - from).TotalSeconds > 60);
                List<TicketPriceAuditRecord> retVal = new List<TicketPriceAuditRecord>();
                foreach (TicketPriceAuditRecord ticketPriceAuditRecord in TicketPriceAuditRecords) {
                    Debug.Tested();
                    Debug.AssertValid(ticketPriceAuditRecord);
                    Debug.AssertValid(ticketPriceAuditRecord.Timestamp);
                    if (from <= ticketPriceAuditRecord.Timestamp) {
                        Debug.Tested();
                        if (ticketPriceAuditRecord.Timestamp < to) {
                            Debug.Tested();
                            retVal.Add(ticketPriceAuditRecord);
                            if (limitResults) {
                                Debug.Tested();
                                if (retVal.Count == 1000) {
                                    Debug.Untested();
                                    break;
                                } else {
                                    Debug.Tested();
                                }
                            } else {
                                Debug.Untested();
                            }
                        } else {
                            Debug.Tested();
                        }
                    } else {
                        Debug.Untested();
                    }
                }
                return retVal;
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_FROM_GREATER_THAN_TO);
            }
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

        #endregion Admin ticketing service methods

    }   // TicketingServiceLogicLayer

}   // BDDReferenceService
