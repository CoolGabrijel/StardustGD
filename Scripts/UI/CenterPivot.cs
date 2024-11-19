using Godot;
using System;

namespace Stardust.Godot.UI
{
	[GlobalClass]
	public partial class CenterPivot : Node
	{
		Control parent;

        public override void _Ready()
        {
			Name = "CenterPivot";

            parent = GetParent<Control>();

			if (parent == null)
			{
				GD.PrintErr($"Parent of {Name} is not a Control node.");
			}
        }

        public override void _Process(double delta)
		{
			parent.PivotOffset = new(parent.Size.X / 2f, parent.Size.Y / 2f);
		}
	} 
}
