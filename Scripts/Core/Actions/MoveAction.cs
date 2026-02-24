namespace Stardust.Actions
{
    public class MoveAction : IUndoableAction
    {
        public MoveAction(PawnType pawn, int cost, RoomType from, RoomType to, Direction startingMovementDirection)
        {
            Pawn = pawn;
            EnergyCost = cost;
            FromRoom = from;
            ToRoom = to;
            StartingMovementDirection = startingMovementDirection;
        }

        public PawnType Pawn { get; private set; }
        public int EnergyCost { get; private set; }
        public RoomType FromRoom { get; private set; }
        public RoomType ToRoom { get; private set; }
        public Direction StartingMovementDirection { get; private set; }

        public void Do()
        {
            Pawn pawn = GameLogic.GetPawnByType(Pawn);
            Room toRoom = GameLogic.RoomManager.GetRoomByType(ToRoom);
            pawn.MoveTo(toRoom);
            GameLogic.EnergyExpended += EnergyCost;
        }

        public void Undo()
        {
            Pawn pawn = GameLogic.GetPawnByType(Pawn);
            Room fromRoom = GameLogic.RoomManager.GetRoomByType(FromRoom);
            pawn.MoveTo(fromRoom);
            //Pawn.LastMovementDirection = StartingMovementDirection;
            GameLogic.EnergyExpended -= EnergyCost;
        }
    }
}
