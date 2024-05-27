using Godot;
using Stardust.Actions;
using System.Linq;
using static Godot.TextServer;

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
        public int Parts = 0;

        // Concorde specific
        public Direction LastMovementDirection = Direction.None;

        public void MoveTo(Room room)
        {
            if (Room != null && Room.Pawns.Contains(this)) Room.Pawns.Remove(this);
            Room = room;
            Room.Pawns.Add(this);
            Moved?.Invoke();
        }

        public int CalculateMoveCost(Room[] path)
        {
            if (Type == PawnType.Concorde)
            {
                return CalculateMoveCostConcorde(path);
            }
            else return path.Length;
        }

        private int CalculateMoveCostConcorde(Room[] path)
        {
            int cost = 0;
            Room previousRoom = Room;

            Direction movDir = LastMovementDirection;
            if (ActionLibrary.Actions.Count > 0 &&
                (ActionLibrary.Actions[^1] is not MoveAction &&
                ActionLibrary.Actions[^1] is not PickUpPart)) // Concorde can only move for free if he didn't do something other than Move or Pick up.
                movDir = Direction.None;

            foreach (Room room in path)
            {
                Direction direction = previousRoom.Neighbours.Where(n => n.Item2 == room).FirstOrDefault().Item1;
                // Are we already moving in a direction
                if (movDir != Direction.None)
                {
                    // Are we moving in the same direction?
                    if (movDir == direction)
                    {
                        // Are there pawns here that can disable the ability?
                        if (room.Pawns.Count > 0)
                        {
                            movDir = Direction.None;
                        }
                    }
                    // Are we moving in another direction?
                    else
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

            // TODO: THIS HAS TO GO SOMEWHERE ELSE. IT CANNOT STAY HERE.
            // We don't want cost calculations overwriting actual movement.
            LastMovementDirection = movDir;
            return cost;
        }
    } 
}
