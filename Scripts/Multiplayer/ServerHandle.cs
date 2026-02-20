using Godot;
using PIOMP;
using PlayerIOClient;
using Stardust.Actions;

namespace Stardust.Godot
{
	public static partial class ServerHandle
    {
        [MessageHandler("ReqCharChange")]
        public static void ReceiveChangeChar(Message _msg)
        {
            int id = _msg.GetInt(0);
            string newChar = _msg.GetString(1);

            UI.LobbyScreen.Lobby.ChangePlayer(id, newChar);

            ServerSend.ChangeChar(id, newChar);
        }

        [MessageHandler("SetReady")]
        public static void GetReady(Message _msg)
        {
            int id = _msg.GetInt(0);
            bool ready = _msg.GetBoolean(1);

            UI.LobbyScreen.Lobby.Ready(id, ready);

            ServerSend.SetReady(id, ready);
        }
        
        [MessageHandler("ReqEndTurn")]
        public static void HandleEndTurn(Message _msg)
        {
            int id = _msg.GetInt(0);

            UI.TurnButtons.NextTurn();

            Room damagedRoom = GameLogic.DamageManager.PreviouslyDamagedRoom;

            ServerSend.EndTurn(id, damagedRoom.RoomType);
        }
        
        [MessageHandler("ReqUndo")]
        public static void HandleUndo(Message _msg)
        {
            int id = _msg.GetInt(0);

            UI.TurnButtons.Undo();

            ServerSend.Undo(id);
        }
        
        [MessageHandler("ReqMove")]
        public static void ReceiveMove(Message _msg)
        {
            uint msgIndex = 0;
            int id = _msg.GetInt(msgIndex++);
            
            PawnType pawnType = (PawnType)_msg.GetInt(msgIndex++);
            int cost =  _msg.GetInt(msgIndex++);
            RoomType toType = (RoomType)_msg.GetInt(msgIndex++);
            RoomType fromType = (RoomType)_msg.GetInt(msgIndex++);
            Direction movDir = (Direction)_msg.GetInt(msgIndex);

            Pawn pawn = null;
            foreach (Pawn turnQueuePawn in GameLogic.TurnQueue.Pawns)
            {
                if (pawnType == turnQueuePawn.Type)
                {
                    pawn = turnQueuePawn;
                    break;
                }
            }

            Room toRoom = GameLogic.RoomManager.GetRoomByType(toType);
            Room fromRoom = GameLogic.RoomManager.GetRoomByType(fromType);
            
            IUndoableAction action = new MoveAction(pawn, cost, fromRoom, toRoom, movDir);
            
            action.Do();
            ActionLibrary.AddAction(action);
            
            ServerSend.Move(id, pawnType, cost, fromType, toType, movDir);
        }
    } 
}
