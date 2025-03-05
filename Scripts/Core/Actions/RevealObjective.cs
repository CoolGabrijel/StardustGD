using Godot;
using System;

namespace Stardust.Actions
{
    public class RevealObjective : IUndoableAction
    {
        public RevealObjective(Pawn pawn = null)
        {
            Pawn = pawn;
        }

        public Pawn Pawn { get; private set; }

        public void Do()
        {
            GameLogic.EnergyExpended++;
            ObjectiveHandler.RevealNewObjective();
        }

        public void Undo()
        {
            GameLogic.EnergyExpended--;
            ObjectiveHandler.UndoRevealNewObjective();
        }
    }
}
