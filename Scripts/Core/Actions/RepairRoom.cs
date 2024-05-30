namespace Stardust.Actions
{
    public class RepairRoom : IUndoableAction
    {
        public RepairRoom(Room room)
        {
            Room = room;
        }

        public Room Room { get; private set; }

        public void Do()
        {
            Room.DamageAmount--;
            ActionLibrary.AddAction(this);
        }

        public void Undo()
        {
            Room.DamageAmount++;
        }
    }
}
