namespace Stardust.Actions
{
    public class LaunchLander : IUndoableAction
    {
        Room airlock = GameLogic.RoomManager.GetRoomByType(RoomType.Airlock);
        Room lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander);
        Room[] marsTiles = new Room[]
        {
                GameLogic.RoomManager.GetRoomByType(RoomType.Gully),
                GameLogic.RoomManager.GetRoomByType(RoomType.Peak),
                GameLogic.RoomManager.GetRoomByType(RoomType.Crater),
                GameLogic.RoomManager.GetRoomByType(RoomType.Plains),
                GameLogic.RoomManager.GetRoomByType(RoomType.Ridge),
        };

        public void Do()
        {
            airlock.Neighbours.Add((Direction.South, lander));
            lander.Neighbours.Add((Direction.North, airlock));

            foreach (Room marsTile in marsTiles)
            {
                marsTile.Neighbours.Remove((Direction.None, lander));
                lander.Neighbours.Remove((Direction.None, marsTile));
            }
        }

        public void Undo()
        {
            airlock.Neighbours.Remove((Direction.South, lander));
            lander.Neighbours.Remove((Direction.North, airlock));

            foreach (Room marsTile in marsTiles)
            {
                marsTile.Neighbours.Add((Direction.None, lander));
                lander.Neighbours.Add((Direction.None, marsTile));
            }
        }
    }
}
