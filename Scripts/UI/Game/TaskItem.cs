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
		Tween flashTween;
		Tween curtainTween;

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

			//flash.Size = new Vector2(0f, Size.Y);
			//flash.Position = Vector2.Zero;
			//curtain.CustomMinimumSize = new Vector2(Size.X, Size.Y);

   //         flashTween?.Kill();
			//flashTween = flash.CreateTween();
			//flashTween.TweenProperty(flash, "position", Vector2.Right * Size.X / 2, .5f);
			//flashTween.Parallel().TweenProperty(flash, "size", new Vector2(Size.X / 2f, Size.Y), .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
			//flashTween.TweenProperty(flash, "position", Vector2.Right * Size.X, .5f);
			//flashTween.Parallel().TweenProperty(flash, "size", new Vector2(0f, Size.Y), .5f);

			//curtainTween?.Kill();
   //         curtainTween = curtain.CreateTween();
   //         curtainTween.TweenProperty(curtain, "custom_minimum_size", Vector2.Zero, 1f);
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

			return "Unidentified Task";
		}
	} 
}
