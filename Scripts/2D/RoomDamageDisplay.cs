using Godot;
using Stardust.Actions;
using System;

namespace Stardust.Godot.UI
{
	public partial class RoomDamageDisplay : Button
    {
        [Export] private Room2D roomGraphic;
        [Export] Label damageLabel;

        Tween colorTween;

        public override void _Ready()
		{
			Pressed += OnClicked;
            MouseEntered += OnMouseEntered;
            MouseExited += OnMouseExited;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (roomGraphic.Room.DamageAmount > 0)
            {
                damageLabel.Text = $"x{roomGraphic.Room.DamageAmount}";
                //damageLabel.Show();
                Show();
                Disabled = false;
            }
            else
            {
                //damageLabel.Hide();
                Hide();
                Disabled = true;
            }
        }

		private void OnClicked()
        {
            Pawn localPlayer = GameStart.LocalPlayer;

            if (GameLogic.TurnQueue.CurrentPawn != localPlayer) return;

            if (RepairRoom.CanRepairRoom(roomGraphic.Room)) new RepairRoom(roomGraphic.Room, localPlayer).Do();
        }

        private void OnMouseEntered()
        {
            if (!RepairRoom.CanRepairRoom(roomGraphic.Room))
            {
                MouseDefaultCursorShape = CursorShape.Arrow;
                return;
            }

            colorTween?.Kill();

            colorTween = CreateTween();
            colorTween.TweenProperty(this, "modulate", new Color(2, 2, 2), .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

            MouseDefaultCursorShape = CursorShape.PointingHand;
        }

        private void OnMouseExited()
        {
            colorTween?.Kill();

            colorTween = CreateTween();
            colorTween.TweenProperty(this, "modulate", Colors.White, .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        public override void _ExitTree()
        {
			Pressed -= OnClicked;
        }
    } 
}
