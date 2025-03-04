using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class ObjectiveDisplay : Control
	{
		[Export] PackedScene blipComponent;
		[Export] Control blipContainer;

		public override void _Ready()
		{
			blipContainer.FreeChildren();

			foreach (Objective objective in ObjectiveHandler.Objectives)
			{
				ObjectiveBlip blip = blipComponent.Instantiate<ObjectiveBlip>();
				blip.Initialize(objective);
				blipContainer.AddChild(blip);
			}
		}

		public override void _Process(double delta)
		{
		}
	} 
}
