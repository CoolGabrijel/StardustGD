using Godot;
using System;
using System.Collections.Generic;

namespace Stardust.Actions
{
	public class Sleep : IUndoableAction
	{

        public Sleep(Pawn pawn)
        {
            Pawn = pawn;
        }

        public Room DamagedRoom { get; set; }
        public Pawn Pawn { get; private set; }
        public EnergyCard[] RefreshedCards { get; private set; }

        private EndTurn endTurnAction;

        public void Do()
        {
            List<EnergyCard> cards = new();

            foreach (EnergyCard card in Pawn.EnergyCards)
            {
                if (card.Exhausted)
                {
                    cards.Add(card);
                    card.Exhausted = false;
                }
            }

            RefreshedCards = cards.ToArray();

            GD.Print($"{Pawn.Type} slept in Habitation and refreshed {RefreshedCards.Length} cards");

            endTurnAction = new EndTurn(true);
            endTurnAction.DamagedRoom = DamagedRoom;

            endTurnAction.Do();
        }

        public void Undo()
        {
            foreach (EnergyCard card in RefreshedCards)
            {
                card.Exhausted = true;
            }

            endTurnAction.Undo();

            GD.Print($"{Pawn.Type} undone sleeping in Habitation and exhausted {RefreshedCards.Length} cards");
        }
    } 
}
