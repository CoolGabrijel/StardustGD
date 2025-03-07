using Godot;
using System;

namespace Stardust.Actions
{
    public class CompleteMarsTask : IUndoableAction
    {
        public CompleteMarsTask(Task marsTask)
        {
            Task = marsTask;
        }

        public Task Task { get; private set; }

        public void Do()
        {
            if (ObjectiveHandler.NextObjective != null && ObjectiveHandler.NextObjective.Tasks[0].Tag == "FirstSteps")
                ObjectiveHandler.RevealNewObjective();
        }

        public void Undo()
        {
            Task.UndoComplete();
        }
    }
}
