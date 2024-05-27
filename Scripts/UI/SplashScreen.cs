using Godot;
using System;

[GlobalClass]
public partial class SplashScreen : Node
{
	public override void _Ready()
	{
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
	}
}
