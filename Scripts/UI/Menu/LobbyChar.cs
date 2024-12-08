using Godot;
using System;

namespace Stardust.Godot.UI
{
	public partial class LobbyChar : Control
	{
        public static LobbyChar LocalLobbyChar { get; set; }

        [Export] PawnType PawnType { get; set; }
        [Export] bool isRandom;

        [ExportGroup("References")]
        [Export] TextureButton btn;
        [Export] TextureRect btnBorder;
        [Export] TextureRect btnMask;
        [Export] Control portraits;
        [Export] ShaderMaterial greyscaleShader;

        private int randIndex = 0;
        private Tween randCycleTween;

        public override void _Ready()
        {
            btn.MouseEntered += OnMouseEntered;
            btn.MouseExited += OnMouseExited;

            for (int i = 0; i < GetParent().GetChildCount(); i++)
            {
                if (GetParent().GetChild(i) == this)
                {
                    if (i % 2 != 0)
                    {
                        btnBorder.FlipV = true;
                        btnMask.FlipV = true;
                        btn.RotationDegrees = -90;
                    }
                }
            }

            for (int i = 0; i < portraits.GetChildCount(); i++)
            {
                portraits.GetChild<TextureRect>(i).Visible = (int)PawnType == i;
            }

            if (isRandom) PlayRandomCycle();
        }

        private void PlayRandomCycle()
        {
            randCycleTween?.Kill();
            Control curPortrait = portraits.GetChild<Control>(randIndex);
            curPortrait.Hide();
            randIndex++;
            if (randIndex >= portraits.GetChildCount()) randIndex = 1;
            curPortrait = portraits.GetChild<Control>(randIndex);
            if (curPortrait.Material == null) curPortrait.Material = greyscaleShader;
            curPortrait.Show();

            randCycleTween = curPortrait.CreateTween();
            randCycleTween.TweenProperty(curPortrait, "modulate", new Color(1, 1, 1, 1f), 2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
            randCycleTween.TweenProperty(curPortrait, "modulate", new Color(1, 1, 1, 0f), 2f).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Circ);
            randCycleTween.Finished += PlayRandomCycle;
        }

        private void OnMouseEntered()
        {
            btnBorder.Modulate = Colors.DarkCyan;
            btn.Modulate = Colors.DarkCyan;
        }

        private void OnMouseExited()
        {
            btnBorder.Modulate = Colors.White;
            btn.Modulate = Colors.White;
        }
    } 
}
