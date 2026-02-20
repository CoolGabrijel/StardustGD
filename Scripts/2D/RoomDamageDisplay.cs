using Godot;
using Stardust.Actions;
using System;

namespace Stardust.Godot.UI
{
	public partial class RoomDamageDisplay : Button
    {
        [Export] private Room2D roomGraphic;
        [Export] Label damageLabel;
        [Export] TextureRect flash;

        Tween colorTween;
        Tween flashTween;

        public override void _Ready()
		{
			Pressed += OnClicked;
            MouseEntered += OnMouseEntered;
            MouseExited += OnMouseExited;

            if (flash != null)
            {
                flashTween = flash.CreateTween();
                flashTween.TweenProperty(flash, "scale", Vector2.One * 5, 3f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
                flashTween.TweenProperty(flash, "scale", Vector2.One * 2, 1f).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
                flashTween.SetLoops();
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (roomGraphic.Room.DamageAmount > 0)
            {
                damageLabel.Text = $"x{roomGraphic.Room.DamageAmount}";
                Show();
                Disabled = false;
            }
            else
            {
                Hide();
                Disabled = true;
            }
        }

		private void OnClicked()
        {
            Pawn localPlayer = GameStart.LocalPlayer;

            if (GameLogic.TurnQueue.CurrentPawn != localPlayer) return;

            if (!RepairRoom.CanRepairRoom(roomGraphic.Room)) return;
            
            new RepairRoom(roomGraphic.Room, localPlayer).Do();
            
            if (PIOMP.Room.IsHost) ServerSend.Repair(GameStart.PlayerId);
            else if (PIOMP.Room.IsInRoom) ClientSend.ReqRepair();
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
