namespace Stardust
{
    public class VideoTask : MarsTask
    {
        public VideoTask(RoomType _roomType) : base(_roomType)
        {
            room = GameLogic.RoomManager.GetRoomByType(_roomType);

            Description = $"Get Objectives to ${_roomType}";
        }

        Room room;

        public override void Activate()
        {
        }

        public override void UndoActivate()
        {
        }
    }
}