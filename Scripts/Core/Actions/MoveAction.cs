using Godot;
using System;

namespace Stardust.Actions
{
    public class MoveAction : IUndoableAction
    {
        public MoveAction(Pawn pawn, int cost, Room from, Room to, Direction startingMovementDirection)
        {
            Pawn = pawn;
            EnergyCost = cost;
            FromRoom = from;
            ToRoom = to;
            StartingMovementDirection = startingMovementDirection;
        }

        public Pawn Pawn { get; private set; }
        public int EnergyCost { get; private set; }
        public Room FromRoom { get; private set; }
        public Room ToRoom { get; private set; }
        public Direction StartingMovementDirection { get; private set; }

        public void Do()
        {
            Pawn.MoveTo(ToRoom);
            GameLogic.EnergyExpended += EnergyCost;
        }

        public void Undo()
        {
            Pawn.MoveTo(FromRoom);
            Pawn.LastMovementDirection = StartingMovementDirection;
            GameLogic.EnergyExpended -= EnergyCost;
        }
    }
}
