using Godot; // Ideally, this shouldn't use Godot. But it's useful for now.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stardust
{
	public static class GameLogic
    {
        /// <summary>
        /// Fired when the players win or lose. Boolean represents that.
        /// </summary>
        public static event Action<bool> OnGameFinished;

        public static int EnergyExpended { get; set; }
        public static RoomManager RoomManager { get; private set; } = new();
        public static TurnQueue TurnQueue { get; private set; }
        public static DamageManager DamageManager { get; private set; } = new(RoomManager);

        public static void BeginGame()
        {
            RoomManager.GenerateRooms();
            ObjectiveHandler.Initialize(6);
            SpawnPawns();
        }

		public static void EndTurn()
		{
            //int randRoomIndex = rng.Next(RoomManager.Rooms.Length);
            //Room randRoom = RoomManager.Rooms[randRoomIndex];
            //randRoom.Damage();

            if (CheckFailState()) AnnounceGameEnd(false);

            ExhaustPawn(TurnQueue.CurrentPawn);

            TurnQueue.Next();

            EnergyExpended = 0;
		}

        private static bool CheckFailState()
        {
            bool fullyExhausted = ExhaustedCheck();
            bool damaged = DamageCheck();

            return fullyExhausted || damaged;

        }

        private static void ExhaustPawn(Pawn pawn)
        {
            EnergyCard card = pawn.EnergyCards.Where((x) => !x.Exhausted).Where((x) => x.Energy >= EnergyExpended).FirstOrDefault();

            if (card == null) return;

            card.Exhausted = true;
            GD.Print($"GameLogic :: Exhausting {card.Energy} Energy");
        }

        /// <returns>True if current pawn is fully exhausted (No cards can be activated)</returns>
        private static bool ExhaustedCheck()
        {
            foreach (EnergyCard card in TurnQueue.CurrentPawn.EnergyCards)
            {
                if (!card.Exhausted)
                {
                    return false;
                }
            }

            return true;
        }

        /// <returns>True if station damage reaches above a certain point</returns>
        private static bool DamageCheck()
        {
            int accumulatedDamage = 0;
            foreach (Room room in RoomManager.Rooms)
            {
                accumulatedDamage += room.DamageAmount;
            }

            return accumulatedDamage > 15;
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
            OnGameFinished?.Invoke(v);
        }
    } 
}
