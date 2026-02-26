using Godot;

namespace Stardust.Actions
{
    public class ConcordeMove : IUndoableAction
    {
        public ConcordeMove(int cost, RoomType from, RoomType to, Direction startingMovementDirection, Direction endingMovementDirection)
        {
            EnergyCost = cost;
            FromRoom = from;
            ToRoom = to;
            StartingMovementDirection = startingMovementDirection;
            EndingMovementDirection = endingMovementDirection;
        }

        public int EnergyCost { get; private set; }
        public RoomType FromRoom { get; private set; }
        public RoomType ToRoom { get; private set; }
        public Direction StartingMovementDirection { get; private set; }
        public Direction EndingMovementDirection { get; private set; }
        
        private Pawn Pawn => GameLogic.GetPawnByType(PawnType.Concorde);

        public void Do()
        {
            Room toRoom = GameLogic.RoomManager.GetRoomByType(ToRoom);
            Pawn.MoveTo(toRoom);
            GameLogic.EnergyExpended += EnergyCost;
            Pawn.LastMovementDirection = EndingMovementDirection;
        }

        public void Undo()
        {
            Room fromRoom = GameLogic.RoomManager.GetRoomByType(FromRoom);
            Pawn.MoveTo(fromRoom);
            Pawn.LastMovementDirection = StartingMovementDirection;
            GameLogic.EnergyExpended -= EnergyCost;
        }
    }
}
