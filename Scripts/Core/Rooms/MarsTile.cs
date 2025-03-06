namespace Stardust
{
	public class MarsTile : Room
    {
        public MarsTile(RoomType type)
        {
            marsTileType = type;
        }

        public override RoomType RoomType => marsTileType;

        private readonly RoomType marsTileType;
    } 
}
