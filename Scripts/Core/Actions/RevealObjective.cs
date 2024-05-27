using Godot;
using System;

namespace Stardust.Actions
{
    public class RevealObjective : IUndoableAction
    {


        public void Do()
        {
            ObjectiveHandler.RevealNewObjective();
        }

        public void Undo()
        {
            ObjectiveHandler.UndoRevealNewObjective();
        }
    }
}
