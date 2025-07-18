using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class MainMenuButton : Button
	{
		[Export] Control ButtonBase { get; set; }
		[Export] Control Graphics { get; set; }

		Tween moveTween;

		public override void _Ready()
		{
			MouseEntered += OnHover;
			MouseExited += OnHoverLeft;
		}

		private void OnHover()
		{
            ButtonBase.Modulate = Colors.White;

			moveTween?.Kill();

			moveTween = CreateTween();
			moveTween.SetTrans(Tween.TransitionType.Quart);
			moveTween.SetEase(Tween.EaseType.Out);
			moveTween.TweenProperty(Graphics, "position", new Vector2(20, 0), .25f);
		}

		private void OnHoverLeft()
        {
            ButtonBase.Modulate = new Color("cacaca");

            moveTween?.Kill();

            moveTween = CreateTween();
            moveTween.SetTrans(Tween.TransitionType.Quart);
            moveTween.SetEase(Tween.EaseType.Out);
            moveTween.TweenProperty(Graphics, "position", Vector2.Zero, .5f);
        }
	} 
}
