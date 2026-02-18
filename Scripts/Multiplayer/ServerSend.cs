using Godot;
using PlayerIOClient;
using System.Numerics;

namespace Stardust.Godot
{
	public static partial class ServerSend
	{
		public static void ChangeChar(int id, string charName)
        {
            Message msg = Message.Create("ChangeChar");

            msg.Add(id);
            msg.Add(charName);

            foreach (PIOMP.RoomClient client in PIOMP.Room.Clients)
            {
                if (client.Id == id)
                {
                    client.RemoveData("Character");
                    client.AddData("Character", charName);
                }
            }

            PIOMP.Server.BroadcastExcept(msg, id);
            PIOMP.Server.ChangeJoinData(id, "Character", charName);
        }

        public static void SetReady(int id, bool ready)
        {
            Message msg = Message.Create("SendReady");

            msg.Add(id);
            msg.Add(ready);

            foreach (PIOMP.RoomClient client in PIOMP.Room.Clients)
            {
                if (client.Id == id)
                {
                    client.RemoveData("Ready");
                    client.AddData("Ready", ready.ToString());
                }
            }

            PIOMP.Server.BroadcastExcept(msg, id);
            PIOMP.Server.ChangeJoinData(id, "Ready", ready.ToString());
        }

        public static void StartGame()
        {
            Message msg = Message.Create("StartGame");

            msg.Add(GameStart.PawnsToSpawn.Length);

            foreach (PawnType pawn in GameStart.PawnsToSpawn)
            {
                msg.Add((int)pawn);
            }

            RoomType[] rooms = FileManager.GetProceduralRooms(GameLogic.RoomManager.Rooms).ToArray();
            msg.Add(rooms.Length);

            foreach (RoomType roomType in rooms)
            {
                msg.Add((int)roomType);
            }

            PIOMP.Server.Broadcast(msg);
        }
    } 
}
