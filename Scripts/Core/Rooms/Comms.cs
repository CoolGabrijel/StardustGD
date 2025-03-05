namespace Stardust
{
    public class Comms : Room
    {
        public override RoomType RoomType => RoomType.Comms;
        public override bool CanBeActivated => !GameLogic.RoomManager.GetRoomByType(RoomType.SolarPanels).Broken &&
            !Broken && ObjectiveHandler.ObjectiveComplete;
    }
}