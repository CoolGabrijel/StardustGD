namespace Stardust.Actions
{
    public class CompleteMarsTask : IUndoableAction
    {
        public CompleteMarsTask(Pawn pawn, Item droppedItem = null)
        {
            Pawn = pawn;
            DroppedItem = droppedItem;

            foreach (Task task in ObjectiveHandler.CurrentObjective.Tasks)
            {
                if (!task.Completed) Task = task;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public Task Task { get; private set; }
        
        // TODO: I AM SETTING THESE DOWN HERE TO [Newtonsoft.Json.JsonIgnore]
        // THEY SHOULDN'T BE. FIGURE OUT HOW TO MAKE IT WORK WITH PawnType AND ItemType
        
        [Newtonsoft.Json.JsonIgnore]
        public Pawn Pawn { get; private set; }
        [Newtonsoft.Json.JsonIgnore]
        public Item DroppedItem { get; private set; }

        public void Do()
        {
            if (DroppedItem != null) Pawn.DropItem(DroppedItem);
            else GameLogic.EnergyExpended++;
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
            if (DroppedItem != null) Pawn.PickUpItem(DroppedItem);
            else GameLogic.EnergyExpended--;

            foreach (Task task in ObjectiveHandler.CurrentObjective.Tasks)
            {
                if (task == Task) return;
            }

            ObjectiveHandler.UndoRevealNewObjective();
        }
    }
}
