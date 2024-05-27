using Godot;
using System;
using System.Collections.Generic;

namespace Stardust.Godot
{
	[Tool]
	public partial class RoomGen2D : Node2D
	{
		[Export] private PackedScene roomPrefab = ResourceLoader.Load<PackedScene>("res://Prefabs/2D/Room2D.tscn");
		[Export] private int sizeOffset = 410;
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
			rm.GenerateRooms();
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

				//switch (room.RoomType)
				//{
				//    case Room.RoomTypes.Lander:
				//        GO.transform.position = new Vector2(airlockPos.x, -1f);
				//        break;
				//    case Room.RoomTypes.Ridge:
				//        GO.transform.position = new Vector2(airlockPos.x, -3f);
				//        break;
				//    case Room.RoomTypes.Plains:
				//        GO.transform.position = new Vector2(airlockPos.x + 1f, -3f);
				//        break;
				//    case Room.RoomTypes.Peak:
				//        GO.transform.position = new Vector2(airlockPos.x - 1f, -3f);
				//        break;
				//    case Room.RoomTypes.Gully:
				//        GO.transform.position = new Vector2(airlockPos.x - 1f, -2f);
				//        break;
				//    case Room.RoomTypes.Crater:
				//        GO.transform.position = new Vector2(airlockPos.x + 1f, -2f);
				//        break;
				//    default:
				//        break;
				//}

				x++;
			}
		}

		private Node2D InstantiateRoomGraphics(Room room)
		{
			Room2D roomNode = roomPrefab.Instantiate<Room2D>();
			roomNode.Name = room.Name;
			roomNode.Initialize(room, RoomToSprite(room), rr2d[room.RoomType].Tex_Activation);
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

			return rr2d[room.RoomType].Texture;
		}

		private void GetRoomResources()
		{
			rr2d = new();

			using var dir = DirAccess.Open(roomResourcePath);
			if (dir == null) return;

			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (!dir.CurrentIsDir())
				{
					RoomRes2D roomRes = ResourceLoader.Load<RoomRes2D>(roomResourcePath + '/' + fileName);
					rr2d.Add(roomRes.Type, roomRes);
				}
				fileName = dir.GetNext();
			}
		}
	} 
}
