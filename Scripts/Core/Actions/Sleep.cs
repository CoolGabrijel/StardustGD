using Godot;
using System;
using System.Collections.Generic;

namespace Stardust.Actions
{
	public class Sleep : IUndoableAction
	{

        public Sleep(PawnType pawn)
        {
            Pawn = pawn;
        }

        public RoomType DamagedRoom { get; set; }
        public PawnType Pawn { get; private set; }
        public EnergyCard[] RefreshedCards { get; private set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool DamagedRoomSet { get; set; }

        private EndTurn endTurnAction;

        public void Do()
        {
            List<EnergyCard> cards = new();
            Pawn pawn = GameLogic.GetPawnByType(Pawn);
            
            foreach (EnergyCard card in pawn.EnergyCards)
            {
                if (card.Exhausted)
                {
                    cards.Add(card);
                    card.Exhausted = false;
                }
            }

            RefreshedCards = cards.ToArray();

            GD.Print($"{Pawn} slept in Habitation and refreshed {RefreshedCards.Length} cards");

            endTurnAction = new EndTurn(true);
            endTurnAction.DamagedRoom = DamagedRoom;
            endTurnAction.DamagedRoomSet = DamagedRoomSet;

            endTurnAction.Do();
        }

        public void Undo()
        {
            foreach (EnergyCard card in RefreshedCards)
            {
                card.Exhausted = true;
            }

            endTurnAction.Undo();

            GD.Print($"{Pawn} undone sleeping in Habitation and exhausted {RefreshedCards.Length} cards");
        }
    } 
}
