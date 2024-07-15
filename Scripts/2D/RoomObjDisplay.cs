using Godot;
using Stardust.Actions;
using System;
using System.Linq;

namespace Stardust.Godot
{
	public partial class RoomObjDisplay : Button
	{
        [Export] private Room2D roomGraphic;
        [Export] Label objLabel;

        public override void _Ready()
        {
            Pressed += OnClick;
        }

        public override void _Process(double delta)
        {
            if (ObjectiveHandler.CurrentObjective == null) return;

            Objective obj = ObjectiveHandler.CurrentObjective;

            int taskCount = 0;

            foreach (Task task in obj.Tasks)
            {
                if (task.Completed) continue;

                if (task.Room.RoomType == roomGraphic.Room.RoomType)
                {
                    taskCount++;
                }
            }

            if (taskCount > 0)
            {
                if (taskCount > 1) objLabel.Text = $"x{taskCount}";
                else objLabel.Text = "";

                Show();
                Disabled = false;
            }
            else
            {
                Hide();
                Disabled = true;
            }
        }

        private void OnClick()
        {
            if (GameStart.LocalPlayer.Room != roomGraphic.Room) return;

            if (GameStart.LocalPlayer.EnergyCards.Where((e) => !e.Exhausted).Max((e) => e.Energy) <= GameLogic.EnergyExpended) return;

            if (roomGraphic.Room.Pawns.Count > roomGraphic.Room.Capacity) return;

            CompleteBaseTask action = new(roomGraphic.Room.RoomType);
            action.Do();

            ObjectiveHandler.CheckAllObjectivesCompleted();
        }

        public override void _ExitTree()
        {
            Pressed -= OnClick;
        }
    } 
}
