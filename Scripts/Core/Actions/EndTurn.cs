using Godot;
using System.Linq;

namespace Stardust.Actions
{
    public class EndTurn : IUndoableAction
    {
        public EndTurn(bool skipActionCardExhaustion = false)
        {
            SkipActionCardExhaustion = skipActionCardExhaustion;
        }

        public Room DamagedRoom { get; set; }
        public int EnergyExpended { get; set; }
        public bool SkipActionCardExhaustion { get; set; }

        private EnergyCard exhaustedCard;

        public void Do()
        {
            if (!SkipActionCardExhaustion)
            {
                exhaustedCard = GetLowestCard();
                ExhaustLowestCard();
            }

            if (!StardustGameConfig.CurrentConfig.DamageDisabled)
            {
                GameLogic.DamageManager.Damage();
                GameLogic.DamageManager.TurnIndex++;
            }

            //if (DamagedRoom == null) DamagedRoom = GameLogic.GetRoomToDamage();
            //DamagedRoom.Damage();

            EnergyExpended = GameLogic.EnergyExpended;
            GameLogic.EndTurn();
        }

        public void Undo()
        {
            //DamagedRoom.DamageAmount--;
            if (exhaustedCard != null) exhaustedCard.Exhausted = false;

            if (!StardustGameConfig.CurrentConfig.DamageDisabled)
            {
                GameLogic.DamageManager.TurnIndex--;
                GameLogic.DamageManager.UndoDamage();
            }

            GameLogic.EnergyExpended = EnergyExpended;
            GameLogic.TurnQueue.Previous();
        }

        private void ExhaustLowestCard()
        {
            if (exhaustedCard == null) return;

            exhaustedCard.Exhausted = true;

            GD.Print($"GameLogic :: Exhausting {exhaustedCard.Energy} Energy");
        }

        private EnergyCard GetLowestCard()
        {
            return GameLogic.TurnQueue.CurrentPawn.EnergyCards.Where((x) => !x.Exhausted).Where((x) => x.Energy >= EnergyExpended).FirstOrDefault();
        }
    }
}
