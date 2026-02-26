using System.Linq;

namespace Stardust.Actions
{
    public partial class WolframHeal : IUndoableAction
    {
        public WolframHeal(PawnType target)
        {
            Target = target;
        }

        public PawnType Target { get; private set; }

        public void Do()
        {
            EnergyCard card = GetHighestCard(true);

            card.Exhausted = false;
            GameLogic.EnergyExpended++;
        }

        public void Undo()
        {
            EnergyCard card = GetHighestCard(false);
            card.Exhausted = true;
            GameLogic.EnergyExpended--;
        }

        private EnergyCard GetHighestCard(bool exhausted)
        {
            Pawn target = GameLogic.GetPawnByType(Target);
            return target.EnergyCards.Where((c) => c.Exhausted == exhausted).OrderByDescending((c) => c.Energy).FirstOrDefault();
        }
    }
}
