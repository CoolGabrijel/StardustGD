using Godot;

namespace Stardust
{
    public class FlagTask : MarsTask
    {
        public FlagTask(RoomType _roomType) : base(_roomType)
        {
            Description = "Plant a Flag on $Peak";
        }

        Item flag = new(ItemType.Flag);

        public override void Activate()
        {
            Room lander = GameLogic.RoomManager.GetRoomByType(RoomType.Lander);
            lander.AddItem(flag);
        }

        public override void UndoActivate()
        {
            foreach (Room room in GameLogic.RoomManager.Rooms)
            {
                if (room.GetItem(ItemType.Flag) != null)
                {
                    room.RemoveItem(flag);
                }
            }
        }
    }
}