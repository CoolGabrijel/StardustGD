using Godot;
using PIOMP;
using PlayerIOClient;

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
    } 
}
