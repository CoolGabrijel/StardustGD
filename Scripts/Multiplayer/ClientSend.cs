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
    }
}
