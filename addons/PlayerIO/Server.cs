using PlayerIOClient;

namespace PIOMP
{
    public static class Server
    {
        public static void BroadcastExcept(Message _msg, int _id)
        {
            Message broadcastMsg = WrapBroadcastExceptMsg(_msg, _id);

            Room.Connection.Send(broadcastMsg);
        }

        public static void Broadcast(Message _msg, bool _includeServer = false)
        {
            Message broadcastMsg = WrapIntoBroadcastMessage(_msg, _includeServer);

            Room.Connection.Send(broadcastMsg);
        }

        public static void ChangeJoinData(int _playerId, string _dataToChange, string _newData)
        {
            Message msg = Message.Create("changeJoinData");

            msg.Add(_playerId);
            msg.Add(_dataToChange);
            msg.Add(_newData);

            Room.Connection.Send(msg);
        }

        private static Message WrapIntoBroadcastMessage(Message _msg, bool _includeServer)
        {
            Message msg;

            if (_includeServer) msg = Message.Create("broadcastserver");
            else msg = Message.Create("broadcast");

            msg.Add(_msg.Type);

            for (uint i = 0; i < _msg.Count; i++)
            {
                msg.Add(_msg[i]);
            }

            return msg;
        }

        static Message WrapBroadcastExceptMsg(Message _msg, int _id)
        {
            Message msg = Message.Create("broadcastexcept");

            msg.Add(_id);

            msg.Add(_msg.Type);

            for (uint i = 0; i < _msg.Count; i++)
            {
                msg.Add(_msg[i]);
            }

            return msg;
        }
    }
}