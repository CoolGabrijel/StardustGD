using Godot;
using System;
using Stardust.Actions;

namespace Stardust.Godot.UI;

public partial class YourTurnNotifier : Control
{
	[Export] private Vector2 startPosition;
	[Export] private Vector2 endPosition;
	[Export] private Control foreground;
	[Export] private Color startColor;
	[Export] private Color endColor;
	[Export] private AudioStreamPlayer audioPlayer;

	private Tween moveTween;
	private Tween colorTween;
	
	public override void _Ready()
	{
		if (!PIOMP.Room.IsInRoom)
		{
			QueueFree();
			return;
		}
		
		EndTurn.OnEndTurn += OnEndTurn;

		colorTween = CreateTween();
		colorTween.TweenProperty(foreground, "modulate", endColor, 0.25f);
		colorTween.TweenProperty(foreground, "modulate", startColor, 0.25f);
		colorTween.SetLoops();
		
		if (GameLogic.TurnQueue.CurrentPawn == GameStart.LocalPlayer)
		{
			moveTween = CreateTween();
			moveTween.SetEase(Tween.EaseType.Out);
			moveTween.SetTrans(Tween.TransitionType.Expo);
			moveTween.TweenProperty(this, "position", endPosition, .75f).From(startPosition);
		}
		else
		{
			Position = startPosition;
		}
	}

	private void OnEndTurn()
	{
		moveTween?.Kill();
		
		if (GameLogic.TurnQueue.CurrentPawn == GameStart.LocalPlayer)
		{
			moveTween = CreateTween();
			moveTween.SetEase(Tween.EaseType.Out);
			moveTween.SetTrans(Tween.TransitionType.Expo);
			moveTween.TweenProperty(this, "position", endPosition, .75f).From(startPosition);
			audioPlayer.Play();
		}
		else
		{
			moveTween = CreateTween();
			moveTween.SetEase(Tween.EaseType.In);
			moveTween.SetTrans(Tween.TransitionType.Expo);
			moveTween.TweenProperty(this, "position", startPosition, .75f);
		}
	}

	public override void _ExitTree()
	{
		EndTurn.OnEndTurn -= OnEndTurn;
	}
}