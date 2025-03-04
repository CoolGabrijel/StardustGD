namespace Stardust
{
    public class Habitation : Room
    {
        public override RoomType RoomType => RoomType.Habitation;
        public override bool CanBeActivated => !GameLogic.RoomManager.GetRoomByType(RoomType.SolarPanels).Broken && !GameLogic.RoomManager.GetRoomByType(RoomType.LifeSupport).Broken && !Broken;
    }
}