using Stardust.Godot;
using System.Linq;

namespace Stardust
{
    public class SampleTask : MarsTask
    {
        public SampleTask(RoomType _roomType) : base(_roomType)
        {
            roomType = _roomType;

            Description = "Gather samples to the $Lander";
        }

        Item sample = new(ItemType.Sample);
        RoomType roomType;

        public override void Activate()
        {
            Room room = GameLogic.RoomManager.GetRoomByType(roomType);

            room.AddItem(sample);
        }

        public override void UndoActivate()
        {
            foreach (Room room in GameLogic.RoomManager.Rooms)
            {
                if (room.GetItem(ItemType.Sample) != null)
                {
                    room.RemoveItem(sample);
                }
            }
        }
    }
}