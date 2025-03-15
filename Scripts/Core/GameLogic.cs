using Godot; // Ideally, this shouldn't use Godot. But it's useful for now.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stardust
{
	public static class GameLogic
    {
        /// <summary>
        /// Fired when the players win or lose. True if Victory.
        /// </summary>
        public static event Action<bool> OnGameFinished;

        public static int EnergyExpended { get; set; }
        public static RoomManager RoomManager { get; private set; } = new();
        public static TurnQueue TurnQueue { get; private set; }
        public static DamageManager DamageManager { get; private set; } = new(RoomManager);

        /// <summary>
        /// Station can handle up to (including) this much damage.
        /// </summary>
        public static int MaxDamage
        {
            get
            {
                if (StardustGameConfig.CurrentConfig.FirstStepsEnabled) return 17;
                else return 15;
            }
        }

        public static void BeginGame(PawnType[] pawns)
        {
            RoomManager.GenerateRooms();
            ObjectiveHandler.Initialize(6);
            SpawnPawns(pawns);
        }

		public static void EndTurn()
		{
            TurnQueue.Next();

            if (CheckFailState()) AnnounceGameEnd(false);

            EnergyExpended = 0;
        }

        /// <summary>
        /// Call to signal the game ended and at which state.
        /// </summary>
        /// <param name="v">True for Victory, False for Defeat.</param>
        public static void AnnounceGameEnd(bool v)
        {
            OnGameFinished?.Invoke(v);
        }

        /// <returns>True if current pawn is fully exhausted or damage exceeds max.</returns>
        private static bool CheckFailState()
        {
            bool fullyExhausted = ExhaustedCheck();
            bool damaged = DamageCheck();

            return fullyExhausted || damaged;
        }

        /// <returns>True if current pawn is fully exhausted (No cards can be activated).</returns>
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

        /// <returns>True if station damage reaches above a certain point.</returns>
        private static bool DamageCheck()
        {
            int accumulatedDamage = 0;
            foreach (Room room in RoomManager.Rooms)
            {
                accumulatedDamage += room.DamageAmount;
            }

            return accumulatedDamage > MaxDamage;
        }

        /// <summary>
        /// Creates the Pawn objects, assigns them energy cards and adds them to the turn queue.
        /// </summary>
        /// <param name="pawnTypes">Characters that will be in the game.</param>
        private static void SpawnPawns(PawnType[] pawnTypes)
        {
            List<Pawn> pawns = new();

            foreach (PawnType type in pawnTypes)
            {
                EnergyCard[] cards;

                if (pawnTypes.Length >= 4)
                {
                    cards = new EnergyCard[5];
                    cards[0] = new(3);
                    cards[1] = new(3);
                    cards[2] = new(4);
                    cards[3] = new(4);
                    cards[4] = new(5);
                }
                else
                {
                    cards = new EnergyCard[6];
                    cards[0] = new(3);
                    cards[1] = new(3);
                    cards[2] = new(4);
                    cards[3] = new(4);
                    cards[4] = new(5);
                    cards[5] = new(5);
                }

                Pawn pawn = new(type, cards);
                pawns.Add(pawn);
            }

            TurnQueue = new(pawns.ToArray());
        }
    } 
}
