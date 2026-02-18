using Godot;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stardust
{
	public static class FileManager
	{
		private static readonly JsonSerializerOptions jsonOptions = new()
		{
			WriteIndented = true,
			Converters = { new JsonStringEnumConverter() }
		};
		
		public static void SaveStation(Room[] rooms)
		{
			DirAccess userDir = DirAccess.Open("user://");
			if (!userDir.DirExists("Stations"))
			{
				userDir.MakeDir("Stations");
			}
			
			using FileAccess file = FileAccess.Open("user://Stations/test.json", FileAccess.ModeFlags.Write);
			List<RoomType> roomTypes = GetProceduralRooms(rooms);
			file.StoreString(JsonSerializer.Serialize(roomTypes, jsonOptions));
		}

		/// <summary>
		/// Filters out static rooms and returns only the rooms that may change game to game.
		/// Used to reconstruct the same station via file storage or networking.
		/// </summary>
		/// <param name="rooms"></param>
		/// <returns></returns>
		public static List<RoomType> GetProceduralRooms(Room[] rooms)
        {
            List<RoomType> roomTypes = new();
            foreach (Room room in rooms)
            {
                // We only want these, because the rest are dependent on them so having them is redundant.
                if (room.RoomType is RoomType.Airlock or RoomType.Comms or RoomType.Habitation or RoomType.AirlockExpanded or RoomType.LifeSupport or RoomType.Workshop)
                    roomTypes.Add(room.RoomType);
            }
			return roomTypes;
        }
    }
}
