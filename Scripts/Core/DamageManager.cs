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
        
        public Room PreviouslyDamagedRoom
        {
            get
            {
                if (roomsDamaged.Count <= 0) return null;

                return roomManager.GetRoomByType(roomsDamaged[^1]);
            }
        }

        public int TurnIndex { get; set; }

        private RoomManager roomManager;
        private List<RoomType> roomsDamaged = new();
        readonly static Random rng = new();

        public void Damage()
        {
            Room room = GetRoomToDamage();

            if (TurnIndex >= roomsDamaged.Count)
            {
                roomsDamaged.Add(room.RoomType);
            }

            if (!IsAuroraInRoom(room))
            {
                room.Damage();
            }
            else
            {
                TryGetAuroraInRoom(room).DamageBlocked();
            }
        }

        public void Damage(Room room)
        {
            if (TurnIndex >= roomsDamaged.Count)
            {
                roomsDamaged.Add(room.RoomType);
            }

            if (!IsAuroraInRoom(room))
            {
                room.Damage();
            }
            else
            {
                TryGetAuroraInRoom(room).DamageBlocked();
            }
        }

        public void UndoDamage()
        {
            Room room = roomManager.GetRoomByType(roomsDamaged[TurnIndex]);

            if (!IsAuroraInRoom(room))
                room.DamageAmount--;
            else
                TryGetAuroraInRoom(room).DamageBlocked();
        }

        public Room GetRoomToDamage()
        {
            if (TurnIndex < roomsDamaged.Count)
            {
                return roomManager.GetRoomByType(roomsDamaged[TurnIndex]);
            }

            List<Room> rooms = new();

            foreach (Room room in roomManager.Rooms)
            {
                if (room is not MarsTile)
                {
                    rooms.Add(room);
                }
            }

            if (PreviouslyDamagedRoom != null) rooms.Remove(PreviouslyDamagedRoom);

            int randRoomIndex = rng.Next(rooms.Count);
            Room randRoom = rooms[randRoomIndex];

            return randRoom;
        }

        private bool IsAuroraInRoom(Room room)
        {
            return TryGetAuroraInRoom(room) != null;
        }

        private Pawn TryGetAuroraInRoom(Room room)
        {
            foreach (Pawn pawn in room.Pawns)
            {
                if (pawn.Type == PawnType.Aurora) return pawn;
            }

            return null;
        }
    } 
}
