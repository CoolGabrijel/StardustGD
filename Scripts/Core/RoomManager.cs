namespace Stardust
{
    public class RoomManager
    {
        public delegate Room[] RoomGenerator(Room[] rooms);

        public Room[] Rooms;

        private RoomGenerator[] generators = new RoomGenerator[]
        {
                RoomGenerators.GenerateBaseRooms
        };

        public void GenerateRooms()
        {
            foreach (RoomGenerator gen in generators)
            {
                Rooms = gen(Rooms);
            }
        }

        public Room GetRoomByType(RoomType type)
        {
            foreach (Room room in Rooms)
            {
                if (room.RoomType == type) return room;
            }

            return null;
        }
    }
}
