using Godot;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Stardust
{
	public static class FileManager
	{
		private static readonly JsonSerializerSettings jsonSettings = new()
		{
			Formatting = Formatting.Indented,
			Converters = new List<JsonConverter>()
			{
				new StringEnumConverter()
			},
			TypeNameHandling = TypeNameHandling.Auto
		};

		public static void SaveReplay(Replay replay)
		{
			DirAccess userDir = DirAccess.Open("user://");
			if (!userDir.DirExists("Replays"))
			{
				userDir.MakeDir("Replays");
			}
			
			string serializedReplay = JsonConvert.SerializeObject(replay, jsonSettings);
			string fileName = Time.GetDatetimeStringFromSystem() + ".json";
			fileName = fileName.Replace(':', '-');
			
			using FileAccess file = FileAccess.Open($"user://Replays/{fileName}", FileAccess.ModeFlags.Write);
			file.StoreString(serializedReplay);
		}
		
		public static void SaveStation(Room[] rooms)
		{
			DirAccess userDir = DirAccess.Open("user://");
			if (!userDir.DirExists("Stations"))
			{
				userDir.MakeDir("Stations");
			}
			
			using FileAccess file = FileAccess.Open("user://Stations/test.json", FileAccess.ModeFlags.Write);
			List<RoomType> roomTypes = GetProceduralRooms(rooms);
			file.StoreString(JsonConvert.SerializeObject(roomTypes, jsonSettings));
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
