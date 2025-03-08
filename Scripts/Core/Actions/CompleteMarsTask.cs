using Godot;
using System;

namespace Stardust.Actions
{
    public class CompleteMarsTask : IUndoableAction
    {
        public CompleteMarsTask(Pawn pawn, Item droppedItem)
        {
            Pawn = pawn;
            DroppedItem = droppedItem;

            foreach (Task task in ObjectiveHandler.CurrentObjective.Tasks)
            {
                if (!task.Completed) Task = task;
            }
        }

        public Task Task { get; private set; }
        public Pawn Pawn { get; private set; }
        public Item DroppedItem { get; private set; }
        public Lander Lander { get; private set; }
        public Room ObjectiveRoom { get; private set; }

        public void Do()
        {
            Pawn.DropItem(DroppedItem);
            Task.Complete();

            foreach (Task task in ObjectiveHandler.CurrentObjective.Tasks)
            {
                if (!task.Completed) return;
            }

            if (ObjectiveHandler.NextObjective != null && ObjectiveHandler.NextObjective.Tasks[0].Tag == "FirstSteps")
                ObjectiveHandler.RevealNewObjective();
        }

        public void Undo()
        {
            Task.UndoComplete();
            Pawn.PickUpItem(DroppedItem);

            foreach (Task task in ObjectiveHandler.CurrentObjective.Tasks)
            {
                if (task == Task) return;
            }

            ObjectiveHandler.UndoRevealNewObjective();
        }
    }
}
