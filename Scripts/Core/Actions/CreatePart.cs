using Godot;

namespace Stardust.Actions
{
    public class CreatePart : IUndoableAction
    {
        public CreatePart(PawnType pawn, int cost, RoomType workshop, ItemType part)
        {
            Pawn = pawn;
            EnergyCost = cost;
            Workshop = workshop;
            Part = part;
        }

        public PawnType Pawn { get; private set; }
        public int EnergyCost { get; private set; }
        public RoomType Workshop { get; private set; }
        public ItemType Part { get; private set; }

        public void Do()
        {
            Room workshop = GameLogic.RoomManager.GetRoomByType(Workshop);
            Item part = new(Part);
            
            workshop.AddItem(part);
            GameLogic.EnergyExpended += EnergyCost;
            GD.Print($"{workshop.Name}: Created Part");
        }

        public void Undo()
        {
            Room workshop = GameLogic.RoomManager.GetRoomByType(Workshop);
            Item part = new(Part);
            
            workshop.RemoveItem(part);
            GameLogic.EnergyExpended -= EnergyCost;
            GD.Print($"{workshop.Name}: Undone Part");
        }
    }
}
