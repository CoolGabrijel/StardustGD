namespace Stardust.Actions
{
	public class DropLander : IUndoableAction
	{
        public void Do()
        {
            Room airlock = GameLogic.RoomManager.GetRoomByType(RoomType.Airlock);
            Lander lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander) as Lander;
            Room[] marsTiles = new Room[]
            {
                GameLogic.RoomManager.GetRoomByType(RoomType.Gully),
                GameLogic.RoomManager.GetRoomByType(RoomType.Peak),
                GameLogic.RoomManager.GetRoomByType(RoomType.Crater),
                GameLogic.RoomManager.GetRoomByType(RoomType.Plains),
                GameLogic.RoomManager.GetRoomByType(RoomType.Ridge),
            };

            airlock.Neighbours.Remove((Direction.South, lander));
            lander.Neighbours.Remove((Direction.North, airlock));

            foreach (Room marsTile in marsTiles)
            {
                marsTile.Neighbours.Add((Direction.None, lander));
                lander.Neighbours.Add((Direction.None, marsTile));
            }

            lander.Land();
            GameLogic.EnergyExpended++;
        }

        public void Undo()
        {
            Room airlock = GameLogic.RoomManager.GetRoomByType(RoomType.Airlock);
            Lander lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander) as Lander;
            Room[] marsTiles = new Room[]
            {
                GameLogic.RoomManager.GetRoomByType(RoomType.Gully),
                GameLogic.RoomManager.GetRoomByType(RoomType.Peak),
                GameLogic.RoomManager.GetRoomByType(RoomType.Crater),
                GameLogic.RoomManager.GetRoomByType(RoomType.Plains),
                GameLogic.RoomManager.GetRoomByType(RoomType.Ridge),
            };

            airlock.Neighbours.Add((Direction.South, lander));
            lander.Neighbours.Add((Direction.North, airlock));

            foreach (Room marsTile in marsTiles)
            {
                marsTile.Neighbours.Remove((Direction.None, lander));
                lander.Neighbours.Remove((Direction.None, marsTile));
            }

            lander.Release();
            GameLogic.EnergyExpended--;
        }
    } 
}
