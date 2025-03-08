namespace Stardust
{
	public class AirlockExpanded : Room
    {
        public override RoomType RoomType => RoomType.Airlock;
        public override bool CanBeActivated => Pawns.Count <= Capacity;
    } 
}
