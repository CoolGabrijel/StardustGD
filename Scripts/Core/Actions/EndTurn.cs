using Godot;
using System;

namespace Stardust.Actions
{
    public class EndTurn : IUndoableAction
    {
        public Room DamagedRoom { get; set; }
        public int EnergyExpended { get; set; }

        public void Do()
        {
            if (DamagedRoom == null) DamagedRoom = GameLogic.GetRoomToDamage();
            DamagedRoom.Damage();
            EnergyExpended = GameLogic.EnergyExpended;
            GameLogic.EndTurn();

            ActionLibrary.AddAction(this); // TODO: Maybe not have this here for Zambuko?
        }

        public void Undo()
        {
            DamagedRoom.DamageAmount--;
            GameLogic.EnergyExpended = EnergyExpended;
            GameLogic.TurnQueue.Previous();
        }
    }
}
