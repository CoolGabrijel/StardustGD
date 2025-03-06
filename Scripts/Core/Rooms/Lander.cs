namespace Stardust
{
	public class Lander : Room
    {
        public override RoomType RoomType => RoomType.Lander;
        public override bool CanBeActivated => !Broken;
    } 
}
