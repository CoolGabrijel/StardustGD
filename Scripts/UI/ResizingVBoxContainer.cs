using Godot;
using System;

[GlobalClass]
public partial class ResizingVBoxContainer : VBoxContainer
{
    [Export] float heightPadding = 0;
    [Export] float widthPadding = 0;

    Control parent;

    public override void _Ready()
    {
        parent = GetParent<Control>();
    }

    public override void _Process(double delta)
    {
        float maxWidth = 0;
        float totalHeight = 0;

        foreach (Control child in parent.FindChildren("*", "Control", false))
        {
            if (child.Size.X > maxWidth) maxWidth = child.Size.X;

            totalHeight += child.Size.Y;
        }

        //Size = new Vector2(Size.X, totalHeight + heightPadding);
        parent.CustomMinimumSize = new Vector2(maxWidth + widthPadding, Size.Y + heightPadding);
    }
}
