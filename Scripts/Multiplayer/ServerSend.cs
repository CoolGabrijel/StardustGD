using Godot;
using PlayerIOClient;
using System.Collections.Generic;
using Stardust.Actions;

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

            // Send all the Pawns (some may be procedural if players chose random)

            msg.Add(GameStart.PawnsToSpawn.Length);

            foreach (KeyValuePair<int, PawnType> player in GameStart.PlayerList)
            {
                msg.Add(player.Key);
                msg.Add((int)player.Value);
            }

            // Send the Rooms

            RoomType[] rooms = FileManager.GetProceduralRooms(GameLogic.RoomManager.Rooms).ToArray();
            msg.Add(rooms.Length);

            foreach (RoomType roomType in rooms)
            {
                msg.Add((int)roomType);
            }

            // Send the Objectives

            List<string> objectives = new();
            foreach (Objective objective in ObjectiveHandler.Objectives)
            {
                objectives.Add(objective.Name);
            }

            msg.Add(objectives.Count);
            foreach (string objective in objectives)
            {
                msg.Add(objective);
            }

            PIOMP.Server.Broadcast(msg);
        }

        public static void EndTurn(int id, RoomType damagedRoom)
        {
            Message msg = Message.Create("EndTurn");
            
            msg.Add((int)damagedRoom);
            
            PIOMP.Server.Broadcast(msg);
        }

        public static void Undo(int id)
        {
            Message msg = Message.Create("Undo");
            
            PIOMP.Server.BroadcastExcept(msg, id);
        }

        public static void Move(int id, PawnType pawn, int cost, RoomType fromRoom, RoomType toRoom, Direction movDir)
        {
            Message msg = Message.Create("Move");
            
            msg.Add((int)pawn);
            msg.Add(cost);
            msg.Add((int)toRoom);
            msg.Add((int)fromRoom);
            msg.Add((int)movDir);
            
            PIOMP.Server.BroadcastExcept(msg, id);
        }

        public static void ActivateRoom(int id, RoomType damagedRoom)
        {
            Message msg = Message.Create("ActivateRoom");
            
            msg.Add(id);
            msg.Add((int)damagedRoom);
            
            PIOMP.Server.BroadcastExcept(msg, id);
        }

        public static void Pickup(int id, ItemType itemType)
        {
            Message msg = Message.Create("Pickup");
            
            msg.Add(id);
            msg.Add((int)itemType);
            
            PIOMP.Server.BroadcastExcept(msg, id);
        }

        public static void Drop(int id, ItemType itemType)
        {
            Message msg = Message.Create("Drop");
            
            msg.Add(id);
            msg.Add((int)itemType);
            
            PIOMP.Server.BroadcastExcept(msg, id);
        }

        public static void Repair(int id)
        {
            Message msg = Message.Create("Repair");
            
            msg.Add(id);
            
            PIOMP.Server.BroadcastExcept(msg, id);
        }

        public static void CompleteTask(int id)
        {
            Message msg = Message.Create("CompleteTask");
            
            msg.Add(id);
                
            PIOMP.Server.BroadcastExcept(msg, id);
        }
    } 
}
