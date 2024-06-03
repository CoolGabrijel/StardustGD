using Godot;

namespace Stardust.Actions
{
    public class ConcordeMove : IUndoableAction
    {
        public ConcordeMove(Pawn pawn, int cost, Room to, Direction startingMovementDirection, Direction endingMovementDirection)
        {
            Pawn = pawn;
            EnergyCost = cost;
            ToRoom = to;
            StartingMovementDirection = startingMovementDirection;
            EndingMovementDirection = endingMovementDirection;
        }

        public Pawn Pawn { get; private set; }
        public int EnergyCost { get; private set; }
        public Room FromRoom { get; private set; }
        public Room ToRoom { get; private set; }
        public Direction StartingMovementDirection { get; private set; }
        public Direction EndingMovementDirection { get; private set; }

        public void Do()
        {
            if (FromRoom == null) FromRoom = Pawn.Room;

            Pawn.MoveTo(ToRoom);
            GameLogic.EnergyExpended += EnergyCost;
            Pawn.LastMovementDirection = EndingMovementDirection;
        }

        public void Undo()
        {
            Pawn.MoveTo(FromRoom);
            Pawn.LastMovementDirection = StartingMovementDirection;
            GameLogic.EnergyExpended -= EnergyCost;
        }
    }
}
