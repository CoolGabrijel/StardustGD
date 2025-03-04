using Godot;
using System;

public static class FreeChildrenExtension
{
    public static void FreeChildren(this Control control)
    {
        foreach (Node child in control.GetChildren())
        {
            child.QueueFree();
        }
    }
}
