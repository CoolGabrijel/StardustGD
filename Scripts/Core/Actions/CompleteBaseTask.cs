using Godot;
using System;

namespace Stardust.Actions
{
    public class CompleteBaseTask : IUndoableAction
    {
        public CompleteBaseTask(RoomType roomType)
        {
            foreach (Task task in ObjectiveHandler.CurrentObjective.Tasks)
            {
                if (task.Room.RoomType == roomType) Task = task;
            }
        }

        public Task Task { get; private set; }

        public void Do()
        {
            Task.Complete();
            GameLogic.EnergyExpended++;
            ActionLibrary.AddAction(this);
        }

        public void Undo()
        {
            Task.UndoComplete();
            GameLogic.EnergyExpended--;
        }
    }
}
