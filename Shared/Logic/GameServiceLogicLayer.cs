using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using BDDReferenceService.Contracts;
using BDDReferenceService.Logic;
using BDDReferenceService.Model;
using Newtonsoft.Json.Linq;

namespace BDDReferenceService
{

    /**
     * Logic layer with helper methods.
     */
    internal static class GameServiceLogicLayer
    {

        /**
         * Permissions.
         */
        #region Game service permissions
        internal const string PERMISSION_CAN_CREATE_GAME = "can-create-game";
        internal const string PERMISSION_CAN_UPDATE_GAME = "can-update-game";
        internal const string PERMISSION_CAN_LOCK_GAME = "can-lock-game";
        internal const string PERMISSION_CAN_UNLOCK_GAME = "can-unlock-game";
        internal const string PERMISSION_CAN_PUBLISH_GAME = "can-publish-game";
        internal const string PERMISSION_CAN_FREEZE_GAME = "can-freeze-game";
        internal const string PERMISSION_CAN_SET_GAME_CLOSE_DATE = "can-set-game-close-date";
        internal const string PERMISSION_CAN_SET_GAME_DONATED_TO_CHARITY = "can-set-game-donated-to-charity";
        internal const string PERMISSION_CAN_DELETE_GAME = "can-delete-game";
        internal const string PERMISSION_CAN_CREATE_DRAW = "can-create-draw";
        internal const string PERMISSION_CAN_UPDATE_DRAW = "can-update-draw";
        internal const string PERMISSION_CAN_SET_DRAW_DATE = "can-set-draw-date";
        internal const string PERMISSION_CAN_SET_DRAW_AMOUNT = "can-set-draw-amount";
        internal const string PERMISSION_CAN_SET_DRAW_AUTO_DRAW = "can-set-draw-auto-draw";
        internal const string PERMISSION_CAN_SET_DRAW_WINNING_NUMBER = "can-set-draw-winning-number";
        internal const string PERMISSION_CAN_CLEAR_DRAW_WINNING_NUMBER = "can-clear-draw-winning-number";
        internal const string PERMISSION_CAN_DELETE_DRAW = "can-delete-draw";
        #endregion Game service permissions

        /**
         * Game service errors.
         */
        #region Game service errors
        internal const string ERROR_GAME_NOT_FOUND = "GAME_NOT_FOUND";
        internal const string ERROR_GAME_HAS_NO_DRAWS = "GAME_HAS_NO_DRAWS";
        internal const string ERROR_GAME_LOCKED = "GAME_LOCKED";
        internal const string ERROR_GAME_NOT_LOCKED = "GAME_NOT_LOCKED";
        internal const string ERROR_GAME_NAME_ALREADY_IN_USE = "GAME_NAME_ALREADY_IN_USE";
        internal const string ERROR_GAME_OPEN_DATE_NOT_BEFORE_CLOSE_DATE = "GAME_OPEN_DATE_NOT_BEFORE_CLOSE_DATE";
        internal const string ERROR_GAME_PUBLISHED = "GAME_PUBLISHED";
        internal const string ERROR_GAME_NOT_PUBLISHED = "GAME_NOT_PUBLISHED";
        internal const string ERROR_GAME_FROZEN = "GAME_FROZEN";
        internal const string ERROR_GAME_NOT_FROZEN = "GAME_NOT_FROZEN";
        internal const string ERROR_DRAW_NOT_FOUND = "DRAW_NOT_FOUND";
        internal const string ERROR_DRAW_NOT_IN_GAME = "DRAW_NOT_IN_GAME";
        internal const string ERROR_GAME_DOES_NOT_MATCH = "GAME_DOES_NOT_MATCH";
        internal const string ERROR_DRAW_DATE_ALREADY_SET = "DRAW_DATE_ALREADY_SET";
        internal const string ERROR_DRAW_ALREADY_DRAWN = "DRAW_ALREADY_DRAWN";
        internal const string ERROR_DRAW_NOT_DRAWN = "DRAW_NOT_DRAWN";
        internal const string ERROR_NOT_TIME_FOR_DRAW = "NOT_TIME_FOR_DRAW";
        internal const string ERROR_DRAW_IS_AUTO_DRAW = "DRAW_IS_AUTO_DRAW";
        internal const string ERROR_INVALID_AUDIT_DATA_TYPE = "INVALID_AUDIT_DATA_TYPE";
        #endregion Game service errors

        /*
         * User game service
         */
        #region User game service

        /**
         * Get a list of all the public games.
         */
        internal static async Task<List<PublicGame>> GetPublicGames(AmazonDynamoDBClient dbClient) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            List<PublicGame> retVal = new List<PublicGame>();
            foreach (Game game in TestDataHelper.Games) {
                Debug.AssertValid(game);
                if (game.Locked && game.Published && !game.Frozen) {
                    Debug.Tested();
                    PublicGame publicGame = PublicGameFromGame(game);
                    Debug.AssertValid(publicGame);
                    retVal.Add(publicGame);
                } else {
                    Debug.Tested();
                }
            }
            return retVal;
        }

        /**
         * Get the published game with the specified ID.
         */
        internal static async Task<PublicGame> GetPublicGame(AmazonDynamoDBClient dbClient, string gameIdHash) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(gameIdHash);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            PublicGame retVal = null;
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
                            retVal = PublicGameFromGame(game);
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_GAME_FROZEN);
                        }
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_NOT_PUBLISHED);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_NOT_LOCKED);
                }
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Get a list of all the draws.
         * Only includes private information is explicitly requested to do so.
         */
        internal static async Task<List<PublicDraw>> GetPublicDraws(AmazonDynamoDBClient dbClient, string gameIdHash) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(gameIdHash);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            List<PublicDraw> retVal = new List<PublicDraw>();
            Game game = TestDataHelper.Games.Find(game_ => (Helper.Hash(game_.ID.ToString()) == gameIdHash));
            Debug.AssertValidOrNull(game);
            if (game != null) {
                Debug.Tested();
                foreach (Draw draw in TestDataHelper.Draws) {
                    Debug.Tested();
                    Debug.AssertValid(draw);
                    if (Helper.Hash(draw.GameID.ToString()) == gameIdHash) {
                        Debug.Tested();
                        PublicDraw publicDraw = PublicDrawFromDraw(draw);
                        Debug.AssertValid(publicDraw);
                        retVal.Add(publicDraw);
                    } else {
                        Debug.Tested();
                    }
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
            return retVal;
        }

        /**
         * Get the draw with the specified hashed ID.
         */
        internal static async Task<PublicDraw> GetPublicDraw(AmazonDynamoDBClient dbClient, string gameIdHash, string drawIdHash) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertString(gameIdHash);
            Debug.AssertString(drawIdHash);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            PublicDraw retVal = null;
            Game game = TestDataHelper.Games.Find(game_ => (Helper.Hash(game_.ID.ToString()) == gameIdHash));
            Debug.AssertValidOrNull(game);
            if (game != null) {
                Debug.Tested();
                Draw draw = TestDataHelper.Draws.Find(draw_ => (Helper.Hash(draw_.ID.ToString()) == drawIdHash));
                Debug.AssertValidOrNull(draw);
                if (draw != null) {
                    Debug.Tested();
                    if (Helper.Hash(draw.GameID.ToString()) == gameIdHash) {
                        Debug.Tested();
                        retVal = PublicDrawFromDraw(draw);
                        Debug.AssertValid(retVal);
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_DRAW_NOT_IN_GAME);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_DRAW_NOT_FOUND);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
			return retVal;
        }

        #endregion User game service

        /*
         * Admin game service
         */
        #region Admin game service

        /**
         * Get a list of all the games.
         * Only includes private information is explicitly requested to do so.
         */
        internal static async Task<List<Game>> GetPrivateGames(AmazonDynamoDBClient dbClient) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            return TestDataHelper.Games;
        }

        /**
         * Get the game with the specified ID.
         */
        internal static async Task<Game> GetPrivateGame(AmazonDynamoDBClient dbClient, string id) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            return TestDataHelper.Games.Find(game_ => (game_.ID == id));
        }

        /**
         * Is the game valid as a request to create/update games?
         */
        internal static GameRequest CheckValidRequestGame(JObject requestBody, bool forCreate) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (!string.IsNullOrEmpty((string)requestBody["name"])) {
                    Debug.Tested();
                    if (APIHelper.IsValidAPIDateString((string)requestBody["openDate"])) {
                        Debug.Tested();
                        if (!APIHelper.RequestBodyContainsField(requestBody, "closeDate", out JToken closeDateField) || (closeDateField.Type == JTokenType.Null) || APIHelper.IsValidAPIDateString((string)closeDateField)) {
                            Debug.Tested();
                            if (APIHelper.RequestBodyContainsField(requestBody, "ticketCount", out JToken ticketCountField) && (ticketCountField.Type == JTokenType.Integer) && ((int)ticketCountField > 0)) {
                                Debug.Tested();
                                if (APIHelper.RequestBodyContainsField(requestBody, "ticketPrice", out JToken ticketPriceField) && (ticketPriceField.Type == JTokenType.Integer) && ((int)ticketPriceField > 0)) {
                                    Debug.Tested();
                                    if (Helper.AllFieldsRecognized(requestBody,
                                                                    new List<string>(new String[]{
                                                                        "name",
                                                                        "openDate",
                                                                        "closeDate",
                                                                        "ticketCount",
                                                                        "ticketPrice",
                                                                        "ticketServiceURL",
                                                                        "donatedToCharity"
                                                                        }))) {
                                        return new GameRequest {
                                            name = (string)requestBody["name"],
                                            openDate = (string)requestBody["openDate"],
                                            closeDate = (string)closeDateField,
                                            ticketCount = (UInt32)ticketCountField,
                                            ticketPrice = (UInt16)ticketPriceField,
                                            ticketServiceURL = (string)requestBody["ticketServiceURL"]
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
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Creates a game.
         */
        internal static async Task<Game> CreateGame(AmazonDynamoDBClient dbClient, GameRequest gameRequest, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(gameRequest);
            Debug.Assert(APIHelper.IsValidAPIDateString(gameRequest.openDate));
            Debug.Assert((gameRequest.closeDate == null) || APIHelper.IsValidAPIDateString(gameRequest.closeDate));
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Games);

            Game retVal = null;

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Check the name is unique
            if (TestDataHelper.Games.Find(game_ => (game_.Name == gameRequest.name)) == null) {
                // The specified name is unique.
                Debug.Tested();
                DateTime openDate = (DateTime)APIHelper.DateFromAPIDateString(gameRequest.openDate);
                DateTime? closeDate = APIHelper.DateFromAPIDateString(gameRequest.closeDate);
                if ((closeDate == null) || (openDate < closeDate)) {
                    Debug.Tested();

                    // Add the new game
                    retVal = new Game()
                    {
                        Name = gameRequest.name,
                        OpenDate = openDate,
                        CloseDate = closeDate,
                        TicketCount = gameRequest.ticketCount,
                        TicketPrice = gameRequest.ticketPrice
                    };
                    AddGame(retVal, loggedInUserId);
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_OPEN_DATE_NOT_BEFORE_CLOSE_DATE);
                }
            } else {
                // The game name has already been used.
                Debug.Tested();
                throw new Exception(ERROR_GAME_NAME_ALREADY_IN_USE);
            }
            return retVal;
        }

        /**
         * Adds a game.
         */
        internal static void AddGame(Game game, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(game);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Games);
            //??--Debug.Assert(!SystemHelper.GetSystemLocked());

            // Set the game's ID
            if (game.ID == Helper.INVALID_ID) {
                game.ID = RandomHelper.Next();
            }
            // Add the game
            TestDataHelper.Games.Add(game);

            // Add the audit
            AddGameAudit(GameAuditRecord.AuditChangeType.create, game.ID, loggedInUserId, game);
        }

        /**
         * Update the game specified by ID.
         */
        internal static async Task UpdateGame(AmazonDynamoDBClient dbClient, string id, GameRequest gameRequest, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertValid(gameRequest);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game existingGame = TestDataHelper.Games.Find(game_ => (game_.ID == id));
            Debug.AssertValidOrNull(existingGame);
            if (existingGame != null) {
                Debug.Tested();
                if (!existingGame.Locked) {
                    Debug.Tested();

                    // Update the game
                    existingGame.Name = gameRequest.name;
                    existingGame.OpenDate = (DateTime)APIHelper.DateFromAPIDateString(gameRequest.openDate);
                    existingGame.CloseDate = APIHelper.DateFromAPIDateString(gameRequest.closeDate);
                    existingGame.TicketCount = gameRequest.ticketCount;
                    existingGame.TicketPrice = gameRequest.ticketPrice;
                    existingGame.TicketServiceURL = gameRequest.ticketServiceURL;
                    existingGame.DonatedToCharity = gameRequest.donatedToCharity;
                
                    // Add the audit
                    AddGameAudit(GameAuditRecord.AuditChangeType.update, id, loggedInUserId, existingGame);
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_LOCKED);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
        }

        /**
         * Lock the game specified by ID.
         */
        internal static async Task LockGame(AmazonDynamoDBClient dbClient, string id, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game existingGame = TestDataHelper.Games.Find(game_ => (game_.ID == id));
            Debug.AssertValidOrNull(existingGame);
            if (existingGame != null) {
                Debug.Tested();
                if (!existingGame.Locked) {
                    if (TestDataHelper.Draws.Exists(draw_ => (draw_.GameID == id))) {
                        Debug.Tested();

                        // Lock the game
                        existingGame.Locked = true;

                        // Add the audit
                        AddGameAudit(GameAuditRecord.AuditChangeType.update, id, loggedInUserId, existingGame);
                    } else {
                        // The game has no draws.
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_HAS_NO_DRAWS);
                    }
                } else {
                    // The game is already locked.
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_LOCKED);
                }
            } else {
                // The specified game does not exist.
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
        }

        /**
         * Unlock the game specified by ID.
         */
        internal static async Task UnlockGame(AmazonDynamoDBClient dbClient, string id, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game existingGame = TestDataHelper.Games.Find(game_ => (game_.ID == id));
            Debug.AssertValidOrNull(existingGame);
            if (existingGame != null) {
                Debug.Tested();
                if (existingGame.Locked) {
                    Debug.Tested();
                    if (!existingGame.Published) {
                        Debug.Tested();

                        // Unlock the game
                        existingGame.Locked = false;

                        // Add the audit
                        AddGameAudit(GameAuditRecord.AuditChangeType.update, id, loggedInUserId, existingGame);
                    } else {
                        // The specified game is published.
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_PUBLISHED);
                    }
                } else {
                    // The specified game is not locked.
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_NOT_LOCKED);
                }
            } else {
                // The specified game does not exist.
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
        }

        /**
         * Publish the game specified by ID.
         */
        internal static async Task PublishGame(AmazonDynamoDBClient dbClient, string id, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game existingGame = TestDataHelper.Games.Find(game_ => (game_.ID == id));
            Debug.AssertValidOrNull(existingGame);
            if (existingGame != null) {
                Debug.Tested();
                if (existingGame.Locked) {
                    Debug.Tested();
                    if (!existingGame.Published) {
                        Debug.Tested();

                        // Publish the game
                        existingGame.Published = true;

                        // Add the audit
                        AddGameAudit(GameAuditRecord.AuditChangeType.update, id, loggedInUserId, existingGame);
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_PUBLISHED);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_NOT_LOCKED);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
        }

        /**
         * Check validity of set game frozen request inputs.
         */
        internal static SetGameFrozenRequest CheckValidSetGameFrozenRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                bool ?frozen = null;
                if (APIHelper.RequestBodyContainsField(requestBody, "frozen", out JToken frozenField)) {
                    Debug.Tested();
                    if (frozenField.Type == JTokenType.Boolean) {
                        Debug.Tested();
                        frozen = (bool)frozenField;
                    } else if (frozenField.Type == JTokenType.String) {
                        Debug.Tested();
                        if (bool.TryParse((string)frozenField, out bool frozen_)) {
                            Debug.Untested();
                            frozen = frozen_;
                        } else {
                            Debug.Tested();
                        }
                    } else {
                        Debug.Tested();
                    }
                } else {
                    Debug.Tested();
                    frozen = false;
                }
                if (frozen != null) {
                    Debug.Tested();
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "frozen"
                                                        }))) {
                        Debug.Tested();
                        return new SetGameFrozenRequest {
                            frozen = (bool)frozen
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
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set the game specified by ID as frozen.
         */
        internal static async Task SetGameFrozen(AmazonDynamoDBClient dbClient, string id, string loggedInUserId, SetGameFrozenRequest setGameFrozenRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setGameFrozenRequest);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game existingGame = TestDataHelper.Games.Find(game_ => (game_.ID == id));
            Debug.AssertValidOrNull(existingGame);
            if (existingGame != null) {
                Debug.Tested();
                if (existingGame.Locked) {
                    Debug.Tested();
                    if (existingGame.Frozen && setGameFrozenRequest.frozen) {
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_FROZEN);
                    } else if (!existingGame.Frozen && !setGameFrozenRequest.frozen) {
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_NOT_FROZEN);
                    } else {
                        Debug.Tested();

                        // Unlock the game
                        existingGame.Frozen = setGameFrozenRequest.frozen;

                        // Add the audit
                        AddGameAudit(GameAuditRecord.AuditChangeType.update, id, loggedInUserId, existingGame);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_NOT_LOCKED);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
        }

        /**
         * Check validity of set game close date request inputs.
         */
        internal static SetGameCloseDateRequest CheckValidSetGameCloseDateRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.IsValidAPIDateString((string)requestBody["closeDate"])) {
                    Debug.Tested();
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "closeDate"
                                                        }))) {
                        Debug.Tested();
                        return new SetGameCloseDateRequest {
                            closeDate = (string)requestBody["closeDate"]
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
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set the game specified by ID as frozen.
         */
        internal static async Task SetGameCloseDate(AmazonDynamoDBClient dbClient, string id, string loggedInUserId, SetGameCloseDateRequest setGameCloseDateRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setGameCloseDateRequest);
            Debug.Assert(APIHelper.IsValidAPIDateString(setGameCloseDateRequest.closeDate));
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game existingGame = TestDataHelper.Games.Find(game_ => (game_.ID == id));
            Debug.AssertValidOrNull(existingGame);
            if (existingGame != null) {
                Debug.Tested();
                if (existingGame.Locked) {
                    Debug.Tested();
                    if (!existingGame.Frozen) {
                        Debug.Tested();

                        // Set the game's close date
                        existingGame.CloseDate = APIHelper.DateFromAPIDateString(setGameCloseDateRequest.closeDate);

                        // Add the audit
                        AddGameAudit(GameAuditRecord.AuditChangeType.update, id, loggedInUserId, existingGame);
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_FROZEN);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_NOT_LOCKED);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
        }

        /**
         * Check validity of set game donated to charity request inputs.
         */
        internal static SetGameDonatedToCharityRequest CheckValidSetGameDonatedToCharityRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                UInt32 ?donatedToCharity = null;
                if (APIHelper.RequestBodyContainsField(requestBody, "donatedToCharity", out JToken donatedToCharityField)) {
                    Debug.Tested();
                    if (donatedToCharityField.Type == JTokenType.Integer) {
                        Debug.Tested();
                        if ((int)donatedToCharityField >= 0) {
                            Debug.Tested();
                            donatedToCharity = (UInt32)donatedToCharityField;
                        } else {
                            Debug.Untested();
                        }
                    } else if (donatedToCharityField.Type == JTokenType.String) {
                        Debug.Tested();
                        if (int.TryParse((string)donatedToCharityField, out int donatedToCharity_)) {
                            Debug.Untested();
                            if (donatedToCharity_ >= 0) {
                                Debug.Untested();
                                donatedToCharity = (UInt32)donatedToCharity_;
                            } else {
                                Debug.Untested();
                            }
                        } else {
                            Debug.Tested();
                        }
                    } else {
                        Debug.Tested();
                    }
                } else {
                    Debug.Untested();
                }
                if (donatedToCharity != null) {
                    Debug.Tested();
                    if (Helper.AllFieldsRecognized(requestBody,
                                                    new List<string>(new String[]{
                                                        "donatedToCharity"
                                                        }))) {
                        Debug.Tested();
                        return new SetGameDonatedToCharityRequest {
                            donatedToCharity = (UInt32)donatedToCharity
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
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set the amount donated to charity of the game specified by ID.
         */
        internal static async Task SetGameDonatedToCharity(AmazonDynamoDBClient dbClient, string id, string loggedInUserId, SetGameDonatedToCharityRequest setGameDonatedToCharityRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setGameDonatedToCharityRequest);
            Debug.AssertValid(TestDataHelper.Games);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game existingGame = TestDataHelper.Games.Find(game_ => (game_.ID == id));
            Debug.AssertValidOrNull(existingGame);
            if (existingGame != null) {
                Debug.Tested();
                if (existingGame.Locked) {
                    Debug.Tested();
                    if (!existingGame.Frozen) {
                        Debug.Tested();

                        // Set the amount donated to charity
                        existingGame.DonatedToCharity = setGameDonatedToCharityRequest.donatedToCharity;

                        // Add the audit
                        AddGameAudit(GameAuditRecord.AuditChangeType.update, id, loggedInUserId, existingGame);
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_FROZEN);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_NOT_LOCKED);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
        }

        /**
         * Delete the game specified by ID.
         */
        internal static async Task DeleteGame(AmazonDynamoDBClient dbClient, string id, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(id);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Games);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game game = TestDataHelper.Games.Find(game_ => (game_.ID == id));
            Debug.AssertValidOrNull(game);
            if (game != null) {
                Debug.Tested();
                if (!game.Locked) {
                    Debug.Tested();
                    TestDataHelper.Draws.RemoveAll(draw_ => (draw_.GameID == id));
                    TestDataHelper.Games.RemoveAll(game_ => (game_.ID == id));

                    // Add the audit
                    AddGameAudit(GameAuditRecord.AuditChangeType.delete, id, loggedInUserId);
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_LOCKED);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
        }

        /**
         * Get a game which is just the public information of the passed in game.
         */
        private static PublicGame PublicGameFromGame(Game game) {
            Debug.Tested();
            Debug.AssertValid(game);
            Debug.Assert(game.Locked);
            Debug.Assert(game.Published);
            Debug.Assert(!game.Frozen);

            PublicGame retVal = new PublicGame();
            retVal.IDHash = Helper.Hash(game.ID.ToString());
            retVal.Name = game.Name;
            retVal.OpenDate = APIHelper.APIDateStringFromDate(game.OpenDate);
            retVal.CloseDate = APIHelper.APIDateStringFromDate(game.CloseDate);
            retVal.TicketCount = game.TicketCount;
            retVal.TicketPrice = game.TicketPrice;
            return retVal;
        }

        /**
         * Add a game audit record.
         */
        private static void AddGameAudit(GameAuditRecord.AuditChangeType changeType,
                                         string gameId,
                                         string userId,
                                         Game game = null) {
            Debug.Tested();
            Debug.AssertID(gameId);
            Debug.AssertID(userId);
            Debug.AssertValidOrNull(game);
            Debug.AssertValid(TestDataHelper.GameAuditRecords);

            GameAuditRecord gameAudit = new GameAuditRecord {
                ID = RandomHelper.Next(),
                Timestamp = DateTime.Now,
                ChangeType = changeType,
                GameID = gameId,
                DataType = GameAuditRecord.AuditDataType.game,
                TargetID = gameId,
                UserID = userId
            };
            if (game != null) {
                Debug.Tested();
                gameAudit.TargetCopy = new Game {
                    ID = game.ID,
                    Name = game.Name,
                    OpenDate = game.OpenDate,
                    CloseDate = game.CloseDate,
                    TicketCount = game.TicketCount,
                    TicketPrice = game.TicketPrice,
                    Locked = game.Locked,
                    Published = game.Published,
                    Frozen = game.Frozen,
                    TicketServiceURL = game.TicketServiceURL,
                    DonatedToCharity = game.DonatedToCharity
                };
            } else {
                Debug.Tested();
            }
            TestDataHelper.GameAuditRecords.Add(gameAudit);
        }

        #endregion Admin game service

        /*
         * Draws
         */
        #region Draws

        /**
         * Get a list of all the draws.
         * Only includes private information is explicitly requested to do so.
         */
        internal static async Task<List<Draw>> GetPrivateDraws(AmazonDynamoDBClient dbClient, string gameId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(gameId);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            List<Draw> retVal = new List<Draw>();
            Game game = TestDataHelper.Games.Find(game_ => (game_.ID == gameId));
            Debug.AssertValidOrNull(game);
            if (game != null) {
                Debug.Tested();
                foreach (Draw draw in TestDataHelper.Draws) {
                    Debug.Tested();
                    Debug.AssertValid(draw);
                    if (draw.GameID == gameId) {
                        Debug.Tested();
                        retVal.Add(draw);
                    } else {
                        Debug.Tested();
                    }
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
            return retVal;
        }

        /**
         * Get the draw with the specified ID.
         */
        internal static async Task<Draw> GetPrivateDraw(AmazonDynamoDBClient dbClient, string gameId, string id) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(gameId);
            Debug.AssertID(id);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Draw retVal = null;
            Game game = TestDataHelper.Games.Find(game_ => (game_.ID == gameId));
            Debug.AssertValidOrNull(game);
            if (game != null)
            {
                Debug.Tested();
                retVal = TestDataHelper.Draws.Find(draw => (draw.ID == id));
                Debug.AssertValidOrNull(retVal);
                if (retVal != null)
                {
                    Debug.Tested();
                    if (retVal.GameID != gameId)
                    {
                        Debug.Tested();
                        throw new Exception(ERROR_DRAW_NOT_IN_GAME);
                    } else {
                        Debug.Tested();
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_DRAW_NOT_FOUND);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
			return retVal;
        }

        /**
         * Is the draw valid as a request to create/update draws?
         */
        internal static DrawRequest CheckValidRequestDraw(JObject requestBody, bool forCreate) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                bool validGameID = false;
                string gameId = Helper.INVALID_ID;
                if (!forCreate) {
                    Debug.Tested();
                    validGameID = true;
                } else if (APIHelper.GetIDFromRequestBody(requestBody, "gameId", out gameId)) {
                    Debug.Tested();
                    Debug.AssertID(gameId);
                    validGameID = true;
                } else {
                    Debug.Tested();
                }
                if (validGameID) {
                    Debug.Tested();
                    if (APIHelper.GetOptionalDateTimeFromRequestBody(requestBody, "drawDate", out string drawDate)) {
                        Debug.Tested();
                        Debug.AssertStringOrNull(drawDate);
                        UInt64? amount = null;
                        if (APIHelper.RequestBodyContainsField(requestBody, "amount", out JToken amountField) && (amountField.Type == JTokenType.Integer) && ((int)amountField > 0)) {
                            Debug.Tested();
                            amount = (UInt64)amountField;
                        } else {
                            Debug.Tested();
                        }
                        if (amount != null) {
                            Debug.Tested();
                            if (APIHelper.GetBooleanFromRequestBody(requestBody, "amountIsUpTo", out bool? amountIsUpTo)) {
                                Debug.Tested();
                                Debug.Assert(amountIsUpTo != null);
                                if (APIHelper.GetOptionalBooleanFromRequestBody(requestBody, "autoDraw", out bool? autoDraw)) {
                                    Debug.Tested();
                                    if (Helper.AllFieldsRecognized(requestBody,
                                                                    new List<string>(new String[]{
                                                                        "gameId",
                                                                        "drawDate",
                                                                        "amount",
                                                                        "amountIsUpTo",
                                                                        "autoDraw"
                                                                        }))) {
                                        Debug.Tested();
                                        return new DrawRequest {
                                            gameId = gameId,
                                            drawDate = drawDate,
                                            amount = (UInt64)amount,
                                            amountIsUpTo = (bool)amountIsUpTo,
                                            autoDraw = ((autoDraw == null) ? false : (bool)autoDraw)
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
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Add a draw.
         */
        internal static async Task<Draw> CreateDraw(AmazonDynamoDBClient dbClient, DrawRequest drawRequest, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(drawRequest);
            Debug.AssertID(drawRequest.gameId);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Draws);

            Draw retVal = null;

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Game existingGame = TestDataHelper.Games.Find(game_ => (game_.ID == drawRequest.gameId));
            Debug.AssertValidOrNull(existingGame);
            if (existingGame != null)
            {
                Debug.Tested();
                if (!existingGame.Locked) {
                    Debug.Tested();
                    // Add the draw to the data store
                    retVal = new Draw() {
                        GameID = drawRequest.gameId,
                        DrawDate = APIHelper.DateTimeFromAPIDateTimeString(drawRequest.drawDate),
                        Amount = drawRequest.amount,
                        AmountIsUpTo = drawRequest.amountIsUpTo,
                        AutoDraw = drawRequest.autoDraw
                    };
                    AddDraw(retVal, loggedInUserId);
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_LOCKED);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_GAME_NOT_FOUND);
            }
            return retVal;
        }

        /**
         * Add a draw.
         */
        internal static async Task AddDraw(Draw draw, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(draw);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Draws);
            //??--Debug.Assert(!SystemHelper.GetSystemLocked());

            // Set the draw's ID
            if (draw.ID == Helper.INVALID_ID)
            {
                draw.ID = RandomHelper.Next();
            }
            // Add the draw
            TestDataHelper.Draws.Add(draw);

            // Audit the draw
            AddDrawAudit(GameAuditRecord.AuditChangeType.create, draw.ID, draw.GameID, loggedInUserId, draw);
        }

        /**
         * Update the draw specified by ID.
         */
        internal static async Task UpdateDraw(AmazonDynamoDBClient dbClient, string drawId, DrawRequest drawRequest, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(drawId);
            Debug.AssertValid(drawRequest);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Draw existingDraw = TestDataHelper.Draws.Find(draw_ => (draw_.ID == drawId));
            Debug.AssertValidOrNull(existingDraw);
            if (existingDraw != null) {
                Debug.Tested();
                if (existingDraw.WinningNumber == null) {
                    // Draw has no winner
                    Debug.Tested();
                    Game game = TestDataHelper.Games.Find(game_ => (game_.ID == existingDraw.GameID));
                    Debug.AssertValidOrNull(game);
                    if (game != null) {
                        Debug.Tested();
                        if (!game.Locked) {
                            Debug.Tested();
                            // Update the draw
                            existingDraw.DrawDate = APIHelper.DateTimeFromAPIDateTimeString(drawRequest.drawDate);
                            existingDraw.Amount = drawRequest.amount;
                            existingDraw.AmountIsUpTo = drawRequest.amountIsUpTo;
                            existingDraw.AutoDraw = drawRequest.autoDraw;
                        
                            // Audit the draw
                            AddDrawAudit(GameAuditRecord.AuditChangeType.update, drawId, game.ID, loggedInUserId, existingDraw);
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_GAME_LOCKED);
                        }
                    } else {
                        // Draw's game not found - should never be the case.
                        Debug.Unreachable();
                        throw new Exception(ERROR_GAME_NOT_FOUND);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_DRAW_ALREADY_DRAWN);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_DRAW_NOT_FOUND);
            }
        }

        /**
         * * Is the set draw date request body valid?
         */
        internal static SetDrawDateRequest CheckValidSetDrawDateRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "gameId", out string gameId)) {
                    Debug.Tested();
                    Debug.AssertID(gameId);
                    if (APIHelper.GetDateTimeFromRequestBody(requestBody, "drawDate", out string drawDate)) {
                        Debug.Tested();
                        Debug.Assert(APIHelper.IsValidAPIDateTimeString(drawDate));
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "gameId",
                                                            "drawDate"
                                                            }))) {
                            Debug.Tested();
                            return new SetDrawDateRequest {
                                gameId = gameId,
                                drawDate = drawDate
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
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set the draw date of the draw specified by ID.
         */
        internal static async Task SetDrawDate(AmazonDynamoDBClient dbClient, string drawId, string loggedInUserId, SetDrawDateRequest setDrawDateRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(drawId);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setDrawDateRequest);
            Debug.AssertID(setDrawDateRequest.gameId);
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(setDrawDateRequest.drawDate));
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Draw existingDraw = TestDataHelper.Draws.Find(draw_ => (draw_.ID == drawId));
            Debug.AssertValidOrNull(existingDraw);
            if (existingDraw != null) {
                Debug.Tested();
                if (existingDraw.GameID == setDrawDateRequest.gameId) {
                    Debug.Tested();
                    if (existingDraw.WinningNumber == null) {
                        // Draw has no winner
                        Debug.Tested();
                        if (existingDraw.DrawDate == null) {
                            Debug.Tested();
                            Game game = TestDataHelper.Games.Find(game_ => (game_.ID == existingDraw.GameID));
                            Debug.AssertValidOrNull(game);
                            if (game != null) {
                                Debug.Tested();
                                if (game.Published) {
                                    // Game is published
                                    Debug.Tested();
                                    if (!game.Frozen) {
                                        // Game is not frozen
                                        Debug.Tested();

                                        // Set the draw date
                                        existingDraw.DrawDate = APIHelper.DateTimeFromAPIDateTimeString(setDrawDateRequest.drawDate);
                                    
                                        // Audit the draw
                                        AddDrawAudit(GameAuditRecord.AuditChangeType.update, drawId, game.ID, loggedInUserId, existingDraw);
                                    } else {
                                        Debug.Tested();
                                        throw new Exception(ERROR_GAME_FROZEN);
                                    }
                                } else {
                                    Debug.Tested();
                                    throw new Exception(ERROR_GAME_NOT_PUBLISHED);
                                }
                            } else {
                                // Draw's game not found - should never be the case.
                                Debug.Unreachable();
                                throw new Exception(ERROR_GAME_NOT_FOUND);
                            }
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_DRAW_DATE_ALREADY_SET);
                        }
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_DRAW_ALREADY_DRAWN);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_DOES_NOT_MATCH);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_DRAW_NOT_FOUND);
            }
        }

        /**
         * Is the set draw amount request body valid?
         */
        internal static SetDrawAmountRequest CheckValidSetDrawAmountRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "gameId", out string gameId)) {
                    Debug.Tested();
                    Debug.AssertID(gameId);
                    UInt64? amount = null;
                    if (APIHelper.RequestBodyContainsField(requestBody, "amount", out JToken amountField) && (amountField.Type == JTokenType.Integer) && ((int)amountField > 0)) {
                        Debug.Tested();
                        amount = (UInt64)amountField;
                    } else {
                        Debug.Tested();
                    }
                    if (amount != null) {
                        Debug.Tested();
                        if (APIHelper.GetBooleanFromRequestBody(requestBody, "amountIsUpTo", out bool? amountIsUpTo)) {
                            Debug.Tested();
                            Debug.Assert(amountIsUpTo != null);
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "gameId",
                                                                "amount",
                                                                "amountIsUpTo"
                                                                }))) {
                                Debug.Tested();
                                return new SetDrawAmountRequest {
                                    gameId = gameId,
                                    amount = (UInt64)amount,
                                    amountIsUpTo = (bool)amountIsUpTo
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
         * Set the amount of the draw specified by ID.
         */
        internal static async Task SetDrawAmount(AmazonDynamoDBClient dbClient, string drawId, string loggedInUserId, SetDrawAmountRequest setDrawAmountRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(drawId);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setDrawAmountRequest);
            Debug.AssertID(setDrawAmountRequest.gameId);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Draw existingDraw = TestDataHelper.Draws.Find(draw_ => (draw_.ID == drawId));
            Debug.AssertValidOrNull(existingDraw);
            if (existingDraw != null) {
                Debug.Tested();
                if (existingDraw.GameID == setDrawAmountRequest.gameId) {
                    Debug.Tested();
                    Game game = TestDataHelper.Games.Find(game_ => (game_.ID == existingDraw.GameID));
                    Debug.AssertValidOrNull(game);
                    if (game != null) {
                        Debug.Tested();
                        if (game.Published) {
                            // Game is published
                            Debug.Tested();
                            if (!game.Frozen) {
                                // Game is not frozen
                                Debug.Tested();

                                // Set the draw amount and amount is up to flag
                                existingDraw.Amount = setDrawAmountRequest.amount;
                                existingDraw.AmountIsUpTo = setDrawAmountRequest.amountIsUpTo;
                            
                                // Audit the draw
                                AddDrawAudit(GameAuditRecord.AuditChangeType.update, drawId, game.ID, loggedInUserId, existingDraw);
                            } else {
                                Debug.Tested();
                                throw new Exception(ERROR_GAME_FROZEN);
                            }
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_GAME_NOT_PUBLISHED);
                        }
                    } else {
                        // Draw's game not found - should never be the case.
                        Debug.Unreachable();
                        throw new Exception(ERROR_GAME_NOT_FOUND);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_DOES_NOT_MATCH);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_DRAW_NOT_FOUND);
            }
        }

        /**
         * Is the set draw auto-draw request body valid?
         */
        internal static SetDrawAutoDrawRequest CheckValidSetDrawAutoDrawRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "gameId", out string gameId)) {
                    Debug.Tested();
                    Debug.AssertID(gameId);
                    if (APIHelper.GetBooleanFromRequestBody(requestBody, "autoDraw", out bool? autoDraw)) {
                        Debug.Tested();
                        Debug.Assert(autoDraw != null);
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "gameId",
                                                            "autoDraw"
                                                            }))) {
                            Debug.Tested();
                            return new SetDrawAutoDrawRequest {
                                gameId = gameId,
                                autoDraw = (bool)autoDraw
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
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Set the auto-draw flag of the draw specified by ID.
         */
        internal static async Task SetDrawAutoDraw(AmazonDynamoDBClient dbClient, string drawId, string loggedInUserId, SetDrawAutoDrawRequest setDrawAutoDrawRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(drawId);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setDrawAutoDrawRequest);
            Debug.AssertID(setDrawAutoDrawRequest.gameId);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Draw existingDraw = TestDataHelper.Draws.Find(draw_ => (draw_.ID == drawId));
            Debug.AssertValidOrNull(existingDraw);
            if (existingDraw != null) {
                Debug.Tested();
                if (existingDraw.WinningNumber == null) {
                    // Draw has no winner
                    Debug.Tested();
                    if (existingDraw.GameID == setDrawAutoDrawRequest.gameId) {
                        Debug.Tested();
                        Game game = TestDataHelper.Games.Find(game_ => (game_.ID == existingDraw.GameID));
                        Debug.AssertValidOrNull(game);
                        if (game != null) {
                            Debug.Tested();
                            if (game.Published) {
                                // Game is published
                                Debug.Tested();
                                if (!game.Frozen) {
                                    // Game is not frozen
                                    Debug.Tested();

                                    // Set the auto-draw flag
                                    existingDraw.AutoDraw = setDrawAutoDrawRequest.autoDraw;
                                
                                    // Audit the draw
                                    AddDrawAudit(GameAuditRecord.AuditChangeType.update, drawId, game.ID, loggedInUserId, existingDraw);
                                } else {
                                    Debug.Tested();
                                    throw new Exception(ERROR_GAME_FROZEN);
                                }
                            } else {
                                Debug.Tested();
                                throw new Exception(ERROR_GAME_NOT_PUBLISHED);
                            }
                        } else {
                            // Draw's game not found - should never be the case.
                            Debug.Unreachable();
                            throw new Exception(ERROR_GAME_NOT_FOUND);
                        }
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_GAME_DOES_NOT_MATCH);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_DRAW_ALREADY_DRAWN);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_DRAW_NOT_FOUND);
            }
        }

        /**
         * Is the set draw winning number request body valid?
         */
        internal static SetDrawWinningNumberRequest CheckValidSetDrawWinningNumberRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "gameId", out string gameId)) {
                    Debug.Tested();
                    Debug.AssertID(gameId);
                    if (APIHelper.RequestBodyContainsField(requestBody, "winningNumber", out JToken winningNumberField) &&
                        (winningNumberField.Type == JTokenType.Integer) &&
                        ((int)winningNumberField >= 0) &&
                        ((int)winningNumberField <= 999999999)) {
                        Debug.Tested();
                        bool validAmount = false;
                        UInt64? amount = null;
                        if (!APIHelper.RequestBodyContainsField(requestBody, "amount", out JToken amountField) || (amountField.Type == JTokenType.Null)) {
                            Debug.Tested();
                            validAmount = true;
                        } else if ((amountField.Type == JTokenType.Integer) && ((int)amountField > 0)) {
                            Debug.Tested();
                            validAmount = true;
                            amount = (UInt64)amountField;
                        } else {
                            Debug.Untested();
                        }
                        if (validAmount) {
                            Debug.Tested();
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "gameId",
                                                                "winningNumber",
                                                                "amount"
                                                                }))) {
                                Debug.Tested();
                                return new SetDrawWinningNumberRequest {
                                    gameId = gameId,
                                    winningNumber = (UInt32)winningNumberField,
                                    amount = amount
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
                        Debug.Untested();
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
         * Set the winning number flag of the draw specified by ID.
         */
        internal static async Task SetDrawWinningNumber(AmazonDynamoDBClient dbClient, string drawId, string loggedInUserId, SetDrawWinningNumberRequest setDrawWinningNumberRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(drawId);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(setDrawWinningNumberRequest);
            Debug.AssertID(setDrawWinningNumberRequest.gameId);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Draw existingDraw = TestDataHelper.Draws.Find(draw_ => (draw_.ID == drawId));
            Debug.AssertValidOrNull(existingDraw);
            if (existingDraw != null) {
                // Draw found
                Debug.Tested();
                if (existingDraw.GameID == setDrawWinningNumberRequest.gameId) {
                    // Specified game matches draw's game
                    Debug.Tested();
                    if (existingDraw.WinningNumber == null) {
                        // Draw has no winner
                        Debug.Tested();
                        if (!existingDraw.AutoDraw) {
                            // Manual draw
                            Debug.Tested();
                            if ((existingDraw.DrawDate != null) && (existingDraw.DrawDate <= DateTime.Now)) {
                                // Time for the draw
                                Debug.Tested();
                                Game game = TestDataHelper.Games.Find(game_ => (game_.ID == existingDraw.GameID));
                                Debug.AssertValidOrNull(game);
                                if (game != null) {
                                    Debug.Tested();
                                    if (game.Published) {
                                        // Game is published
                                        Debug.Tested();
                                        if (!game.Frozen) {
                                            // Game is not frozen
                                            Debug.Tested();

                                            // Set the draw date
                                            existingDraw.WinningNumber = setDrawWinningNumberRequest.winningNumber;
                                            //??++existingDraw.WinningTicketID = ;
                                            //??++existingDraw.WinningUserID = ;
                                            existingDraw.DrawnDate = DateTime.Now;
                                            if (setDrawWinningNumberRequest.amount != null) {
                                                Debug.Tested();
                                                existingDraw.Amount = (UInt64)setDrawWinningNumberRequest.amount;
                                                existingDraw.AmountIsUpTo = false;
                                            } else {
                                                Debug.Tested();
                                            }
                                            // Audit the draw
                                            AddDrawAudit(GameAuditRecord.AuditChangeType.update, drawId, game.ID, loggedInUserId, existingDraw);
                                        } else {
                                            Debug.Tested();
                                            throw new Exception(ERROR_GAME_FROZEN);
                                        }
                                    } else {
                                        Debug.Tested();
                                        throw new Exception(ERROR_GAME_NOT_PUBLISHED);
                                    }
                                } else {
                                    // Draw's game not found - should never be the case.
                                    Debug.Unreachable();
                                    throw new Exception(ERROR_GAME_NOT_FOUND);
                                }
                            } else {
                                Debug.Tested();
                                throw new Exception(ERROR_NOT_TIME_FOR_DRAW);
                            }
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_DRAW_IS_AUTO_DRAW);
                        }
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_DRAW_ALREADY_DRAWN);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_DOES_NOT_MATCH);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_DRAW_NOT_FOUND);
            }
        }

        /**
         * Is the clear draw winning number request body valid?
         */
        internal static ClearDrawWinningNumberRequest CheckValidClearDrawWinningNumberRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (APIHelper.GetIDFromRequestBody(requestBody, "gameId", out string gameId)) {
                    Debug.Tested();
                    Debug.AssertID(gameId);
                    if (APIHelper.GetOptionalBooleanFromRequestBody(requestBody, "autoDraw", out bool? autoDraw)) {
                        Debug.Tested();
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "gameId",
                                                            "autoDraw"
                                                            }))) {
                            Debug.Tested();
                            return new ClearDrawWinningNumberRequest {
                                gameId = gameId,
                                autoDraw = autoDraw
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
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
         * Clear the winning number flag of the draw specified by ID.
         */
        internal static async Task ClearDrawWinningNumber(AmazonDynamoDBClient dbClient, string drawId, string loggedInUserId, ClearDrawWinningNumberRequest clearDrawWinningNumberRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(drawId);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(clearDrawWinningNumberRequest);
            Debug.AssertID(clearDrawWinningNumberRequest.gameId);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Draw existingDraw = TestDataHelper.Draws.Find(draw_ => (draw_.ID == drawId));
            Debug.AssertValidOrNull(existingDraw);
            if (existingDraw != null) {
                Debug.Tested();
                if (existingDraw.GameID == clearDrawWinningNumberRequest.gameId) {
                    // Specified game matches draw's game
                    Debug.Tested();
                    Game game = TestDataHelper.Games.Find(game_ => (game_.ID == existingDraw.GameID));
                    Debug.AssertValidOrNull(game);
                    if (game != null) {
                        Debug.Tested();
                        if (game.Published) {
                            // Game is published
                            Debug.Tested();
                            if (!game.Frozen) {
                                // Game is not frozen
                                Debug.Tested();
                                if (existingDraw.WinningNumber != null) {
                                    // Draw has a winner to clear
                                    Debug.Tested();

                                    // Clear the winning ticket information
                                    existingDraw.WinningNumber = null;
                                    existingDraw.WinningTicketID = null;
                                    existingDraw.WinningUserID = null;
                                    if (clearDrawWinningNumberRequest.autoDraw != null) {
                                        Debug.Tested();
                                        existingDraw.AutoDraw = (bool)clearDrawWinningNumberRequest.autoDraw;
                                    } else {
                                        Debug.Tested();
                                    }
                                    // Audit the draw
                                    AddDrawAudit(GameAuditRecord.AuditChangeType.update, drawId, game.ID, loggedInUserId, existingDraw);
                                } else {
                                    Debug.Tested();
                                    throw new Exception(ERROR_DRAW_NOT_DRAWN);
                                }
                            } else {
                                Debug.Tested();
                                throw new Exception(ERROR_GAME_FROZEN);
                            }
                        } else {
                            Debug.Tested();
                            throw new Exception(ERROR_GAME_NOT_PUBLISHED);
                        }
                    } else {
                        // Draw's game not found - should never be the case.
                        Debug.Unreachable();
                        throw new Exception(ERROR_GAME_NOT_FOUND);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_DOES_NOT_MATCH);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_DRAW_NOT_FOUND);
            }
        }

        /**
         * Delete the draw specified by ID.
         */
        internal static async Task DeleteDraw(AmazonDynamoDBClient dbClient, string gameId, string drawId, string loggedInUserId) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(gameId);
            Debug.AssertID(drawId);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(TestDataHelper.Draws);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            Draw draw = TestDataHelper.Draws.Find(draw_ => (draw_.ID == drawId));
            Debug.AssertValidOrNull(draw);
            if (draw != null) {
                Debug.Tested();
                if (draw.GameID == gameId) {
                    Debug.Tested();
                    if (draw.WinningNumber == null) {
                        // Draw has no winner
                        Debug.Tested();
                        Game game = TestDataHelper.Games.Find(game_ => (game_.ID == draw.GameID));
                        Debug.AssertValidOrNull(game);
                        if (game != null) {
                            Debug.Tested();
                            if (!game.Locked) {
                                Debug.Tested();

                                // Delete the draw
                                TestDataHelper.Draws.Remove(draw);

                                // Audit the draw
                                AddDrawAudit(GameAuditRecord.AuditChangeType.delete, drawId, game.ID, loggedInUserId);
                            } else {
                                Debug.Tested();
                                throw new Exception(ERROR_GAME_LOCKED);
                            }
                        } else {
                            // Draw's game not found - should never be the case.
                            Debug.Unreachable();
                            throw new Exception(ERROR_GAME_NOT_FOUND);
                        }
                    } else {
                        Debug.Tested();
                        throw new Exception(ERROR_DRAW_ALREADY_DRAWN);
                    }
                } else {
                    Debug.Tested();
                    throw new Exception(ERROR_GAME_DOES_NOT_MATCH);
                }
            } else {
                Debug.Tested();
                throw new Exception(ERROR_DRAW_NOT_FOUND);
            }
        }

        /**
         * Get a draw which is just the public information of the passed in draw.
         */
        private static PublicDraw PublicDrawFromDraw(Draw draw) {
            Debug.Tested();
            Debug.AssertValid(draw);

            PublicDraw retVal = new PublicDraw();
            retVal.IDHash = Helper.Hash(draw.ID.ToString());
            retVal.GameIDHash = Helper.Hash(draw.GameID.ToString());
            retVal.DrawDate = APIHelper.APIDateTimeStringFromDateTime(draw.DrawDate);
            retVal.DrawnDate = APIHelper.APIDateTimeStringFromDateTime(draw.DrawnDate);
            retVal.Amount = draw.Amount;
            retVal.AmountIsUpTo = draw.AmountIsUpTo;
            retVal.WinningNumber = draw.WinningNumber;
            return retVal;
        }

        /**
         * Add a draw audit record.
         */
        private static void AddDrawAudit(GameAuditRecord.AuditChangeType changeType,
                                         string drawId,
                                         string gameId,
                                         string userId,
                                         Draw draw = null) {
            Debug.Tested();
            Debug.AssertID(drawId);
            Debug.AssertID(userId);
            Debug.AssertValidOrNull(draw);
            Debug.AssertValid(TestDataHelper.GameAuditRecords);

            GameAuditRecord drawAudit = new GameAuditRecord {
                ID = RandomHelper.Next(),
                Timestamp = DateTime.Now,
                ChangeType = changeType,
                GameID = gameId,
                DataType = GameAuditRecord.AuditDataType.draw,
                TargetID = drawId,
                UserID = userId
            };
            if (draw != null) {
                Debug.Tested();
                drawAudit.TargetCopy = new Draw {
                    ID = draw.ID,
                    GameID = draw.GameID,
                    DrawDate = draw.DrawDate,
                    DrawnDate = draw.DrawnDate,
                    Amount = draw.Amount,
                    AmountIsUpTo = draw.AmountIsUpTo,
                    WinningNumber = draw.WinningNumber,
                    WinningTicketID = draw.WinningTicketID,
                    WinningUserID = draw.WinningUserID,
                    AutoDraw = draw.AutoDraw,
                    AutoDrawn = draw.AutoDrawn
                };
            } else {
                Debug.Tested();
            }
            TestDataHelper.GameAuditRecords.Add(drawAudit);
        }

        #endregion Draws

        /*
         * Game audit records.
         */
        #region Game audit records

        /**
         * Check validity of get audit records request inputs.
         */
        internal static GetAuditRecordsRequest CheckValidGetAuditRecordsRequest(JObject requestBody) {
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
                        bool validDataType = false;
                        Int32? dataType = null;
                        if (APIHelper.RequestBodyContainsField(requestBody, "dataType", out JToken dataTypeField)) {
                            Debug.Tested();
                            if (dataTypeField.Type == JTokenType.Integer) {
                                Debug.Tested();
                                validDataType = true;
                                dataType = (Int32)dataTypeField;
                            } else if (dataTypeField.Type == JTokenType.String) {
                                Debug.Tested();
                                if (int.TryParse((string)dataTypeField, out int dataType_)) {
                                    Debug.Tested();
                                    validDataType = true;
                                    dataType = (Int32)dataType_;
                                } else {
                                    Debug.Tested();
                                }
                            } else {
                                Debug.Tested();
                            }
                        } else {
                            Debug.Tested();
                            validDataType = true;
                        }
                        if (validDataType) {
                            if (Helper.AllFieldsRecognized(requestBody,
                                                            new List<string>(new String[]{
                                                                "from",
                                                                "to",
                                                                "dataType"
                                                                }))) {
                                Debug.Tested();
                                return new GetAuditRecordsRequest {
                                    from = from,
                                    to = to,
                                    dataType = dataType
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
         * Get a list of all the audit records in a time range.
         */
        internal static async Task<List<GameAuditRecord>> GetAuditRecords(AmazonDynamoDBClient dbClient, GetAuditRecordsRequest getAuditRecordsRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(getAuditRecordsRequest);
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(getAuditRecordsRequest.from));
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(getAuditRecordsRequest.to));
            Debug.AssertValid(TestDataHelper.GameAuditRecords);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            DateTime from = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(getAuditRecordsRequest.from);
            DateTime to = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(getAuditRecordsRequest.to);
            if (from <= to) {
                Debug.Tested();
                if ((getAuditRecordsRequest.dataType == null) ||
                    (getAuditRecordsRequest.dataType == (Int32)GameAuditRecord.AuditDataType.game) ||
                    (getAuditRecordsRequest.dataType == (Int32)GameAuditRecord.AuditDataType.draw)) {
                    Debug.Tested();
                    double seconds = (to - from).TotalSeconds;
                    bool limitResults = (seconds > 60);
                    List<GameAuditRecord> retVal = new List<GameAuditRecord>();
                    foreach (GameAuditRecord gameAuditRecord in TestDataHelper.GameAuditRecords) {
                        Debug.Tested();
                        Debug.AssertValid(gameAuditRecord);
                        Debug.AssertValid(gameAuditRecord.Timestamp);
                        if (from <= gameAuditRecord.Timestamp) {
                            Debug.Tested();
                            if (gameAuditRecord.Timestamp < to) {
                                Debug.Tested();
                                if ((getAuditRecordsRequest.dataType == null) || (getAuditRecordsRequest.dataType == (Int32)gameAuditRecord.DataType)) {
                                    Debug.Tested();
                                    retVal.Add(gameAuditRecord);
                                    if (limitResults) {
                                        // Results will be limited to 1000 records
                                        Debug.Tested();
                                        if (retVal.Count == 1000) {
                                            // Limit of 1000 records reached
                                            Debug.Untested();
                                            break;
                                        } else {
                                            // Limit not yet reached
                                            Debug.Tested();
                                        }
                                    } else {
                                        // Results are not limited to 1000 records
                                        Debug.Untested();
                                    }
                                } else {
                                    // Audit record type does not match
                                    Debug.Tested();
                                }
                            } else {
                                // Audit timestamp on or after to timestamp
                                Debug.Tested();
                            }
                        } else {
                            // Audit timestamp before from timestamp
                            Debug.Tested();
                        }
                    }
                    return retVal;
                } else {
                    // Invalid/unrecognized audit data type
                    Debug.Tested();
                    throw new Exception(ERROR_INVALID_AUDIT_DATA_TYPE);
                }
            } else {
                // From field greater than to field
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_FROM_GREATER_THAN_TO);
            }
        }

        #endregion Game audit records

    }   // GameServiceLogicLayer

}   // BDDReferenceService
