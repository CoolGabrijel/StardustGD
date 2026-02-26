using Godot;
using System;

namespace Stardust.Actions
{
    public class ZambukoEndTurn : IUndoableAction
    {
        public RoomType DamagedRoom { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public EndTurn EndTurn { get; private set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool DamagedRoomSet { get; set; }
        
        private IUndoableAction roomAction;

        public void Do()
        {
            Pawn zambuko = GameLogic.GetPawnByType(PawnType.Zambuko);

            if (zambuko == null)
                throw new Exception("ZambukoEndTurn :: Zambuko not found.");

            if (zambuko.Room.CanBeActivated)
            {
                switch (zambuko.Room.RoomType)
                {
                    case RoomType.Workshop:
                        roomAction = new CreatePart(zambuko.Type, 1, zambuko.Room.RoomType, ItemType.Part);
                        break;
                    case RoomType.Habitation:
                        Sleep sleep = new(zambuko);
                        sleep.DamagedRoom = DamagedRoom;
                        sleep.DamagedRoomSet = DamagedRoomSet;
                        roomAction = sleep;
                        break;
                    case RoomType.Comms:
                        roomAction = new RevealObjective();
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
                if (DamagedRoomSet)
                {
                    EndTurn.DamagedRoom = DamagedRoom;
                    EndTurn.DamagedRoomSet = DamagedRoomSet;
                }

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
