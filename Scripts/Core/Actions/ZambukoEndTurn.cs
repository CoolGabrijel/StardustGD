using Godot;
using System;

namespace Stardust.Actions
{
    public class ZambukoEndTurn : IUndoableAction
    {
        public EndTurn EndTurn { get; private set; }
        IUndoableAction roomAction;

        public void Do()
        {
            Pawn zambuko = null;
            foreach (Pawn pawn in GameLogic.TurnQueue.Pawns)
            {
                if (pawn.Type == PawnType.Zambuko)
                {
                    zambuko = pawn;
                    break;
                }
            }

            if (zambuko == null)
                throw new Exception("ZambukoEndTurn :: Zambuko not found.");

            if (zambuko.Room.CanBeActivated)
            {
                switch (zambuko.Room.RoomType)
                {
                    case RoomType.Workshop:
                        roomAction = new CreatePart(zambuko, 1, zambuko.Room, new(ItemType.Part));
                        break;
                    case RoomType.Habitation:
                        roomAction = new Sleep(zambuko);
                        break;
                    case RoomType.Comms:
                        roomAction = new RevealObjective(zambuko);
                        break;
                    case RoomType.Airlock:
                        roomAction = new DropLander();
                        break;
                    case RoomType.Lander:
                        roomAction = new LaunchLander();
                        break;
                    default:
                        break;
                }
            }

            roomAction?.Do();

            if (roomAction is not Sleep) // Sleep already automatically ends turn.
            {
                if (EndTurn == null) EndTurn = new EndTurn();

                GameLogic.EnergyExpended--; // We don't want the action to use any energy.
                EndTurn.Do();
            }
        }

        public void Undo()
        {
            roomAction?.Undo();
            EndTurn?.Undo();
        }
    }
}
