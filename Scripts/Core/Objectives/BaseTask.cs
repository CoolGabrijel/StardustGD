namespace Stardust
{
    public sealed class BaseTask : Task
    {
        public BaseTask(RoomType _roomType) : base(_roomType)
        {
            RoomType = _roomType;

            Room = GameLogic.RoomManager.GetRoomByType(RoomType);
        }

        public override string Tag => "Base";

        public readonly RoomType RoomType;
        public readonly Room Room;

        public override void Activate()
        {
            //Room.AddTask(this);
        }

        public override void UndoActivate()
        {

        }
    }
}