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

        public static Room[] GenerateBaseRooms(RoomType[] roomTypes)
        {
            List<Room> staticRooms = new();
            List<Room> procRooms = new();

            foreach (RoomType roomType in roomTypes)
            {
                Room room = roomType switch
                {
                    RoomType.Workshop => new Workshop(),
                    RoomType.Habitation => new Habitation(),
                    RoomType.LifeSupport => new LifeSupport(),
                    RoomType.Airlock => new Airlock(),
                    RoomType.Comms => new Comms(),
                    _ => throw new Exception(
                        $"Room type {roomType} not supported in RoomGenerators::GenerateBaseRooms(RoomType)")
                };

                procRooms.Add(room);
            }

            // Engines always go first
            Room prevRoom = new Engines();
            staticRooms.Add(prevRoom);

            // Then the rooms between Engine and Cockpit
            foreach (Room room in procRooms)
            {
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

        public static Room[] GenerateFirstStepsRooms(Room[] rooms)
        {
            AirlockExpanded newAirlock = new();

            // Find the base airlock and replace it
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].RoomType == RoomType.Airlock)
                {
                    newAirlock.Neighbours = rooms[i].Neighbours;
                    rooms[i] = newAirlock;

                    for (int j = 0; j < rooms.Length; j++)
                    {
                        if (rooms[j].RoomType == RoomType.SolarPanels)
                        {
                            Room solar = rooms[j];
                            //newAirlock.Neighbours.Add((Direction.North, solar));
                            solar.Neighbours.Clear();
                            solar.Neighbours.Add((Direction.South, newAirlock));
                            break;
                        }
                    }
                    //break;
                }
                else
                {
                    // We gotta find rooms neighbouring airlock and replace their neighbours.
                    for (int j = 0; j < rooms[i].Neighbours.Count; j++)
                    {
                        var neighbour = rooms[i].Neighbours[j];
                        if (neighbour.Item2.RoomType == RoomType.Airlock)
                        {
                            rooms[i].Neighbours.Remove(neighbour);
                            rooms[i].Neighbours.Add((neighbour.Item1, newAirlock));
                        }
                    }
                }
            }

            List<Room> newRooms = new(rooms);

            Lander lander = new();
            newAirlock.Neighbours.Add((Direction.South, lander));
            lander.Neighbours.Add((Direction.North, newAirlock));
            newRooms.Add(lander);

            MarsTile gully = new(RoomType.Gully);
            MarsTile peak = new(RoomType.Peak);
            MarsTile crater = new(RoomType.Crater);
            MarsTile plains = new(RoomType.Plains);
            MarsTile ridge = new(RoomType.Ridge);

            gully.Neighbours.Add((Direction.South, peak));
            peak.Neighbours.Add((Direction.North, gully));
            peak.Neighbours.Add((Direction.East, ridge));
            ridge.Neighbours.Add((Direction.West, peak));
            ridge.Neighbours.Add((Direction.East, plains));
            plains.Neighbours.Add((Direction.West, ridge));
            plains.Neighbours.Add((Direction.North, crater));
            crater.Neighbours.Add((Direction.South, plains));

            newRooms.Add(gully);
            newRooms.Add(peak);
            newRooms.Add(crater);
            newRooms.Add(plains);
            newRooms.Add(ridge);

            return newRooms.ToArray();
        }
    }
}
