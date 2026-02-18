using Godot;
using PIOMP;
using PlayerIOClient;
using System.Collections.Generic;

namespace Stardust.Godot
{
    public static partial class ClientHandle
    {
        [MessageHandler("ChangeChar")]
        public static void ReceiveCharChange(Message _msg)
        {
            int id = _msg.GetInt(0);
            string charName = _msg.GetString(1);

            UI.LobbyScreen.Lobby.ChangePlayer(id, charName);
        }


        [MessageHandler("SendReady")]
        public static void GetReady(Message _msg)
        {
            int id = _msg.GetInt(0);
            bool ready = _msg.GetBoolean(1);

            UI.LobbyScreen.Lobby.Ready(id, ready);
        }

        [MessageHandler("StartGame")]
        public static void StartGame(Message _msg)
        {
            uint msgIndex = 0;
            int pawnCount = _msg.GetInt(msgIndex++);

            PawnType[] pawns = new PawnType[pawnCount];

            for (int i = 0; i < pawnCount; i++)
            {
                pawns[i] = (PawnType)_msg.GetInt(msgIndex++);
            }

            GameStart.PawnsToSpawn = pawns;

            UI.MainMenuScreen.Instance.GetTree().ChangeSceneToFile("res://Scenes/Game2D.tscn");
        }
    }
}
