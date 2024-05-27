namespace Stardust
{
    public class Workshop : Room
    {
        public override RoomType RoomType => RoomType.Workshop;
        public override bool CanBeActivated => true;
    }
}
