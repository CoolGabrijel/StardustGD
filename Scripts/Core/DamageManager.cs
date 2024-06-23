using System;
using System.Collections.Generic;

namespace Stardust
{
	public class DamageManager
	{
        public DamageManager(RoomManager roomManager)
        {
            this.roomManager = roomManager;
        }

        public int TurnIndex { get; set; }

        private RoomManager roomManager;
        private List<RoomType> roomsDamaged = new();
        readonly static Random rng = new();
        private Room previouslyDamagedRoom
        {
            get
            {
                if (roomsDamaged.Count <= 0) return null;

                return roomManager.GetRoomByType(roomsDamaged[^1]);
            }
        }

        public void Damage()
        {
            Room room = GetRoomToDamage();

            if (TurnIndex >= roomsDamaged.Count)
            {
                roomsDamaged.Add(room.RoomType);
            }

            room.Damage();
        }

        public void UndoDamage()
        {
            roomManager.GetRoomByType(roomsDamaged[TurnIndex]).DamageAmount--;
        }

        public Room GetRoomToDamage()
        {
            if (TurnIndex < roomsDamaged.Count)
            {
                return roomManager.GetRoomByType(roomsDamaged[TurnIndex]);
            }

            List<Room> rooms = new List<Room>(roomManager.Rooms);

            if (previouslyDamagedRoom != null) rooms.Remove(previouslyDamagedRoom);

            int randRoomIndex = rng.Next(rooms.Count);
            Room randRoom = rooms[randRoomIndex];

            return randRoom;
        }
    } 
}
