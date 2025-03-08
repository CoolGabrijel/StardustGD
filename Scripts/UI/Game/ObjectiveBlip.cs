using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class ObjectiveBlip : Control
	{
		[Export] TextureRect textureRect;
		[Export] Color inactiveColor;
		[Export] Color completedColor;
		[Export] Color activeColor;
		[Export] Color marsCompletedColor;
		[Export] Color marsActiveColor;

		Objective objective;
		Tween activeTween;

		public void Initialize(Objective obj)
		{
			objective = obj;
		}

        public override void _Process(double delta)
        {
			if (objective == null) return;

			if (objective.Completed)
			{
				activeTween?.Kill();
				activeTween = null;
				if (objective.Tasks[0].Tag == "FirstSteps") Modulate = marsCompletedColor;
				else Modulate = completedColor;
				return;
			}
			else if (ObjectiveHandler.CurrentObjective == objective)
			{
				if (activeTween == null)
				{
					Color color = activeColor;
					Color compColor = completedColor;
					if (objective.Tasks[0].Tag == "FirstSteps")
					{
						color = marsActiveColor;
						compColor = marsCompletedColor;
					}

					activeTween = CreateTween();
					activeTween.TweenProperty(this, "modulate", color, 1f);
					activeTween.TweenProperty(this, "modulate", compColor, 1f);
					activeTween.SetLoops();
				}
				//Modulate = activeColor;
				return;
			}

            activeTween?.Kill();
            activeTween = null;
            Modulate = inactiveColor;
        }
    } 
}
