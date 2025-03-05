namespace Stardust
{
    public class Aurora : Pawn
    {
        public Aurora(PawnType type) : base(type)
        {
        }

        public event System.Action OnDamageBlocked;

        public void DamageBlocked()
        {
            OnDamageBlocked?.Invoke();
        }
    }
}
