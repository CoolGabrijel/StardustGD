using Godot;
using System;

namespace Stardust.Actions
{
    public class RevealObjective : IUndoableAction
    {
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
