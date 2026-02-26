using Godot;
using Stardust;
using System;

namespace Stardust.Actions
{
    public class DropItem : IUndoableAction
    {
        public DropItem(PawnType pawn, RoomType room, ItemType part)
        {
            Pawn = pawn;
            Room = room;
            Part = part;
        }

        public PawnType Pawn { get; set; }
        public RoomType Room { get; set; }
        public ItemType Part { get; set; }

        public void Do()
        {
            Pawn pawn = GameLogic.GetPawnByType(Pawn);
            Room room = GameLogic.RoomManager.GetRoomByType(Room);
            Item part = null;
            foreach (Item item in pawn.Inventory)
            {
                if (item.Type == Part)
                {
                    part = item;
                    break;
                }
            }
            
            pawn.DropItem(part);
            room.AddItem(part);
            GD.Print($"{Pawn}: Dropped {Part} in {Room}");
        }

        public void Undo()
        {
            Pawn pawn = GameLogic.GetPawnByType(Pawn);
            Room room = GameLogic.RoomManager.GetRoomByType(Room);
            Item part = room.GetItem(Part);
            
            room.RemoveItem(part);
            pawn.PickUpItem(part);
            GD.Print($"{Pawn}: Undone {Part} Drop in {Room}");
        }
    } 
}
