namespace Stardust.Actions
{
    public class EndTurn : IUndoableAction
    {
        public Room DamagedRoom { get; set; }
        public int EnergyExpended { get; set; }

        public void Do()
        {
            GameLogic.DamageManager.Damage();
            //if (DamagedRoom == null) DamagedRoom = GameLogic.GetRoomToDamage();
            //DamagedRoom.Damage();

            EnergyExpended = GameLogic.EnergyExpended;
            GameLogic.EndTurn();

            GameLogic.DamageManager.TurnIndex++;

            ActionLibrary.AddAction(this); // TODO: Maybe not have this here for Zambuko?
        }

        public void Undo()
        {
            //DamagedRoom.DamageAmount--;
            GameLogic.DamageManager.TurnIndex--;
            GameLogic.DamageManager.UndoDamage();
            GameLogic.EnergyExpended = EnergyExpended;
            GameLogic.TurnQueue.Previous();
        }
    }
}
