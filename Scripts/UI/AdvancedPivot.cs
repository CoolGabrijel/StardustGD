using Godot;
using System;

[Tool]
[GlobalClass]
public partial class AdvancedPivot : Node
{
	[Export] public Vector2 pivot;

    Control parent;

    public override void _Ready()
    {
        parent = GetParent<Control>();
    }

	public override void _Process(double delta)
	{
		if (parent == null) return;

        parent.PivotOffset = new(parent.Size.X * pivot.X, parent.Size.Y * pivot.Y);
    }
}
