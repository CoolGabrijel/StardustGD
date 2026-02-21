using Godot;
using System;

[GlobalClass]
public partial class ButtonHoverEnlarge : Node
{
	[Export] private Control target;
	
	private Control parent;
	private Tween tween;

	public override void _Ready()
	{
		parent = GetParent<Control>();

		if (parent == null) return;

		parent.MouseEntered += OnHover;
		parent.MouseExited += OnLeave;
	}

	private void OnHover()
	{
		tween?.Kill();

		tween = CreateTween();
		tween.TweenProperty(target, "scale", Vector2.One * 1.5f, 0.1f);
	}

	private void OnLeave()
	{
		tween?.Kill();

		tween = CreateTween();
		tween.TweenProperty(target, "scale", Vector2.One, 0.1f);
	}
}
