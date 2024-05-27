using System;
using System.Collections.Generic;

namespace Stardust
{
    /// <summary>
    /// <para>
    /// This class holds methods that manipulate the Rooms given to them.
    /// </para>
    /// <see cref="GenerateBaseRooms(Room[])"/> Generates Rooms for the base game.
    /// </summary>
    public static class RoomGenerators
    {
        public static Room[] GenerateBaseRooms(Room[] rooms)
        {
            List<Room> staticRooms = new();
            List<Room> procRooms = new()
            {
                new LifeSupport(),
                new Airlock(),
                new Comms(),
                new Workshop(),
                new Habitation()
            };
            Random rand = new();

            // Engines always go first
            Room prevRoom = new Engines();
            staticRooms.Add(prevRoom);

            // Then the procedural stuff
            while (procRooms.Count > 0)
            {
                int randNum = rand.Next(0, procRooms.Count);
                Room room = procRooms[randNum];
                procRooms.Remove(room);
                staticRooms.Add(room);

                // Connect it to the previous room and make it the previous room
                prevRoom.Neighbours.Add((Direction.East, room));
                room.Neighbours.Add((Direction.West, prevRoom));
                prevRoom = room;
            }

            // Find the airlock and give it solar panels
            foreach (Room room in staticRooms)
            {
                if (room.RoomType == RoomType.Airlock)
                {
                    Room solar = new SolarPanels();
                    room.Neighbours.Add((Direction.North, solar));
                    solar.Neighbours.Add((Direction.South, room));
                    staticRooms.Add(solar);
                    break;
                }
            }

            // Finally, add the Cockpit, and give it the neighbours
            Room cockpit = new Cockpit();
            staticRooms.Add(cockpit);
            cockpit.Neighbours.Add((Direction.West, prevRoom));
            prevRoom.Neighbours.Add((Direction.East, cockpit));

            return staticRooms.ToArray();
        }
    }
}
