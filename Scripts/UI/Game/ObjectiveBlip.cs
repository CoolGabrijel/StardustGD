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
				Modulate = completedColor;
				return;
			}
			else if (ObjectiveHandler.CurrentObjective == objective)
			{
				if (activeTween == null)
				{
					activeTween = CreateTween();
					activeTween.TweenProperty(this, "modulate", activeColor, 1f);
					activeTween.TweenProperty(this, "modulate", completedColor, 1f);
					activeTween.SetLoops();
				}
				//Modulate = activeColor;
				return;
			}

            activeTween?.Kill();
            Modulate = inactiveColor;
        }
    } 
}
