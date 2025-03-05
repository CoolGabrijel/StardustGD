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
                if (task.Room.RoomType == roomType && !task.Completed) Task = task;
            }
        }

        public Task Task { get; private set; }

        public void Do()
        {
            Task.Complete();
            GameLogic.EnergyExpended++;
        }

        public void Undo()
        {
            Task.UndoComplete();
            GameLogic.EnergyExpended--;
        }
    }
}
