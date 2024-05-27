using Godot;
using System;
using System.Collections.Generic;

namespace Stardust
{
	public static class GameLogic
    {
        public static int EnergyExpended { get; set; }
        public static RoomManager RoomManager { get; private set; } = new();
        public static TurnQueue TurnQueue { get; private set; }

        static Random rng = new Random();

        public static void BeginGame()
        {
            RoomManager.GenerateRooms();
            ObjectiveHandler.Initialize(2);
            SpawnPawns();
        }

		public static void EndTurn()
		{
            int randRoomIndex = rng.Next(RoomManager.Rooms.Length);
            Room randRoom = RoomManager.Rooms[randRoomIndex];
            randRoom.Damage();

            if (CheckFailState()) AnnounceGameEnd(false);

            TurnQueue.Next();

            EnergyExpended = 0;
		}

        private static bool CheckFailState()
        {
            bool fullyExhausted = ExhaustedCheck();
            bool damaged = DamageCheck();

            return fullyExhausted || damaged;

        }

        /// <returns>True if every pawn is fully exhausted (No cards can be activated)</returns>
        private static bool ExhaustedCheck()
        {
            foreach (Pawn pawn in TurnQueue.Pawns)
            {
                bool fullyExhausted = true;
                foreach (EnergyCard card in pawn.EnergyCards)
                {
                    if (!card.Exhausted)
                    {
                        fullyExhausted = false;
                        break;
                    }
                }

                if (fullyExhausted) return true;
            }

            return false;
        }

        /// <returns>True if station damage reaches above a certain point</returns>
        private static bool DamageCheck()
        {
            int accumulatedDamage = 0;
            foreach (Room room in RoomManager.Rooms)
            {
                accumulatedDamage += room.DamageAmount;
            }

            return accumulatedDamage > 10;
        }

        private static void SpawnPawns()
        {
            PawnType[] pawnTypes = new PawnType[]
            {
                PawnType.Concorde,
                PawnType.Zambuko,
                PawnType.Rosetta,
                PawnType.Aurora
            };

            List<Pawn> pawns = new();

            foreach (PawnType type in pawnTypes)
            {
                EnergyCard[] cards = new EnergyCard[5];
                cards[0] = new(3);
                cards[1] = new(3);
                cards[2] = new(4);
                cards[3] = new(4);
                cards[4] = new(5);

                Pawn pawn = new(type, cards);
                pawns.Add(pawn);
            }

            TurnQueue = new(pawns.ToArray());
        }

        public static void AnnounceGameEnd(bool v)
        {
            GD.Print($"Game End State: {v}");
        }
    } 
}
