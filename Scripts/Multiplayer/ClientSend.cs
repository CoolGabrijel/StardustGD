using Godot;
using PlayerIOClient;

namespace Stardust.Godot
{
    public static partial class ClientSend
    {
        public static void ReqCharChange(string newChar)
        {
            Message msg = Message.Create("ReqCharChange");
            
            msg.Add(newChar);
            
            PIOMP.Room.Connection.Send(msg);
        }

        public static void SetReady(bool ready)
        {
            Message msg = Message.Create("SetReady");
            
            msg.Add(ready);
            
            PIOMP.Room.Connection.Send(msg);
        }
        
        public static void ReqUndo()
        {
            Message msg = Message.Create("ReqUndo");
            
            PIOMP.Room.Connection.Send(msg);
        }
        
        public static void ReqEndTurn()
        {
            Message msg = Message.Create("ReqEndTurn");
            
            PIOMP.Room.Connection.Send(msg);
        }

        public static void ReqMove(PawnType pawn, int cost, RoomType fromRoom, RoomType toRoom, Direction movDir)
        {
            Message msg = Message.Create("ReqMove");
            
            msg.Add((int)pawn);
            msg.Add(cost);
            msg.Add((int)toRoom);
            msg.Add((int)fromRoom);
            msg.Add((int)movDir);
            
            PIOMP.Room.Connection.Send(msg);
        }
        
        public static void ReqActivateRoom()
        {
            Message msg = Message.Create("ReqActivateRoom");
            
            PIOMP.Room.Connection.Send(msg);
        }
    }
}
