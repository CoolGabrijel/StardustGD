using Godot;
using System;

public static class FreeChildrenExtension
{
    public static void FreeChildren(this Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            child.QueueFree();
        }
    }
}
