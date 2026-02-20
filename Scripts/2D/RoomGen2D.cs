using Godot;
using System;
using System.Collections.Generic;

namespace Stardust.Godot
{
	[Tool]
	public partial class RoomGen2D : Node2D
	{
		[Export] private PackedScene roomPrefab = ResourceLoader.Load<PackedScene>("res://Prefabs/2D/Room2D.tscn");
		[Export] private int sizeOffset = 1125;
		[Export] private string roomResourcePath = "res://Rooms/2D";
		[ExportCategory("Debug Tools")]
		[Export] private bool GenRooms
		{
			get { return false; }
			set { _Ready(); }
		}

		private RoomManager rm;
		private Dictionary<RoomType, RoomRes2D> rr2d;
		private Dictionary<RoomType, Node2D> rooms = new();

		public override void _Ready()
		{
			if (!Engine.IsEditorHint()) return;

			Generate(new());
		}

		public Node2D GetRoomNodeByType(RoomType type)
		{
			return rooms[type];
		}

		public void Generate(RoomManager rm)
		{
			this.rm = rm;
			//rm.GenerateRooms();
			RemoveChildren();
			GetRoomResources();
			GenerateSprites();
		}

		private void GenerateSprites()
		{
			GD.Print($"RoomGen2D :: Generating Sprites");

			int x = -3;
			Vector2 airlockPos = Vector2.Zero;

			foreach (Room room in rm.Rooms)
			{
				Node2D roomInstance = InstantiateRoomGraphics(room);
				rooms.Add(room.RoomType, roomInstance);
				roomInstance.GlobalPosition = new Vector2(x * sizeOffset, 0f);

				if (room.RoomType == RoomType.Airlock)
				{
					airlockPos = roomInstance.GlobalPosition;
				}

				if (room.RoomType == RoomType.SolarPanels)
				{
					roomInstance.GlobalPosition = new Vector2(airlockPos.X, -1f * sizeOffset);
					x--;
				}

				if (room.RoomType == RoomType.Lander)
                {
                    roomInstance.GlobalPosition = new Vector2(airlockPos.X, 1f * sizeOffset);
                }

				switch (room.RoomType)
				{
					case RoomType.Lander:
                        roomInstance.GlobalPosition = new Vector2(airlockPos.X, 1f * sizeOffset);
                        break;
					case RoomType.Ridge:
                        roomInstance.GlobalPosition = new Vector2(airlockPos.X, 3f * sizeOffset);
						GD.Print(airlockPos.X);
                        break;
					case RoomType.Plains:
                        roomInstance.GlobalPosition = new Vector2(airlockPos.X + sizeOffset, 3f * sizeOffset);
                        break;
					case RoomType.Peak:
                        roomInstance.GlobalPosition = new Vector2(airlockPos.X - sizeOffset, 3f * sizeOffset);
                        break;
					case RoomType.Gully:
                        roomInstance.GlobalPosition = new Vector2(airlockPos.X - sizeOffset, 2f * sizeOffset);
                        break;
					case RoomType.Crater:
                        roomInstance.GlobalPosition = new Vector2(airlockPos.X + sizeOffset, 2f * sizeOffset);
                        break;
					default:
						break;
				}

				x++;
			}
		}

		private Node2D InstantiateRoomGraphics(Room room)
		{
			Room2D roomNode = roomPrefab.Instantiate<Room2D>();
			roomNode.Name = room.Name;
			Texture2D texActivation = rr2d[room.RoomType].Tex_Activation;
			if (room.RoomType == RoomType.Airlock && StardustGameConfig.CurrentConfig.FirstStepsEnabled) texActivation = rr2d[RoomType.AirlockExpanded].Tex_Activation;

            roomNode.Initialize(room, RoomToSprite(room), texActivation);
			AddChild(roomNode);


			return roomNode;
		}

		private void RemoveChildren()
		{
			foreach (Node child in GetChildren())
			{
				child.QueueFree();
			}
		}

		private Texture2D RoomToSprite(Room room)
		{
			if (rr2d == null) return null;

			if (room.RoomType == RoomType.Airlock && StardustGameConfig.CurrentConfig.FirstStepsEnabled) return rr2d[RoomType.AirlockExpanded].Texture;

			return rr2d[room.RoomType].Texture;
		}

		private void GetRoomResources()
		{
			if (rr2d != null) return;
			rr2d = new();

			using var dir = DirAccess.Open(roomResourcePath);
			if (dir == null) return;

			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (!string.IsNullOrEmpty(fileName))
			{
				if (!dir.CurrentIsDir())
				{
					// I don't even fully understand why this needs to be done...
					// When you export the project, the .tres files get converted to .tres.remap for performance or something
					// However, the ResourceLoader doesn't know how to load them... ?
					// So apparently the .tres files DO still exist, just are not visible to DirAccess...
					// We want to grab those. So just take the .remaps that exist and remove the extension...
					if (fileName.EndsWith(".remap"))
					{
						fileName = fileName.Remove(fileName.Length - 6);
					}

					RoomRes2D roomRes = ResourceLoader.Load<RoomRes2D>(roomResourcePath + '/' + fileName);
					rr2d.TryAdd(roomRes.Type, roomRes);
				}
				fileName = dir.GetNext();
			}
		}
	} 
}
