using Godot;

namespace Stardust.Actions
{
    public class RepairRoom : IUndoableAction
    {
        public RepairRoom(Room room, Pawn pawn)
        {
            Room = room;
            Pawn = pawn;
            part = room.GetItem(ItemType.Part);
        }

        public Room Room { get; private set; }
        public Pawn Pawn { get; private set; }

        private Item part;

        public void Do()
        {
            Room.DamageAmount--;
            Room.RemoveItem(part);
            GameLogic.EnergyExpended += 1;
            ActionLibrary.AddAction(this); // It should maybe not do this.
            GD.Print($"{Pawn}: Repaired {Room.Name}");
        }

        public void Undo()
        {
            Room.DamageAmount++;
            Room.AddItem(part);
            GameLogic.EnergyExpended -= 1;
            GD.Print($"{Pawn}: Undone Repair in {Room.Name}");
        }

        public static bool CanRepairRoom(Room room)
        {
            bool roomHasParts = room.Parts > 0;
            bool roomHasDamage = room.DamageAmount > 0;

            return roomHasParts && roomHasDamage;
        }
    }
}
