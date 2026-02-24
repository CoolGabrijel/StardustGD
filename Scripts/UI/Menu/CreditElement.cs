using Godot;
using System;

namespace Stardust.Godot.UI;

public partial class CreditElement : Control
{
	private static RandomNumberGenerator rng = new();
	private Timer timer;
	
	public override void _Ready()
	{
		timer = new Timer();
		AddChild(timer);
		timer.Start(1);
		timer.Timeout += TimerOnTimeout;
	}

	private void TimerOnTimeout()
	{
		RotationDegrees = rng.RandfRange(-10, 10);
	}
}
