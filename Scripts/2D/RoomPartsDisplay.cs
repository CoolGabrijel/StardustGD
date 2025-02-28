using Godot;
using Stardust.Actions;
using System;

namespace Stardust.Godot
{
	public partial class RoomPartsDisplay : Button
	{
		[Export] private Room2D roomGraphic;
        [Export] Label partsLabel;

        Tween colorTween;

        public override void _Ready()
        {
            Pressed += OnClick;
            MouseEntered += OnMouseEntered;
            MouseExited += OnMouseExited;
        }

        public override void _Process(double delta)
        {
            if (roomGraphic.Room.Parts > 0)
            {
                Show();
                partsLabel.Text = $"x{roomGraphic.Room.Parts}";
                Disabled = false;
            }
            else
            {
                Hide();
                Disabled = true;
            }
        }

        private void OnClick()
        {
            Pawn player = GameStart.LocalPlayer;
            if (!player.CanPickUp) return;

            PickUpPart PickupAction = new(player, roomGraphic.Room, roomGraphic.Room.GetItem(ItemType.Part));
            PickupAction.Do();
            ActionLibrary.AddAction(PickupAction);
        }

        private void OnMouseEntered()
        {
            colorTween?.Kill();

            colorTween = CreateTween();
            colorTween.TweenProperty(this, "modulate", new Color(2, 2, 2), .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        private void OnMouseExited()
        {
            colorTween?.Kill();

            colorTween = CreateTween();
            colorTween.TweenProperty(this, "modulate", Colors.White, .2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }

        public override void _ExitTree()
        {
            Pressed -= OnClick;
        }
    } 
}
