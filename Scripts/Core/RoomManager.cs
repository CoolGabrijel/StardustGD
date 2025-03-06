using System.Collections.Generic;

namespace Stardust
{
    public class RoomManager
    {
        public delegate Room[] RoomGenerator(Room[] rooms);

        public Room[] Rooms;

        public void GenerateRooms()
        {
            List<RoomGenerator> generators = new();
            generators.Add(RoomGenerators.GenerateBaseRooms);

            if (StardustGameConfig.CurrentConfig.FirstStepsEnabled) generators.Add(RoomGenerators.GenerateFirstStepsRooms);

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
