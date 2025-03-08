namespace Stardust
{
	public class Lander : Room
    {
        public event System.Action LanderMoved;

        public override RoomType RoomType => RoomType.Lander;
        public override bool CanBeActivated => !Broken;
        public bool HasLanded => Neighbours[0].Item2 is MarsTile;

        public void Land()
        {
            LanderMoved?.Invoke();
        }

        public void Release()
        {
            LanderMoved?.Invoke();
        }
    } 
}
