using Godot;
using System;

namespace Stardust.Actions
{
    public class PickUpPart : IUndoableAction
    {
        public PickUpPart(Pawn pawn, Room room, Item part)
        {
            Pawn = pawn;
            Room = room;
            Part = part;
        }

        public Pawn Pawn { get; set; }
        public Room Room { get; set; }
        public Item Part { get; set; }

        public void Do()
        {
            Room.RemoveItem(Part);
            Pawn.PickUpItem(Part);
            GD.Print($"{Pawn}: Picked up part in {Room.Name}");
        }

        public void Undo()
        {
            Room.AddItem(Part);
            Pawn.DropItem(Part);
            GD.Print($"{Pawn}: Undone part pickup in {Room.Name}");
        }
    }
}
