using Godot;
using PlayerIOClient;
using System.Collections.Generic;
using System.Linq;

namespace PIOMP
{
    public static class Room
    {
        public static event System.Action OnRoomJoined;
        public static event System.Action<RoomClient> OnNewClientJoined;
        public static event System.Action<RoomClient> OnClientLeft;

        public static Connection Connection { get; private set; }
        public static Dictionary<string, string> RoomData { get; private set; } = new Dictionary<string, string>();

        public static int PlayerId { get; private set; }

        public static bool IsHost { get; private set; }

        public static bool IsInRoom
        {
            get
            {
                return Connection != null && Connection.Connected;
            }
        }

        public static RoomClient[] Clients
        {
            get
            {
                return clients.Values.ToArray();
            }
        }

        static Dictionary<int, RoomClient> clients = new();

        public static void SetPlayerId(int _id)
        {
            PlayerId = _id;
        }

        public static void SetRoomData(Dictionary<string, string> _roomData)
        {
            RoomData = _roomData;
        }

        public static void AddClient(RoomClient _client)
        {
            clients.Add(_client.Id, _client);
            OnNewClientJoined?.Invoke(_client);
        }

        public static void RemoveClient(int _id)
        {
            RoomClient client = clients[_id];
            clients.Remove(_id);
            OnClientLeft?.Invoke(client);
        }

        public static void CreateRoom(Dictionary<string, string> _roomData = null, Dictionary<string, string> _joinData = null)
        {
            MultiplayerUtils.Client.Multiplayer.CreateJoinRoom(
                null,
                "Relay",
                true,
                _roomData,
                _joinData,
                OnJoinRoom,
                OnFailToCreateRoom
                );

            IsHost = true;
            RoomData = _roomData;

            GD.Print("PIOMP :: Room Created");
        }

        public static void JoinRoom(string _roomId, Dictionary<string, string> _joinData = null)
        {
            MultiplayerUtils.Client.Multiplayer.JoinRoom(_roomId, _joinData, OnJoinRoom, OnFailToJoinRoom);

            IsHost = false;
        }

        public static void LeaveRoom()
        {
            IsHost = false;
            Connection?.Disconnect();
            Clear();
        }

        static void Clear()
        {
            clients.Clear();
            RoomData.Clear();
        }

        static void OnJoinRoom(Connection _connection)
        {
            Connection = _connection;

            Connection.OnMessage += MessageListener.OnMessage;
            Connection.OnDisconnect += OnConnectionDisconnect;

            OnRoomJoined?.Invoke();

            GD.Print("PIOMP :: Room Joined");
        }

        static void OnConnectionDisconnect(object _sender, string _msg)
        {
            IsHost = false;
            Connection.OnMessage -= MessageListener.OnMessage;
            Connection.OnDisconnect -= OnConnectionDisconnect;

            Connection = null;

            GD.Print("PIOMP :: Room Left");
            GD.Print(_msg);
        }

        static void OnFailToCreateRoom(PlayerIOError _error)
        {
            GD.PrintErr("PIOMP :: Failed to Create Room");
            GD.PrintErr(_error);
        }

        static void OnFailToJoinRoom(PlayerIOError _error)
        {
            GD.PrintErr("PIOMP :: Failed to Join Room");
            GD.PrintErr(_error);
        }
    }
}