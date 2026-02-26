using Godot;

namespace Stardust.Actions
{
    public class RepairRoom : IUndoableAction
    {
        public RepairRoom(RoomType room, PawnType pawn)
        {
            Room = room;
            Pawn = pawn;
            part = GameLogic.RoomManager.GetRoomByType(room).GetItem(ItemType.Part);
        }

        public RoomType Room { get; private set; }
        public PawnType Pawn { get; private set; }

        private Item part;

        public void Do()
        {
            Room room = GameLogic.RoomManager.GetRoomByType(Room);
            room.DamageAmount--;
            room.RemoveItem(part);
            GameLogic.EnergyExpended += 1;
            ActionLibrary.AddAction(this); // It should maybe not do this.
            GD.Print($"{Pawn}: Repaired {room.Name}");
        }

        public void Undo()
        {
            Room room = GameLogic.RoomManager.GetRoomByType(Room);
            room.DamageAmount++;
            room.AddItem(part);
            GameLogic.EnergyExpended -= 1;
            GD.Print($"{Pawn}: Undone Repair in {room.Name}");
        }

        public static bool CanRepairRoom(Room room)
        {
            bool roomHasParts = room.Parts > 0;
            bool roomHasDamage = room.DamageAmount > 0;

            return roomHasParts && roomHasDamage;
        }
    }
}
