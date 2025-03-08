using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class TaskItem : Control
	{
		[Export] Label label;
		[Export] TextureRect textureRect;
		[Export] ColorRect flash;
		[Export] ColorRect curtain;
		[Export] AnimationPlayer player;

		Task task;

		public void Initialize(Task newTask, Texture2D texture = null)
		{
			task = newTask;
			label.Text = GetShortDescription(newTask);
			if (texture != null)
			{
				textureRect.Texture = texture;
			}
			else
			{
				textureRect.GetParent<Control>().Visible = false;
			}

			VisibilityChanged += OnVisibilityChanged;
            newTask.StatusChange += OnTaskStatusChange;
		}

        public override void _Process(double delta)
        {
			if (Visible)
			{
				if (!IsActive() || task.Completed) Visible = false;
			}
			else
			{
				if (IsActive() && !task.Completed) Visible = true;
			}
        }

        private void OnTaskStatusChange()
        {
			Visible = !task.Completed;
        }

        public override void _Ready()
        {
            OnVisibilityChanged();
        }

        private void OnVisibilityChanged()
		{
			if (!Visible) return;

			player.Play("OnActivate");
        }

		private bool IsActive()
		{
            if (ObjectiveHandler.CurrentObjective == null) return false;

            Objective obj = ObjectiveHandler.CurrentObjective;

            foreach (Task objTask in obj.Tasks)
            {
                if (objTask == task)
                {
					return true;
                }
            }

			return false;
        }

		private string GetShortDescription(Task task)
		{
			if (task.Tag == "Base") return $"Do Task - {task.Room.Name}";
			else if (task.Tag == "FirstSteps")
			{
				if (task is FlagTask) return $"Plant Flag - Peak";
				else if (task is SampleTask) return $"Bring Samples to Lander";
			}

			return "Unidentified Task";
		}
	} 
}
