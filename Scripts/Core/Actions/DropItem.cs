using Godot;
using Stardust;
using System;

namespace Stardust.Actions
{
    public class DropItem : IUndoableAction
    {
        public DropItem(Pawn pawn, Room room, Item part)
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
            Pawn.DropItem(Part);
            Room.AddItem(Part);
            GD.Print($"{Pawn.Type}: Dropped {Part.Type} in {Room.Name}");
        }

        public void Undo()
        {
            Room.RemoveItem(Part);
            Pawn.PickUpItem(Part);
            GD.Print($"{Pawn.Type}: Undone {Part.Type} Drop in {Room.Name}");
        }
    } 
}
