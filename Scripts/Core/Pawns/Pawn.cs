using Godot;
using Stardust.Actions;
using System.Collections.Generic;
using System.Linq;

namespace Stardust
{
    public class Pawn
    {
        public Pawn(PawnType type)
        {
            Type = type;
        }

        public Pawn(PawnType type, EnergyCard[] cards)
        {
            Type = type;
            EnergyCards = cards;
        }

        public event System.Action Moved;
        public event System.Action<Item> OnItemPickedUp;
        public event System.Action<Pawn, Item> OnItemDropped;
        public event System.Action OnDamageBlocked;

        public PawnType Type { get; private set; }
        public int OwnerId { get; private set; }
        public Room Room { get; private set; }
        public EnergyCard[] EnergyCards { get; private set; }
        public int InventoryCapacity
        {
            get
            {
                if (Type == PawnType.Rosetta) return 99;
                else return 1;
            }
        }
        public bool CanPickUp => Inventory.Count < InventoryCapacity;
        public int Parts => Inventory.Where((item) => item.Type == ItemType.Part).Count();
        public List<Item> Inventory { get; private set; } = new();

        // Concorde specific
        public Direction LastMovementDirection { get; set; } = Direction.None;

        public void MoveTo(Room room)
        {
            if (Room != null && Room.Pawns.Contains(this)) Room.Pawns.Remove(this);
            Room = room;
            Room.Pawns.Add(this);
            Moved?.Invoke();
        }

        public void DamageBlocked()
        {
            OnDamageBlocked?.Invoke();
        }

        public void PickUpItem(Item item)
        {
            Inventory.Add(item);
            OnItemPickedUp?.Invoke(item);
        }

        public void DropItem(Item item)
        {
            Inventory.Remove(item);
            OnItemDropped?.Invoke(this, item);
        }

        public int CalculateMoveCost(Room[] path)
        {
            if (Type == PawnType.Concorde)
            {
                return CalculateMoveCostConcorde(path);
            }
            else if (Room is MarsTile && (path[^1] is MarsTile || path[^1].RoomType == RoomType.Lander)) // Uuuuugh this is a bit of a bandaid but my pathfinding implementation suuuuucks.
            {
                foreach (var neighbour in Room.Neighbours)
                {
                    if (neighbour.Item2 == path[^1]) return 1;
                }
                return 2;
            }
            else return path.Length;
        }

        private int CalculateMoveCostConcorde(Room[] path)
        {
            if ((path[0] is Lander || path[0] is MarsTile) && (path[^1] is Lander || path[^1] is MarsTile))
                return 1;

            int cost = 0;
            Room previousRoom = Room;

            Direction movDir = LastMovementDirection;
            if (ActionLibrary.Actions.Count > 0 &&
                (ActionLibrary.Actions[^1] is not ConcordeMove &&
                ActionLibrary.Actions[^1] is not PickUpPart)) // Concorde can only move for free if he didn't do something other than Move or Pick up.
                movDir = Direction.None;

            foreach (Room room in path)
            {
                Direction direction = previousRoom.Neighbours.Where(n => n.Item2 == room).FirstOrDefault().Item1;

                // Are there pawns in the room we're in besides us?
                if (previousRoom.Pawns.Where((p) => p.Type != PawnType.Concorde).Any())
                {
                    movDir = Direction.None;
                }

                // Are we already moving in a direction?
                if (movDir != Direction.None)
                {
                    // Are we moving in another direction?
                    if (movDir != direction)
                    {
                        movDir = direction;
                        cost++;
                    }
                }
                else
                {
                    movDir = direction;

                    // Are there pawns here that can disable the ability?
                    if (room.Pawns.Count > 0)
                    {
                        movDir = Direction.None;
                    }

                    cost++;
                }

                previousRoom = room;
            }
            return cost;
        }
    } 
}
