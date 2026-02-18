using PlayerIOClient;
using System.Reflection;
using Godot;

namespace PIOMP
{
    public static class MultiplayerUtils
    {
        public static Client Client { get; set; }

        public static bool HasAuthenticated
        {
            get
            {
                return Client != null;
            }
        }

        public static void Tick() => MessageListener.Tick();

        public static void CreateMessageHandlers(Assembly _assembly)
        {
            MessageListener.CreateMessageHandlers(_assembly);
        }

        public static void Disconnect()
        {
            Room.LeaveRoom();
            Client?.Logout();
        }
    }
}