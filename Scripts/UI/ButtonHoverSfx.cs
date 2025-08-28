using Godot;
using System;

[GlobalClass]
public partial class ButtonHoverSfx : AudioStreamPlayer
{
	[Export] float pitchRandomMax = 1.1f;
	[Export] float pitchRandomMin = 0.9f;

	static RandomNumberGenerator rng = new();
	private Control parent;

	public override void _Ready()
	{
		parent = GetParent<Control>();

        if (parent == null) return;

		parent.MouseEntered += OnHover;
	}

	private void OnHover()
	{
		if (parent is BaseButton button)
		{
			if (button.Disabled) return;
		}
		
		PitchScale = rng.RandfRange(pitchRandomMin, pitchRandomMax);
		Play();
	}
}
