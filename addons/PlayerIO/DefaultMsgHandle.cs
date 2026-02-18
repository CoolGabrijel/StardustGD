using Godot;
using PlayerIOClient;
using System.Collections.Generic;

namespace PIOMP
{
    internal static class DefaultMsgHandle
    {
        [MessageHandler("Welcome")]
        public static void RoomInfo(Message _msg)
        {
            uint msgIndex = 0;

            int id = _msg.GetInt(msgIndex); // This is our Id
            msgIndex++;

            Room.SetPlayerId(id);

            // Then we get the room data
            int dataCount = _msg.GetInt(msgIndex);
            msgIndex++;

            Dictionary<string, string> roomData = new Dictionary<string, string>();

            for (int i = 0; i < dataCount; i++)
            {
                string key = _msg.GetString(msgIndex);
                msgIndex++;
                string data = _msg.GetString(msgIndex);
                msgIndex++;

                roomData.Add(key, data);
            }

            Room.SetRoomData(roomData);
            GD.Print("PIOMP :: Data set");

            // Then get the id of others and ourselves along with usernames and join data
            int playerCount = _msg.GetInt(msgIndex);
            msgIndex++;

            for (uint i = 0; i < playerCount; i++)
            {
                string username = _msg.GetString(msgIndex); // Username
                msgIndex++;
                int playerId = _msg.GetInt(msgIndex); // Id
                msgIndex++;

                RoomClient client = new RoomClient(playerId, username);

                int joinDataCount = _msg.GetInt(msgIndex);
                msgIndex++;

                for (int i2 = 0; i2 < joinDataCount; i2++) // Join Data
                {
                    string dataKey = _msg.GetString(msgIndex);
                    msgIndex++;
                    string data = _msg.GetString(msgIndex);
                    msgIndex++;

                    client.AddData(dataKey, data);
                }

                Room.AddClient(client);
            }
        }

        [MessageHandler("NewPlayer")]
        public static void OnNewPlayerJoined(Message _msg)
        {
            uint msgIndex = 0;

            string username = _msg.GetString(msgIndex); // Username
            msgIndex++;
            int playerId = _msg.GetInt(msgIndex); // Id
            msgIndex++;

            RoomClient client = new RoomClient(playerId, username);

            int joinDataCount = _msg.GetInt(msgIndex);
            msgIndex++;

            for (int i = 0; i < joinDataCount; i++) // Join Data
            {
                string dataKey = _msg.GetString(msgIndex);
                msgIndex++;
                string data = _msg.GetString(msgIndex);
                msgIndex++;

                client.AddData(dataKey, data);
            }

            Room.AddClient(client);
        }

        [MessageHandler("UserLeft")]
        static void UserLeft(Message _msg)
        {
            uint msgIndex = 0;

            int playerId = _msg.GetInt(msgIndex); // Id
            msgIndex++;

            Room.RemoveClient(playerId);
        }

        [MessageHandler("Jump")]
        public static void OnJump(Message _msg)
        {
            GD.Print("PIOMP :: Got Jump Message");
            GD.Print(_msg);
        }
    }
}