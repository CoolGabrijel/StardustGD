namespace Stardust.Actions
{
    public class EndTurn : IUndoableAction
    {
        public Room DamagedRoom { get; set; }
        public int EnergyExpended { get; set; }
        public Room PreviouslyDamagedRoom { get; set; }

        public void Do()
        {
            PreviouslyDamagedRoom = GameLogic.PreviouslyDamagedRoom;
            if (DamagedRoom == null) DamagedRoom = GameLogic.GetRoomToDamage();
            DamagedRoom.Damage();
            GameLogic.PreviouslyDamagedRoom = DamagedRoom;
            EnergyExpended = GameLogic.EnergyExpended;
            GameLogic.EndTurn();

            ActionLibrary.AddAction(this); // TODO: Maybe not have this here for Zambuko?
        }

        public void Undo()
        {
            GameLogic.PreviouslyDamagedRoom = PreviouslyDamagedRoom;
            DamagedRoom.DamageAmount--;
            GameLogic.EnergyExpended = EnergyExpended;
            GameLogic.TurnQueue.Previous();
        }
    }
}
