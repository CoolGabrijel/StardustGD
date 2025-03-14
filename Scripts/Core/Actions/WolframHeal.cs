using System.Linq;

namespace Stardust.Actions
{
    public partial class WolframHeal : IUndoableAction
    {
        public WolframHeal(Pawn target)
        {
            Target = target;
        }

        public EnergyCard Card { get; private set; }
        public Pawn Target { get; private set; }

        public void Do()
        {
            if (Card == null)
            {
                Card = GetHighestCard();
            }

            Card.Exhausted = false;
            GameLogic.EnergyExpended++;
        }

        public void Undo()
        {
            Card.Exhausted = true;
            GameLogic.EnergyExpended--;
        }

        private EnergyCard GetHighestCard()
        {
            return Target.EnergyCards.Where((c) => c.Exhausted).OrderByDescending((c) => c.Energy).FirstOrDefault();
        }
    }
}
