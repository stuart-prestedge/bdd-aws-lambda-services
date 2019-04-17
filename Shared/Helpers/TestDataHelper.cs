using System;
using System.Collections.Generic;
using BDDReferenceService.Contracts;
using BDDReferenceService.Model;

namespace BDDReferenceService
{
    internal static class TestDataHelper
    {

        /**
         * The data.
         */
        internal static List<Game> Games = new List<Game>();
        internal static List<Draw> Draws = new List<Draw>();
        internal static List<Ticket> Tickets = new List<Ticket>();
        internal static List<TicketGiftOffer> TicketGiftOffers = new List<TicketGiftOffer>();
        internal static List<GameAuditRecord> GameAuditRecords = new List<GameAuditRecord>();
        internal static List<TicketAudit> TicketAudits = new List<TicketAudit>();
        internal static List<Syndicate> Syndicates = new List<Syndicate>();
        internal static List<SyndicateMember> SyndicateMembers = new List<SyndicateMember>();
        internal static List<SyndicateAuditRecord> SyndicateAuditRecords = new List<SyndicateAuditRecord>();
        internal static List<CountLog> CountLogs = new List<CountLog>();

        /**
         * Most recent ticket gift offer ID (used to get ID emailed to recipient).
         */
        internal static string TicketGiftOfferId = null;

        /**
         * Most recent reserved ticket ID (used to test failure to get ticket that is reserved and not owned).
         */
        internal static string ReservedTicketId = null;

        /**
         * Constructor.
         */
        static TestDataHelper()
        {
            Debug.Tested();

            //???SetupTestData();
        }

        /**
         * Setup the test data.
         */
        //??? private static void SetupTestData()
        // {
        //     Debug.Tested();

        //     string userId = "1";
        //     int gameNumber = 2000;
        //     int drawNumber = 3000;

        //     // Published game
        //     Game game = new Game()
        //     {
        //         ID = (gameNumber++).ToString(),
        //         Name = "Game 1 (published)",
        //         OpenDate = new DateTime(2019, 1, 1),
        //         CloseDate = new DateTime(2020, 1, 1),
        //         TicketCount = 100,
        //         TicketPrice = 500
        //     };
        //     GameServiceLogicLayer.AddGame(game, userId);
        //     Debug.AssertValid(game);
        //     Debug.AssertID(game.ID);
        //     Draw draw = new Draw()
        //     {
        //         ID = (drawNumber++).ToString(),
        //         GameID = game.ID,
        //         DrawDate = new DateTime(2020, 1, 1)
        //     };
        //     GameServiceLogicLayer.AddDraw(draw, userId);
        //     GameServiceLogicLayer.LockGame(game.ID, userId);
        //     GameServiceLogicLayer.PublishGame(game.ID, userId);

        //     // Unlocked game
        //     game = new Game()
        //     {
        //         ID = (gameNumber++).ToString(),
        //         Name = "Game 2 (not locked)",
        //         OpenDate = new DateTime(2019, 1, 1),
        //         CloseDate = new DateTime(2020, 1, 1),
        //         TicketCount = 100,
        //         TicketPrice = 500
        //     };
        //     GameServiceLogicLayer.AddGame(game, userId);
        //     Debug.AssertValid(game);
        //     Debug.AssertID(game.ID);
        //     draw = new Draw()
        //     {
        //         ID = (drawNumber++).ToString(),
        //         GameID = game.ID,
        //         DrawDate = new DateTime(2020, 1, 1)
        //     };
        //     GameServiceLogicLayer.AddDraw(draw, userId);

        //     // Locked game (not published)
        //     game = new Game()
        //     {
        //         ID = (gameNumber++).ToString(),
        //         Name = "Game 3 (locked)",
        //         OpenDate = new DateTime(2019, 1, 1),
        //         CloseDate = new DateTime(2020, 1, 1),
        //         TicketCount = 100,
        //         TicketPrice = 500
        //     };
        //     GameServiceLogicLayer.AddGame(game, userId);
        //     Debug.AssertValid(game);
        //     Debug.AssertID(game.ID);
        //     draw = new Draw()
        //     {
        //         ID = (drawNumber++).ToString(),
        //         GameID = game.ID,
        //         DrawDate = new DateTime(2020, 1, 1)
        //     };
        //     GameServiceLogicLayer.AddDraw(draw, userId);
        //     GameServiceLogicLayer.LockGame(game.ID, userId);

        //     // Frozen game (published)
        //     game = new Game()
        //     {
        //         ID = (gameNumber++).ToString(),
        //         Name = "Game 4 (frozen)",
        //         OpenDate = new DateTime(2019, 1, 1),
        //         CloseDate = new DateTime(2020, 1, 1),
        //         TicketCount = 100,
        //         TicketPrice = 500
        //     };
        //     GameServiceLogicLayer.AddGame(game, userId);
        //     Debug.AssertValid(game);
        //     Debug.AssertID(game.ID);
        //     draw = new Draw()
        //     {
        //         ID = (drawNumber++).ToString(),
        //         GameID = game.ID,
        //         DrawDate = new DateTime(2020, 1, 1)
        //     };
        //     GameServiceLogicLayer.AddDraw(draw, userId);
        //     GameServiceLogicLayer.LockGame(game.ID, userId);
        //     GameServiceLogicLayer.PublishGame(game.ID, userId);
        //     GameServiceLogicLayer.SetGameFrozen(game.ID, userId, new SetGameFrozenRequest() { frozen = true });

        //     // Second published game
        //     game = new Game()
        //     {
        //         ID = (gameNumber++).ToString(),
        //         Name = "Game 5 (published)",
        //         OpenDate = new DateTime(2019, 1, 1),
        //         CloseDate = new DateTime(2020, 1, 1),
        //         TicketCount = 100,
        //         TicketPrice = 500
        //     };
        //     GameServiceLogicLayer.AddGame(game, userId);
        //     Debug.AssertValid(game);
        //     Debug.AssertID(game.ID);
        //     draw = new Draw()
        //     {
        //         ID = (drawNumber++).ToString(),
        //         GameID = game.ID,
        //         DrawDate = new DateTime(2020, 1, 1)
        //     };
        //     GameServiceLogicLayer.AddDraw(draw, userId);
        //     GameServiceLogicLayer.LockGame(game.ID, userId);
        //     GameServiceLogicLayer.PublishGame(game.ID, userId);
        // }

        /**
         * Clear the test data.
         */
        private static void ClearTestData()
        {
            Debug.Tested();

            Games = new List<Game>();
            Draws = new List<Draw>();
            TicketGiftOffers = new List<TicketGiftOffer>();
            Tickets = new List<Ticket>();
            GameAuditRecords = new List<GameAuditRecord>();
            TicketAudits = new List<TicketAudit>();
            Syndicates = new List<Syndicate>();
            SyndicateMembers = new List<SyndicateMember>();
            SyndicateAuditRecords = new List<SyndicateAuditRecord>();
            CountLogs = new List<CountLog>();
            TicketGiftOfferId = Helper.INVALID_ID;
            ReservedTicketId = Helper.INVALID_ID;
        }

        /**
         * Clear the test data.
         */
        internal static void Reset()
        {
            Debug.Tested();

            ClearTestData();
            //??--SetupTestData();
        }

    }   // BDDReferenceService
        
}   // BDDReferenceService
